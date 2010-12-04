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
	/// Encapsulation of counters tracked by a ControlFactory,
	/// as well as the counter enabled/disabled state.
	/// Per-control counting happens within the ControlCatalog
	/// in the ControlFactory.
	/// </summary>
	public class ControlFactoryCounters : GlobalSliceCounter
	{
		/// <summary>
		/// Create a new counter instance
		/// </summary>
		public ControlFactoryCounters()
		{
			ResetTimesliceCounter();
		}



		/// <summary>
		/// Determines if the ControlFactory instance should be counting
		/// individual controls
		/// </summary>
		public bool IsControlUseCountingEnabled { get; set; }





		/// <summary>
		/// ControlMap type usage, keyed on Type.
		/// This is used for counting usage of each type of registered control.
		/// </summary>
		private Dictionary<Type, ItemCounter> ControlUsageCount = new Dictionary<Type, ItemCounter>();

		/// <summary>
		/// Increments the usage count on the given control type
		/// and returns the current count.
		/// </summary>
		/// <param name="controlType"></param>
		/// <returns></returns>
		public long IncrementControlUsageCount(Type controlType)
		{
			if (!IsControlUseCountingEnabled)
			{
				return 0;
			}

			if (!ControlUsageCount.ContainsKey(controlType))
			{
				return 0;
			}

			return ControlUsageCount[controlType].IncrementCount();
		}

		/// <summary>
		/// Gets the current count of controls of the given type
		/// </summary>
		/// <param name="controlType"></param>
		/// <returns></returns>
		public long GetControlUsageCount(Type controlType)
		{
			if (!IsControlUseCountingEnabled)
			{
				return 0;
			}

			if (!ControlUsageCount.ContainsKey(controlType))
			{
				return 0;
			}

			return ControlUsageCount[controlType].GetCount(); ;
		}

		/// <summary>
		/// Resets all per-control counters to zero
		/// </summary>
		public void ResetControlUsageCount()
		{
			foreach (KeyValuePair<Type, ItemCounter> keyset in ControlUsageCount)
			{
				keyset.Value.ResetCount();
			}
		}


		/// <summary>
		/// Adds a new type to the list of items being counted.
		/// </summary>
		/// <param name="controlType"></param>
		public void AddCountedControlType(Type controlType)
		{
			if (!ControlUsageCount.ContainsKey(controlType))
			{
				ControlUsageCount.Add(controlType, new ItemCounter());
			}
		}


	}

}
