using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Negroni.OpenSocial.DataContracts.v1;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.myspace.com/2009/comments", Name = "comment")]
	public class Comment
	{
		/// <summary>
		/// Person who authored and posted this comment
		/// </summary>
		[DataMemberAttribute(Name = "author", EmitDefaultValue = false)]
		public Person Author { get; set; }

		/// <summary>
		/// Unique ID of this comment
		/// </summary>
		[DataMemberAttribute(Name = "commentId", EmitDefaultValue = false)]
		public string CommentId { get; set; } // Id of the comment

        /// <summary>
        /// Parent comment ID of this comment
        /// </summary>
        [DataMemberAttribute(Name = "parentCommentId", EmitDefaultValue = false)]
        public string ParentCommentId { get; set; }

        /// <summary>
        /// Parent comment author ID of this comment
        /// </summary>
        [DataMemberAttribute(Name = "parentCommentAuthorId", EmitDefaultValue = false)]
        public string ParentCommentAuthorId { get; set; }

        /// <summary>
		/// Comment body text
		/// </summary>
		[DataMemberAttribute(Name = "body", EmitDefaultValue = false)]
		public string Body { get; set; } // comment body

		/// <summary>
		/// Date and time comment was posted
		/// </summary>
		[DataMemberAttribute(Name = "postedDate", EmitDefaultValue = false)]
		public string PostedDate { get; set; } // date when the comment was posted

        /// <summary>
        /// Video release id of the video comment if comment type of video
        /// </summary>
        [DataMemberAttribute(Name = "videoReleaseId", EmitDefaultValue = false)]
        public string VideoReleaseId { get; set; } 

        /// <summary>
		/// Culture in which this comment was posted
		/// </summary>
		[DataMemberAttribute(Name = "culture", EmitDefaultValue = false)]
		public string Culture { get; set; }

		/// <summary>
		/// Resource to which this comment pertains
		/// </summary>
		[DataMemberAttribute(Name = "resource", EmitDefaultValue = false)]
		public GenericResource Resource { get; set; }

	}
}
