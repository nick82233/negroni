using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline.Tests.TestData
{
	/// <summary>
	/// Test data object that does not implement IJsonSerializable
	/// </summary>
	class ComplexExpressionResolverDataObject : SimpleExpressionResolverDataObject, IExpressionEvaluator
	{
		public ComplexExpressionResolverDataObject() { }

		public ComplexExpressionResolverDataObject(int myint, string mystring, double myfloat) 
			: base(myint, mystring, myfloat)
		{}

		public ComplexExpressionResolverDataObject(string myString, IExpressionEvaluator nestedObject)
		{
			MyString = myString;
			_nestedObject = nestedObject;
		}

		private IExpressionEvaluator _nestedObject = null;

		/// <summary>
		/// Accessor for nestedObject.
		/// Performs lazy load upon first request
		/// </summary>
		public IExpressionEvaluator NestedObject
		{
			get
			{
				if (_nestedObject == null)
				{
					_nestedObject = new SimpleExpressionResolverDataObject();
				}
				return _nestedObject;
			}
		}


		#region IExpressionEvaluator Members

		public object ResolveExpressionValue(string expression)
		{
			if(expression.Contains(".")){
				int pos = expression.IndexOf(".");
				string first = expression.Substring(0, pos);
				string theRest = expression.Substring(pos + 1);
				if (first.Equals("nested", StringComparison.InvariantCultureIgnoreCase))
				{
					return NestedObject.ResolveExpressionValue(theRest);
				}
				else
				{
					return null;
				}
			}
			else{
				return base.ResolveExpressionValue(expression);
			}

		}

		#endregion
	}
}
