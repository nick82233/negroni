using System;
using System.Collections.Generic;
using System.Text;

using Negroni.DataPipeline;

namespace Negroni.DataPipeline.Tests.TestData
{
	class SampleIJsonSerializable : IJsonSerializable
	{
		public SampleIJsonSerializable() { }

		public SampleIJsonSerializable(string data)
		{
			MyData = data;
		}

		public string MyData { get; set; }


		#region IJsonSerializable Members

		public string ToJSON()
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);

			WriteAsJSON(writer);
			writer.Flush();

			return DataContext.GetStreamContent(stream);
		}

		public void WriteAsJSON(System.IO.TextWriter writer)
		{
			if (MyData == null)
			{
				writer.Write("{value:null}");
			}
			else
			{
				writer.Write("{value: " + JsonData.JSSafeQuote(MyData) + "}");
			}
		}

		#endregion
	}
}
