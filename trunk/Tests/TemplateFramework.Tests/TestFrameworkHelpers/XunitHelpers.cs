#if XUNIT

/* ************************************************************
 *  Compatibility classes to allow xUnit builds to compile and run.
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 ***************************************************************
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAssert = Xunit.Assert;
using Xunit.Extensions;

namespace Negroni.TemplateFramework.Test
{
	static class Assert
	{
		static public void AreEqual(bool expected, bool actual)
		{
			XAssert.Equal<bool>(expected, actual);
		}
		static public void AreEqual(int expected, int actual)
		{
			XAssert.Equal<int>(expected, actual);
		}
		static public void AreEqual(object expected, object actual)
		{
			XAssert.Equal<object>(expected, actual);
		}
		static public void AreEqual(object expected, object actual, string message)
		{
			try
			{
				XAssert.Equal<object>(expected, actual);
			}
			catch (Exception ex)
			{
				throw new Exception(message, ex);
			}
		}
		static public void AreNotEqual(object expected, object actual, string message)
		{
			try
			{
				XAssert.NotEqual<object>(expected, actual);
			}
			catch (Exception ex)
			{
				throw new Exception(message, ex);
			}
		}
		static public void AreNotEqual(string expected, string actual)
		{
			XAssert.NotEqual<string>(expected, actual);
		}
		static public void AreNotEqual(int expected, int actual)
		{
			XAssert.NotEqual<int>(expected, actual);
		}
		static public void AreNotEqual(double expected, double actual)
		{
			XAssert.NotEqual<double>(expected, actual);
		}
		static public void Fail(string message)
		{
			throw new Exception(message);
		}
		static public void Greater(int expectedGreater, int actual)
		{
			Greater(expectedGreater, actual, string.Empty);
		}
		static public void Greater(int expectedGreater, int actual, string message)
		{
			if (expectedGreater <= actual)
			{
				throw new Exception(message);
			}
		}
		static public void Less(int expectedLess, int actual, string message)
		{
			if (expectedLess >= actual)
			{
				throw new Exception(message);
			}
		}
		static public void IsFalse(bool condition)
		{
			XAssert.False(condition);
		}
		static public void IsFalse(bool condition, string message)
		{
			try
			{
				XAssert.False(condition);
			}
			catch (Exception ex)
			{
				throw new Exception(message, ex);
			}
		}
		static public void IsTrue(bool condition)
		{
			XAssert.True(condition);
		}
		static public void IsTrue(bool condition, string message)
		{
			try
			{
				XAssert.True(condition);
			}
			catch (Exception ex)
			{
				throw new Exception(message, ex);
			}
		}
		static public void IsNotEmpty(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new Exception();
			}
		}
		static public void IsNotNull(object value)
		{
			IsNotNull(value, string.Empty);
		}
		static public void IsNotNull(object value, string message)
		{
			if (value == null)
			{
				throw new Exception(message);
			}
		}
		static public void IsNull(object value)
		{
			IsNull(value, string.Empty);
		}
		static public void IsNull(object value, string message)
		{
			if (value != null)
			{
				throw new Exception(message);
			}
		}

	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	class TestFixtureAttribute : System.Attribute
	{

	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	class TestAttribute : Xunit.FactAttribute
	{

	}

	//[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	//class ExpectedExceptionAttribute : System.Attribute
	//{
	//    public ExpectedExceptionAttribute(Type exception) { }
	//}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	class RowTestAttribute : Xunit.Extensions.TheoryAttribute
	{}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	class RowAttribute : Xunit.Extensions.InlineDataAttribute
	{
		public RowAttribute(params object[] dataValues)
			: base(dataValues)
		{

		}
	
	}

}

#endif