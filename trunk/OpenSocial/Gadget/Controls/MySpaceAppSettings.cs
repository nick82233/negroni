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
//using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
    /// <summary>
    /// Encapsulation of the MySpace extension to gadget ModulePrefs 
    /// that contain app settings that are specific to myspace.
    /// This information is defined in markup by the tag
    /// &lt;Optional feature="MySpace-Settings" &gt;
    /// </summary>
    public class MySpaceAppSettings
    {
        /// <summary>
        /// Key used in gadget markup to identify MySpace settings
        /// </summary>
        public const string FEATURE_KEY = "MySpace-Settings";

        public MySpaceAppSettings() { }

        public MySpaceAppSettings(ModuleFeature feature)
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
                case "agerestriction":
					int val = 0;
					if (Int32.TryParse(param.Value, out val))
					{
						AppAgeRestriction = val;
					}
					else
					{
						AppAgeRestriction = 0;
					}
                    break;
                case "appcategory1":
                case "appcategory2":
                    ushort idVal;
                    if (ushort.TryParse(param.Value, out idVal))
                    {
                        if (key == "appcategory1")
                        {
                            this.PrimaryCategoryId = idVal;
                        }
                        else
                        {
                            this.SecondaryCategoryId = idVal;
                        }
                    }
                    break;
            }

        }


        /// <summary>
        /// Default language used when listing app in the App Gallery
        /// </summary>
        public string DefaultGalleryLanguage { get; set; }

        /// <summary>
        /// ID of the primary category of the app
        /// </summary>
        public ushort PrimaryCategoryId { get; set; }

        /// <summary>
        /// ID of the secondary category for the app
        /// </summary>
        public ushort SecondaryCategoryId { get; set; }


		/// <summary>
		/// Age restriction enum value as an int.
		/// </summary>
        public int AppAgeRestriction { get; set; }


    }
}
