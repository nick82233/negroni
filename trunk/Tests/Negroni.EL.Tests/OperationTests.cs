using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Negroni.OpenSocial.EL.Elements;
using System.Collections;
using Negroni.OpenSocial.EL.Operators;

namespace Negroni.OpenSocial.EL.Test
{
	[TestFixture]
	[TestsOn(typeof(Operator))]
	public class OperationTests
	{

		private static readonly IElementValue[] TestCases = new IElementValue[]{
			new ElementLiteral(TokenType.IntegerLiteral, 1),
			new ElementLiteral(TokenType.IntegerLiteral, 0),
			new ElementLiteral(TokenType.DecimalLiteral, 1.1d),
			new ElementLiteral(TokenType.IntegerLiteral ,1),
			new ElementLiteral(TokenType.IntegerLiteral, -1),
			new ElementObject(null),
			new ElementLiteral(TokenType.StringLiteral, ""),
			new ElementLiteral(TokenType.StringLiteral, "a"),
			new ElementLiteral(TokenType.StringLiteral, "0"),
			new ElementLiteral(TokenType.StringLiteral, "1"),
			new ElementLiteral(TokenType.BooleanLiteral, true),
			new ElementLiteral(TokenType.BooleanLiteral, false),
			new ElementLiteral(TokenType.DecimalLiteral, double.PositiveInfinity),
			new ElementLiteral(TokenType.DecimalLiteral, double.NegativeInfinity),
			new ElementObject(new object()),
			new ElementObject(new object[]{}),
		};

		private static void TestUnitaryOperator(UnitaryOperator unitaryOperator, IElementValue val, object exptectedValue){
			bool error = false;
			IElementValue value = null;
			try
			{
				value = unitaryOperator.Apply(val);
			}
			catch (ELOperationException)
			{
				if (!"error".Equals(exptectedValue))
				{
					Assert.Fail(
						string.Format("Exception throw when not expected for operation {0} with value {1} [{2}]",
						unitaryOperator.OperatorType, val.Type, val.Value ?? "null"));
				}
				error = true;
			}
			catch (Exception e)
			{
				Assert.Fail(
						string.Format("Invalid exception type throw for operation {0} with value {1} [{2}].\r\n{3}",
						unitaryOperator.OperatorType, val.Type, val.Value ?? "null"));
			}

			if ("error".Equals(exptectedValue))
			{
				Assert.IsTrue(error,
					string.Format("Error expected for operation {0} with value left {1} [{2}] but not error throwed",
					unitaryOperator.OperatorType, val.Type, val.Value ?? "null"));
			}
			else
			{
				Assert.AreEqual(value.Value, exptectedValue,
					string.Format("Expected [{0}] and actual [{1}] values do not match for operation {2} with value {3} [{4}]",
					exptectedValue, value.Value, unitaryOperator.OperatorType, val.Type,  val.Value ?? "null"));
			}
			
		}

		private static void TestFunctionOperator(FunctionOperator functionOperator, IElementValue[] val, object exptectedValue)
		{
			StringBuilder pars = new StringBuilder();
			
			foreach (IElementValue item in val){
				pars.AppendFormat("{0} [{1}] ", item.Type, item.Value ?? "null");
			}
			
			bool error = false;
			IElementValue value = null;
			try
			{
				value = functionOperator.Apply(val);
			}
			catch (ELOperationException)
			{
				if (!"error".Equals(exptectedValue))
				{
					Assert.Fail(
						string.Format("Exception throw when not expected for function {0} with values ({1})",
						functionOperator.OperatorType, pars));
				}
				error = true;
			}
			catch (Exception)
			{
				Assert.Fail(
						string.Format("Invalid exception type throw for operation {0} with values ({1})",
						functionOperator.OperatorType, pars));
			}

			if ("error".Equals(exptectedValue))
			{
				Assert.IsTrue(error,
					string.Format("Error expected for operation {0} but not error throwed with values ({1})",
					functionOperator.OperatorType, pars));
			}
			else
			{
				Assert.AreEqual(value.Value, exptectedValue,
					string.Format("Expected [{0}] and actual [{1}] values do not match for operation {2} with values ({3})",
					exptectedValue, value.Value, functionOperator.OperatorType, pars));
			}

		}

