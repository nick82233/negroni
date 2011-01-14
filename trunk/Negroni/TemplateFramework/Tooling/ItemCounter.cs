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
	/// Simple thread-safe encapsulation for count and a lock object
	/// </summary>
	public class ItemCounter
	{
		/// <summary>
		/// Count of the items
		/// </summary>
		private long count = 0;

		private object countLock = new object();

		/// <summary>
		/// Increment the current count
		/// </summary>
		/// <returns></returns>
		public long IncrementCount()
		{
			long retVal;
			lock (countLock)
			{
				retVal = ++count;
			}
			return retVal;
		}


		/// <summary>
		/// Resets the count to zero
		/// </summary>
		public void ResetCount()
		{
			lock (countLock)
			{
				count = 0;
			}
		}

		/// <summary>
		/// Returns the number of items counted
		/// </summary>
		/// <returns></returns>
		public long GetCount()
		{
			return count;
		}
	}
}
