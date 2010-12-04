using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Negroni.ServiceLayer.RestExtensions.DataContracts
{
	[DataContract(Name="itemCount")]
	public class ItemCount
	{
		[DataMember(Name="id")]
		public string Id { get; set; }

		[DataMember(Name="name")]
		public string Name { get; set; }

		/// <summary>
		/// Location URI for this resource
		/// </summary>
		[DataMember(Name = "uri", EmitDefaultValue = false)]
		public string Uri { get; set; }

		/// <summary>
		/// Total value represented by this count.
		/// Often this is the sum of individual values
		/// </summary>
		[DataMember(Name = "total")]
		public int Total { get; set; }


		/// <summary>
		/// Count of the individual data points to make up total
		/// </summary>
		[DataMember(Name = "count", EmitDefaultValue = false)]
		public int Count { get; set; }

	}
}
