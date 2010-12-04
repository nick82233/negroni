using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Negroni.OpenSocial.DataContracts.v1;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Person
	{
		public Person()
		{
			this.Id = "-1";
		}

		[DataMemberAttribute(Name = "aboutMe", EmitDefaultValue = false)]
		public string AboutMe { get; set; }

		[DataMemberAttribute(Name = "accounts", EmitDefaultValue = false)]
		public Account Accounts { get; set; }

		[DataMemberAttribute(Name = "activities", EmitDefaultValue = false)]
		public List<string> Activities { get; set; }

		[DataMemberAttribute(Name = "addresses", EmitDefaultValue = false)]
		public List<Address> Addresses { get; set; }

		[DataMemberAttribute(Name = "age", EmitDefaultValue = false)]
		public string Age { get; set; }

		[DataMemberAttribute(Name = "anniversary", EmitDefaultValue = false)]
		public string Anniversary { get; set; }

		[DataMemberAttribute(Name = "userAppData", EmitDefaultValue = false)]
		public UserAppData UserAppData { get; set; }

		[DataMemberAttribute(Name = "birthday", EmitDefaultValue = false)]
		public string Birthday { get; set; }

		[DataMemberAttribute(Name = "bodyType", EmitDefaultValue = false)]
		public BodyType BodyType { get; set; }

		[DataMemberAttribute(Name = "books", EmitDefaultValue = false)]
		public List<string> Books { get; set; }

		[DataMemberAttribute(Name = "cars", EmitDefaultValue = false)]
		public List<string> Cars { get; set; }

		[DataMemberAttribute(Name = "children", EmitDefaultValue = false)]
		public List<string> Children { get; set; }

		[DataMemberAttribute(Name = "connected", EmitDefaultValue = false)]
		public ValueSet Connected { get; set; }

		[DataMemberAttribute(Name = "currentLocation", EmitDefaultValue = false)]
		public Address CurrentLocation { get; set; }

		[DataMemberAttribute(Name = "displayName", EmitDefaultValue = false)]
		public string DisplayName { get; set; }

		[DataMemberAttribute(Name = "drinker", EmitDefaultValue = false)]
		public ValueSet Drinker { get; set; }

		[DataMemberAttribute(Name = "emails", EmitDefaultValue = false)]
		public PluralValueSet[] Emails { get; set; }

		[DataMemberAttribute(Name = "ethnicity", EmitDefaultValue = false)]
		public string Ethnicity { get; set; }

		[DataMemberAttribute(Name = "fashion", EmitDefaultValue = false)]
		public string Fashion { get; set; }

		[DataMemberAttribute(Name = "food", EmitDefaultValue = false)]
		public List<string> Food { get; set; }

		[DataMemberAttribute(Name = "gender", EmitDefaultValue = false)]
		public string Gender { get; set; }

		[DataMemberAttribute(Name = "happiestWhen", EmitDefaultValue = false)]
		public string HappiestWhen { get; set; }

		[DataMemberAttribute(Name = "hasApp", EmitDefaultValue = false)]
		public string HasApp { get; set; }

		[DataMemberAttribute(Name = "heroes", EmitDefaultValue = false)]
		public List<string> Heroes { get; set; }

		[DataMemberAttribute(Name = "humor", EmitDefaultValue = false)]
		public string Humor { get; set; }

		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		[DataMemberAttribute(Name = "ims", EmitDefaultValue = false)]
		public PluralValueSet[] Ims { get; set; }

		[DataMemberAttribute(Name = "interests", EmitDefaultValue = false)]
		public List<string> Interests { get; set; }

		[DataMemberAttribute(Name = "jobInterests", EmitDefaultValue = false)]
		public string JobInterests { get; set; }

		[DataMemberAttribute(Name = "languagesSpoken", EmitDefaultValue = false)]
		public List<string> LanguagesSpoken { get; set; }

		[DataMemberAttribute(Name = "livingArrangement", EmitDefaultValue = false)]
		public string LivingArrangement { get; set; }

		[DataMemberAttribute(Name = "lookingFor", EmitDefaultValue = false)]
		public List<ValueSet> LookingFor { get; set; }

		[DataMemberAttribute(Name = "movies", EmitDefaultValue = false)]
		public List<string> Movies { get; set; }

		[DataMemberAttribute(Name = "music", EmitDefaultValue = false)]
		public List<string> Music { get; set; }

		[DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
		public Name Name { get; set; }

		[DataMemberAttribute(Name = "networkPresence", EmitDefaultValue = false)]
		public ValueSet NetworkPresence { get; set; }

		[DataMemberAttribute(Name = "nickname", EmitDefaultValue = false)]
		public string Nickname { get; set; }

		[DataMemberAttribute(Name = "organizations", EmitDefaultValue = false)]
		public Organization[] Organizations { get; set; }

		[DataMemberAttribute(Name = "pets", EmitDefaultValue = false)]
		public string Pets { get; set; }

		[DataMemberAttribute(Name = "phoneNumbers", EmitDefaultValue = false)]
		public PluralValueSet[] PhoneNumbers { get; set; }

		[DataMemberAttribute(Name = "photos", EmitDefaultValue = false)]
		public PluralValueSet[] Photos { get; set; }

		[DataMemberAttribute(Name = "politicalViews", EmitDefaultValue = false)]
		public string PoliticalViews { get; set; }

		[DataMemberAttribute(Name = "preferredUsername", EmitDefaultValue = false)]
		public string PreferredUsername { get; set; }

		[DataMemberAttribute(Name = "profileSong", EmitDefaultValue = false)]
		public Url ProfileSong { get; set; }

		[DataMemberAttribute(Name = "profileUrl", EmitDefaultValue = false)]
		public string ProfileUrl { get; set; }

		[DataMemberAttribute(Name = "profileVideo", EmitDefaultValue = false)]
		public Url ProfileVideo { get; set; }

		[DataMemberAttribute(Name = "published", EmitDefaultValue = false)]
		public string Published { get; set; }

		[DataMemberAttribute(Name = "quotes", EmitDefaultValue = false)]
		public List<string> Quotes { get; set; }

		[DataMemberAttribute(Name = "relationships", EmitDefaultValue = false)]
		public List<string> Relationships { get; set; }

		[DataMemberAttribute(Name = "relationshipStatus", EmitDefaultValue = false)]
		public string RelationshipStatus { get; set; }

		[DataMemberAttribute(Name = "religion", EmitDefaultValue = false)]
		public string Religion { get; set; }

		[DataMemberAttribute(Name = "romance", EmitDefaultValue = false)]
		public string Romance { get; set; }

		[DataMemberAttribute(Name = "scaredOf", EmitDefaultValue = false)]
		public string ScaredOf { get; set; }

		[DataMemberAttribute(Name = "sexualOrientation", EmitDefaultValue = false)]
		public string SexualOrientation { get; set; }

		[DataMemberAttribute(Name = "smoker", EmitDefaultValue = false)]
		public ValueSet Smoker { get; set; }

		[DataMemberAttribute(Name = "sports", EmitDefaultValue = false)]
		public List<string> Sports { get; set; }

		[DataMemberAttribute(Name = "status", EmitDefaultValue = false)]
		public string Status { get; set; }

		[DataMemberAttribute(Name = "tags", EmitDefaultValue = false)]
		public List<string> Tags { get; set; }

		[DataMemberAttribute(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl { get; set; }

		[DataMemberAttribute(Name = "turnOffs", EmitDefaultValue = false)]
		public List<string> TurnOffs { get; set; }

		[DataMemberAttribute(Name = "turnOns", EmitDefaultValue = false)]
		public List<string> TurnOns { get; set; }

		[DataMemberAttribute(Name = "tvShows", EmitDefaultValue = false)]
		public List<string> TvShows { get; set; }

		[DataMemberAttribute(Name = "updated", EmitDefaultValue = false)]
		public string Updated { get; set; }

		[DataMemberAttribute(Name = "urls", EmitDefaultValue = false)]
		public Url[] Urls { get; set; }

		[DataMemberAttribute(Name = "utcOffset", EmitDefaultValue = false)]
		public string UtcOffset { get; set; }


		[DataMemberAttribute(Name = "msLargeImage", EmitDefaultValue = false)]
		public string LargeImage;

		[DataMemberAttribute(Name = "msMediumImage", EmitDefaultValue = false)]
		public string MediumImage;

		[DataMemberAttribute(Name = "msStatusMood", EmitDefaultValue = false)]
		public StatusMood StatusMood;

		[DataMemberAttribute(Name = "msUserType", EmitDefaultValue = false)]
		public string UserType;

		[DataMemberAttribute(Name = "msZodiacSign", EmitDefaultValue = false)]
		public string ZodiacSign;

	}

}
