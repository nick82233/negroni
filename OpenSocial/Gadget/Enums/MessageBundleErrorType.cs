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

namespace Negroni.OpenSocial.Gadget
{
	/// <summary>
	/// Enumeration of different error types that can be found in message bundles
	/// </summary>
	public enum MessageBundleErrorType
	{
		/// <summary>
		/// No error found
		/// </summary>
		None = 0,
		/// <summary>
		/// Content of message contains illegal data - markup
		/// </summary>
		IllegalContent = 1,
		/// <summary>
		/// Message references another message in a cyclic manner
		/// </summary>
		CircularReference = 2,
		/// <summary>
		/// Locale references an invalid or bad URI for an external messagebundle
		/// </summary>
		BadExternalUri = 3
	}
}
