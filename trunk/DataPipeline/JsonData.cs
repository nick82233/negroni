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
using System.Runtime.Serialization;
using System.Text;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Wrapper class for serializing and deserializing arbitrary JSON data.
	/// Data is placed into a Dictionary<string, object> structure.
	/// </summary>
	/// <remarks>
	/// Another option is for actual JSON serialization must be done with a 3.5 assembly using
	/// the DataContractJsonSerializer object.  Until Platform codebase
	/// is upgraded, this exists outside the MySpace.DataPipeline assembly.
	/// </remarks>
	/// <example>
	/// Also consider support for DataContractJsonSerializer
	/// 
	/// DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(JsonObjectDictionary));  
    /// MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));  
	/// JsonObjectDictionary jsonObj = (JsonObjectDictionary)dcjs.ReadObject(ms);  
	/// Console.WriteLine("Item count: " + jsonObj.DataDictionary.Count);  
	/// </example>
	[Serializable]
	public class JsonData : ISerializable, IExpressionEvaluator, IEnumerableDataWrapper, IJsonSerializable
	{

		/// <summary>
		/// Encapsulation of the trace occurring as we walk the string.
		/// </summary>
		class JsonTracePos
		{
			const int BUFFER_SIZE = 255;

			public bool InKey = false;

			public bool InValue = false;

			/// <summary>
			/// Clears the context to identify that we're between keys and values
			/// </summary>
			public void ClearContext()
			{
				InKey = InValue = false;
				bufferPos = 0;
			}

			/// <summary>
			/// Depth of the trace.  Starts at 0 for top-level
			/// </summary>
			public int Depth { get; set; }
			/// <summary>
			/// Flag to indicate if we're in a quoted string
			/// </summary>
			public bool InQuotedString { get; set; }

			/// <summary>
			/// Quotation character used on string
			/// </summary>
			public char QuoteChar { get; set; }

			/// <summary>
			/// Current absolute position in the trace.
			/// </summary>
			public int CurrentPos { get; set; }

			/// <summary>
			/// Previous Key that was identified
			/// </summary>
			public string PreviousKey { get; set; }

			/// <summary>
			/// Buffer to hold current object contents
			/// </summary>
			char[] buffer = new char[BUFFER_SIZE];

			/// <summary>
			/// Position in the buffer
			/// </summary>
			int bufferPos = 0;

			/// <summary>
			/// Overflow StringBuilder buffer
			/// </summary>
			StringBuilder overflowSb = null;

			/// <summary>
			/// Flag to indicate buffer has spilled over into StringBuilder
			/// </summary>
			bool usingStringBuilder = false;

			/// <summary>
			/// Indicates we're in an un-quoted value
			/// </summary>
			public bool InUnQuotedValue { get; set; }

			/// <summary>
			/// Indicates we're in an un-key value
			/// </summary>
			public bool InUnQuotedKey { get; set; }

			public string FlushBuffer()
			{
				if (!usingStringBuilder)
				{
					if(bufferPos == 0){
						return null;
					}
					else
					{
						string s = new string(buffer, 0, bufferPos);
						bufferPos = 0;
						return s;
					}					
				}
				else
				{
					if (bufferPos > 0)
					{
						overflowSb.Append(buffer, 0, bufferPos);
					}
					string s = overflowSb.ToString();
					bufferPos = 0;
					usingStringBuilder = false;
					return s;
				}
			}

			/// <summary>
			/// Adds a character to the buffer
			/// </summary>
			/// <param name="c"></param>
			public void AppendChar(char c)
			{
				if (bufferPos + 1 < BUFFER_SIZE)
				{
					buffer[bufferPos++] = c;
				}
				else
				{
					if (overflowSb == null)
					{
						overflowSb = new StringBuilder();
					}
					overflowSb.Append(buffer, 0, bufferPos);
					usingStringBuilder = true;
					bufferPos = 0;
					buffer[bufferPos++] = c;
				}
			}
		}


		#region Internal data storage structures

		private Dictionary<string, object> _dataDictionary = null;

		/// <summary>
		/// Internal dictionary that holds data, if specified as an object hash
		/// </summary>
		public Dictionary<string, object> DataDictionary
		{
			get
			{
				if (_dataDictionary == null)
				{
					_dataDictionary = new Dictionary<string, object>();
				}
				return _dataDictionary;
			}
		}

		private List<object> _dataList = null;

		/// <summary>
		/// Internal List that holds data, if specified as an Array
		/// </summary>
		public List<object> DataList
		{
			get
			{
				if (_dataList == null)
				{
					_dataList = new List<object>();
				}
				return _dataList;
			}
		}
		#endregion

		#region Data Properties

		private bool _isArray = false;

		/// <summary>
		/// Indicates that this contains an Array structure
		/// </summary>
		public bool IsArray
		{
			get
			{
				return _isArray;
			}
			protected set
			{
				_isArray = value;
			}
		}

		private bool _isObjectHash = true;

		/// <summary>
		/// Indicates that this contains an ObjectHash structure
		/// </summary>
		public bool IsObjectHash
		{
			get
			{
				return _isObjectHash;
			}
			protected set
			{
				_isObjectHash = value;
			}
		}

		/// <summary>
		/// Length of object data - either array length or top-level object hash length
		/// </summary>
		public int Length
		{
			get
			{
				if (IsArray)
				{
					return DataList.Count;
				}
				else
				{
					return DataDictionary.Count;
				}
			}
		}


		/// <summary>
		/// Raw json string that was loaded
		/// </summary>
		public string RawJSON { get; set; }

		#endregion



		public JsonData()
		{}

		public JsonData(string json)
			:this()
		{
			LoadJsonString(json);			
		}

		protected JsonData(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (var entry in info)
			{
				DataDictionary.Add(entry.Name, entry.Value);
			}
		}

		/// <summary>
		/// Loads a new JSON string into the object
		/// </summary>
		/// <param name="json"></param>
		public void LoadJsonString(string json)
		{
			DataList.Clear();
			DataDictionary.Clear();
			if (string.IsNullOrEmpty(json))
			{
				RawJSON = null;
				IsArray = false;
				IsObjectHash = false;
				return;
			}
			RawJSON = json = json.Trim();


			if (string.IsNullOrEmpty(json))
			{
				return;
			}
			if (json.StartsWith("["))
			{
				if (json.EndsWith("]"))
				{
					IsArray = true;
					IsObjectHash = false;
					LoadArrayString(DataList, json);
				}
				else
				{
					throw new ArgumentException("Malformed JSON Array data: " + json);
				}
			}
			else if (json.StartsWith("{"))
			{
				if (json.EndsWith("}"))
				{
					IsArray = false;
					IsObjectHash = true;
					LoadObjectString(DataDictionary, json);
				}
				else
				{
					throw new ArgumentException("Malformed JSON object data: " + json);
				}
			}
			else
			{
				throw new ArgumentException("Malformed JSON data: " + json);
			}
		}

		/// <summary>
		/// Tests to see if the current character is a quote mark, optionally 
		/// matching a previous quote mark.
		/// </summary>
		/// <param name="current"></param>
		/// <param name="matchQuote"></param>
		/// <returns></returns>
		bool IsCurrentCharQuote(char current, char matchQuote)
		{
			if (matchQuote == '\'' || matchQuote == '"')
			{
				return current == matchQuote;
			}
			else
			{
				return (current == '\'' || current == '"');
			}
		}

		/// <summary>
		/// Tests to see if the current character is the starting delimiter of an object
		/// </summary>
		/// <param name="current"></param>
		/// <param name="matchQuote"></param>
		/// <returns></returns>
		bool IsCurrentCharObjectStart(char current)
		{
			return (current == '{' || current == '[');
		}


		/// <summary>
		/// Path for loading a JSON object
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="json"></param>
		private void LoadObjectString(Dictionary<string, object> dictionary, string json)
		{
			if (string.IsNullOrEmpty(json) || !ValueIsObject(json))
			{
				return;
			}
			int startBracketPos = json.IndexOf("{");
			int endBracketPos = json.LastIndexOf("}");
			json = json.Substring(startBracketPos + 1, endBracketPos - startBracketPos - 1);

			//walk the string
			JsonTracePos tracePos = new JsonTracePos();
			char prevChar = '\0'; ;
			char curChar;
			bool isEscaped = false;

			
			//bool inKey = false;
			//bool inValue = false;
			//bool unQuotedValue = false;

			//string lastKey = null;

			for (tracePos.CurrentPos = 0; tracePos.CurrentPos < json.Length; tracePos.CurrentPos++)
			{
				curChar = json[tracePos.CurrentPos];
				if (!tracePos.InKey && !tracePos.InValue)
				{
					if (curChar == ':' || curChar == ',' || curChar == '\t' 
						|| curChar == '\n' || curChar == '\r'
						|| curChar == ' ')
					{
						continue;
					}
					if (tracePos.PreviousKey != null)
					{
						if (IsCurrentCharObjectStart(curChar))
						{
							//recurse and add object result
							char matchChar = '\0';
							if (curChar == '{') matchChar = '}';
							else if (curChar == '[') matchChar = ']';

							int embeddedObjectEndPos = FindMatchedCloseCharPosition(json, tracePos.CurrentPos);
							if (embeddedObjectEndPos == -1)
							{
								throw new Exception("Malformed JSON data");
							}
							//todo - walk backwards to identify correct delimiter
							string embeddedJson = json.Substring(tracePos.CurrentPos, (embeddedObjectEndPos - tracePos.CurrentPos) + 1);

							dictionary[tracePos.PreviousKey] = GetResolvedValue(embeddedJson);
							tracePos.CurrentPos = embeddedObjectEndPos + 1;
							tracePos.PreviousKey = null;
							tracePos.ClearContext();
							continue;
						}
						else if(!IsCurrentCharQuote(curChar, '\0'))
						{
							tracePos.InValue = true;
							tracePos.InUnQuotedValue = true;
							tracePos.AppendChar(curChar);
							continue;
						}
					}
					else if (!IsCurrentCharQuote(curChar, tracePos.QuoteChar))
					{
						//in unquoted key
						tracePos.InKey = true;
						tracePos.InUnQuotedKey = true;
						tracePos.AppendChar(curChar);
						continue;
					}
				}

				if (IsCurrentCharQuote(curChar, tracePos.QuoteChar))
				{
					if (!tracePos.InKey && !tracePos.InValue)
					{
						tracePos.QuoteChar = curChar;

						if (tracePos.PreviousKey == null)
						{
							tracePos.InKey = true;
						}
						else
						{
							tracePos.InValue = true;
						}
					}
					else if (isEscaped)
					{
						tracePos.AppendChar(curChar);
					}
					else
					{
						tracePos.QuoteChar = '\0';
						if (tracePos.InKey)
						{
							tracePos.PreviousKey = tracePos.FlushBuffer();
							tracePos.InKey = false;
						}
						else
						{
							string val = tracePos.FlushBuffer();
							//resolve if array or object
							dictionary[tracePos.PreviousKey] = GetResolvedValue(val);
							tracePos.ClearContext();
							tracePos.PreviousKey = null;
						}
					}
					continue;
				}
				else
				{
					if (tracePos.InValue && tracePos.InUnQuotedValue && curChar == ',')
					{
						string val = tracePos.FlushBuffer();
						//resolve if array or object
						dictionary[tracePos.PreviousKey] = GetResolvedValue(val);
						tracePos.InValue = false;
						tracePos.InUnQuotedValue = false;
						tracePos.PreviousKey = null;
						continue;
					}
					else if(tracePos.InKey && tracePos.InUnQuotedKey &&
						(curChar == ' ' || curChar == ':' || curChar == '\t' 
						|| curChar == '\n' || curChar == '\r'))
					{
						string val = tracePos.FlushBuffer();
						tracePos.PreviousKey = val;
						tracePos.InUnQuotedKey = false;
						tracePos.InKey = false;
						continue;
					}
					else
					{
						tracePos.AppendChar(curChar);
					}
				}
				
			}

			//look to flush final unquoted value
			if (tracePos.InValue && tracePos.InUnQuotedValue && tracePos.PreviousKey != null)
			{
				string val = tracePos.FlushBuffer();
				dictionary[tracePos.PreviousKey] = GetResolvedValue(val);
			}
			else if (tracePos.InKey || tracePos.InValue)
			{
				throw new Exception("Malformed JSON data");
			}
		}

		/// <summary>
		/// Searches for the matched closing character (array bracket or object close)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		private int FindMatchedCloseCharPosition(string source, int startIndex)
		{
			if (string.IsNullOrEmpty(source) || startIndex >= source.Length)
			{
				return -1;
			}
			int arrayDepthCount = 0;
			int objectDepthCount = 0;

			char startChar = source[startIndex];
			char endChar;
			if (startChar == '{')
			{
				endChar = '}';
			}
			else if (startChar == '[')
			{
				endChar = ']';
			}
			else
			{
				return -1;
			}
			for (int i = startIndex + 1; i < source.Length; i++)
			{
				if (source[i] == '{')
				{
					objectDepthCount++;
				}
				else if (source[i] == '[')
				{
					arrayDepthCount++;
				}
				if (endChar == source[i] && (objectDepthCount == 0 && arrayDepthCount == 0))
				{
					return i;
				}
				else if (source[i] == '}')
				{
					objectDepthCount--;
				}
				else if (source[i] == ']')
				{
					arrayDepthCount--;
				}
			}
			return -1;
		}


		/// <summary>
		/// Path for loading a JSON array
		/// </summary>
		/// <param name="json"></param>
		private void LoadArrayString(List<object> list, string json)
		{
			if (string.IsNullOrEmpty(json) || !ValueIsArray(json))
			{
				return;
			}
			int startBracketPos = json.IndexOf("[");
			int endBracketPos = json.LastIndexOf("]");
			json = json.Substring(startBracketPos + 1, endBracketPos - startBracketPos - 1);

			//walk the string
			JsonTracePos tracePos = new JsonTracePos();
			char prevChar = '\0'; ;
			char curChar;
			bool isEscaped = false;

			for (tracePos.CurrentPos = 0; tracePos.CurrentPos < json.Length; tracePos.CurrentPos++)
			{
				curChar = json[tracePos.CurrentPos];
				if (!tracePos.InValue)
				{
					if (curChar == ',' || curChar == '\t'
						|| curChar == '\n' || curChar == '\r'
						|| curChar == ' ')
					{
						continue;
					}
					if (IsCurrentCharObjectStart(curChar))
					{
						//recurse and add object result
						char matchChar = '\0';
						if (curChar == '{') matchChar = '}';
						else if (curChar == '[') matchChar = ']';

						int embeddedObjectEndPos = FindMatchedCloseCharPosition(json, tracePos.CurrentPos);
						if (embeddedObjectEndPos == -1)
						{
							throw new Exception("Malformed JSON data");
						}
						//todo - walk backwards to identify correct delimiter
						string embeddedJson = json.Substring(tracePos.CurrentPos, (embeddedObjectEndPos - tracePos.CurrentPos) + 1);

						list.Add(GetResolvedValue(embeddedJson));
						tracePos.CurrentPos = embeddedObjectEndPos + 1;
						tracePos.ClearContext();
						continue;
					}
					else if (!IsCurrentCharQuote(curChar, '\0'))
					{
						tracePos.InValue = true;

						tracePos.InUnQuotedValue = true;
						tracePos.AppendChar(curChar);
						continue;
					}
				}

				if (IsCurrentCharQuote(curChar, tracePos.QuoteChar))
				{
					if (!tracePos.InValue)
					{
						tracePos.QuoteChar = curChar;
						tracePos.InValue = true;
					}
					else if (isEscaped)
					{
						tracePos.AppendChar(curChar);
					}
					else
					{
						tracePos.QuoteChar = '\0';
						string val = tracePos.FlushBuffer();
						//resolve if array or object
						list.Add(GetResolvedValue(val));
						tracePos.ClearContext();
					}
					continue;
				}
				else
				{
					if (tracePos.InValue && tracePos.InUnQuotedValue && curChar == ',')
					{
						string val = tracePos.FlushBuffer();
						//resolve if array or object
						list.Add(GetResolvedValue(val));
						tracePos.InValue = false;
						tracePos.InUnQuotedValue = false;
						continue;
					}
					else
					{
						tracePos.AppendChar(curChar);
					}
				}
				
			}

			//look to flush final unquoted value
			if (tracePos.InValue && tracePos.InUnQuotedValue)
			{
				string val = tracePos.FlushBuffer();
				list.Add(GetResolvedValue(val));
			}
			else if (tracePos.InKey || tracePos.InValue)
			{
				throw new Exception("Malformed JSON data");
			}
		}


		private object GetResolvedValue(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return null;
			}
			if (source.StartsWith("'") || source.StartsWith("\""))
			{
				source = StripQuotes(source.Trim());
			}
			if (ValueIsArray(source) || ValueIsObject(source))
			{
				return new JsonData(source);
			}
			else
			{
				if (source == "null")
				{
					return null;
				}

				double dblVal;
				bool bVal;
				if (Double.TryParse(source, out dblVal))
				{
					return dblVal;
				}
				else if (Boolean.TryParse(source, out bVal))
				{
					return bVal;
				}
				else
				{
					return source;
				}
			}
		}

		/// <summary>
		/// Parses a selector string into it's attribute (data member) and value components,
		/// stripping off quote marks
		/// </summary>
		/// <param name="selector">selector in form @attrib='valueLiteral'</param>
		/// <param name="dataMember"></param>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		internal static bool ParseSelectorString(string selector, out string dataMember, out string targetValue)
		{
			dataMember = null;
			targetValue = null;
			if (string.IsNullOrEmpty(selector))
			{
				return false;
			}
			if (selector.StartsWith("@"))
			{
				int eqpos = selector.IndexOf("=");
				if (eqpos > -1)
				{
					dataMember = selector.Substring(1, eqpos - 1);
					if (eqpos < selector.Length - 1)
					{
						targetValue = StripQuotes(selector.Substring(eqpos + 1));
						return true;
					}
				}
			}
			return false;
		}

		#region IExpressionEvaluator Members

		/// <summary>
		/// Resolve a dot-notation reference to values in this object
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public object ResolveExpressionValue(string expression)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return null;
			}

			//look for reserved values:
			if (expression == "length")
			{
				return Length;
			}


			if (IsArray)
			{
				string ndx, after= null;
				if (expression.StartsWith("["))
				{
					int bend = expression.IndexOf("]");
					if (bend == -1)
					{
						throw new ArgumentException("Bad resolver expression in JSON: " + expression);
					}
					ndx = expression.Substring(1, bend - 1);
					if (!expression.EndsWith("]"))
					{
						if (expression.Substring(bend + 1, 1) == ".")
						{
							after = expression.Substring(bend + 2);
						}
						else
						{
							after = expression.Substring(bend + 1);
						}
						
					}					
				}
				else
				{
					ndx = expression;
				}
				int pos;
				object item = null;
				if (Int32.TryParse(ndx, out pos))
				{
					if (pos > -1 && DataList.Count > pos)
					{
						item = DataList[pos];
					}
				}
				//and @attrib values
				else if (ndx.StartsWith("@"))
				{
					string attrib;
					string val;
					if (ParseSelectorString(ndx, out attrib, out val))
					{
						//cycle array
						for (int i = 0; i < DataList.Count; i++)
						{
							if (DataList[i] is IExpressionEvaluator)
							{
								IExpressionEvaluator evalObj = DataList[i] as IExpressionEvaluator;
								object actualVal = evalObj.ResolveExpressionValue(attrib);
								if (!string.IsNullOrEmpty(val)
									&& val.Equals(actualVal))
								{
									item = evalObj;
									break;
								}
							}
						}
					}

				}


				if (string.IsNullOrEmpty(after))
				{
					return item;
				}
				else if (item is IExpressionEvaluator)
				{
					return ((IExpressionEvaluator)item).ResolveExpressionValue(after);
				}
			}
			else
			{
				string key = ExtractJSONKey(expression);
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				if (DataDictionary.ContainsKey(key))
				{
					return DataDictionary[key];
				}
				else
				{
					int dotPos = key.IndexOf(".");
					int bracketPos = key.IndexOf("[");

					if (dotPos == -1 && bracketPos == -1)
					{
						return null;
					}
					bool isDot = false;
					bool isIndex = false;

					if (dotPos > -1 && bracketPos > -1)
					{
						if (dotPos < bracketPos)
						{
							isDot = true;
						}
						else
						{
							isIndex = true;
						}
					}
					else if (dotPos > -1)
					{
						isDot = true;
					}
					else if (bracketPos > -1)
					{
						isIndex = true;
					}


					if (isDot)
					{
						string first = key.Substring(0, dotPos);
						string theRest = key.Substring(dotPos + 1);
						if (DataDictionary.ContainsKey(first))
						{
							object val = DataDictionary[first];
							if (val is IExpressionEvaluator)
							{
								return ((IExpressionEvaluator)val).ResolveExpressionValue(theRest);
							}
							else
							{
								return null;
							}
						}
						else
						{
							return null;
						}
					}
					else if (isIndex)
					{
						string first = key.Substring(0, bracketPos);
						string theRest = key.Substring(bracketPos);
						if (theRest.EndsWith("]") && theRest.StartsWith("["))
						{
							theRest = theRest.Substring(1, theRest.Length - 2);
						}
						if (DataDictionary.ContainsKey(first))
						{
							object val = DataDictionary[first];
							if (val is IExpressionEvaluator)
							{
								return ((IExpressionEvaluator)val).ResolveExpressionValue(theRest);
							}
							else
							{
								return null;
							}
						}
						else
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}
			}
			return null;
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, object> keyset in DataDictionary)
			{
				info.AddValue(keyset.Key, keyset.Value);
			}
		}

		#endregion


		#region static parsing methods
		/// <summary>
		/// Retrieve the key, stripping off any quote characters.
		/// If quotes are not found, echo back the key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string ExtractJSONKey(string key)
		{
			return StripQuotes(key);
		}

		/// <summary>
		/// Strips any quotes off a string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string StripQuotes(string str)
		{
			if (string.IsNullOrEmpty(str)) return null;
			string quoteChar = GetQuoteChar(str);
			if (quoteChar == null) return str;

			int startPos = str.IndexOf(quoteChar);
			int endPos = str.LastIndexOf(quoteChar);

			if (startPos == -1 || endPos == -1 || startPos >= endPos)
			{
				return null;
			}
			return str.Substring(startPos + 1, endPos - startPos - 1);
		}

		/// <summary>
		/// Determines quote character used.
		/// Returns null if no quotes found
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static string GetQuoteChar(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			else
			{
				str = str.Trim();
			}
			if (!str.StartsWith("\"") && !str.StartsWith("'"))
			{
				return null;
			}
			int dqPos = str.IndexOf("\"");
			int sqPos = str.IndexOf("'");

			if (dqPos > -1 || sqPos > -1)
			{
				if (dqPos > -1 && sqPos > -1)
				{
					if (dqPos < sqPos)
					{
						return "\"";
					}
					else
					{
						return "'";
					}
				}
				else if (sqPos > -1)
				{
					return "'";
				}
				else
				{
					return "\"";
				}
			}
			else
			{
				return null; //should return key?
			}
		}

		/// <summary>
		/// Tests to see if the source is a JSON object
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static bool ValueIsObject(string source)
		{
			if (string.IsNullOrEmpty(source)) return false;

			return (source.StartsWith("{") && source.EndsWith("}"));
		}

		/// <summary>
		/// Tests to see if the source is a JSON array
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static bool ValueIsArray(string source)
		{
			if (string.IsNullOrEmpty(source)) return false;

			return (source.StartsWith("[") && source.EndsWith("]"));
		}

		#endregion

		#region static helper methods
		/// <summary>
		/// Safely wraps quotation marks around a javascript string.
		/// This escapes quote marks and newlines within str and returns str
		/// wrapped in double quote marks.
		/// </summary>
		/// <remarks>
		/// In addition to escaping quote marks and line feeds, this also
		/// </remarks>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string JSSafeQuote(string str)
		{
			//remove carriage returns
			if (string.IsNullOrEmpty(str))
			{
				return "\"\"";
			}
			//if was a trailing newline, remove
			if (str.EndsWith("\r\n"))
			{
				str = str.Substring(0, str.Length - 2);
			}

			if (str.EndsWith("\n"))
			{
				str = str.Substring(0, str.Length - 1);
			}

			//Windows carriage return/linefeed
			if (str.IndexOf("\r\n") > -1)
			{
				str = str.Replace("\r\n", "\\n");
			}
			//Normal newline
			if (str.IndexOf('\n') > -1)
			{
				str = str.Replace("\n", "\\n");
			}
			str = JSDoubleQuoteEscape(str);

			//remove trailing slash to avoid escaping the final quote mark
			if (str.Length > 0 &&
				str[str.Length - 1] == '\\')
			{
				// if there are three trailing slashes put a space before the final slash
				// and escape that slash
				if (str.Length >= 3 && str.Substring(str.Length - 3) == @"\\\")
				{
					str = str.Substring(0, str.Length - 1) + @" \\";
				}
				else if (str.Length >= 2 && str.Substring(str.Length - 2) == @"\\")
				{
					//escape both final slashes
					str = str + @"\\";
				}
				else
				{
					str = str + @"\";
				}

			}
			return "\"" + str + "\"";
		}

		/// <summary>
		/// Arbitrary token to utilize as an interim token for complex
		/// replace operations where the replaced secquence must be marked
		/// in the interim to avoid destroying matching, but legal sequences.
		/// </summary>
		private const string MARKER_TOKEN = "@@@###@@@";

		/// <summary>
		/// Properly escapes double quote marks within the candidate string.
		/// </summary>
		/// <param name="str">string to escape</param>
		/// <returns></returns>
		private static string JSDoubleQuoteEscape(string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return String.Empty;
			}
			if (-1 == str.IndexOf("\""))
			{
				return str;
			}
			else
			{
				//hunt for unescaped quotes and fix
				if (-1 == str.IndexOf("\\\""))
				{
					//just replace - no escaped strings
					return str.Replace("\"", "\\\"");
				}
				else
				{
					string markerStr = MARKER_TOKEN;
					str = str.Replace("\\\"", MARKER_TOKEN);
					//and replace other quotes
					str = str.Replace("\"", "\\\"");
					//and replace previous escaped strings
					return str.Replace(markerStr, "\\\"");
				}
			}
		}
		#endregion

		#region IEnumerableDataWrapper Members

		public System.Collections.IEnumerable EnumerableData
		{
			get {
				if (IsArray)
				{
					return DataList;
				}
				else
				{
					return DataDictionary;
				}
			}
		}

		#endregion

		#region IJsonSerializable Members

		public void WriteAsJSON(System.IO.TextWriter writer)
		{
			if (string.IsNullOrEmpty(RawJSON))
			{
				writer.Write("null");
			}
			else
			{
				writer.Write(RawJSON);
			}
		}

		#endregion
	}
}
