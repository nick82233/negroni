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
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Context object for a repeater loop
	/// </summary>
	public class LoopContext : IExpressionEvaluator
	{

		public LoopContext() { }

		public LoopContext(int count)
		{
			Count = count;
		}

		public LoopContext(int count, int startIndex)
			: this(count)
		{
			Index = startIndex;
		}

		/// <summary>
		/// Current zero-based index of the repeater loop
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Total count of items being repeated on
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// A unique ID of the template being rendered. This value is useful for generating HTML IDs.
		/// </summary>
		/// <remarks>
		/// This is actually not that useful.
		/// </remarks>
		public string UniqueId { get; set; }

		#region IExpressionEvaluator Members

		/// <summary>
		/// Evaluates an expression language request
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public object ResolveExpressionValue(string expression)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return null;
			}
			if (expression.Equals("count", StringComparison.InvariantCultureIgnoreCase))
			{
				return Count;
			}
			else if (expression.Equals("index", StringComparison.InvariantCultureIgnoreCase))
			{
				return Index;
			}
			else if (expression.Equals("uniqueid", StringComparison.InvariantCultureIgnoreCase))
			{
				return UniqueId;
			}
			return null;
		}

		#endregion
	}
}
