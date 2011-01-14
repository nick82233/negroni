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

namespace Negroni.DataPipeline.Security
{
	public class SecurityPolicy
	{

		private EL_Escaping _el_Escaping = EL_Escaping.None;

		/// <summary>
		/// Escaping to be applied to EL statements by the DataContext
		/// </summary>
		public EL_Escaping EL_Escaping
		{
			get
			{
				return _el_Escaping;
			}
			set
			{
				_el_Escaping = value;
			}
		}

		/// <summary>
		/// Escaping to be applied to UserPref variables by the DataContext
		/// </summary>
		public UserPrefEscaping UserPrefEscaping { get; set; }
	}
}
