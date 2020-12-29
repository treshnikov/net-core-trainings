using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BusinessLogic.FilterParser
{
    public static class Tokenizer
    {
        public static Queue<Token> Tokenize(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return new Queue<Token>();
            }
            
            using var reader = new StringReader(filter);
            
            var result = new Queue<Token>();
            while (true)
            {
                var b = reader.Peek();
                if (b == -1)
                {
                    break;
                }
                
                var c = (char) b;

                if (char.IsWhiteSpace(c))
                {
                    reader.Read();
                    continue;
                }
                
                if (char.IsLetterOrDigit(c) || c == '.')
                {
                    var token = new Token(ReadIdent(reader));
                    result.Enqueue(token);
                }
                else if (c == '(' || c == ')')
                {
                    result.Enqueue(new Token(c.ToString()));
                    reader.Read();
                }
                else if (c == '\'')
                {
                    var token = new Token(ReadDateTime(reader));
                    result.Enqueue(token);
                }
                else 
                {
                    throw new FilterException($"Unknown character: {c}");
                }
            }
                
            return result;
        }

        private static string ReadDateTime(StringReader reader)
        {
            var stringBuilder = new StringBuilder();
            var first = true;
            while (true)
            {
                var c = (char) reader.Peek();
                stringBuilder.Append(c);
                reader.Read(); // need to go to the next char
                
                if (c == '\'')
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }

                    break;
                }
            }

            return stringBuilder.ToString();
        }

        private static string ReadIdent(StringReader reader)
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                var c = (char) reader.Peek();
                if (!char.IsLetterOrDigit(c) && c != '.')
                {
                    break;
                }
                
                stringBuilder.Append(c);
                reader.Read(); // need to go to the next char
            }

            return stringBuilder.ToString();
        }
    }
}