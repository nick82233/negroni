using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;



namespace Negroni.OpenSocial.DataContracts
{
	/// <summary>
	/// Generic REST request wrapper for public endpoints.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract]
	public class BaseRestRequest
	{
		[DataMemberAttribute(Name = "itemsPerPage", EmitDefaultValue = false)]
		public string ItemsPerPage { get; set; }

		[DataMemberAttribute(Name = "startIndex", EmitDefaultValue = false)]
		public string StartIndex { get; set; }

		[DataMemberAttribute(Name = "filter", EmitDefaultValue = false)]
		public string Filter { get; set; }
	}
}
