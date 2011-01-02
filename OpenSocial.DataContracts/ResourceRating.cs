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
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Name = "rating")]
	public class ResourceRating
	{

		/// <summary>
		/// User performing this rating action
		/// </summary>
		[DataMember(Name = "userId", EmitDefaultValue = false)]
		public string UserId { get; set; }

		/// <summary>
		/// UniqueId of the resource item this comment pertains to
		/// </summary>
		[DataMemberAttribute(Name = "resourceId", EmitDefaultValue = false)]
		public string ResourceId { get; set; }


		/// <summary>
		/// Value of the rating.  Typically 100 == like, 0 == dislike
		/// </summary>
		[DataMember(Name = "userRating", EmitDefaultValue = false)]
		public string UserRating { get; set; }


		/// <summary>
		/// Member to get or set RateOn
		/// </summary>
		[DataMember(Name = "rateOn", EmitDefaultValue = false)]
		public string RateOn { get; set; }


		/// <summary>
		/// LocationID for page where rating was performed, or null for unknown
		/// </summary>
		[DataMember(Name = "locationId", EmitDefaultValue = false)]
		public string LocationId { get; set; }

		/// <summary>
		/// Member to get or set Resource
		/// </summary>
		[DataMember(Name = "resource", EmitDefaultValue = false)]
		public GenericResource Resource { get; set; }

	}
}
