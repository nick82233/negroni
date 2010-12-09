using System;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;

namespace Negroni.DataPipeline.Tests.TestData
{
	class SampleSimpleDataControl : IDataContextInvokable
	{
		public SampleSimpleDataControl() { }
		public SampleSimpleDataControl(object data)
		{
			SetTestData(data);
		}
		#region IDataContextInvokable Members


		public void SetTestData(object data)
		{
			Value = data;
		}

		object dataObject = null;


		public string Key
		{
			get;
			set;
		}
			
		public string RawTag
		{
			get; set;
		}

		public object Value { get; set; }


		#endregion
	}
}
