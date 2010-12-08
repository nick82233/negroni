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
	/// Key constants for gadget lifecycle event callbacks defined with Link items
	/// </summary>
	public static class LifecycleEventKey
	{
		public const string ADD_APP = "event.addapp";
		public const string REMOVE_APP = "event.removeapp";
	}
}
