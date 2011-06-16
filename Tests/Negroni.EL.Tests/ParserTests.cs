using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Negroni.OpenSocial.EL.Elements;

namespace Negroni.OpenSocial.EL.Tests
{
	[TestFixture]
	[TestsOn(typeof(Parser))]
	public class ParserTests
	{
	
		private class TestToken{

			public static TestToken OpenParenthesisInstance = new TestToken("(", TokenType.OpenParenthesis);
			public static TestToken CloseParenthesisInstance = new TestToken(")", TokenType.CloseParenthesis);
			public static TestToken OpenBracketInstance = new TestToken("[", TokenType.OpenBracket);
			public static TestToken CloseBracketInstance = new TestToken("]", TokenType.CloseBracket);
			public static TestToken DotInstance = new TestToken(".", TokenType.Dot);
			public static TestToken TernaryIfInstance = new TestToken("?", TokenType.TernaryIf);
			public static TestToken TernaryElseInstance = new TestToken(":", TokenType.TernaryElse);
		
			public TestToken(string name, TokenType tokenType, IList<IList<TestToken>> parms ){
				Name = name;
				Parms = parms;
				TokenType = tokenType;
				
			}
		
			public TestToken(string name, TokenType tokenType) : this(name, tokenType,  new List<IList<TestToken>>()){
			}
			
			//private TestToken(){
			//    Parms = new List<IList<TestToken>>();
			//}
			
			public string Name{get; set;}
			public IList<IList<TestToken>> Parms{get; private set;}
			public TokenType TokenType{get; set;}
		}
	
		private static void ValidateTokens(IList<TestToken> expectedTokens, IList<Element> tokens, string expression){
			Assert.AreEqual(expectedTokens.Count, tokens.Count, 
				string.Format("Invalid number of tokens for expression {0}" , expression));

			for (int j = 0; j < expectedTokens.Count; j++)
			{
				Assert.AreEqual(expectedTokens[j].Name, tokens[j].ToString(), 
					string.Format("Invalid number of tokens for expression {0}", expression));
				
				Assert.AreEqual(expectedTokens[j].TokenType, tokens[j].Type, 
					string.Format("Invalid type for expression {0}", expression));
				
				if (expectedTokens[j].TokenType == TokenType.Function){
					ElementFunction elementFunction = (ElementFunction)tokens[j];
					Assert.AreEqual(expectedTokens[j].Parms.Count, elementFunction.Parameters.Count);
					for (int k = 0; k < expectedTokens[j].Parms.Count; k++){
						ValidateTokens(expectedTokens[j].Parms[k], elementFunction.Parameters[k], expression);
					}
				}
			}
		}
	
		private static void TestParsing(string expression, IList<TestToken> expectedTokens){
			IList<Element> output = null;
			try{
				output = Parser.Parse(expression);
			}
			catch (ELException){
				Assert.Fail(string.Format("No ELException expected for expression {0}", expression));
			}
			catch (Exception e){
				Console.WriteLine(e);
				
				Assert.Fail(string.Format("No Exception expected for expression {0}", expression));
			}
			ValidateTokens(expectedTokens, output, expression);
		}
	
