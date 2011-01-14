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
	/// Base class for implementing a counter that tracks a global
	/// value and exposes a time slice value
	/// </summary>
	public class GlobalSliceCounter
	{

		/// <summary>
		/// Determines if global counting is enabled
		/// </summary>
		public bool IsGlobalCounterEnabled { get; set; }

		public void ResetCounters()
		{
			lock (globalCountLock)
			{
				_globalCount = 0;
			}
			ResetTimesliceCounter();
		}

		#region Global counting
		/// <summary>
		/// Lock object for incrementing the global count
		/// </summary>
		private object globalCountLock = new object();

		private int _globalCount = 0;

		/// <summary>
		/// Increments the global counter.
		/// If the global counter is not enabled this does nothing.
		/// </summary>
		public void IncrementGlobalCount()
		{
			if (!IsGlobalCounterEnabled) return;

			lock (globalCountLock)
			{
				_globalCount++;
			}
		}

		/// <summary>
		/// Number of items counted in the full lifetime of the object
		/// </summary>
		/// <returns></returns>
		public int GetGlobalCount()
		{
			return _globalCount;
		}

		#endregion

		#region slice counting for global counter

		/// <summary>
		/// Lock object for incrementing the global count
		/// </summary>
		private object sliceCountLock = new object();

		private int _sliceStartCount = 0; // initial number of items being counted
		private int _sliceStartMilliseconds; //miliseconds at start of slice
		private bool sliceCounterOn = false;

		/// <summary>
		/// Resets the timeslice counter for global control count.
		/// </summary>
		public void ResetTimesliceCounter()
		{
			lock (sliceCountLock)
			{
				_sliceStartCount = _globalCount;
				_sliceStartMilliseconds = Environment.TickCount;
				sliceCounterOn = true;
			}
		}

		/// <summary>
		/// Gets the current CounterSlice values for the GlobalControlCount.
		/// Note: You must have previously called ResetTimesliceCounter for this to work.
		/// </summary>
		/// <returns></returns>
		public CounterSlice GetCurrentCountTimeslice()
		{
			CounterSlice slice = new CounterSlice();
			if (!sliceCounterOn)
			{
				//mark off of global counter

				return slice;
			}
			lock (sliceCountLock)
			{
				slice.StartMilliseconds = _sliceStartMilliseconds;
				slice.Count = _globalCount - _sliceStartCount;
				slice.SliceEndMilliseconds = Environment.TickCount;
			}
			return slice;
		}


		#endregion



	}
}
