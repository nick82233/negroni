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
