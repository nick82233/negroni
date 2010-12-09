using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline.Tests.TestData
{
	class Samples
	{
		public const int INTVAL = 1;
		public const string STRVAL = "My Test String";
		public const double DBLVAL = 1.1;

		public static SimpleDataObject GetSimpleDataObject()
		{
			SimpleDataObject data = new SimpleDataObject(INTVAL, STRVAL, DBLVAL);
			return data;
		}

		public static SimpleDataObject GetJsonSerializableObject()
		{
			SimpleDataObject data = new SimpleDataObject(INTVAL, STRVAL, DBLVAL);
			return data;
		}


		public static SampleSimpleDataControl GetSimpleDataControl()
		{
			SampleSimpleDataControl control = new SampleSimpleDataControl(GetSimpleDataObject());
			return control;
		}

		public static SampleSimpleDataControl GetJsonStringDataControl()
		{
			SampleSimpleDataControl control = new SampleSimpleDataControl(GetJsonSerializableObject());
			return control;
		}

		public static DataItem GetSimpleDataItem(string key)
		{
			DataItem item = new DataItem(key, GetSimpleDataControl());
			item.Key = key;
//			item.Data
			return item;
		}
		public static DataItem GetJsonSerializableDataItem(string key)
		{
			DataItem item = new DataItem(key, GetJsonStringDataControl());
			item.Key = key;
			return item;
		}

	}
}
