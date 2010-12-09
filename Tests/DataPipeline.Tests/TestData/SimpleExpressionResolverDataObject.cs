using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline.Tests.TestData
{
	/// <summary>
	/// Test data object that does not implement IJsonSerializable
	/// </summary>
	class SimpleExpressionResolverDataObject : SimpleDataObject, IExpressionEvaluator
	{
		public SimpleExpressionResolverDataObject() { }

		public SimpleExpressionResolverDataObject(int myint, string mystring, double myfloat) 
			: base(myint, mystring, myfloat)
		{}



		#region IExpressionEvaluator Members

		public object ResolveExpressionValue(string expression)
		{
			if (expression == "MyInt")
			{
				return MyInt;
			}
			else if (expression == "MyFloat")
			{
				return MyFloat;
			}
			else if (expression == "MyString")
			{
				return MyString;
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