		private static void TestBinaryOperator(BinaryOperator binaryOperator, IElementValue left, IElementValue right, object exptectedValue){
			bool error = false;
			IElementValue value = null;
			try{
				value = binaryOperator.Apply(left, right);
			}
			catch (ELOperationException){
				if (!"error".Equals(exptectedValue)){
					Assert.Fail(
						string.Format("Exception throw when not expected for operation {0} with values left {3} [{1}] right {4} [{2}]", 
						binaryOperator.OperatorType, left.Value??"null", right.Value??"null", left.Type, right.Type));
				}
				error = true;
			}
			catch(Exception e){
				Assert.Fail(
						string.Format("Invalid exception type throw for operation {0} with values left {4} [{1}] right {5} [{2}].\r\n{3}",
						binaryOperator.OperatorType, left.Value == null?"null":left.Value, right.Value == null? "null": right.Value, 
						e, left.Type, right.Type));
			}
			
			if ("error".Equals(exptectedValue)){
				Assert.IsTrue(error, 
					string.Format("Error expected for operation {0} with values left {3} [{1}] right {4} [{2}] but not error throwed", 
					binaryOperator.OperatorType, left.Value==null?"null":left.Value, right.Value==null?"null":right.Value, left.Type, right.Type));
			}
			else{
				Assert.AreEqual(value.Value, exptectedValue, 
					string.Format("Expected [{0}] and actual  [{1}] values do not match for operation {2} with values left {5} [{3}] right {6} [{4}]",
					exptectedValue, value.Value, binaryOperator.OperatorType, left.Value == null?"null":left.Value, right.Value == null?"null":right.Value,
					left.Type, right.Type));
			}
		}
		
