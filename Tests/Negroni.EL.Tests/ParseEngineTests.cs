using System;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;
using Negroni.OpenSocial.EL;
using MbUnit.Framework;

namespace OpenSocial.EL.Test
{
	[TestFixture]
	[TestsOn(typeof(Engine))]
	public class ParseEngineTests
	{
		/// <summary>
		/// Formats a variable as a key
		/// </summary>
		/// <param name="variable"></param>
		/// <returns></returns>
		string AsKey(string variable)
		{
			return "${" + variable + "}";
		}

		[Test]
		public void GreaterThanOrEqualToIntegerPositiveAssertion()
		{
			string expression = "${MyVariable>=12}";

			string key = "MyVariable";
			string val = "13";
			
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void ListGreaterThanOrEqualToIntegerPositiveAssertion()
		{
			string expression = "${MyVariable[0]>=12}";

			string key = "MyVariable";
			int[] val = { 13 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void GreaterThanOrEqualToIntegerNegativeAssertion()
		{
			string expression = "${MyVariable>=12}";

			string key = "MyVariable";
			string val = "11";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListGreaterThanOrEqualToIntegerNegativeAssertion()
		{
			string expression = "${MyVariable[0]>=12}";

			string key = "MyVariable";
			int[] val = { 11 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void GreaterThanIntegerPositiveAssertion()
		{
			string expression = "${MyVariable>12}";

			string key = "MyVariable";
			string val = "13";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListGreaterThanIntegerPositiveAssertion()
		{
			string expression = "${MyVariable[0]>12}";

			string key = "MyVariable";
			int[] val = { 13 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void GreaterThanIntegerNegativeAssertion()
		{
			string expression = "${MyVariable>12}";

			string key = "MyVariable";
			string val = "11";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListGreaterThanIntegerNegativeAssertion()
		{
			string expression = "${MyVariable[0]>12}";

			string key = "MyVariable";
			int[] val = { 11 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void LessThanOrEqualToIntegerPositiveAssertion()
		{
			string expression = "${MyVariable<=12}";

			string key = "MyVariable";
			string val = "11";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListLessThanOrEqualToIntegerPositiveAssertion()
		{
			string expression = "${MyVariable[0]<=12}";

			string key = "MyVariable";
			int[] val = { 11 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void LessThanOrEqualsIntegerNegativeAssertion()
		{
			string expression = "${MyVariable<=10}";

			string key = "MyVariable";
			string val = "11";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListLessThanOrEqualsIntegerNegativeAssertion()
		{
			string expression = "${MyVariable[0]<=10}";

			string key = "MyVariable";
			int[] val = { 11 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}


		[Test]
		public void EqualsIntegerNegativeAssertion()
		{
			string expression = "${MyVariable==10}";

			string key = "MyVariable";
			string val = "11";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListEqualsIntegerNegativeAssertion()
		{
			string expression = "${MyVariable[0]==10}";

			string key = "MyVariable";
			int[] val = { 11 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void EqualsIntegerPositiveAssertion()
		{
			string expression = "${MyVariable==10}";

			string key = "MyVariable";
			int val = 10;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)resolved_exp.ResolvedValue);
		}

		[Test]
		public void ListEqualsIntegerPositiveAssertion()
		{
			string expression = "${MyVariable[0]==10}";
			string key = "MyVariable";
			int[] val = { 10 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			
			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void EqualsStringNegativeAssertion()
		{
			string expression = "${MyVariable=='asdf'}";

			string key = "MyVariable";
			string val = "asdfsa";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void ListEqualsStringNegativeAssertion()
		{
			string expression = "${MyVariable=='asdf'}";

			string key = "MyVariable";
			string[] val = { "asdfsa" };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.IsFalse((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void EqualsStringPositiveAssertion()
		{
			string expression = "${MyVariable=='asdf'}";

			string key = "MyVariable";
			string val = "asdf";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp =  Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void ListEqualsStringPositiveAssertion()
		{
			string expression = "${MyVariable[0]=='asdf'}";

			string key = "MyVariable";
			string[] val = { "asdf" };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);


			ResolvedExpression resolved_exp =  Engine.ResolveExpression(expression, dc);
			Assert.IsTrue((bool)(resolved_exp.ResolvedValue));
		}

		[Test]
		public void MultiplicationAssertion()
		{
			string expression = "${MyVariable*24}";

			string key = "MyVariable";
			int val = 12;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp =  Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)(resolved_exp.ResolvedValue), (12 * 24));
		}

		[Test]
		public void ListMultiplicationAssertion()
		{
			string expression = "${MyVariable[0]*24}";

			string key = "MyVariable";
			int[] val = { 12 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 * 24));
		}

		[Test]
		public void DivisionAssertion()
		{
			string expression = "${MyVariable/2}";

			string key = "MyVariable";
			int val = 12;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 / 2));
		}

		[Test]
		public void ListDivisionAssertion()
		{
			string expression = "${MyVariable[0]/2}";

			string key = "MyVariable";
			int[] val = { 12 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 / 2));
		}

		[Test]
		public void AdditionAssertion()
		{
			string expression = "${MyVariable+24}";

			string key = "MyVariable";
			int val = 12;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 + 24));
		}

		[Test]
		public void ListAdditionAssertion()
		{
			string expression = "${MyVariable[0]+24}";

			string key = "MyVariable";
			int[] val = { 12 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 + 24));
		}

		[Test]
		public void SubtractionAssertion()
		{
			string expression = "${MyVariable-24}";

			string key = "MyVariable";
			int val = 12;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 - 24));
		}

		[Test]
		public void ListSubtractionAssertion()
		{
			string expression = "${MyVariable[0]-24}";

			string key = "MyVariable";
			int[] val = { 12 };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 - 24));
		}

		[Test]
		public void RightSideStringAdditionAssertion()
		{
			string expression = "${MyVariable+'def'}";

			string key = "MyVariable";
			string val = "abc";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, "abc" + "def");
		}

		[Test]
		public void ListRightSideStringAdditionAssertion()
		{
			string expression = "${MyVariable[0]+'def'}";

			string key = "MyVariable";
			string[] val = { "abc" };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, "abc" + "def");
		}

		[Test]
		public void LeftSideStringAdditionAssertion()
		{
			string expression = "${'abc'+MyVariable}";

			string key = "MyVariable";
			string val = "def";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, "abc" + "def");
		}

		[Test]
		public void ListLeftSideStringAdditionAssertion()
		{
			string expression = "${'abc'+MyVariable[0]}";

			string key = "MyVariable";
			string[] val = { "def" };
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, "abc" + "def");
		}

		[Test]
		public void CompoundAdditionAssertion()
		{
			string expression = "${MyVariable+24+8}";

			string key = "MyVariable";
			int val = 12;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (12 + 24 + 8));
		}

		[Test]
		public void CompoundSubtractionAssertion()
		{
			string expression = "${MyVariable-24-8}";

			string key = "MyVariable";
			int val = 48;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, (48 - 24 - 8));
		}

		[Test]
		public void CompoundPrecedenceAssertion()
		{
			string expression = "${MyVariable+24*8}";

			string key = "MyVariable";
			string val = "48";
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, ("48192"));
		}

		[Test]
		public void CompoundOverridePrecedenceAssertion()
		{
			string expression = "${(MyVariable+24)*8}";

			string key = "MyVariable";
			int val = 48;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((int)resolved_exp.ResolvedValue, ((48 + 24) * 8));
		}

		[Test]
		public void TernaryOperationAssertion()
		{
			string expression = "${MyVariable==48?'y' + 'e' + 's':'n' + 'o'}";

			string key = "MyVariable";
			int val = 48;
			DataContext dc = new DataContext();
			dc.RegisterDataItem(key, val);

			ResolvedExpression resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((string)resolved_exp.ResolvedValue, "yes");
		}
	
		[Test]
		public void FunctionAssertion(){
			string expression;
			ResolvedExpression resolved_exp;
			DataContext dc = new DataContext();

			expression = "${empty('')}";
			resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((bool)resolved_exp.ResolvedValue, true);
			
			expression = "${empty ( '' )}";
			resolved_exp = Engine.ResolveExpression(expression, dc);
			Assert.AreEqual((bool)resolved_exp.ResolvedValue, true);
		}

	}
}
