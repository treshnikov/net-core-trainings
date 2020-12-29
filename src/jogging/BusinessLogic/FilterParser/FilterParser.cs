using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogic.FilterParser
{
    public class FilterParser<T> where T: class
    {
        private readonly Queue<Token> _tokens;

        public FilterParser(string filter)
        {
            PropertiesList.Add(typeof(T));
            _tokens = Tokenizer.Tokenize(filter);
        }

        public IQueryable<T> Filter(IQueryable<T> query)
        {
            if (_tokens.Count == 0)
            {
                return query;
            }

            var argParam = Expression.Parameter(typeof(T), "data");
            var expression = ParseExpression(argParam);

            if (_tokens.Count > 0)
            {
                var token = _tokens.Dequeue();
                throw new FilterException($"Expected end of expression but {token.Ident} found");
            }
            
            var lambda = Expression.Lambda<Func<T, bool>>(expression, argParam);
            
            return query.Where(lambda);
        }

        private Expression ParseExpression(ParameterExpression argParam)
        {
            var expressions = new List<Expression>();
            var isFirst = true;
            while (true)
            {
                string expressionOperator = null;
                if (!isFirst)
                {
                    var token = DequeueToken();
                    expressionOperator = token.Ident;
                }

                var expression = PeekToken().Ident == "("
                    ? ParseParenthesisExpression(argParam)
                    : ParseSimpleExpression(argParam);
                
                // Because "and" operator has the biggest precedence,
                // it's better to apply it on the fly to the last and current items
                // So in the end we will have a list of the expressions that should be combined by "or" operator.
                if (!isFirst && expressionOperator == "and")
                {
                    expressions[^1] = Expression.AndAlso(expressions[^1], expression);
                }
                else
                {
                    expressions.Add(expression);
                }

                if (_tokens.Count == 0 || _tokens.Peek().Ident == ")")
                {
                    break;
                }

                isFirst = false;
            }

            if (!expressions.Any())
            {
                throw new FilterException("Expected expression but nothing found");
            }

            var result = expressions.First();
            for (var i = 1; i < expressions.Count; i++)
            {
                result = Expression.OrElse(result, expressions[i]);
            }
            
            return result;
        }

        private Token PeekToken()
        {
            if (_tokens.Count == 0)
            {
                throw new FilterException("Filter query unexpectedly ended");
            }
            
            return _tokens.Peek();
        }

        private Token DequeueToken()
        {
            if (_tokens.Count == 0)
            {
                throw new FilterException("Filter query unexpectedly ended");
            }
            
            return _tokens.Dequeue();
        }

        private Expression ParseSimpleExpression(ParameterExpression argParam)
        {
            var parameterExpression = ParseProperty(argParam, out var propertyName);
            var operatorIdent = ParseOperator();
            var valueExpression = ParseValue(propertyName);

            Expression result = null;

            switch (operatorIdent)
            {
                case "eq":
                    result = Expression.Equal(parameterExpression, valueExpression);
                    break;
                
                case "gt":
                    result = Expression.GreaterThan(parameterExpression, valueExpression);
                    break;
                
                case "lt":
                    result = Expression.LessThan(parameterExpression, valueExpression);
                    break;
                
                case "ne":
                    result = Expression.NotEqual(parameterExpression, valueExpression);
                    break;
                
                default:
                    throw new FilterException($"Unknown operator: {operatorIdent}");
            }

            return result;
        }

        private string ParseOperator()
        {
            var token = DequeueToken();
            var operatorIdent = token.Ident;
            return operatorIdent;
        }

        private Expression ParseParenthesisExpression(ParameterExpression argParam)
        {
            var token = DequeueToken();
            if (token.Ident != "(")
            {
                throw new FilterException($"Expected '(' but '{token.Ident}' found");
            }

            var expression = ParseExpression(argParam);
            
            token = DequeueToken();
            if (token.Ident != ")")
            {
                throw new FilterException($"Expected ')' but '{token.Ident}' found");
            }

            return expression;
        }

        private Expression ParseValue(string propertyName)
        {
            var token = DequeueToken();
            var property = PropertiesList.Get(typeof(T), propertyName);

            object value;
            if (property.Type == typeof(int))
            {
                if (!int.TryParse(token.Ident, out var result))
                {
                    throw new FilterException($"Expected int value but {token.Ident} found");
                }

                value = result;
            }
            else if (property.Type == typeof(double))
            {
                if (!double.TryParse(token.Ident, out var result))
                {
                    throw new FilterException($"Expected double value but {token.Ident} found");
                }

                value = result;
            }
            else if (property.Type == typeof(DateTime))
            {
                var strValue = token.Ident.Trim('\'').Trim();
                var formats = new[]
                {
                    "yyyy-MM-dd",
                    "yyyy-MM-dd hh:mm:ss"
                };
                
                if (!DateTime.TryParseExact(strValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    throw new FilterException($"Expected DateTime but {strValue} found");
                }

                value = result;
            }
            else if (property.Type == typeof(string))
            {
                value = token.Ident.Trim('\'');
            }
            else
            {
                throw new FilterException($"Unexpected type: {property.Type}");
            }
            
            return Expression.Constant(value);
        }

        private Expression ParseProperty(ParameterExpression argParam, out string propertyName)
        {
            var token = DequeueToken();
            var property = PropertiesList.Get(typeof(T), token.Ident);

            propertyName = property.Name;
            return Expression.Property(argParam, property.Name);
        }
    }
}