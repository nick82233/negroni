using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline.Tests.TestData
{
	/// <summary>
	/// Test data object that does not implement IJsonSerializable
	/// </summary>
	class SimpleDataObject
	{
		public SimpleDataObject() { }

		public SimpleDataObject(int myint, string mystring, double myfloat) 
		{
			MyInt = myint;
			MyString = mystring;
			MyFloat = myfloat;
		}


		public int MyInt { get; set; }

		public string MyString { get; set; }

		public double MyFloat { get; set; }

	}
}
