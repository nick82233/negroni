using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts.v1
{


	[CollectionDataContract(Name = "entryCollection", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class EntryCollection : List<Entry>
	{
		public EntryCollection(int capacity)
			: base(capacity)
		{
		}

		public EntryCollection()
		{
		}
	}

	[DataContractAttribute(Name = "response", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	[Obsolete("Use the GenericRestResponse<T> object")]
	public partial class LegacyResponse
	{
		[DataMemberAttribute(Name = "itemsPerPage", EmitDefaultValue = false)]
		public string ItemsPerPage;

		[DataMemberAttribute(Name = "startIndex", EmitDefaultValue = false)]
		public string StartIndex;

		[DataMemberAttribute(Name = "totalResults", EmitDefaultValue = false)]
		public string TotalResults;

		[DataMemberAttribute(Name = "isFiltered", EmitDefaultValue = false)]
		public string IsFiltered;

		[DataMemberAttribute(Name = "isSorted", EmitDefaultValue = false)]
		public string IsSorted;

		[DataMemberAttribute(Name = "isUpdatedSince", EmitDefaultValue = false)]
		public string IsUpdatedSince;

		[DataMemberAttribute(Name = "group", EmitDefaultValue = false)]
		public Group Group;

		[DataMemberAttribute(Name = "activity", EmitDefaultValue = false)]
		public Activity Activity;

		[DataMemberAttribute(Name = "person", EmitDefaultValue = false)]
		public Person Person;

		[DataMemberAttribute(Name = "album", EmitDefaultValue = false)]
		public Album Album;

		[DataMemberAttribute(Name = "mediaItem", EmitDefaultValue = false)]
		public MediaItem MediaItem;

		[DataMemberAttribute(Name = "message", EmitDefaultValue = false)]
		public Message Message;

		[DataMemberAttribute(Name = "userAppData", EmitDefaultValue = false)]
		public UserAppData UserAppData;

		//
		// <xs:complexType name="Entry" >
		//  <xs:choice>
		//    <xs:element minOccurs="0" name="activity" type="tns:Activity" />
		//    <xs:element minOccurs="0" name="person" type="tns:Person" />
		//    <xs:element minOccurs="0" name="group" type="tns:Group" />
		//  </xs:choice>
		//</xs:complexType>
		//        
		[DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
		public EntryCollection Entry;

		//
		// TODO: what is the format for this field? I believe it is supposed to define extended fields...
		//
		[DataMemberAttribute(Name = "map", EmitDefaultValue = false)]
		public string Map;

		[DataMemberAttribute(Name = "numOmittedEntries", EmitDefaultValue = false)]
		public string NumOmittedEntries;
	}

	[DataContractAttribute(Name = "entry", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Entry
	{
		[DataMemberAttribute(Name = "activity", EmitDefaultValue = false)]
		public Activity Activity;

		[DataMemberAttribute(Name = "person", EmitDefaultValue = false)]
		public Person Person;

		[DataMemberAttribute(Name = "group", EmitDefaultValue = false)]
		public Group Group;

		[DataMemberAttribute(Name = "album", EmitDefaultValue = false)]
		public Album Album;

		[DataMemberAttribute(Name = "mediaItem", EmitDefaultValue = false)]
		public MediaItem MediaItem;

		[DataMemberAttribute(Name = "message", EmitDefaultValue = false)]
		public Message Message;

		[DataMemberAttribute(Name = "userAppData", EmitDefaultValue = false)]
		public UserAppData UserAppData;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "entry")]
	public partial class AppDataEntry
	{
		[DataMemberAttribute(Name = "key", EmitDefaultValue = false)]
		public string Key;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}


	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Activity
	{
		[DataMemberAttribute(Name = "appId", EmitDefaultValue = false)]
		public string AppId;

		[DataMemberAttribute(Name = "body", EmitDefaultValue = false)]
		public string Body;

		[DataMemberAttribute(Name = "bodyId", EmitDefaultValue = false)]
		public string BodyId;

		[DataMemberAttribute(Name = "externalId", EmitDefaultValue = false)]
		public string ExternalId;

		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id = "myspace.com.activity.-1";

		[DataMemberAttribute(Name = "mediaItems", EmitDefaultValue = false)]
		public MediaItem[] MediaItems;

		[DataMemberAttribute(Name = "postedTime", EmitDefaultValue = false)]
		public string PostedTime;

		[DataMemberAttribute(Name = "priority", EmitDefaultValue = false)]
		public string Priority;

		[DataMemberAttribute(Name = "streamFaviconUrl", EmitDefaultValue = false)]
		public string StreamFaviconUrl;

		[DataMemberAttribute(Name = "streamSourceUrl", EmitDefaultValue = false)]
		public string StreamSourceUrl;

		[DataMemberAttribute(Name = "streamTitle", EmitDefaultValue = false)]
		public string StreamTitle;

		[DataMemberAttribute(Name = "streamUrl", EmitDefaultValue = false)]
		public string StreamUrl;

		[DataMemberAttribute(Name = "templateParams", EmitDefaultValue = false)]
		public ActivityTemplateParams TemplateParams;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;

		[DataMemberAttribute(Name = "titleId", EmitDefaultValue = false)]
		public string TitleId;

		[DataMemberAttribute(Name = "url", EmitDefaultValue = false)]
		public string Url;

		[DataMemberAttribute(Name = "userId", EmitDefaultValue = false)]
		public string UserId;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class ActivityTemplateParams
	{
		[DataMemberAttribute(Name = "PersonKey", EmitDefaultValue = false)]
		public string PersonKey;

		[DataMemberAttribute(Name = "PersonKey.DisplayName", EmitDefaultValue = false)]
		public string PersonKeyDisplayName;

		[DataMemberAttribute(Name = "PersonKey.Id", EmitDefaultValue = false)]
		public string PersonKeyId;

		[DataMemberAttribute(Name = "PersonKey.ProfileUrl", EmitDefaultValue = false)]
		public string PersonKeyProfileUrl;

		[DataMemberAttribute(Name = "person", EmitDefaultValue = false)]
		public Person Person;
	}
	/*
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Person
	{
		[DataMemberAttribute(Name = "aboutMe", EmitDefaultValue = false)]
		public string AboutMe;

		[DataMemberAttribute(Name = "accounts", EmitDefaultValue = false)]
		public Account Accounts;

		[DataMemberAttribute(Name = "activities", EmitDefaultValue = false)]
		public osstrings Activities;

		[DataMemberAttribute(Name = "addresses", EmitDefaultValue = false)]
		public List<Address> Addresses;

		[DataMemberAttribute(Name = "age", EmitDefaultValue = false)]
		public string Age;

		[DataMemberAttribute(Name = "anniversary", EmitDefaultValue = false)]
		public string Anniversary;

		[DataMemberAttribute(Name = "userAppData", EmitDefaultValue = false)]
		public UserAppData UserAppData;

		[DataMemberAttribute(Name = "birthday", EmitDefaultValue = false)]
		public string Birthday;

		[DataMemberAttribute(Name = "bodyType", EmitDefaultValue = false)]
		public BodyType BodyType;

		[DataMemberAttribute(Name = "books", EmitDefaultValue = false)]
		public osstrings Books;

		[DataMemberAttribute(Name = "cars", EmitDefaultValue = false)]
		public osstrings Cars;

		[DataMemberAttribute(Name = "children", EmitDefaultValue = false)]
		public osstrings Children;

		[DataMemberAttribute(Name = "connected", EmitDefaultValue = false)]
		public Presence Connected;

		[DataMemberAttribute(Name = "currentLocation", EmitDefaultValue = false)]
		public Address CurrentLocation;

		[DataMemberAttribute(Name = "displayName", EmitDefaultValue = false)]
		public string DisplayName;

		[DataMemberAttribute(Name = "drinker", EmitDefaultValue = false)]
		public Drinker Drinker;

		[DataMemberAttribute(Name = "emails", EmitDefaultValue = false)]
		public PluralPersonField[] Emails;

		[DataMemberAttribute(Name = "ethnicity", EmitDefaultValue = false)]
		public string Ethnicity;

		[DataMemberAttribute(Name = "fashion", EmitDefaultValue = false)]
		public string Fashion;

		[DataMemberAttribute(Name = "food", EmitDefaultValue = false)]
		public osstrings Food;

		[DataMemberAttribute(Name = "gender", EmitDefaultValue = false)]
		public string Gender;

		[DataMemberAttribute(Name = "happiestWhen", EmitDefaultValue = false)]
		public string HappiestWhen;

		[DataMemberAttribute(Name = "hasApp", EmitDefaultValue = false)]
		public string HasApp;

		[DataMemberAttribute(Name = "heroes", EmitDefaultValue = false)]
		public osstrings Heroes;

		[DataMemberAttribute(Name = "humor", EmitDefaultValue = false)]
		public string Humor;

		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id = "-1";

		[DataMemberAttribute(Name = "ims", EmitDefaultValue = false)]
		public PluralPersonField[] Ims;

		[DataMemberAttribute(Name = "interests", EmitDefaultValue = false)]
		public osstrings Interests;

		[DataMemberAttribute(Name = "jobInterests", EmitDefaultValue = false)]
		public string JobInterests;

		[DataMemberAttribute(Name = "languagesSpoken", EmitDefaultValue = false)]
		public osstrings LanguagesSpoken;

		[DataMemberAttribute(Name = "livingArrangement", EmitDefaultValue = false)]
		public string LivingArrangement;

		[DataMemberAttribute(Name = "lookingFor", EmitDefaultValue = false)]
		public LookingFor[] LookingFor;

		[DataMemberAttribute(Name = "movies", EmitDefaultValue = false)]
		public osstrings Movies;

		[DataMemberAttribute(Name = "music", EmitDefaultValue = false)]
		public osstrings Music;

		[DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
		public Name Name;

		[DataMemberAttribute(Name = "networkPresence", EmitDefaultValue = false)]
		public NetworkPresence NetworkPresence;

		[DataMemberAttribute(Name = "nickname", EmitDefaultValue = false)]
		public string Nickname;

		[DataMemberAttribute(Name = "organizations", EmitDefaultValue = false)]
		public Organization[] Organizations;

		[DataMemberAttribute(Name = "pets", EmitDefaultValue = false)]
		public string Pets;

		[DataMemberAttribute(Name = "phoneNumbers", EmitDefaultValue = false)]
		public PluralPersonField[] PhoneNumbers;

		[DataMemberAttribute(Name = "photos", EmitDefaultValue = false)]
		public PluralPersonField[] Photos;

		[DataMemberAttribute(Name = "politicalViews", EmitDefaultValue = false)]
		public string PoliticalViews;

		[DataMemberAttribute(Name = "preferredUsername", EmitDefaultValue = false)]
		public string PreferredUsername;

		[DataMemberAttribute(Name = "profileSong", EmitDefaultValue = false)]
		public Url ProfileSong;

		[DataMemberAttribute(Name = "profileUrl", EmitDefaultValue = false)]
		public string ProfileUrl;

		[DataMemberAttribute(Name = "profileVideo", EmitDefaultValue = false)]
		public Url ProfileVideo;

		[DataMemberAttribute(Name = "published", EmitDefaultValue = false)]
		public string Published;

		[DataMemberAttribute(Name = "quotes", EmitDefaultValue = false)]
		public osstrings Quotes;

		[DataMemberAttribute(Name = "relationships", EmitDefaultValue = false)]
		public osstrings Relationships;

		[DataMemberAttribute(Name = "relationshipStatus", EmitDefaultValue = false)]
		public string RelationshipStatus;

		[DataMemberAttribute(Name = "religion", EmitDefaultValue = false)]
		public string Religion;

		[DataMemberAttribute(Name = "romance", EmitDefaultValue = false)]
		public string Romance;

		[DataMemberAttribute(Name = "scaredOf", EmitDefaultValue = false)]
		public string ScaredOf;

		[DataMemberAttribute(Name = "sexualOrientation", EmitDefaultValue = false)]
		public string SexualOrientation;

		[DataMemberAttribute(Name = "smoker", EmitDefaultValue = false)]
		public Smoker Smoker;

		[DataMemberAttribute(Name = "sports", EmitDefaultValue = false)]
		public osstrings Sports;

		[DataMemberAttribute(Name = "status", EmitDefaultValue = false)]
		public string Status;

		[DataMemberAttribute(Name = "tags", EmitDefaultValue = false)]
		public osstrings Tags;

		[DataMemberAttribute(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl;

		[DataMemberAttribute(Name = "turnOffs", EmitDefaultValue = false)]
		public osstrings TurnOffs;

		[DataMemberAttribute(Name = "turnOns", EmitDefaultValue = false)]
		public osstrings TurnOns;

		[DataMemberAttribute(Name = "tvShows", EmitDefaultValue = false)]
		public osstrings TvShows;

		[DataMemberAttribute(Name = "updated", EmitDefaultValue = false)]
		public string Updated;

		[DataMemberAttribute(Name = "urls", EmitDefaultValue = false)]
		public Url[] Urls;

		[DataMemberAttribute(Name = "utcOffset", EmitDefaultValue = false)]
		public string UtcOffset;
	}
	*/
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Group
	{
		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class BodyType
	{
		[DataMemberAttribute(Name = "build", EmitDefaultValue = false)]
		public string Build;

		[DataMemberAttribute(Name = "eyeColor", EmitDefaultValue = false)]
		public string EyeColor;

		[DataMemberAttribute(Name = "hairColor", EmitDefaultValue = false)]
		public string HairColor;

		[DataMemberAttribute(Name = "height", EmitDefaultValue = false)]
		public string Height;

		[DataMemberAttribute(Name = "weight", EmitDefaultValue = false)]
		public string Weight;
	}


	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Organization
	{
		[DataMemberAttribute(Name = "address", EmitDefaultValue = false)]
		public Address Address;

		[DataMemberAttribute(Name = "department", EmitDefaultValue = false)]
		public string Department;

		[DataMemberAttribute(Name = "description", EmitDefaultValue = false)]
		public string Description;

		[DataMemberAttribute(Name = "endDate", EmitDefaultValue = false)]
		public string EndDate;

		[DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
		public string Name;

		[DataMemberAttribute(Name = "startDate", EmitDefaultValue = false)]
		public string StartDate;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;

		[DataMemberAttribute(Name = "field", EmitDefaultValue = false)]
		public string Field;

		[DataMemberAttribute(Name = "subField", EmitDefaultValue = false)]
		public string SubField;

		[DataMemberAttribute(Name = "webpage", EmitDefaultValue = false)]
		public string Webpage;

		[DataMemberAttribute(Name = "salary", EmitDefaultValue = false)]
		public string Salary;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Name
	{
		[DataMemberAttribute(Name = "additionalName", EmitDefaultValue = false)]
		public string AdditionalName;

		[DataMemberAttribute(Name = "familyName", EmitDefaultValue = false)]
		public string FamilyName;

		[DataMemberAttribute(Name = "givenName", EmitDefaultValue = false)]
		public string GivenName;

		[DataMemberAttribute(Name = "honorificPrefix", EmitDefaultValue = false)]
		public string HonorificPrefix;

		[DataMemberAttribute(Name = "honorificSuffix", EmitDefaultValue = false)]
		public string HonorificSuffix;

		[DataMemberAttribute(Name = "formatted", EmitDefaultValue = false)]
		public string Formatted;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Album
	{
		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id;

		[DataMemberAttribute(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl;

		[DataMemberAttribute(Name = "caption", EmitDefaultValue = false)]
		public string Caption;

		[DataMemberAttribute(Name = "description", EmitDefaultValue = false)]
		public string Description;

		[DataMemberAttribute(Name = "location", EmitDefaultValue = false)]
		public Address Location;

		[DataMemberAttribute(Name = "ownerId", EmitDefaultValue = false)]
		public string OwnerId;

		[DataMemberAttribute(Name = "mediaType", EmitDefaultValue = false)]
		public string MediaType;

		[DataMemberAttribute(Name = "mediaMimeType", EmitDefaultValue = false)]
		public string[] MediaMimeType;

		[DataMemberAttribute(Name = "mediaItemCount", EmitDefaultValue = false)]
		public string MediaItemCount;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class MediaItem
	{
		[DataMemberAttribute(Name = "mimeType", EmitDefaultValue = false)]
		public string MimeType;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;

		[DataMemberAttribute(Name = "url", EmitDefaultValue = false)]
		public string Url;

		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;

		[DataMemberAttribute(Name = "created", EmitDefaultValue = false)]
		public string Created;

		[DataMemberAttribute(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl;

		[DataMemberAttribute(Name = "description", EmitDefaultValue = false)]
		public string Description;

		[DataMemberAttribute(Name = "duration", EmitDefaultValue = false)]
		public string Duration;

		[DataMemberAttribute(Name = "location", EmitDefaultValue = false)]
		public Address Location;

		[DataMemberAttribute(Name = "language", EmitDefaultValue = false)]
		public string Language;

		[DataMemberAttribute(Name = "albumId", EmitDefaultValue = false)]
		public string AlbumId;

		[DataMemberAttribute(Name = "fileSize", EmitDefaultValue = false)]
		public string FileSize;

		[DataMemberAttribute(Name = "startTime", EmitDefaultValue = false)]
		public string StartTime;

		[DataMemberAttribute(Name = "rating", EmitDefaultValue = false)]
		public string Rating;

		[DataMemberAttribute(Name = "numVotes", EmitDefaultValue = false)]
		public string NumVotes;

		[DataMemberAttribute(Name = "numComments", EmitDefaultValue = false)]
		public string NumComments;

		[DataMemberAttribute(Name = "numViews", EmitDefaultValue = false)]
		public string NumViews;

		[DataMemberAttribute(Name = "tags", EmitDefaultValue = false)]
		public string[] Tags;

		[DataMemberAttribute(Name = "taggedPeople", EmitDefaultValue = false)]
		public string[] TaggedPeople;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Drinker
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Presence
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Smoker
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class CreateActivityPriority
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class EscapeType
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Message
	{
		[DataMemberAttribute(Name = "body", EmitDefaultValue = false)]
		public string Body;

		[DataMemberAttribute(Name = "bodyId", EmitDefaultValue = false)]
		public string BodyId;

		[DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
		public string Title;

		[DataMemberAttribute(Name = "titleId", EmitDefaultValue = false)]
		public string TitleId;

		[DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
		public string Id;

		[DataMemberAttribute(Name = "recipients", EmitDefaultValue = false)]
		public string[] Recipients;

		[DataMemberAttribute(Name = "senderId", EmitDefaultValue = false)]
		public string SenderId;

		[DataMemberAttribute(Name = "timeSent", EmitDefaultValue = false)]
		public string TimeSent;

		[DataMemberAttribute(Name = "inReplyTo", EmitDefaultValue = false)]
		public string InReplyTo;

		[DataMemberAttribute(Name = "replies", EmitDefaultValue = false)]
		public string[] Replies;

		[DataMemberAttribute(Name = "status", EmitDefaultValue = false)]
		public string Status;

		[DataMemberAttribute(Name = "appUrl", EmitDefaultValue = false)]
		public Url AppUrl;

		[DataMemberAttribute(Name = "collectionIds", EmitDefaultValue = false)]
		public string[] CollectionIds;

		[DataMemberAttribute(Name = "updated", EmitDefaultValue = false)]
		public string Updated;

		[DataMemberAttribute(Name = "urls", EmitDefaultValue = false)]
		public Url[] Urls;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public ValueSet Type;
	}
	/*
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class MessageType
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Environment
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class LookingFor
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class NetworkPresence
	{
		[DataMemberAttribute(Name = "displayValue", EmitDefaultValue = false)]
		public string DisplayValue;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}
	*/

	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class PluralPersonField
	{

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;

		[DataMemberAttribute(Name = "primary", EmitDefaultValue = false)]
		public string Primary;
	}
}