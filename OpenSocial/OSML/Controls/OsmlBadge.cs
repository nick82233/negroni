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
using Negroni.DataPipeline;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{

	/// <summary>
	/// Badge control for a person, usually includes picture and name with link to profile.
	/// </summary>
	[MarkupTag("os:Badge")]
	public class OsmlBadge : BaseGadgetControl
	{
		const string REPLACE_KEY = "##person##";
		const string BADGE_TEMPLATE =
@"<div style='width:180px;height:60px;border:2px solid #2358B8;margin:2px;padding:3px;' personid=""${" + REPLACE_KEY + @".id}"">
<a href=""${" + REPLACE_KEY + @".profileUrl}"" target='_blank' ><img src=""${" + REPLACE_KEY + @".thumbnailUrl}"" width='30' height='45' style='border:0' />${" + REPLACE_KEY + @".displayName}</a>
</div>";

		public OsmlBadge() { }
		/*
		public OsmlBadge(IAccountLite person)
		{
			PersonData = person;
		}
		*/
		public OsmlBadge(string markup)
		{
			LoadTag(markup);
		}
		/*
		public OsmlBadge(string markup, string personReference)
		{
			PersonData = MyDataContext.GetPerson(personReference);
		}
		 */ 

		public override void Render(System.IO.TextWriter writer)
		{
			string personKey = GetAttribute("person");
			if(string.IsNullOrEmpty(personKey)){
				return;
			}
			if(personKey.StartsWith(DataContext.VARIABLE_START)){
				personKey = DataContext.GetVariableExpression(personKey);
			}
			string inst = BADGE_TEMPLATE.Replace(REPLACE_KEY, personKey);
			writer.WriteLine(ResolveDataContextVariables(inst, MyDataContext));
		}
	}
}
