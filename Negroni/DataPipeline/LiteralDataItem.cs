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
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// A wrapper class for literal objects registered in the data context.
	/// Data Value is set at create time.  InvokeTarget is an empty wrapper.
	/// </summary>
	public class LiteralDataItem : IDataContextInvokable
	{
		public LiteralDataItem() { }

		public LiteralDataItem(string key, object value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Key to this item
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Value representation of this object
		/// </summary>
		public object Value { get; set; }


		private string _rawTag = null;
		/// <summary>
		/// Raw representation of a literal data item
		/// </summary>
		public string RawTag
		{
			get
			{
				if (null == _rawTag && !string.IsNullOrEmpty(Key))
				{
					string k = Key.Replace("\"", "'");
					string val;
					if(null == Value){
						val = string.Empty;
					}
					else{
						val = Value.ToString();
					}
					string template = "<LiteralDataItem key=\"{0}\" value=\"{1}\" />";
					_rawTag = string.Format(template, k, val);
				}
				return _rawTag;
			}
			set
			{
				_rawTag = value;
			}
		}

	}
}
