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
	/// Specialized counter to handle hit/miss counting.
	/// This is primarily used for cache counting.
	/// </summary>
	public class HitMissCounter
	{

		public HitMissCounter()
		{
			Hits = new ItemCounter();
			Misses = new ItemCounter();
		}


		private bool _enableCounters = false;
		/// <summary>
		/// Flag to enable/disable counters on the front cache
		/// </summary>
		public bool EnableCounters
		{
			get
			{
				return _enableCounters;
			}
			set
			{
				if (value != _enableCounters)
				{
					_enableCounters = value;
					Hits.ResetCount();
					Misses.ResetCount();
				}
			}
		}

		#region  Hit/Miss counters

		private ItemCounter Hits = null;

		private ItemCounter Misses = null;

		/// <summary>
		/// Number of hits from counted
		/// </summary>
		/// <returns></returns>
		public long GetHits()
		{
			if (EnableCounters)
			{
				return Hits.GetCount();
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Number of misses counted
		/// </summary>
		/// <returns></returns>
		public long GetMisses()
		{
			if (EnableCounters)
			{
				return Misses.GetCount();
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Increment the count of hits and return the current count.
		/// </summary>
		/// <returns>Current count, or zero if disabled</returns>
		public long IncrementHits()
		{
			if (EnableCounters)
			{
				return Hits.IncrementCount();
			}
			return 0;
		}

		/// <summary>
		/// Increment the count of misses and return the current count.
		/// </summary>
		/// <returns>Current count, or zero if disabled</returns>
		public long IncrementMisses()
		{
			if (EnableCounters)
			{
				return Misses.IncrementCount();
			}
			return 0;
		}
		#endregion
	}
}
