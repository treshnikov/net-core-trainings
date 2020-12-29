using System.Linq;
using BusinessLogic.FilterParser;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.FilterParser
{
    [TestFixture]
    public class TokenizerTest
    {
        [Test]
        public void Tokenize_WithSimpleIntValue_ShouldParseIt()
        {
            // arrange
            // act
            var tokens = Tokenizer.Tokenize("       distance       eq       5       ");

            // assert
            tokens.Count.ShouldBe(3);
            tokens.ShouldSatisfyAllConditions(
                () => tokens.ElementAt(0).Ident.ShouldBe("distance"),
                () => tokens.ElementAt(1).Ident.ShouldBe("eq"),
                () => tokens.ElementAt(2).Ident.ShouldBe("5"));
        }
        
        [Test]
        public void Tokenize_WithParenthesisAndDoubleValue_ShouldParseIt()
        {
            // arrange
            // act
            var tokens = Tokenizer.Tokenize("(longitude eq 5.25)").ToArray();

            // assert
            tokens.Length.ShouldBe(5);
            tokens.ShouldSatisfyAllConditions(
                () => tokens[0].Ident.ShouldBe("("),
                () => tokens[1].Ident.ShouldBe("longitude"),
                () => tokens[2].Ident.ShouldBe("eq"),
                () => tokens[3].Ident.ShouldBe("5.25"),
                () => tokens[4].Ident.ShouldBe(")"));
        }
        
        [Test]
        public void Tokenize_WithDateTimeValue_ShouldParseIt()
        {
            // arrange
            // act
            var tokens = Tokenizer.Tokenize("(time eq '2011-01-01 05:00:00')").ToArray();

            // assert
            tokens.Length.ShouldBe(5);
            tokens.ShouldSatisfyAllConditions(
                () => tokens[0].Ident.ShouldBe("("),
                () => tokens[1].Ident.ShouldBe("time"),
                () => tokens[2].Ident.ShouldBe("eq"),
                () => tokens[3].Ident.ShouldBe("'2011-01-01 05:00:00'"),
                () => tokens[4].Ident.ShouldBe(")"));
        }
        
        [Test]
        public void Tokenize_WithStringValue_ShouldParseIt()
        {
            // arrange
            // act
            var tokens = Tokenizer.Tokenize("(username eq 'asd zxc')").ToArray();

            // assert
            tokens.Length.ShouldBe(5);
            tokens.ShouldSatisfyAllConditions(
                () => tokens[0].Ident.ShouldBe("("),
                () => tokens[1].Ident.ShouldBe("username"),
                () => tokens[2].Ident.ShouldBe("eq"),
                () => tokens[3].Ident.ShouldBe("'asd zxc'"),
                () => tokens[4].Ident.ShouldBe(")"));
        }

        [Test]
        public void Tokenize_WithEmptyParameter_ShouldReturnEmptyQueue()
        {
            // arrange
            // act
            var tokens = Tokenizer.Tokenize(null).ToArray();

            // assert
            tokens.Length.ShouldBe(0);
        }
    }
}