using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts.v1
{
	//Obsolete class
    public partial class LegacyResponse
    {
        [DataMemberAttribute(Name = "appInviteMessage", EmitDefaultValue = false)]
        public AppInviteMessage AppInviteMessage;

        //[DataMemberAttribute(Name = "Request", EmitDefaultValue = false)]
        //public DataPipelineRequestParameters OriginalRequestParams;
    }


    public partial class Album
    {
        [DataMemberAttribute(Name = "msPrivacyLevel", EmitDefaultValue = false)]
        public string PrivacyLevel;
    }

    public partial class MediaItem
    {
        [DataMemberAttribute(Name = "msCategories", EmitDefaultValue = false)]
        public string[] Categories;

        [DataMemberAttribute(Name = "msPrivacyLevel", EmitDefaultValue = false)]
        public string PrivacyLevel;

        [DataMemberAttribute(Name = "msMediaItemUri", EmitDefaultValue = false)]
        public string Uri;

        [DataMemberAttribute(Name = "msNumTags", EmitDefaultValue = false)]
        public string NumTags;

        [DataMemberAttribute(Name = "msTaggedPeople", EmitDefaultValue = false)]
        public List<TaggedPerson> MSTaggedPeople;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "taggedPerson")]
    public class TaggedPerson : Person
    {
        [DataMemberAttribute(Name = "x1", EmitDefaultValue = false)]
        public string X1;

        [DataMemberAttribute(Name = "y1", EmitDefaultValue = false)]
        public string Y1;

        [DataMemberAttribute(Name = "x2", EmitDefaultValue = false)]
        public string X2;

        [DataMemberAttribute(Name = "y2", EmitDefaultValue = false)]
        public string Y2;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "entry")]
    public partial class Params
    {
        [DataMemberAttribute(Name = "key", EmitDefaultValue = false)]
        public string Key;

        [DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
        public string Value;
    }

    public partial class ActivityTemplateParams
    {
        [DataMemberAttribute(Name = "msParameters", EmitDefaultValue = false)]
        public Params[] Params;
    }

    public partial class Activity
    {
        [DataMemberAttribute(Name = "actor", EmitDefaultValue = false)]
        public ActivityObject Actor;

        [DataMemberAttribute(Name = "context", EmitDefaultValue = false)]
        public ActivityContext Context;

        [DataMemberAttribute(Name = "links", EmitDefaultValue = false)]
        public string[] Links;

        [DataMemberAttribute(Name = "objects", EmitDefaultValue = false)]
        public ActivityObject[] Objects;

        [DataMemberAttribute(Name = "source", EmitDefaultValue = false)]
        public ActivityObject Source;

        [DataMemberAttribute(Name = "target", EmitDefaultValue = false)]
        public ActivityObject Target;

        [DataMemberAttribute(Name = "verbs", EmitDefaultValue = false)]
        public string[] Verbs;

        [DataMemberAttribute(Name = "msActivityType", EmitDefaultValue = false)]
        public string ActivityType;

        [DataMemberAttribute(Name = "msPerson", EmitDefaultValue = false)]
        public Person MSPerson;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "activityObject")]
    public class ActivityObject
    {
        [DataMemberAttribute(Name = "owner", EmitDefaultValue = false)]
        public ActivityObject Owner;

        [DataMemberAttribute(Name = "detail", EmitDefaultValue = false)]
        public string Detail;

        [DataMemberAttribute(Name = "links", EmitDefaultValue = false)]
        public string[] Links;

        [DataMemberAttribute(Name = "source", EmitDefaultValue = false)]
        public ActivityObject Source;

        [DataMemberAttribute(Name = "time", EmitDefaultValue = false)]
        public string Time;

        [DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
        public string Id;

        [DataMemberAttribute(Name = "title", EmitDefaultValue = false)]
        public string Title;

        [DataMemberAttribute(Name = "objectTypes", EmitDefaultValue = false)]
        public string[] ObjectTypes;

        [DataMemberAttribute(Name = "vevent", EmitDefaultValue = false)]
        public VEvent VEvent;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "activityContext")]
    public class ActivityContext
    {
        [DataMemberAttribute(Name = "location", EmitDefaultValue = false)]
        public Address Location;

        [DataMemberAttribute(Name = "msStatusMood", EmitDefaultValue = false)]
        public StatusMood StatusMood;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "activityVerb")]
    public class ActivityVerb
    {
        [DataMemberAttribute(Name = "uri", EmitDefaultValue = false)]
        public string Uri;

        [DataMemberAttribute(Name = "derivesFrom", EmitDefaultValue = false)]
        public ActivityVerb DerivesFrom;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "activityObjectType")]
    public class ActivityObjectType
    {
        [DataMemberAttribute(Name = "uri", EmitDefaultValue = false)]
        public string Uri;

        [DataMemberAttribute(Name = "derivesFrom", EmitDefaultValue = false)]
        public ActivityObjectType DerivesFrom;
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial", Name = "vevent")]
    public class VEvent
    {
        [DataMemberAttribute(Name = "dtStart", EmitDefaultValue = false)]
        public string StartDate;

        [DataMemberAttribute(Name = "location", EmitDefaultValue = false)]
        public Address Location;

        [DataMemberAttribute(Name = "summary", EmitDefaultValue = false)]
        public string Summary;

        [DataMemberAttribute(Name = "uid", EmitDefaultValue = false)]
        public string UniqueId;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/statusmood", Name = "source")]
    public class Source
    {
        [DataMemberAttribute(Name = "externalId", EmitDefaultValue = false)]
        public string ExternalId; //id of the user on the app from where user's statusmood was updated

        [DataMemberAttribute(Name = "appId", EmitDefaultValue = false)]
        public string AppId; // appId for MDP apps

        [DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
        public string Name; // source(app) name for offsite (MySpace) apps like twitter, from where status was updated

        [DataMemberAttribute(Name = "url", EmitDefaultValue = false)]
        public string Url; // it could be url to user's profile or statusmood module for that source or link to that service

        [DataMemberAttribute(Name = "imageUrl", EmitDefaultValue = false)]
        public string ImageUrl; // image url for that app 
    }

    public partial class Message
    {
        [DataMemberAttribute(Name = "msDraftGuid", EmitDefaultValue = false)]
        public string DraftGuid;

        [DataMemberAttribute(Name = "msMessageType", EmitDefaultValue = false)]
        public string MSMessageType;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/statusmood")]
    public class StatusMood
    {
        [DataMemberAttribute(Name = "userId", EmitDefaultValue = false)]
        public string UserId;

        [DataMemberAttribute(Name = "author", EmitDefaultValue = false)]
        public Person Person;

        [DataMemberAttribute(Name = "moodId", EmitDefaultValue = false)]
        public string MoodId; // Id of the mood supported on MySpace

        [DataMemberAttribute(Name = "moodName", EmitDefaultValue = false)]
        public string MoodName; // custom mood value

        [DataMemberAttribute(Name = "moodPictureName", EmitDefaultValue = false)]
        public string MoodPictureName; // picture name

        [DataMemberAttribute(Name = "moodPictureUrl", EmitDefaultValue = false)]
        public string MoodPictureUrl; // picture Url

        [DataMemberAttribute(Name = "status", EmitDefaultValue = false)]
        public string Status; // status value

        [DataMemberAttribute(Name = "statusId", EmitDefaultValue = false)]
        public string StatusId; // statusId

        [DataMemberAttribute(Name = "moodStatusLastUpdated", EmitDefaultValue = false)]
        public string MoodStatusLastUpdated;

        [DataMemberAttribute(Name = "currentLocation", EmitDefaultValue = false)]
        public Address CurrentLocation;

        [DataMemberAttribute(Name = "recentComments", EmitDefaultValue = false)]
        public List<Comment> RecentComments;

        [DataMemberAttribute(Name = "numComments", EmitDefaultValue = false)]
        public string TotalComments;

        [DataMemberAttribute(Name = "source", EmitDefaultValue = false)]
        public Source Source;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/statusmood")]
    public class StatusMoodList
    {
        [DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
        public List<StatusMood> StatusMoodCollection;

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

        [DataMemberAttribute(Name = "numOmittedEntries", EmitDefaultValue = false)]
        public string NumOmittedEntries;
    }


    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/comments", Name = "profileComments")]
    public class ProfileComments
    {
        [DataMemberAttribute(Name = "personId", EmitDefaultValue = false)]
        public string PersonId; // Id of the user whose comments retrieved

        [DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
        public List<Comment> Comments; // picture Url

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

        [DataMemberAttribute(Name = "numOmittedEntries", EmitDefaultValue = false)]
        public string NumOmittedEntries;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/comments", Name = "statusmoodComments")]
    public class StatusMoodComments
    {
        [DataMemberAttribute(Name = "personId", EmitDefaultValue = false)]
        public string PersonId; // Id of the user whose comments retrieved

        [DataMemberAttribute(Name = "postedDate", EmitDefaultValue = false)]
        public string PostedDate;

        [DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
        public List<Comment> Comments;

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

        [DataMemberAttribute(Name = "numOmittedEntries", EmitDefaultValue = false)]
        public string NumOmittedEntries;

        [DataMemberAttribute(Name = "statusId", EmitDefaultValue = false)]
        public string StatusId;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/comments", Name = "mediaItemComments")]
    public class MediaItemComments
    {
        [DataMemberAttribute(Name = "personId", EmitDefaultValue = false)]
        public string PersonId; // Id of the user whose comments retrieved

        [DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
        public string Id;

        [DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
        public List<Comment> Comments;

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

        [DataMemberAttribute(Name = "numOmittedEntries", EmitDefaultValue = false)]
        public string NumOmittedEntries;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/statusmood")]
    public class MoodDefinition
    {
        [DataMemberAttribute(Name = "moodId", EmitDefaultValue = false)]
        public string MoodId; // Id of the mood supported on MySpace

        [DataMemberAttribute(Name = "moodName", EmitDefaultValue = false)]
        public string MoodName; // custom mood value

        [DataMemberAttribute(Name = "moodPictureName", EmitDefaultValue = false)]
        public string MoodPictureName; // picture name

        [DataMemberAttribute(Name = "moodPictureUrl", EmitDefaultValue = false)]
        public string MoodPictureUrl; // picture Url
    }

    [CollectionDataContractAttribute(Namespace = "http://ns.myspace.com/2009/statusmood")]
    public class MoodDefinitionList : List<MoodDefinition>
    {
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/videos", Name = "category")]
    public class Category
    {
        [DataMemberAttribute(Name = "id", EmitDefaultValue = false)]
        public string Id; // category Id

        [DataMemberAttribute(Name = "name", EmitDefaultValue = false)]
        public string Name; // category name

        [DataMemberAttribute(Name = "description", EmitDefaultValue = false)]
        public string Description; // category desc
    }

    [CollectionDataContractAttribute(Namespace = "http://ns.myspace.com/2009/videos", Name="videoCategories")]
    public class VideoCategoryList : List<Category>
    {
    }

    [DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public class Result
    {
        [DataMemberAttribute(Name = "statusLink", EmitDefaultValue = false)]
        public string StatusLink; // This will be a URI to newly created resource
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appinviterequest")]
    public class AppInviteRequest
    {
	    [DataMemberAttribute(Name = "recipientIds", EmitDefaultValue = false)]
	    public List<int> RecipientIds;

	    [DataMemberAttribute(Name = "inviteText", EmitDefaultValue = false)]
	    public string InviteText;

	    [DataMemberAttribute(Name = "appParams", EmitDefaultValue = false)]
	    public Params[] AppParams;
    }

    /// <summary>
    /// This is the response to an appp invite REST request
    /// Not related to user responses to app invites
    /// </summary>
    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appinviteresult")]
    public class AppInviteResponse
    {
	    [DataMemberAttribute(Name = "successfulRecipientIds", EmitDefaultValue = false)]
	    public List<int> SuccessfulRecipientIds;

	    [DataMemberAttribute(Name = "failedRecipientIds", EmitDefaultValue = false)]
	    public List<int> FailedRecipientIds;

	    [DataMemberAttribute(Name = "statusLinks", EmitDefaultValue = false)]
	    public List<string> StatusLinks; // URIs of each newly created app invite resource
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appinvitefields")]
    public class AppInviteMessage
    {
	    [DataMemberAttribute(Name = "messageId", EmitDefaultValue = false)]
	    public int MessageId;

	    [DataMemberAttribute(Name = "senderId", EmitDefaultValue = false)]
	    public int SenderId;

	    [DataMemberAttribute(Name = "recipeintId", EmitDefaultValue = false)]
	    public int RecipientId;

	    [DataMemberAttribute(Name = "createdDate", EmitDefaultValue = false)]
	    public DateTime CreatedDate;

	    [DataMemberAttribute(Name = "replacementXml", EmitDefaultValue = false)]
	    public string ReplacementXml;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/notification")]
    public class Notification
    {
        [DataMemberAttribute(Name = "templateId", EmitDefaultValue = false)]
        public string TemplateId;

        [DataMemberAttribute(Name = "recipientIds", EmitDefaultValue = false)]
        public List<string> RecipientIds;

        [DataMemberAttribute(Name = "templateParameters", EmitDefaultValue = false)]
        public Params[] TemplateParameters;

        [DataMemberAttribute(Name = "mediaItems", EmitDefaultValue = false)]
        public List<MediaItem> MediaItems;
    }

    /// <summary>
    /// Application GadgetXML data contract.
    /// This contains the actual app markup that can be converted to a live <c>GadgetMaster</c>
    /// and rendered.
    /// </summary>
    [DataContractAttribute(Name = "gadgetXmlDefinition", Namespace = "http://ns.opensocial.org/2008/markup")]
    public class GadgetXmlDefinition
    {
	    /// <summary>
	    /// Internal application ID
	    /// </summary>
	    [DataMemberAttribute(Name = "appId")]
	    public string AppId;

	    /// <summary>
	    /// Application gadget XML content of the app gadget.
	    /// </summary>
	    [DataMemberAttribute(Name = "gadgetXml")]
	    public string GadgetXml;

	    /// <summary>
	    /// Custom tag templates defined. May be all or just external templates
	    /// </summary>
	    [DataMemberAttribute(Name = "templates")]
	    public string Templates;

	    /// <summary>
	    /// Message Bundles defined for this gadget
	    /// </summary>
	    [DataMemberAttribute(Name = "messageBundles")]
	    public string MessageBundles;


	    /// <summary>
	    /// OpenSocial version.  Should be "0.9" and
	    /// match PreferredOpenSocialVersion
	    /// </summary>
	    [DataMemberAttribute(Name = "osVersion")]
	    public string OpenSocialVersion;

	    /// <summary>
	    /// Offset string used to reconstruct the Offsets object
	    /// and define all controls within the gadget
	    /// </summary>
	    [DataMemberAttribute(Name = "offsets")]
	    public string Offsets;

	    /// <summary>
	    /// Parsing factory key
	    /// </summary>
	    [DataMemberAttribute(Name = "controlFactoryKey")]
	    public string ControlFactoryKey;

	    /// <summary>
	    /// Private version of the app. Values are:
	    /// published | latest | dev1 | pub1
	    /// </summary>
	    [DataMemberAttribute(Name = "appVersion")]
	    public string AppVersion;
    }

    /// <summary>
    /// Update result for GadgetXmlUpdater.  Includes any parsing errors discovered
    /// when attempting to save the app.
    /// </summary>
    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/applifecycle")]
    public class AppLifecycleOperationResult
    {
	    /// <summary>
	    /// Result of the update operation.
	    /// Will be either success or failure
	    /// </summary>
	    [DataMemberAttribute(Name = "operationResult", IsRequired = true)]
	    public string OperationResult;

	    /// <summary>
	    /// Originally requested action
	    /// </summary>
	    [DataMemberAttribute(Name = "requestedAction", IsRequired = true)]
	    public string RequestedAction;

	    /// <summary>
	    /// If update fails this will hold the error message
	    /// </summary>
	    [DataMemberAttribute(Name = "errorMessage")]
	    public string ErrorMessage;

	    /// <summary>
	    /// ApplicationID affected by this operation
	    /// </summary>
	    [DataMemberAttribute(Name = "appId")]
	    public string AppId;

    }

    #region App GadgetXML update related contracts

    /// <summary>
    /// Update result for GadgetXmlUpdater.  Includes any parsing errors discovered
    /// when attempting to save the app.
    /// </summary>
    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appmetainfo")]
    public class AppGadgetUpdateResult
    {
	    /// <summary>
	    /// Result of the update operation.
	    /// Will be either success or failure
	    /// </summary>
	    [DataMemberAttribute(Name = "updateResult", IsRequired = true)]
	    public string UpdateResult;

	    /// <summary>
	    /// List of parsing errors in the app gadget
	    /// </summary>
	    [DataMemberAttribute(Name = "parseErrors")]
	    public List<GadgetParseError> ParseErrors;

	    /// <summary>
	    /// List of errors within the data request tags
	    /// </summary>
	    [DataMemberAttribute(Name = "dataRequestErrors")]
	    public List<GadgetDataKeyError> DataRequestErrors;

	    /// <summary>
	    /// List of errors within the message bundles
	    /// </summary>
	    [DataMemberAttribute(Name = "messageBundleErrors")]
	    public List<GadgetDataKeyError> MessageBundleErrors;

	    /// <summary>
	    /// Errors that occurred when processing expression language statements
	    /// during a rendering of the app.
	    /// </summary>
	    [DataMemberAttribute(Name = "expressionLanguageRenderErrors")]
	    public List<GadgetDataKeyError> ExpressionLanguageRenderErrors;

	    /// <summary>
	    /// Errors that occurred when processing the metadata contained in the
	    /// gadget XML.
	    /// </summary>
        [DataMemberAttribute(Name = "metaDataErrors")]
        public List<GadgetMetaDataError> MetaDataErrors;

	    /// <summary>
	    /// If update fails this will hold the error message
	    /// </summary>
	    [DataMemberAttribute(Name = "errorMessage")]
	    public string ErrorMessage;

		/// <summary>
		/// If any non-critical problems were found, they are listed here
		/// </summary>
		[DataMemberAttribute(Name = "warnings")]
		public List<string> Warnings;

	    /// <summary>
	    /// App meta information
	    /// </summary>
	    [DataMemberAttribute(Name = "appInfo")]
	    public AppMetaInformation AppInfo;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appmetainfo")]
    public class GadgetMetaDataError
    {
        public GadgetMetaDataError() { }
    	
        public GadgetMetaDataError(string message)
	    {
		    this.Message = message;
	    }

        public GadgetMetaDataError(string message, string dataItemName)
	    {
		    this.Message = message;
            this.DataItemName = dataItemName;
	    }

        /// <summary>
        /// Error message
        /// </summary>
        [DataMemberAttribute(Name = "message")]
        public string Message;

        /// <summary>
        /// Data item name
        /// </summary>
        [DataMemberAttribute(Name = "dataItemname")]
        public string DataItemName;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appmetainfo")]
    public class GadgetParseError
    {
	    /// <summary>
	    /// Error message
	    /// </summary>
	    [DataMemberAttribute(Name = "message")]
	    public string Message;

	    /// <summary>
	    /// Surrounding XML location information
	    /// </summary>
	    [DataMemberAttribute(Name = "location")]
	    public string Location;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appmetainfo")]
    public class GadgetDataKeyError
    {

	    public GadgetDataKeyError() { }
	    public GadgetDataKeyError(string key)
	    {
		    Key = key;
	    }
	    public GadgetDataKeyError(string key, string message)
	    {
		    Key = key;
		    Message = message;
	    }

	    /// <summary>
	    /// Data key affected by this error
	    /// </summary>
	    [DataMemberAttribute(Name = "key")]
	    public string Key;

	    /// <summary>
	    /// Error message
	    /// </summary>
	    [DataMemberAttribute(Name = "message")]
	    public string Message;
    }

    /// <summary>
    /// Meta information about an app that doesn't contain the actual app markup.
    /// This is returned by the GadgetXmlUpdater
    /// </summary>
    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/appmetainfo")]
    public class AppMetaInformation
    {
	    /// <summary>
	    /// Internal application ID
	    /// </summary>
	    [DataMemberAttribute(Name = "appId", IsRequired = true)]
	    public string AppId;

	    /// <summary>
	    /// Application name in the default language
	    /// </summary>
	    [DataMemberAttribute(Name = "name")]
	    public string Name;

	    /// <summary>
	    /// App description in the default language
	    /// </summary>
	    [DataMemberAttribute(Name = "description")]
	    public string Description;

	    /// <summary>
	    /// Thumbnail image URL (large image)
	    /// </summary>
	    [DataMemberAttribute(Name = "thumbnail")]
	    public string Thumbnail;

	    /// <summary>
	    /// Icon image URL (small image)
	    /// </summary>
	    [DataMemberAttribute(Name = "icon")]
	    public string Icon;

	    /// <summary>
	    /// OpenSocial version.  Should be "0.9" and
	    /// match PreferredOpenSocialVersion
	    /// </summary>
	    [DataMemberAttribute(Name = "osVersion")]
	    public string OpenSocialVersion;
    }
    #endregion

    public partial class Entry
    {
        [DataMemberAttribute(Name = "appInviteMessage", EmitDefaultValue = false)]
        public AppInviteMessage AppInviteMessage;

        [DataMemberAttribute(Name = "friendRequest", EmitDefaultValue = false)]
        public FriendRequest FriendRequest;

        [DataMemberAttribute(Name = "friendRequestResult", EmitDefaultValue = false)]
        public FriendRequestResult FriendRequestResult;
    }

    [DataContractAttribute(Name = "entry", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public partial class FriendRequest
    {
        [DataMemberAttribute(Name = "action", EmitDefaultValue = false)]
        public string Action;

        [DataMemberAttribute(Name = "friendCategoryIds", EmitDefaultValue = false)]
        public List<string> FriendCategoryIds;

        [DataMemberAttribute(Name = "friend", EmitDefaultValue = false)]
        public Person Friend;

        [DataMemberAttribute(Name = "markAsSpam", EmitDefaultValue = false)]
        public string MarkAsSpam;

        [DataMemberAttribute(Name = "notes", EmitDefaultValue = false)]
        public string Notes;

        [DataMemberAttribute(Name = "requestDate", EmitDefaultValue = false)]
        public string RequestDate;

        [DataMemberAttribute(Name = "requestId", EmitDefaultValue = false)]
        public string RequestId;

        [DataMemberAttribute(Name = "subscribeToActivities", EmitDefaultValue = false)]
        public string SubscribeToActivities;
    }

    [DataContractAttribute(Name = "entry", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public partial class FriendRequestResult
    {
        [DataMemberAttribute(Name = "requestId", EmitDefaultValue = false)]
        public string RequestId;

        [DataMemberAttribute(Name = "action", EmitDefaultValue = false)]
        public string Action;

        [DataMemberAttribute(Name = "success", EmitDefaultValue = false)]
        public string Success;

        [DataMemberAttribute(Name = "reason", EmitDefaultValue = false)]
        public string Reason;

        [DataMemberAttribute(Name = "friendId", EmitDefaultValue = false)]
        public string FriendId;
    }

    //[DataContractAttribute(Name = "entry", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    //public partial class FriendRequest
    //{
    //  [DataMemberAttribute(Name = "requestId", EmitDefaultValue = false)]
    //  public int RequestId;

    //  [DataMemberAttribute(Name = "friendId", EmitDefaultValue = false)]
    //  public int FriendId;

    //  [DataMemberAttribute(Name = "requestDate", EmitDefaultValue = false)]
    //  public DateTime RequestDate;

    //  [DataMemberAttribute(Name = "showFullName", EmitDefaultValue = false)]
    //  public bool ShowFullName;

    //  [DataMemberAttribute(Name = "notes", EmitDefaultValue = false)]
    //  public string Notes;

    //  [DataMemberAttribute(Name = "subscribeToActivities", EmitDefaultValue = false)]
    //  public bool SubscribeToActivities;

    //  [DataMemberAttribute(Name = "markAsSpam", EmitDefaultValue = false)]
    //  public bool MarkAsSpam;
    //}

    //[DataContractAttribute(Name = "entry", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    //public partial class FriendRequestSend
    //{
    // [DataMemberAttribute(Name = "friendId", EmitDefaultValue = false)]
    //  public int FriendId;

    //  [DataMemberAttribute(Name = "friendCategories\\friendCategoryId ", EmitDefaultValue = false)]
    //  public long[] FriendCategoryIds;

    //  [DataMemberAttribute(Name = "firstName", EmitDefaultValue = false)]
    //  public string FirstName;

    //  [DataMemberAttribute(Name = "lastName", EmitDefaultValue = false)]
    //  public string LastName;

    //  [DataMemberAttribute(Name = "emailAddress", EmitDefaultValue = false)]
    //  public string EmailAddress;

    //  [DataMemberAttribute(Name = "showFullname", EmitDefaultValue = false)]
    //  public bool ShowFullname;

    //  [DataMemberAttribute(Name = "subcribeToActivities", EmitDefaultValue = false)]
    //  public bool SubcribeToActivities;

    //  [DataMemberAttribute(Name = "notes", EmitDefaultValue = false)]
    //  public string Notes;
    //}

    //[DataContractAttribute(Name = "StatusLinks", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    //public partial class StatusLinks
    //{
    //    [DataMemberAttribute(Name = "link", EmitDefaultValue = false)]
    //    public string[] Link;
    //}

    [DataContractAttribute(Name = "AppAnalyticsDataPoints", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public class AppAnalyticsDataPoints
    {
        [DataMemberAttribute(Name = "appid", EmitDefaultValue = false)]
        public int ApplicationId { get; set; }

        [DataMemberAttribute(Name = "category", EmitDefaultValue = false)]
        public string Category { get; set; }

        [DataMemberAttribute(Name = "categoryname", EmitDefaultValue = false)]
        public string CategoryName { get; set; }

        [DataMemberAttribute(Name = "context", EmitDefaultValue = false)]
        public int Context { get; set; } //OpenSocial.EventContext ENUM

        [DataMemberAttribute(Name = "errormessage", EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMemberAttribute(Name = "friendlyname", EmitDefaultValue = false)]
        public string FriendlyName { get; set; }

        [DataMemberAttribute(Name = "messagetype", EmitDefaultValue = false)]
        public int MessageType { get; set; }  //OpenSocial.EventMetric ENUM

        [DataMemberAttribute(Name = "segment", EmitDefaultValue = true)]
        public int Segment { get; set; } //OpenSocial.Segment ENUM

        [DataMemberAttribute(Name = "segmentname", EmitDefaultValue = false)]
        public string SegmentName { get; set; }
    }

    [DataContractAttribute(Name = "AppAnalyticsData", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public class AppAnalyticsData 
    {
        [DataMemberAttribute(Name = "appid", EmitDefaultValue = false)]
        public int ApplicationId { get; set; }

        [DataMemberAttribute(Name = "category", EmitDefaultValue = false)]
        public string Category { get; set; }

        [DataMemberAttribute(Name = "context", EmitDefaultValue = false)]
        public int Context { get; set; } //OpenSocial.EventContext ENUM

        [DataMemberAttribute(Name = "errormessage", EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }
        
        [DataMemberAttribute(Name = "messagetype", EmitDefaultValue = false)]
        public int MessageType { get; set; }  //OpenSocial.EventMetric ENUM

        [DataMemberAttribute(Name = "segment", EmitDefaultValue = true)]
        public int Segment { get; set; } //OpenSocial.Segment ENUM

        [DataMemberAttribute(Name = "points", EmitDefaultValue = false)]
        public List<KeyValuePair<DateTime, long>> Points { get; set; }
    }

    [DataContractAttribute(Name = "AppAnalyticsDataResponse", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public partial class AppAnalyticsDataPointsResponse
    {
        [DataMemberAttribute(Name = "datapoints", EmitDefaultValue = false)]
        public List<AppAnalyticsDataPoints> DataPoints;
    }

    [DataContractAttribute(Name = "AppAnalyticsDataResponse", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public partial class AppAnalyticsDataResponse
    {
        [DataMemberAttribute(Name = "dataset", EmitDefaultValue = false)]
        public List<AppAnalyticsData> DataSet;
        
        [DataMemberAttribute(Name = "datapoints", EmitDefaultValue = false)]
        public List<AppAnalyticsDataPoints> DataPoints;
    }

    [DataContractAttribute(Namespace = "http://ns.myspace.com/2009/invites")]
    public class Invites
    {
        [DataMemberAttribute(Name = "emailIds", EmitDefaultValue = false)]
        public List<string> EmailIds;

        /// <summary>
        // culture name in the format "<languagecode2>-<country/regioncode2>"
        /// </summary>
        [DataMemberAttribute(Name = "culture", EmitDefaultValue = false)]
        public string culture;
    }

	/*
    /// <summary>
    /// Originating request when processed from a data pipeline tag
    /// </summary>
    [DataContractAttribute(Name = "request", Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public partial class DataPipelineRequestParameters
    {
        public DataPipelineRequestParameters() { }

        public DataPipelineRequestParameters(OsDataRequest dataRequest)
        {
            if (null == dataRequest) return;

            if (!string.IsNullOrEmpty(dataRequest.Fields))
                this.Fields = dataRequest.Fields;
            
            if (!string.IsNullOrEmpty(dataRequest.FilterBy))
                this.FilterBy = dataRequest.FilterBy;
            
            if (!string.IsNullOrEmpty(dataRequest.FilterOp))
                this.FilterOp = dataRequest.FilterOp;

            if (!string.IsNullOrEmpty(dataRequest.FilterValue))
                this.FilterValue = dataRequest.FilterValue;
            
            if (!string.IsNullOrEmpty(dataRequest.GroupId))            
                this.GroupId = dataRequest.GroupId;

            if (dataRequest.HasAttribute("href"))
                this.Href = dataRequest.GetAttribute("href");
            
            if (dataRequest.HasAttribute("params"))
                this.Params = dataRequest.GetAttribute("params");
            
            if (!string.IsNullOrEmpty(dataRequest.SortBy))
                this.SortBy = dataRequest.SortBy;
            
            if (!string.IsNullOrEmpty(dataRequest.SortOrder))
                this.SortOrder = dataRequest.SortOrder;
            
            if (!string.IsNullOrEmpty(dataRequest.StartIndex))
                this.StartIndex = dataRequest.StartIndex;
            
            if (!string.IsNullOrEmpty(dataRequest.UserId))
                this.UserId = dataRequest.UserId;
            
            if (!string.IsNullOrEmpty(dataRequest.Count))
                this.Count = dataRequest.Count;
        }

        [DataMemberAttribute(Name = "userId", EmitDefaultValue = false)]
        public string UserId;
        
        [DataMemberAttribute(Name = "groupId", EmitDefaultValue = false)]        
        public string GroupId;
        
        [DataMemberAttribute(Name = "fields", EmitDefaultValue = false)]
        public string Fields;

        [DataMemberAttribute(Name = "startIndex", EmitDefaultValue = false)]
        public string StartIndex;
        
        [DataMemberAttribute(Name = "count", EmitDefaultValue = false)]
        public string Count;
        
        [DataMemberAttribute(Name = "sortBy", EmitDefaultValue = false)]
        public string SortBy;
        
        [DataMemberAttribute(Name = "sortOrder", EmitDefaultValue = false)]
        public string SortOrder;
        
        [DataMemberAttribute(Name = "filterBy", EmitDefaultValue = false)]
        public string FilterBy;
        
        [DataMemberAttribute(Name = "filterOp", EmitDefaultValue = false)]
        public string FilterOp;
        
        [DataMemberAttribute(Name = "filterValue", EmitDefaultValue = false)]
        public string FilterValue;
        
        [DataMemberAttribute(Name = "activityIds", EmitDefaultValue = false)]
        public string ActivityIds;
        
        [DataMemberAttribute(Name = "href", EmitDefaultValue = false)]
        public string Href;
        
        [DataMemberAttribute(Name = "params", EmitDefaultValue = false)]        
        public string Params;
    }*/
}