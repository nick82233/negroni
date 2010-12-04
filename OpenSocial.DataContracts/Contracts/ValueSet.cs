using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContract(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class ValueSet
	{
		/// <summary>
		/// Value for display purposes
		/// </summary>
		[DataMember(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue { get; set; }

		/// <summary>
		/// Underlying value
		/// </summary>
		[DataMember(Name = "value", EmitDefaultValue = false)]
		public string Value { get; set; }
	}
}
