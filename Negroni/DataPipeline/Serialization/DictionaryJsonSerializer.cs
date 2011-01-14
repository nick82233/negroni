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

namespace Negroni.DataPipeline.Serialization
{
	/// <summary>
	/// Serializes an arbitrary dictionary into JSON
	/// </summary>
	class DictionaryJsonSerializer : IJsonSerializable
	{

		private IDictionary<string, object> _targetDictionary = null;

		/// <summary>
		/// Accessor for targetDictionary.
		/// Performs lazy load upon first request
		/// </summary>
		public IDictionary<string, object> TargetDictionary
		{
			get
			{
				if (_targetDictionary == null)
				{
					_targetDictionary = new Dictionary<string, object>();
				}
				return _targetDictionary;
			}
			set
			{
				_targetDictionary = value;
			}
		}

		/// <summary>
		/// Flag to allow reflection-based serialization.
		/// DO NOT USE THIS - IMPLEMENT IJsonSerializable ON YOUR DATA CLASSES
		/// </summary>
		public bool AllowReflectiveSerialization { get; set; }

		public DictionaryJsonSerializer() { }

		public DictionaryJsonSerializer(IDictionary<string, object> dictionary)
		{
			TargetDictionary = dictionary;
		}

		#region IJsonSerializable Members

		public void WriteAsJSON(System.IO.TextWriter writer)
		{
			if (TargetDictionary.Count == 0)
			{
				WriteEmptyArray(writer);
				return;
			}

			writer.WriteLine("{");

			foreach (KeyValuePair<string, object> keyset in TargetDictionary)
			{
				writer.Write(JsonData.JSSafeQuote(keyset.Key));
				writer.Write(":");
				if (keyset.Value is IJsonSerializable)
				{
					((IJsonSerializable)keyset.Value).WriteAsJSON(writer);
				}
				else
				{
					if (AllowReflectiveSerialization)
					{
						writer.Write(BaseJsonSerializer.ExpensiveGetJsonDataAsString(keyset.Value));
					}
					else
					{
						writer.Write(JsonData.JSSafeQuote(keyset.ToString()));
					}
				}

			}
		}

		/// <summary>
		/// Writes an empty JSON object
		/// </summary>
		/// <param name="writer"></param>
		private void WriteEmptyObject(System.IO.TextWriter writer)
		{
			writer.Write("[]");
			return;
		}

		/// <summary>
		/// Writes an empty javascript array
		/// </summary>
		/// <param name="writer"></param>
		private void WriteEmptyArray(System.IO.TextWriter writer)
		{
			writer.Write("[]");
			return;
		}

		#endregion
	}
}
