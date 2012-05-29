/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


using Negroni.DataPipeline.Serialization;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Representation of an item in the DataContext's data set.
	/// 
	/// </summary>
	/// <remarks>
	/// Actual data value is not serialized since this is reinitialized with
	/// each given userId
	/// </remarks>
	public class DataItem
	{
		#region Constructors

		public DataItem()
		{
			_viewContext = string.Empty;
		}

		public DataItem(string key, IDataContextInvokable dataControl)
			: this(key, dataControl, null)
		{}

		public DataItem(string key, IDataContextInvokable dataControl, string viewContext)
		{
			this.Key = key;
			this.ViewContext = viewContext;
			this.DataControl = dataControl;
		}
		#endregion

		/// <summary>
		/// Key defined for use with this item.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Flag to permit reflection-based JSON serialization.
		/// This is normally false as there is a significant performance hit
		/// to using reflection-based serialization.
		/// </summary>
		public bool AllowReflectiveJsonSerialization { get; set; }

		/// <summary>
		/// Secondarily chained data item under the same key.  
		/// When multiple data items are registered under the same key,
		/// the chain holds non-matching items from different scopes
		/// </summary>
		public DataItem ChainedDataItem { get; set; }

		/// <summary>
		/// Adds a new DataItem to the chain
		/// </summary>
		/// <param name="dataItem"></param>
		public DataItem AppendChainedDataItem(DataItem dataItem)
		{
			if (this.ChainedDataItem == null)
			{
				this.ChainedDataItem = dataItem;
				return dataItem;
			}
			else
			{
				return this.ChainedDataItem.AppendChainedDataItem(dataItem);
			}
		}

		private string _viewContext = null;

		/// <summary>
		/// Comma-delimited list of views for which this data item is valid
		/// </summary>
		public string ViewContext
		{
			get
			{ return _viewContext; }
			set { _viewContext = value; }
		}
		/// <summary>
		/// Tests to see if this data item or (chained data item)
		/// is valid for the specified view
		/// </summary>
		/// <param name="viewName"></param>
		public bool IsValidForView(string viewName)
		{
			if (IsCurrentValidForView(viewName))
			{
				return true;
			}
			else if (ChainedDataItem != null)
			{
				return ChainedDataItem.IsValidForView(viewName);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Internal method to test this instance.
		/// </summary>
		/// <param name="viewName"></param>
		/// <returns></returns>
		private bool IsCurrentValidForView(string viewName)
		{
			if (string.IsNullOrEmpty(ViewContext) || ViewContext == "*")
			{
				return true;
			}
			if (string.IsNullOrEmpty(viewName))
			{
				return false;
			}
			return (ViewContext.ToLower().Contains(viewName.ToLower()));
		}



		private IDataContextInvokable _dataControl = null;

		/// <summary>
		/// DataPipeline control definition for this item.
		/// </summary>
		public IDataContextInvokable DataControl
		{
			get
			{
				return _dataControl;
			}
			set
			{
				_dataControl = value;
			}
		}

		/// <summary>
		/// Gets the appropriate data control without knowledge of the view
		/// </summary>
		/// <returns></returns>
		public IDataContextInvokable GetDataControl()
		{
			return GetDataControl(null);
		}
		/// <summary>
		/// Gets the data control appropriate for the identified view
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public IDataContextInvokable GetDataControl(string view) 
		{
			if (IsCurrentValidForView(view))
			{
				return DataControl;
			}
			else if (ChainedDataItem != null)
			{
				return ChainedDataItem.GetDataControl(view);
			}
			else
			{
				return null;
			}
		}


		public DataItem GetViewSpecificDataItem(string view)
		{
			if (IsCurrentValidForView(view))
			{
				return this;
			}
			else if (ChainedDataItem != null)
			{
				return ChainedDataItem.GetViewSpecificDataItem(view);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Writes the underlying data to stream in JSON format.
		/// Does not include the Key value in JSON object.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteAsJSON(TextWriter writer)
		{
			object data = Data;
			if (null == data)
			{
				writer.Write("{}");
				return;
			}
			if (data is IJsonSerializable)
			{
				/*
				using (MemoryStream tmpStream = new MemoryStream())
				{
					StreamWriter tmpWriter = new StreamWriter(tmpStream);

					((IJsonSerializable)data).WriteAsJSON(tmpWriter);
					tmpWriter.Flush();
					tmpStream.Seek(0, SeekOrigin.Begin);
					StreamReader tmpReader = new StreamReader(tmpStream);
					string json = tmpReader.ReadToEnd();
					if (string.IsNullOrEmpty(json))
					{
						writer.Write("{}"); //empty object
					}
					else
					{
						writer.Write(json);
					}

				}
				 * */
				((IJsonSerializable)data).WriteAsJSON(writer);
				return;
			}
			else if (data is string)
			{
				writer.Write(JsonData.JSSafeQuote((string)data));
			}
			else if (data is IDictionary<string, object> ||
				data is IDictionary<string, string>)
			{
				DictionaryJsonSerializer ser = new DictionaryJsonSerializer(data as Dictionary<string, object>);
				ser.AllowReflectiveSerialization = AllowReflectiveJsonSerialization;
				ser.WriteAsJSON(writer);
			}
			else if (data is IEnumerable)
			{
				EnumerableJsonSerializer ser = new EnumerableJsonSerializer(data as IEnumerable);
				ser.WriteAsJSON(writer);
			}
			else
			{
				if (AllowReflectiveJsonSerialization)
				{
					writer.Write(BaseJsonSerializer.ExpensiveGetJsonDataAsString(data, AllowReflectiveJsonSerialization));
				}
				else
				{
					writer.Write(JsonData.JSSafeQuote(data.ToString()));
				}
			}
		}



		private bool _excludeFromClientContext = false;

		/// <summary>
		/// Marks an item to be suppressed from the client-side DataContext representation
		/// </summary>
		public bool ExcludeFromClientContext
		{
			get
			{
				return _excludeFromClientContext;
			}
			set
			{
				_excludeFromClientContext = value;
			}
		}

		private bool _requestServerResolution = true;

		/// <summary>
		/// Indicates this item desires to be resolved server-side.
		/// For internal OpenSocial endpoints, this is typically true.
		/// For external data sources (os:HttpRequest), this is typically false
		/// </summary>
		/// <remarks>
		/// Requesting server resolution does not guarantee server-side resolution.
		/// The app must be marked to allow server-side resolution of external data
		/// with the maint key "OSML_ServerResolveExternalData" and in the app record
		/// by an admin.  This is typically reserved for partner apps.
		/// </remarks>
		public bool RequestServerResolution
		{
			get
			{
				return _requestServerResolution;
			}
			set
			{
				_requestServerResolution = value;
			}
		}



		[NonSerialized]
		private object _data = null;

		/// <summary>
		/// Actual resolved data for this DataItem.
		/// When dealing with scoped data, call GetData(view)
		/// </summary>
		public object Data
		{
			get
			{
				if (_data != null)
				{
					return _data;
				}
				else if (DataControl != null && DataControl.Value != null)
				{
					return DataControl.Value;
				}
				return null;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// Gets resolved data from this or any Chained data item.
		/// This ignores view scoping and retrieves the first instance of the value
		/// </summary>
		public object GetData()
		{
			return GetData(null);
		}
		/// <summary>
		/// Gets resolved data from this or any Chained data item
		/// where data is valid for view.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public object GetData(string view)
		{
			if (string.IsNullOrEmpty(view)|| IsCurrentValidForView(view))
			{
				return Data;
			}
			else if (ChainedDataItem != null && ChainedDataItem.IsValidForView(view))
			{
				return ChainedDataItem.GetData(view);
			}
			else
			{
				return null;
			}
		}

	}
}