		[Test]
		public void LiteralsParsingTest(){

			TestParsing("true", new List<TestToken> { new TestToken("true", TokenType.BooleanLiteral) });
			TestParsing("false", new List<TestToken> { new TestToken("false", TokenType.BooleanLiteral) });
			TestParsing("1", new List<TestToken> { new TestToken("1", TokenType.IntegerLiteral) });
			TestParsing("1.0", new List<TestToken> { new TestToken("1", TokenType.IntegerLiteral) });
			TestParsing("1.1", new List<TestToken> { new TestToken("1.1", TokenType.DecimalLiteral) });
			TestParsing("1.2e1", new List<TestToken> { new TestToken("12", TokenType.IntegerLiteral) });
			TestParsing("1.2e+1", new List<TestToken> { new TestToken("12", TokenType.IntegerLiteral) });
			TestParsing("1.2e-1", new List<TestToken> { new TestToken("0.12", TokenType.DecimalLiteral) });
			TestParsing("1.23456789e1", new List<TestToken> { new TestToken("12.3456789", TokenType.DecimalLiteral) });
			TestParsing("1.2345678912e-1", new List<TestToken> { new TestToken("0.1234567891", TokenType.DecimalLiteral) });
			TestParsing("\"\"", new List<TestToken> { new TestToken("", TokenType.StringLiteral) });
			TestParsing("''", new List<TestToken> { new TestToken("", TokenType.StringLiteral) });
			TestParsing("\"a\"", new List<TestToken> { new TestToken("a", TokenType.StringLiteral) });
			TestParsing("\"'\"", new List<TestToken> { new TestToken("'", TokenType.StringLiteral) });
			TestParsing("\"\\\"\"", new List<TestToken> { new TestToken("\"", TokenType.StringLiteral) });
			TestParsing("'\\''", new List<TestToken> { new TestToken("'", TokenType.StringLiteral) });
		}

