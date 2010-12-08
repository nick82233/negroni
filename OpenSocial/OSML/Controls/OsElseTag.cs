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

using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;


namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Any element which as had a repeat attribute applied.
	/// </summary>
	[MarkupTag("osx:Else")]
	[AttributeTagAlternative("osx-else")]
	[Obsolete("Retiring this and replacing with osx:True/osx:False constructs")]
	public class OsElseTag : BaseContainerControl
	{
		public OsElseTag() { }

		private OsIfTag _matchingIfTag = null;

		/// <summary>
		/// Accessor for matchingIfTag.
		/// Performs lazy load upon first request
		/// </summary>
		public OsIfTag MatchingIfTag
		{
			get
			{
				return _matchingIfTag;
			}
			set
			{
				_matchingIfTag = value;
			}
		}


		public override void Render(System.IO.TextWriter writer)
		{
			if (MatchingIfTag == null)
			{
				return;
			}
			if (MatchingIfTag.IfConditionResult == false)
			{
				base.Render(writer);
			}
		}



	}
}
