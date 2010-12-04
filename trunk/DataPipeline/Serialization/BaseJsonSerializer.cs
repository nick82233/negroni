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

namespace Negroni.DataPipeline.Serialization
{
	/// <summary>
	/// Base class for JSON serialization
	/// </summary>
	public class BaseJsonSerializer
	{
		#region Static methods

		/// <summary>
		/// DO NOT INVOKE UNLESS YOU HATE PERFORMANCE
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public string ExpensiveGetJsonDataAsString(object data)
		{
			return ExpensiveGetJsonDataAsString(data, false);
		}

		/// <summary>
		/// DO NOT INVOKE UNLESS YOU HATE PERFORMANCE
		/// Returns a JSON string representation of the resolved data.
		/// This normally used only for testing.  
		/// Rely on <c>DataContext.WriteClientContext</c> method instead.
		/// </summary>
		/// <returns></returns>
		static public string ExpensiveGetJsonDataAsString(object data, bool allowReflectiveSerialization)
		{
			if (null == data)
			{
				return null;
			}
			if (data is IJsonSerializable)
			{
				MemoryStream s = new MemoryStream();
				StreamWriter w = new StreamWriter(s);
				((IJsonSerializable)data).WriteAsJSON(w);
				return DataContext.GetStreamContent(s);
			}
			else if (data is string)
			{
				return JsonData.JSSafeQuote((string)data);
			}
			else if (allowReflectiveSerialization)
			{
				return ReflectiveToJson(data);
			}
			else
			{
				return JsonData.JSSafeQuote(data.ToString());
			}
		}


		/// <summary>
		/// Use reflection to emit all properties of the data item.
		/// Normally used only for testing or development.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string ReflectiveToJson(object data)
		{
			StringBuilder json = new StringBuilder();
			json.Append("{");
			Type t = data.GetType();
			PropertyInfo[] props = t.GetProperties();
			BindingFlags flags = BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance;
			for (int i = 0; i < props.Length; i++)
			{
				PropertyInfo p = props[i];
				Type pt = p.PropertyType;
				object val = null;
				try
				{
					val = t.InvokeMember(p.Name, flags, null, data, null);
				}
				catch (NotImplementedException /*nex*/)
				{
					val = null;
				}
				catch (Exception)
				{
					val = null;
				}

				if (i > 0)
				{
					json.Append(",");
				}
				json.Append(JsonData.JSSafeQuote(p.Name)).Append(":");

				if (val == null)
				{
					json.Append("null");
				}
				else if (pt == typeof(string))
				{
					json.Append(JsonData.JSSafeQuote((string)val));
				}
				else
				{
					json.Append(JsonData.JSSafeQuote(val.ToString()));
				}

			}
			json.Append("}");

			return json.ToString();
		}
		#endregion

		/// <summary>
		/// Writes an empty JSON object
		/// </summary>
		/// <param name="writer"></param>
		protected void WriteEmptyObject(System.IO.TextWriter writer)
		{
			writer.Write("{}");
			return;
		}

		/// <summary>
		/// Writes an empty javascript array
		/// </summary>
		/// <param name="writer"></param>
		protected void WriteEmptyArray(System.IO.TextWriter writer)
		{
			writer.Write("[]");
			return;
		}


	}
}
