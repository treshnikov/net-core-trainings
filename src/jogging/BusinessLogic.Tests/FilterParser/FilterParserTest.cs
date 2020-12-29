using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.FilterParser;
using Domain;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.FilterParser
{
    [TestFixture]
    public class FilterParserTest
    {
        private List<JoggingData> _joggingDataList;
        private List<User> _userList;

        [SetUp]
        public void Setup()
        {
            _joggingDataList = new List<JoggingData>
            {
                new JoggingData
                {
                    Id = 1,
                    Distance = 5,
                    Longitude = 0,
                    Date = new DateTime(2011, 1, 1)
                },
                new JoggingData
                {
                    Id = 2,
                    Distance = 10,
                    Longitude = 5,
                    Date = new DateTime(2011, 1, 2)
                },
                new JoggingData
                {
                    Id = 3,
                    Distance = 15,
                    Longitude = 3.5,
                    Date = new DateTime(2011, 1, 3)
                }
            };

            _userList = new List<User>
            {
                new User
                {
                    Id = 1,
                    UserName = "Asd",
                    Password = "123"
                },
                new User
                {
                    Id = 2,
                    UserName = "Asd Zxc Qwe",
                    Password = "234"
                }
            };
        }
        
        [Test]
        public void Filter_WithEmptyParameter_ShouldReturnSameList()
        {
            // arrange
            var parser = new FilterParser<JoggingData>(null);
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(3);
            result.ShouldSatisfyAllConditions(
                () => result[0].Id.ShouldBe(1),
                () => result[1].Id.ShouldBe(2),
                () => result[2].Id.ShouldBe(3));
        }

        [Test]
        public void Filter_WithSimpleIntValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("distance eq 5");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(1);
            result[0].Id.ShouldBe(1);
        }
        
        [Test]
        public void Filter_WithParenthesisIntValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("(distance eq 5) or (distance eq 10)");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(1);
            result[1].Id.ShouldBe(2);
        }
        
        [Test]
        public void Filter_WithTwoSimpleIntValues_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("distance eq 5 or distance eq 10");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(1);
            result[1].Id.ShouldBe(2);
        }
        
        [Test]
        public void Filter_WithMultipleExpressions_ShouldFilterItemsRegardingOperatorPrecedence()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("distance gt 9 and distance lt 11 or distance gt 14 and distance lt 16");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(2);
            result[1].Id.ShouldBe(3);
        }

        [Test]
        public void Filter_WithExtraParenthesis_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("(((distance gt 9 and distance lt 11) or (distance gt 14 and distance lt 16)))");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(2);
            result[1].Id.ShouldBe(3);
        }
        
        [Test]
        public void Filter_WithSimpleDoubleValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("longitude gt 1.5");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(2);
            result[1].Id.ShouldBe(3);
        }
        
        [Test]
        public void Filter_WithSimpleDateValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("date ne '2011-01-02'");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(2);
            result[0].Id.ShouldBe(1);
            result[1].Id.ShouldBe(3);
        }
        
        [Test]
        public void Filter_WithSimpleDateTimeValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("date gt '2011-01-02 00:00:01'");
            
            // act
            var result = parser.Filter(new EnumerableQuery<JoggingData>(_joggingDataList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(1);
            result[0].Id.ShouldBe(3);
        }

        [Test]
        public void Filter_WithSimpleStringValue_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<User>("username eq 'Asd'");
            
            // act
            var result = parser.Filter(new EnumerableQuery<User>(_userList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(1);
            result[0].Id.ShouldBe(1);
        }
        
        [Test]
        public void Filter_WithSimpleStringValueWithSpaces_ShouldFilterItems()
        {
            // arrange
            var parser = new FilterParser<User>("username eq 'Asd Zxc Qwe'");
            
            // act
            var result = parser.Filter(new EnumerableQuery<User>(_userList)).ToArray();
            
            // arrange
            result.Length.ShouldBe(1);
            result[0].Id.ShouldBe(2);
        }

        [Test]
        public void Filter_WithWrongParenthesis_ShouldThrowException()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("(distance gt 9))");
            
            // act
            Should.Throw<FilterException>(() => parser.Filter(new EnumerableQuery<JoggingData>(new JoggingData[0])));

            // assert

        }
        
        [Test]
        public void Filter_WithWrongExpressionDefinition_ShouldThrowException()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("distance gt 9 and");
            
            // act
            Should.Throw<FilterException>(() => parser.Filter(new EnumerableQuery<JoggingData>(new JoggingData[0])));

            // assert

        }
        
        [Test]
        public void Filter_WithWrongOperator_ShouldThrowException()
        {
            // arrange
            var parser = new FilterParser<JoggingData>("distance qw 9");
            
            // act
            Should.Throw<FilterException>(() => parser.Filter(new EnumerableQuery<JoggingData>(new JoggingData[0])));

            // assert

        }
        
        [Test]
        public void Filter_WithWrongField_ShouldThrowException()
        {
            // arrange
            var parser = new FilterParser<User>("password ne '123'");
            
            // act
            Should.Throw<FilterException>(() => parser.Filter(new EnumerableQuery<User>(_userList)));
         
            // assert
        }
    }
}