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
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Encapsulation of the MySpace extension to gadget ModulePrefs 
	/// that contain view settings that are specific to myspace.
	/// This information is defined in markup by the tag
	/// &lt;Optional feature="MySpace-Views" &gt;
	/// </summary>
	public class MySpaceViewSettings
	{
		/// <summary>
		/// Key used in gadget markup to identify MySpace settings
		/// </summary>
		public const string FEATURE_KEY = "MySpace-Views";

		/// <summary>
		/// Default location (left/right) when profile surface exists
		/// and mount not specified
		/// </summary>
		public const string DEFAULT_PROFILE_LOCATION = "left";

		public MySpaceViewSettings() { }

		public MySpaceViewSettings(ModuleFeature feature)
		{
			LoadFeatureSettings(feature);
		}

		public void LoadFeatureSettings(ModuleFeature feature)
		{
			foreach (ParamControl param in feature.Params)
			{
				AddParamSetting(param);
			}
		}

		/// <summary>
		/// Resolves a param control value and applies its setting
		/// </summary>
		/// <param name="param"></param>
		void AddParamSetting(ParamControl param)
		{
			if (param == null || string.IsNullOrEmpty(param.Name))
			{
				return;
			}
            string key = param.Name.ToLowerInvariant();
			switch (key)
			{
				case "profilesize":
					ProfileSize = new ViewSize(param.Value);
					break;
				case "homesize":
					HomeSize = new ViewSize(param.Value);
					break;
				case "canvassize":
					CanvasSize = new ViewSize(param.Value);
					break;
				case "profilelocation":
					ProfileLocation = param.Value;
					break;
			}

		}

		public ViewSize CanvasSize { get; set; }

		public ViewSize HomeSize { get; set; }

		public ViewSize ProfileSize { get; set; }


		private string _profileLocation = null;
		/// <summary>
		/// Location (left or right) to render the profile view
		/// </summary>
		public string ProfileLocation { get { return _profileLocation; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_profileLocation = null;
				}
				else
				{
					if (value.Equals("right", StringComparison.InvariantCultureIgnoreCase))
					{
						_profileLocation = "right";
					}
					else
					{
						_profileLocation = DEFAULT_PROFILE_LOCATION;
					}
				}
			}
		}

		/// <summary>
		/// Returns the mount location for the profile view
		/// </summary>
		/// <returns></returns>
		public string GetProfileMount()
		{
			if (!string.IsNullOrEmpty(ProfileLocation))
			{
				if (ProfileLocation.Equals("right", StringComparison.InvariantCultureIgnoreCase))
				{
					return "profile.right";
				}
				else
				{
					return "profile.left";
				}
			}
			else
			{
				if (null == ProfileSize)
				{
					return null;
				}
				else
				{
					return "profile.left";
				}
			}					
		}

	}
}
