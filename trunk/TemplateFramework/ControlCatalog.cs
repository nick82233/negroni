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
using Negroni.TemplateFramework.Parsing;

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// Internally used catalog of available Controls for a given ParseContext.
	/// Holds all dictionaries used to catalog Controls avaialabe within the
	/// context based on different lookup mechanisms.
	/// </summary>
	internal class ControlCatalog
	{
		public ControlCatalog(ParseContext context)
		{
			MyContext = context;
			this.ClearControlMaps();

		}

		/// <summary>
		/// ParseContext which identifies the primary context
		/// </summary>
		public ParseContext MyContext { get; set; }


		/// <summary>
		/// Indicates if this ControlCatalog represents <c>ParseContext.DefaultContext</c>
		/// in addition to the context identified in <c>MyContext</c>.
		/// </summary>
		public bool IsDefaultContextCatalog = false;


		/// <summary>
		/// OffsetKey of the containing control type.
		/// This value is used to generate RootOffsetItems for a trace OffsetList.
		/// </summary>
		public string ContextOffsetKey
		{
			get;
			set;
		}

		/// <summary>
		/// Number of items registered in this catalog
		/// </summary>
		public int Count
		{
			get
			{
				return this.ControlMapOffsetKey.Count;
			}
		}

		/// <summary>
		/// Entry point for adding a new <c>ControlMap</c> to
		/// all relevant Dictionaries
		/// </summary>
		/// <param name="controlMap"></param>
		/// <returns></returns>
		public ControlMap Add(ControlMap controlMap)
		{
			if(null == controlMap){
				throw new ArgumentNullException();
			}

			if (null != controlMap.MarkupTag)
			{
				if (!controlMap.HasAttributeDependency())
				{
                    ControlMapMarkup.Add(controlMap.MarkupTag.ToLowerInvariant(), controlMap);
					if (controlMap.AdditionalMarkupTags.Count > 0)
					{
						foreach (string tag in controlMap.AdditionalMarkupTags)
						{
							ControlMapMarkup.Add(tag.ToLowerInvariant(), controlMap);
						}
					}
				}
			}
			if (null != controlMap.ControlType)
			{
				ControlMapControlType.Add(controlMap.ControlType, controlMap);
			}
			ControlMapOffsetKey.Add(controlMap.OffsetKey, controlMap);

			return controlMap;
		}


		/// <summary>
		/// ControlMap keyed on markup tag
		/// </summary>
		public Dictionary<string, ControlMap> ControlMapMarkup { get; set; }

		/// <summary>
		/// ControlMap keyed on OffsetKey string
		/// </summary>
		public Dictionary<string, ControlMap> ControlMapOffsetKey { get; set; }

		/// <summary>
		/// ControlMap keyed on control Type
		/// </summary>
		public Dictionary<Type, ControlMap> ControlMapControlType { get; set; }


		/// <summary>
		/// Clear and intialize all dictionaries
		/// </summary>
		public void ClearControlMaps()
		{
			if (null == ControlMapMarkup)
			{
				ControlMapMarkup = new Dictionary<string, ControlMap>();
			}
			else
			{
				ControlMapMarkup.Clear();
			}
			if (null == ControlMapOffsetKey)
			{
				ControlMapOffsetKey = new Dictionary<string, ControlMap>();
			}
			else
			{
				ControlMapOffsetKey.Clear();
			}
			if (null == ControlMapControlType)
			{
				ControlMapControlType = new Dictionary<Type, ControlMap>();
			}
			else
			{
				ControlMapControlType.Clear();
			}

		}

	}

}