		[Test]
		public void AdditionTest(){
			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { 2, 1, 2.1, 2, 0, 1, "1", "1a", "10", "11", 2, 1, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "0", "0a", "00", "01", 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 2.1, 1.1, 2.2, 2.1, 0.1, 1.1, "1.1", "1.1a", "1.10", "1.11", 2.1, 1.1, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 2, 1, 2.1, 2, 0, 1, "1", "1a", "10", "11", 2, 1, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, -1, 0.1, 0, -2, -1, "-1", "-1a", "-10", "-11", 0, -1, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "null", "nulla", "null0", "null1", 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { "1", "0", "1.1", "1", "-1", "null", "", "a", "0", "1", "true", "false", "Infinity", "-Infinity", "error", "error" });
			expectedValues.Add(new object[] { "a1", "a0", "a1.1", "a1", "a-1", "anull", "a", "aa", "a0", "a1", "atrue", "afalse", "aInfinity", "a-Infinity", "error", "error" });
			expectedValues.Add(new object[] { "01", "00", "01.1", "01", "0-1", "0null", "0", "0a", "00", "01", "0true", "0false", "0Infinity", "0-Infinity", "error", "error" });
			expectedValues.Add(new object[] { "11", "10", "11.1", "11", "1-1", "1null", "1", "1a", "10", "11", "1true", "1false", "1Infinity", "1-Infinity", "error", "error" });
			expectedValues.Add(new object[] { 2, 1, 2.1, 2, 0, 1, "true", "truea", "true0", "true1", 2, 1, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "false", "falsea", "false0", "false1", 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, "Infinity", "Infinitya", "Infinity0", "Infinity1", double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, "error", "error", "error" });
			expectedValues.Add(new object[] { double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, "-Infinity", "-Infinitya", "-Infinity0", "-Infinity1", double.NegativeInfinity, double.NegativeInfinity, "error", double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++){
				for (int j = 0; j < TestCases.Length; j++){
					TestBinaryOperator(AdditionOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void SubstractionTest()
		{
			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { 0, 1, -0.1, 0, 2, 1, "error", "error", 1, 0, 0, 1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { -1, 0, -1.1, -1, 1, 0, "error", "error", 0, -1, -1, 0, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0.1, 1.1, 0, 0.1, 2.1, 1.1, "error", "error", 1.1, 0.1, 0.1, 1.1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 1, -0.1, 0, 2, 1, "error", "error", 1, 0, 0, 1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { -2, -1, -2.1, -2, 0, -1, "error", "error", -1, -2, -2, -1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { -1, 0, -1.1, -1, 1, 0, "error", "error", 0, -1, -1, 0, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { -1, 0, -1.1, -1, 1, 0, "error", "error", 0, -1, -1, 0, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 1, -0.1, 0, 2, 1, "error", "error", 1, 0, 0, 1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 1, -0.1, 0, 2, 1, "error", "error", 1, 0, 0, 1, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { -1, 0, -1.1, -1, 1, 0, "error", "error", 0, -1, -1, 0, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, "error", "error", double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, "error", double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, "error", "error", double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(SubtractionOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}


		[Test]
		public void MultiplicationTest()
		{
		
			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "error", "error", 0, 1, 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 0, 0, 0, 0, 0, "error", "error", 0, 0, 0, 0, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { 1.1, 0, 1.21, 1.1, -1.1, 0, "error", "error", 0, 1.1, 1.1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "error", "error", 0, 1, 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { -1, 0, -1.1, -1, 1, 0, "error", "error", 0, -1, -1, 0, double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 0, 0, 0, 0, 0, "error", "error", 0, 0, 0, 0, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { 0, 0, 0, 0, 0, 0, "error", "error", 0, 0, 0, 0, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "error", "error", 0, 1, 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 1, 0, 1.1, 1, -1, 0, "error", "error", 0, 1, 1, 0, double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { 0, 0, 0, 0, 0, 0, "error", "error", 0, 0, 0, 0, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { double.PositiveInfinity, "error", double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, "error", "error", "error", "error", double.PositiveInfinity, double.PositiveInfinity, "error", double.PositiveInfinity, double.NegativeInfinity, "error", "error" });
			expectedValues.Add(new object[] { double.NegativeInfinity, "error", double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, "error", "error", "error", "error", double.NegativeInfinity, double.NegativeInfinity, "error", double.NegativeInfinity, double.PositiveInfinity, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(MultiplicationOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void DivisionTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { 1, double.PositiveInfinity, 1d / 1.1d, 1, -1, double.PositiveInfinity, "error", "error", double.PositiveInfinity, 1, 1, double.PositiveInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 1.1, double.PositiveInfinity, 1, 1.1, -1.1, double.PositiveInfinity, "error", "error", double.PositiveInfinity, 1.1, 1.1, double.PositiveInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 1, double.PositiveInfinity, 1d / 1.1d, 1, -1, double.PositiveInfinity, "error", "error", double.PositiveInfinity, 1, 1, double.PositiveInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { -1, double.NegativeInfinity, -1d / 1.1d, -1, 1, double.NegativeInfinity, "error", "error", double.NegativeInfinity, -1, -1, double.NegativeInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 1, double.PositiveInfinity, 1d / 1.1d, 1, -1, double.PositiveInfinity, "error", "error", double.PositiveInfinity, 1, 1, double.PositiveInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 1, double.PositiveInfinity, 1d / 1.1d, 1, -1, double.PositiveInfinity, "error", "error", double.PositiveInfinity, 1, 1, double.PositiveInfinity, 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity, "error", "error", double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.NegativeInfinity, "error", "error", double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });

			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(DivisionOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void ModuloTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { 0, "error", 1, 0, 0, "error", "error", "error", "error", 0, 0, "error", 1, 1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 0.1, "error", 0, 0.1, 0.1, "error", "error", "error", "error", 0.1, 0.1, "error", 1.1, 1.1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 1, 0, 0, "error", "error", "error", "error", 0, 0, "error", 1, 1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", -1, 0, 0, "error", "error", "error", "error", 0, 0, "error", -1, -1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 1, 0, 0, "error", "error", "error", "error", 0, 0, "error", 1, 1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 1, 0, 0, "error", "error", "error", "error", 0, 0, "error", 1, 1, "error", "error" });
			expectedValues.Add(new object[] { 0, "error", 0, 0, 0, "error", "error", "error", "error", 0, 0, "error", 0, 0, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });


			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(ModuloOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void EqualsTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false });
			expectedValues.Add(new object[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true });

			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(EqualsOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
			
			//Diferent objects and arrays test
			
			TestBinaryOperator(EqualsOperator.Instance, new ElementObject(new object()), new ElementObject(new object()), false);
			TestBinaryOperator(EqualsOperator.Instance, new ElementObject(new object[] { 1 }), new ElementObject(new object[] { 1 }), false);
		}

		[Test]
		public void NotEqualsTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { false, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { false, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true });
			expectedValues.Add(new object[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false });


			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(NotEqualsOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}

			TestBinaryOperator(NotEqualsOperator.Instance, new ElementObject(new object()), new ElementObject(new object()), true);
			TestBinaryOperator(NotEqualsOperator.Instance, new ElementObject(new object[] { 1 }), new ElementObject(new object[] { 1 }), true);
		}

		[Test]
		public void LessThanTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { false, false, true, false, false, "error", false, true, false, false, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, false, true, true, false, "error", false, true, false, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, true, false, false, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { false, false, true, false, false, "error", false, true, false, false, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, false, "error", false, true, true, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", false, true, true, true, true, true, true, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, false, false, false, true, true, false, false, "error", "error" });
			expectedValues.Add(new object[] { true, false, true, true, false, "error", false, true, false, true, true, true, true, false, "error", "error" });
			expectedValues.Add(new object[] { false, false, true, false, false, "error", false, true, false, false, true, true, true, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", false, false, false, false, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", false, false, false, false, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, true, false, false, "error", "error", false, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", false, true, true, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(LessThanOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void LessThanEqualsTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { true, false, true, true, false, "error", false, true, false, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, false, "error", false, true, true, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { false, false, true, false, false, "error", false, true, false, false, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, false, true, true, false, "error", false, true, false, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", false, true, true, true, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, true, true, true, true, true, true, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, true, false, false, true, true, false, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, false, "error", false, true, true, true, true, true, true, false, "error", "error" });
			expectedValues.Add(new object[] { true, false, true, true, false, "error", false, true, false, true, true, true, true, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", false, false, false, false, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", false, false, false, false, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, true, false, false, "error", "error", true, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", false, true, true, true, "error", "error", true, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(LessThanEqualsOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void GreaterThanTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { false, true, false, false, true, "error", true, false, true, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, true, "error", true, false, false, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { true, true, false, true, true, "error", true, false, true, true, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, true, false, false, true, "error", true, false, true, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", true, false, false, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", false, false, false, false, false, false, false, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, false, true, true, false, false, true, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, true, "error", true, false, false, false, false, false, false, true, "error", "error" });
			expectedValues.Add(new object[] { false, true, false, false, true, "error", true, false, true, false, false, false, false, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", true, true, true, true, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", true, true, true, true, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, false, true, true, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", true, false, false, false, "error", "error", false, false, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(GreaterThanOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}


		[Test]
		public void GreaterThanEquals()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { true, true, false, true, true, "error", true, false, true, true, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, true, false, false, true, "error", true, false, true, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, false, true, true, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { true, true, false, true, true, "error", true, false, true, true, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, true, "error", true, false, false, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", true, false, false, false, false, false, false, false, "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, true, true, true, false, false, true, true, "error", "error" });
			expectedValues.Add(new object[] { false, true, false, false, true, "error", true, false, true, false, false, false, false, true, "error", "error" });
			expectedValues.Add(new object[] { true, true, false, true, true, "error", true, false, true, true, false, false, false, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", true, true, true, true, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", true, true, true, true, "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { true, true, true, true, true, "error", true, false, true, true, "error", "error", true, true, "error", "error" });
			expectedValues.Add(new object[] { false, false, false, false, false, "error", true, false, false, false, "error", "error", false, true, "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(GreaterThanEqualsOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void AndTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", true, false, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", false, false, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(AndOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}

		[Test]
		public void OrTest()
		{

			IList<object[]> expectedValues = new List<object[]>();
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", true, true, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", true, false, "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });
			expectedValues.Add(new object[] { "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error", "error" });



			for (int i = 0; i < TestCases.Length; i++)
			{
				for (int j = 0; j < TestCases.Length; j++)
				{
					TestBinaryOperator(OrOperator.Instance, TestCases[i], TestCases[j], expectedValues[i][j]);
				}
			}
		}
		
		[Test]
		public void EmptyTest(){
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(true);
			expectedValues.Add(true);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(false);
			expectedValues.Add(true);

			for (int i = 0; i < TestCases.Length; i++)
			{
				TestFunctionOperator(EmptyOperator.Instance, new IElementValue[]{ TestCases[i] }, expectedValues[i]);
			}

			TestFunctionOperator(EmptyOperator.Instance, new IElementValue[] { new ElementObject(new object[] { 1 }) }, false);
			TestFunctionOperator(EmptyOperator.Instance, new IElementValue[] { new ElementObject(new List<object>(new object[] { 1 }))}, false);
			TestFunctionOperator(EmptyOperator.Instance, new IElementValue[] { new ElementObject(new List<object>(new object[] { })) }, true);
		}

		[Test]
		public void NegativeTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add(-1.1);
			expectedValues.Add(-1);
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(0);
			expectedValues.Add(-1);
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add("error");
			expectedValues.Add("error");



			for (int i = 0; i < TestCases.Length; i++)
			{
				TestUnitaryOperator(NegativeOperator.Instance, TestCases[i], expectedValues[i]);
			}

		}

		[Test]
		public void NotTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(false);
			expectedValues.Add(true);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");

			for (int i = 0; i < TestCases.Length; i++)
			{
				TestUnitaryOperator(NotOperator.Instance, TestCases[i], expectedValues[i]);
			}

		}

		[Test]
		public void ToNumberTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(1.1);
			expectedValues.Add(1);
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(1);
			expectedValues.Add(1.5);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add("error");
			expectedValues.Add(124);

			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "1a"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "1.5.1"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "Infinity"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "-Infinity"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "a12"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "12.4e1"));

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(ToNumberOperator.Instance, new IElementValue[] { localTestCases[i] }, expectedValues[i]);
			}

		}

		[Test]
		public void ToBooleanTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(true);
			expectedValues.Add(false);
			expectedValues.Add(true);
			expectedValues.Add(true);
			expectedValues.Add(true);
			expectedValues.Add(false);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(true);
			expectedValues.Add(false);
			expectedValues.Add(true);
			expectedValues.Add(true);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(true);
			expectedValues.Add(false);

			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "true"));
			localTestCases.Add(new ElementLiteral(TokenType.StringLiteral, "false"));

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(ToBooleanOperator.Instance, new IElementValue[]{ localTestCases[i] }, expectedValues[i]);
			}

		}

		[Test]
		public void ToStringTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add("1");
			expectedValues.Add("0");
			expectedValues.Add("1.1");
			expectedValues.Add("1");
			expectedValues.Add("-1");
			expectedValues.Add("null");
			expectedValues.Add("");
			expectedValues.Add("a");
			expectedValues.Add("0");
			expectedValues.Add("1");
			expectedValues.Add("true");
			expectedValues.Add("false");
			expectedValues.Add("Infinity");
			expectedValues.Add("-Infinity");
			expectedValues.Add("object");
			expectedValues.Add("array");


			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(ToStringOperator.Instance, new IElementValue[]{ localTestCases[i] }, expectedValues[i]);
			}

		}

		[Test]
		public void ToMathRoundTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(-2);
			expectedValues.Add(-2);
			expectedValues.Add(-1);
			expectedValues.Add(1);
			expectedValues.Add(2);
			expectedValues.Add(2);



			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.7));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.7));

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(MathRoundOperator.Instance, new IElementValue[] { localTestCases[i]}, expectedValues[i]);
			}

		}


		[Test]
		public void ToMathFloorTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(-2);
			expectedValues.Add(-2);
			expectedValues.Add(-2);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(1);



			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.7));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.7));

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(MathFloorOperator.Instance, new IElementValue[]{ localTestCases[i]}, expectedValues[i]);
			}

		}

		[Test]
		public void ToMathCeilTest()
		{
			IList<object> expectedValues = new List<object>();
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(2);
			expectedValues.Add(1);
			expectedValues.Add(-1);
			expectedValues.Add(0);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(0);
			expectedValues.Add(1);
			expectedValues.Add(1);
			expectedValues.Add(0);
			expectedValues.Add(double.PositiveInfinity);
			expectedValues.Add(double.NegativeInfinity);
			expectedValues.Add("error");
			expectedValues.Add("error");
			expectedValues.Add(-1);
			expectedValues.Add(-1);
			expectedValues.Add(-1);
			expectedValues.Add(2);
			expectedValues.Add(2);
			expectedValues.Add(2);



			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.7));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, -1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.2));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.5));
			localTestCases.Add(new ElementLiteral(TokenType.DecimalLiteral, 1.7));

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(MathCeilOperator.Instance, new IElementValue[]{ localTestCases[i]}, expectedValues[i]);
			}

		}

		[Test]
		public void HtmlEncodeTest()
		{
			/*
			 new ElementLiteral(TokenType.IntegerLiteral, 1),
				new ElementLiteral(TokenType.IntegerLiteral, 0),
				new ElementLiteral(TokenType.DecimalLiteral, 1.1d),
				new ElementLiteral(TokenType.IntegerLiteral ,1),
				new ElementLiteral(TokenType.IntegerLiteral, -1),
				new ElementObject(null),
				new ElementLiteral(TokenType.StringLiteral, ""),
				new ElementLiteral(TokenType.StringLiteral, "a"),
				new ElementLiteral(TokenType.StringLiteral, "0"),
				new ElementLiteral(TokenType.StringLiteral, "1"),
				new ElementLiteral(TokenType.BooleanLiteral, true),
				new ElementLiteral(TokenType.BooleanLiteral, false),
				new ElementLiteral(TokenType.DecimalLiteral, double.PositiveInfinity),
				new ElementLiteral(TokenType.DecimalLiteral, double.NegativeInfinity),
				new ElementObject(new object()),
				new ElementObject(new object[]{}),*/

			IList<object> expectedValues = new List<object>();
			expectedValues.Add("1");
			expectedValues.Add("0");
			expectedValues.Add("1.1");
			expectedValues.Add("1");
			expectedValues.Add("-1");
			expectedValues.Add("null");
			expectedValues.Add("");
			expectedValues.Add("a");
			expectedValues.Add("0");
			expectedValues.Add("1");
			expectedValues.Add("true");
			expectedValues.Add("false");
			expectedValues.Add("Infinity");
			expectedValues.Add("-Infinity");
			expectedValues.Add("error");
			expectedValues.Add("error");


			IList<IElementValue> localTestCases = new List<IElementValue>(TestCases);

			for (int i = 0; i < localTestCases.Count; i++)
			{
				TestFunctionOperator(HtmlEncodeOperator.Instance, new IElementValue[] { localTestCases[i] }, expectedValues[i]);
			}

		}
	}
}
