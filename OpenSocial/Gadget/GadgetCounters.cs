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
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Tooling;

namespace Negroni.OpenSocial.Gadget
{
	/// <summary>
	/// Encapsulation of counters managed for GadgetMaster
	/// </summary>
	public class GadgetCounters
	{
		public GadgetCounters()
		{
			RenderCount.ResetTimesliceCounter();
		}

		private GlobalSliceCounter _renderCount = null;

		/// <summary>
		/// Count of rendering calls
		/// </summary>
		public GlobalSliceCounter RenderCount
		{
			get
			{
				if (null == _renderCount)
				{
					_renderCount = new GlobalSliceCounter();
				}
				return _renderCount;
			}
		}


		private GlobalSliceCounter _parseCount = null;

		/// <summary>
		/// Count of Parse calls
		/// </summary>
		public GlobalSliceCounter ParseCount
		{
			get
			{
				if (null == _parseCount)
				{
					_parseCount = new GlobalSliceCounter();
				}
				return _parseCount;
			}
		}

	}
}
