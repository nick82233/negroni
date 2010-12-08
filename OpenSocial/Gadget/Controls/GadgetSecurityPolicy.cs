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
//using Negroni.Applications;
using Negroni.TemplateFramework;
using Negroni.DataPipeline.Security;

namespace Negroni.OpenSocial.Gadget.Controls
{
	#region Event delegate support

	internal delegate void SecurityPolicyChangeDelegate(object sender, SecurityPolicyChangedEventArgs e);

	/// <summary>
	/// Event Arguments for when a security policy changes in the gadget
	/// </summary>
	class SecurityPolicyChangedEventArgs : EventArgs
	{
		public SecurityPolicyChangedEventArgs() { }

		public SecurityPolicyChangedEventArgs(EL_Escaping elEscaping)
		{
			UpdatedEL_Escaping = elEscaping;
		}
		public SecurityPolicyChangedEventArgs(EL_Escaping elEscaping, UserPrefEscaping userPrefEscaping)
			: this(elEscaping)
		{
			UpdatedUserPrefEscaping = userPrefEscaping;
		}
		/// <summary>
		/// New EL_Escaping value
		/// </summary>
		public EL_Escaping UpdatedEL_Escaping { get; set; }

		/// <summary>
		/// New UserPrefEscaping value
		/// </summary>
		public UserPrefEscaping UpdatedUserPrefEscaping { get; set; }
	}
#endregion


	/// <summary>
	/// Encapsulation of the SecurityPolicy feature from ModulePrefs 
	/// that contain view settings that are specific to myspace.
	/// This information is defined in markup by the tag
	/// &lt;Optional feature="security-profile" &gt;
	/// </summary>
	public class GadgetSecurityPolicy : SecurityPolicy
	{


		/// <summary>
		/// Key used in gadget markup to identify MySpace settings
		/// </summary>
		public const string FEATURE_KEY = "security-policy";

		public GadgetSecurityPolicy() { }

		public GadgetSecurityPolicy(ModuleFeature feature)
		{
			LoadFeatureSettings(feature);
		}

		/// <summary>
		/// Event fired whenever the SecurityPolicy values change
		/// </summary>
		internal event SecurityPolicyChangeDelegate SecurityPolicyChanged;

		protected void OnSecurityPolicyChanged()
		{
			if (SecurityPolicyChanged != null)
			{
				SecurityPolicyChanged(this, new SecurityPolicyChangedEventArgs(this.EL_Escaping, this.UserPrefEscaping));
			}
		}


		public void LoadFeatureSettings(ModuleFeature feature)
		{
			foreach (ParamControl param in feature.Params)
			{
				AddParamSetting(param);
			}
			OnSecurityPolicyChanged();
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
				case "el_escaping":
					if ("html".Equals(param.Value, StringComparison.InvariantCultureIgnoreCase))
					{
						this.EL_Escaping = EL_Escaping.Html;
					}
					else
					{
						this.EL_Escaping = EL_Escaping.None;
					}
					break;
				case "userpref_escaping":
					if ("html".Equals(param.Value, StringComparison.InvariantCultureIgnoreCase))
					{
						this.UserPrefEscaping = UserPrefEscaping.Html;
					}
					else
					{
						this.UserPrefEscaping = UserPrefEscaping.None;
					}
					break;
			}
		}

	}
}
