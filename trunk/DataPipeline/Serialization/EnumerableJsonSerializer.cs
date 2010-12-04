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
using System.Text;

namespace Negroni.DataPipeline.Serialization
{
	/// <summary>
	/// Serializes an arbitrary enumerable into JSON
	/// </summary>
	class EnumerableJsonSerializer : BaseJsonSerializer, IJsonSerializable
	{

		private IEnumerable _targetCollection = null;

		/// <summary>
		/// Accessor for TargetCollection
		/// </summary>
		public IEnumerable TargetCollection
		{
			get
			{
				if (_targetCollection == null)
				{
					_targetCollection = new ArrayList();
				}
				return _targetCollection;
			}
			set
			{
				_targetCollection = value;
			}
		}

		/// <summary>
		/// Flag to allow reflection-based serialization.
		/// DO NOT USE THIS - IMPLEMENT IJsonSerializable ON YOUR DATA CLASSES
		/// </summary>
		public bool AllowReflectiveSerialization { get; set; }

		public EnumerableJsonSerializer() { }

		public EnumerableJsonSerializer(IEnumerable collection)
		{
			TargetCollection = collection;
		}

		#region IJsonSerializable Members

		public void WriteAsJSON(System.IO.TextWriter writer)
		{
			/*
							IEnumerable simpleList = repeatObjectList as IEnumerable;
			if (simpleList != null)
			{
				//System.Collections.Hashtable

				foreach (object item in simpleList)
				{
					object realItem = item;
					if (item is DictionaryEntry)
					{
						realItem = ((DictionaryEntry)item).Value;
					}
*/
			bool wroteOpener = false;
			bool isArray = true;

			foreach (object item in TargetCollection)
			{
				object realItem = item;

				//first loop determine if array or object
				//others prepend a comma
				if (wroteOpener)
				{
					writer.WriteLine(", ");
				}
				else
				{
					if (item is DictionaryEntry)
					{
						writer.WriteLine("{");
						isArray = false;
					}
					else
					{
						writer.WriteLine("[");
						isArray = true;
					}
					wroteOpener = true;
				}

				if (item is DictionaryEntry)
				{
					realItem = ((DictionaryEntry)item).Value;
					writer.Write(JsonData.JSSafeQuote(((DictionaryEntry)item).Key.ToString()));
					writer.Write(":");
				}

				if (realItem is IJsonSerializable)
				{
					((IJsonSerializable)realItem).WriteAsJSON(writer);
				}
				else
				{
					if (AllowReflectiveSerialization)
					{
						writer.Write(BaseJsonSerializer.ExpensiveGetJsonDataAsString(realItem));
					}
					else
					{
						writer.Write(JsonData.JSSafeQuote(realItem.ToString()));
					}
				}
			}
			if (!wroteOpener)
			{
				WriteEmptyArray(writer);
			}
			else
			{
				if (isArray)
				{
					writer.WriteLine("]");
				}
				else
				{
					writer.WriteLine("}");
				}
			}
		}

		#endregion
	}
}