		[Test]
		public void VariableParsingTest()
		{
			TestParsing("one", new List<TestToken> { 
				new TestToken("one", TokenType.Variable) });
			TestParsing("one.two", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.DotInstance, 
				new TestToken("two", TokenType.Variable) });
			TestParsing("a.b.c", new List<TestToken> { 
				new TestToken("a", TokenType.Variable), 
				TestToken.DotInstance, 
				new TestToken("b", TokenType.Variable), 
				TestToken.DotInstance, 
				new TestToken("c", TokenType.Variable) });
			TestParsing("one[two]", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance,
				new TestToken("two", TokenType.Variable), 
				TestToken.CloseBracketInstance });
			TestParsing("one[(two)]", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance, 
				TestToken.OpenParenthesisInstance, 
				new TestToken("two", TokenType.Variable), 
				TestToken.CloseParenthesisInstance, 
				TestToken.CloseBracketInstance });
			TestParsing("one[true]", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance, 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.CloseBracketInstance });
			TestParsing("one[1]", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.CloseBracketInstance });
			TestParsing("one[1].new", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.CloseBracketInstance, 
				TestToken.DotInstance, 
				new TestToken("new", TokenType.Variable) });
			TestParsing("one[\"test\"]", new List<TestToken> { 
				new TestToken("one", TokenType.Variable), 
				TestToken.OpenBracketInstance, 
				new TestToken("test", TokenType.StringLiteral), 
				TestToken.CloseBracketInstance });
		}

		[Test]
		public void NumericBinaryOperatorsParsingTest()
		{
			TestParsing("1+1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("+", TokenType.BinaryOperator) });
			TestParsing("1-1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("-", TokenType.BinaryOperator) });
			TestParsing("1*1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("*", TokenType.BinaryOperator) });
			TestParsing("1/1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("/", TokenType.BinaryOperator) });
			TestParsing("1 div 1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("/", TokenType.BinaryOperator) });
			TestParsing("1%1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("%", TokenType.BinaryOperator) });
			TestParsing("1 mod 1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("%", TokenType.BinaryOperator) });
			TestParsing("1 mod (1)", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.OpenParenthesisInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("%", TokenType.BinaryOperator) });
		}

		[Test]
		public void EqualityBinaryOperatorsParsingTest()
		{
			TestParsing("true==false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("==", TokenType.BinaryOperator) });
			TestParsing("true eq false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("==", TokenType.BinaryOperator) });
			TestParsing("true != false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("!=", TokenType.BinaryOperator) });
			TestParsing("true ne false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("!=", TokenType.BinaryOperator) });

			TestParsing("1 > 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken(">", TokenType.BinaryOperator) });
			TestParsing("1 gt 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken(">", TokenType.BinaryOperator) });

			TestParsing("1 >= 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken(">=", TokenType.BinaryOperator) });
			TestParsing("1 ge 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral),
				new TestToken(">=", TokenType.BinaryOperator) });

			TestParsing("1 < 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("<", TokenType.BinaryOperator) });
			TestParsing("1 lt 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("<", TokenType.BinaryOperator) });

			TestParsing("1 <= 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("<=", TokenType.BinaryOperator) });
			TestParsing("1 le 2", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("<=", TokenType.BinaryOperator) });
		}

		[Test]
		public void LogicalBinaryOperatorsParsingTest()
		{
			TestParsing("true&&false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("&&", TokenType.BinaryOperator) });
			TestParsing("true and false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral),
				new TestToken("&&", TokenType.BinaryOperator) });
			TestParsing("true and(false)", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.OpenParenthesisInstance, 
				new TestToken("false", TokenType.BooleanLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("&&", TokenType.BinaryOperator) });

			TestParsing("true||false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("||", TokenType.BinaryOperator) });
			TestParsing("true or false", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("||", TokenType.BinaryOperator) });
			TestParsing("true or(false)", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.OpenParenthesisInstance, 
				new TestToken("false", TokenType.BooleanLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("||", TokenType.BinaryOperator) });

		}

		[Test]
		public void UnitaryOperatorsParsingTest()
		{
			TestParsing("-1", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("-", TokenType.UnitaryOperator) });
			TestParsing("-(1)", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("-", TokenType.UnitaryOperator) });
			TestParsing("2+-1", new List<TestToken> { 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("-", TokenType.UnitaryOperator), 
				new TestToken("+", TokenType.BinaryOperator) });

			TestParsing("!true", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("!", TokenType.UnitaryOperator) });
			TestParsing("!(true)", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("!", TokenType.UnitaryOperator) });
			TestParsing("false == !true", new List<TestToken> { 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("!", TokenType.UnitaryOperator), 
				new TestToken("==", TokenType.BinaryOperator) });

			TestParsing("not true", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("!", TokenType.UnitaryOperator) });
			TestParsing("not(true)", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("!", TokenType.UnitaryOperator) });
			TestParsing("false == not true", new List<TestToken> { 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("!", TokenType.UnitaryOperator), 
				new TestToken("==", TokenType.BinaryOperator) });


		}

		[Test]
		public void FunctionsParsingTest()
		{

			IList<TestToken> tokens;
			TestToken token;
			IList<IList<TestToken>> parms;
			IList<TestToken> param;

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("true", TokenType.BooleanLiteral));
			parms.Add(param);
			
			token = new TestToken("empty", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("empty(true)", tokens);
			TestParsing("empty (true)", tokens);
			
			
			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("true", TokenType.BooleanLiteral));
			parms.Add(param);

			token = new TestToken("toNumber", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("toNumber(true)", tokens);
			TestParsing("toNumber (true)", tokens);


			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("true", TokenType.BooleanLiteral));
			parms.Add(param);

			token = new TestToken("toString", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("toString(true)", tokens);
			TestParsing("toString (true)", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("true", TokenType.BooleanLiteral));
			parms.Add(param);

			token = new TestToken("toBoolean", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("toBoolean(true)", tokens);
			TestParsing("toBoolean (true)", tokens);
			
			
			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("1.1", TokenType.DecimalLiteral));
			parms.Add(param);

			token = new TestToken("mathRound", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("mathRound(1.1)", tokens);
			TestParsing("mathRound (1.1)", tokens);


			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("1.5", TokenType.DecimalLiteral));
			parms.Add(param);

			token = new TestToken("mathFloor", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("mathFloor(1.5)", tokens);
			TestParsing("mathFloor (1.5)", tokens);


			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("1.5", TokenType.DecimalLiteral));
			parms.Add(param);

			token = new TestToken("mathCeil", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("mathCeil(1.5)", tokens);
			TestParsing("mathCeil (1.5)", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("text", TokenType.StringLiteral));
			parms.Add(param);

			token = new TestToken("os:htmlEncode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("os:htmlEncode(\"text\")", tokens);
			TestParsing("os:htmlEncode (\"text\")", tokens);

			token = new TestToken("htmlEncode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("htmlEncode(\"text\")", tokens);
			TestParsing("htmlEncode (\"text\")", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("text", TokenType.StringLiteral));
			parms.Add(param);
			
			token = new TestToken("os:htmlDecode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("os:htmlDecode(\"text\")", tokens);
			TestParsing("os:htmlDecode (\"text\")", tokens);

			token = new TestToken("htmlDecode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("htmlDecode(\"text\")", tokens);
			TestParsing("htmlDecode (\"text\")", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("text", TokenType.StringLiteral));
			parms.Add(param);

			token = new TestToken("os:urlEncode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("os:urlEncode(\"text\")", tokens);
			TestParsing("os:urlEncode (\"text\")", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("text", TokenType.StringLiteral));
			parms.Add(param);

			token = new TestToken("os:urlDecode", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("os:urlDecode(\"text\")", tokens);
			TestParsing("os:urlDecode (\"text\")", tokens);

			tokens = new List<TestToken>();
			parms = new List<IList<TestToken>>();
			param = new List<TestToken>();
			param.Add(new TestToken("text", TokenType.StringLiteral));
			parms.Add(param);

			token = new TestToken("os:jsStringEscape", TokenType.Function, parms);
			tokens.Add(token);
			TestParsing("os:jsStringEscape(\"text\")", tokens);
			TestParsing("os:jsStringEscape (\"text\")", tokens);
		}


		[Test]
		public void TernaryOperatorsParsingTest()
		{
			TestParsing("true?1:2", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.TernaryIfInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.TernaryElseInstance, 
				new TestToken("2", TokenType.IntegerLiteral) });
			TestParsing("true == false?1+4:2-5", new List<TestToken> { 
				new TestToken("true", TokenType.BooleanLiteral), 
				new TestToken("false", TokenType.BooleanLiteral), 
				new TestToken("==", TokenType.BinaryOperator),
				TestToken.TernaryIfInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("4", TokenType.IntegerLiteral), 
				new TestToken("+", TokenType.BinaryOperator), 
				TestToken.TernaryElseInstance, 
				new TestToken("2", TokenType.IntegerLiteral), 
				new TestToken("5", TokenType.IntegerLiteral), 
				new TestToken("-", TokenType.BinaryOperator) });
			TestParsing("1+(true?1:2)", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.OpenParenthesisInstance, 
				new TestToken("true", TokenType.BooleanLiteral), 
				TestToken.TernaryIfInstance, 
				new TestToken("1", TokenType.IntegerLiteral), 
				TestToken.TernaryElseInstance, 
				new TestToken("2", TokenType.IntegerLiteral), 
				TestToken.CloseParenthesisInstance, 
				new TestToken("+", TokenType.BinaryOperator) });
		}

		[Test]
		public void VerifyVarialbesThatStartWithSameLetterAsOperatorsTest()
		{
			string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", 
				"o","p","q","r","s","t","u","v","w","x","y","z"};
			foreach (string letter in letters)
			{
				TestParsing(letter, new List<TestToken> { new TestToken(letter, TokenType.Variable) });
			}
		}

		[Test]
		public void ParenthesisParsingTest()
		{
			TestParsing("(one)", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				new TestToken("one", TokenType.Variable), 
				TestToken.CloseParenthesisInstance });
			TestParsing("((one))", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				TestToken.OpenParenthesisInstance, 
				new TestToken("one", TokenType.Variable), 
				TestToken.CloseParenthesisInstance, 
				TestToken.CloseParenthesisInstance });
			TestParsing("(one + 1)", new List<TestToken> { 
				TestToken.OpenParenthesisInstance, 
				new TestToken("one", TokenType.Variable), 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("+", TokenType.BinaryOperator), 
				TestToken.CloseParenthesisInstance });
		}

		[Test]
		public void PrecedenenceTest()
		{
			TestParsing("1 + 3 * 4", new List<TestToken> { 
				new TestToken("1", TokenType.IntegerLiteral), 
				new TestToken("3", TokenType.IntegerLiteral), 
				new TestToken("4", TokenType.IntegerLiteral), 
				new TestToken("*", TokenType.BinaryOperator), 
				new TestToken("+", TokenType.BinaryOperator) });
		}
	}
}
