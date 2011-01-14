/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline
{
	[Flags]
	public enum OpenSocialPermissions : int
	{
		None = 0x0,
		AccessToBasicInfo = 0x1,
		AccessToFullContactInfo = 0x2,
		AccessToFullProfileInfo = 0x4,
		AccessToBlurbs = 0x8,
		AccessToInterests = 0x10,
		AccessToProfileDetails = 0x20,
		AccessToStatusMood = 0x40,
		AccessToProfileComments = 0x80,
		AccessToGroups = 0x100,
		AccessToPrivatePhotosAndVideos = 0x200,
		AccessToAddPhotosAndAlbums = 0x400,
		AccessToAddResource = 0x800,
		AccessToUpdateResource = 0x1000,
		AccessToDeleteResource = 0x2000,
		AccessToUpdateProfile = 0x4000,
		AccessToReceivingNotifications = 0x8000,
		AccessToSendingNotifications = 0x10000,
		AccessToPublishAppActivities = 0x20000,
		AccessToReceiveAppActivities = 0x40000,
		AccessToUpdateStatusMood = 0x80000,
		AccessToReadFriendRequest = 0x100000,
		AccessToUpdateFriendRequest = 0x200000,
		AccessToDeleteMediaItem = 0x400000,
		AccessToDeleteStatusMood = 0x800000,
		AccessToMessaging = 0x1000000,
		AccessToDeleteFriendship = 0x2000000,
		AccessToSendMySpaceInvites = 0x4000000,
	}
}
