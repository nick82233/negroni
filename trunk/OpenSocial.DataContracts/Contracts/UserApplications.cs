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
