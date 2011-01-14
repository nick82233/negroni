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
using Negroni.DataPipeline.Security;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Settings for specific variable escaping and legacy support.
	/// </summary>
	public class DataContextSettings
	{
		/// <summary>
		/// Support resolving legacy message bundle variable format __MSG_something__
		/// </summary>
		public bool SupportLegacyMessageVariables = true;

		private SecurityPolicy _securityPolicy = null;

		/// <summary>
		/// SecurityPolicy to use for this DataContext
		/// </summary>
		public SecurityPolicy SecurityPolicy
		{
			get
			{
				if (_securityPolicy == null)
				{
					_securityPolicy = new SecurityPolicy();
				}
				return _securityPolicy;
			}
		}

		public DataResolutionMechanism ResolutionMechanism { get; set; }

		public DataContextSettings()
		{
			ResolutionMechanism = DataResolutionMechanism.OpenSocialRest09;
		}
	}
}
