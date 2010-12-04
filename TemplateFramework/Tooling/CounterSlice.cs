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

namespace Negroni.TemplateFramework.Tooling
{
	/// <summary>
	/// A timeslice counter for returning atomic answers
	/// </summary>
	public struct CounterSlice
	{
		/// <summary>
		/// Actual Count of items
		/// </summary>
		public int Count;

		/// <summary>
		/// Textual label for this CounterSlice
		/// </summary>
		public string Label;

		/// <summary>
		/// Ticks at the start of this slice
		/// </summary>
		public int StartMilliseconds;

		/// <summary>
		/// Ticks at the end of this slice
		/// </summary>
		public int SliceEndMilliseconds;

		/// <summary>
		/// Calculate the time size of this sample slice.
		/// </summary>
		/// <returns></returns>
		public TimeSpan GetTimeSlice()
		{
			return new TimeSpan(0, 0, 0, 0, SliceEndMilliseconds - StartMilliseconds);
		}
	}
}
