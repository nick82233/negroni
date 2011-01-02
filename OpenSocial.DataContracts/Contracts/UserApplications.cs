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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	/// <summary>
	/// Object encapsulating a particular user/owner and their associated apps
	/// </summary>
	[DataContract(Name = "userApplication", Namespace = "http://ns.myspace.com/roa/09/userapplication")]
	public class UserApplications
	{
		public UserApplications()
		{
			Applications = new List<ApplicationItem>();
		}
		/// <summary>
		/// User owning these apps
		/// </summary>
		[DataMember(Name = "owner", EmitDefaultValue = false)]
		public Person Owner { get; set; }

		/// <summary>
		/// Total apps installed by this user
		/// </summary>
		[DataMemberAttribute(Name = "totalApps", EmitDefaultValue = false)]
		public string TotalApps { get; set; }

		/// <summary>
		/// List of sponsored applications to show for this user
		/// </summary>
		[DataMemberAttribute(Name = "sponsored", EmitDefaultValue = false)]
		public List<ApplicationItem> SponsoredApps { get; set; }

		/// <summary>
		/// Culture being used for display purposes.  Typically en-US
		/// </summary>
		[DataMemberAttribute(Name = "displayCulture", EmitDefaultValue = false)]
		public string DisplayCulture { get; set; }

		/// <summary>
		/// Member to get or set Applications
		/// </summary>
		[DataMember(Name = "applications", EmitDefaultValue = false)]
		public List<ApplicationItem> Applications { get; set; }

	}
}
