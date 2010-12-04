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
	[DataContract(Name = "request", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class GenericRestRequest<T> : BaseRestRequest
	{
		[DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
		public T Entry { get; set; }

	}
}
