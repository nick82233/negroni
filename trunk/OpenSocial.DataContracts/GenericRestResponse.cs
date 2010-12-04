using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;



namespace Negroni.OpenSocial.DataContracts
{
	/// <summary>
	/// Response wrapper contract to be used for all public REST endpoint responses
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract(Name = "response", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class GenericRestResponse<T> : BaseResponseEnvelope
	{
		/// <summary>
		/// Actual piece of data being sent in the envelope
		/// </summary>
		[DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
		public T Entry { get; set; }

	}
}
