using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.myspace.com/2009/comments", Name = "resource")]
	public class GenericResource
	{
		/// <summary>
		/// Type of resource this comment pertains to
		/// </summary>
		[DataMemberAttribute(Name = "resourceType", EmitDefaultValue = false)]
		public string ResourceType { get; set; }

		/// <summary>
		/// UniqueId of the resource item this comment pertains to
		/// </summary>
		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string ResourceId { get; set; }

		/// <summary>
		/// Native culture of this resource
		/// </summary>
		[DataMemberAttribute(Name = "culture", EmitDefaultValue = false)]
		public string Culture { get; set; }

		/// <summary>
		/// Textual name of this resource
		/// </summary>
		[DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
		public string Name { get; set; }

		/// <summary>
		/// URI for the thumbnail image of this resource
		/// </summary>
		[DataMemberAttribute(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl { get; set; }

		/// <summary>
		/// Member to get or set MediumImageUrl
		/// </summary>
		[DataMember(Name = "mediumImageUrl", EmitDefaultValue = false)]
		public string MediumImageUrl { get; set; }

		/// <summary>
		/// Member to get or set LargeImageUrl
		/// </summary>
		[DataMember(Name = "largeImageUrl", EmitDefaultValue = false)]
		public string LargeImageUrl { get; set; }

		/// <summary>
		/// URI to access this resource
		/// </summary>
		[DataMemberAttribute(Name = "resourceUrl", EmitDefaultValue = false)]
		public string ResourceUrl { get; set; }

	}
}
