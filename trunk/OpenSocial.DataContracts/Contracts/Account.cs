using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Account
	{
		[DataMemberAttribute(Name = "domain", EmitDefaultValue = false)]
		public string Domain;

		[DataMemberAttribute(Name = "primary", EmitDefaultValue = false)]
		public string Primary;

		[DataMemberAttribute(Name = "userid", EmitDefaultValue = false)]
		public string Userid;

		[DataMemberAttribute(Name = "username", EmitDefaultValue = false)]
		public string Username;

		[DataMemberAttribute(Name = "userId", EmitDefaultValue = false)]
		public string UserId;
	}

}
