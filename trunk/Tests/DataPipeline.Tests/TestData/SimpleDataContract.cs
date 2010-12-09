using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.DataPipeline.Tests.TestData
{
	[DataContract(Name="simpleDataContract")]
	public class SimpleDataContract
	{
		public SimpleDataContract() { }

		public SimpleDataContract(int myint, string mystring, double myfloat) 
		{
			MyInt = myint;
			MyString = mystring;
			MyFloat = myfloat;
		}

		[DataMember(Name="myInt")]
		public int MyInt = 0;

		[DataMember(Name = "myString")]
		public string MyString = null;

		[DataMember(Name = "myFloat")]
		public double MyFloat = 0;
	}
}
