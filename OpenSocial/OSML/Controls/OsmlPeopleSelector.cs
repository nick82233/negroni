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
using System.Collections;


namespace Negroni.OpenSocial.OSML.Controls
{
	[MarkupTag("os:PeopleSelector")]
	public class OsmlPeopleSelector : BaseGadgetControl
	{
		static readonly string scriptTemplate =
@"<script type='text/javascript'>
var peopleSelect_##ID## = new MyOpenSpace.Widgets.PeopleSelector(""{0}"", ""{1}"",""{2}"",{3}, ""{4}"",""{5}"");
</script>
";
		
		const string REPLACE_KEY = "##person##";
		const string ID_KEY = "##ID##";
		const string OPTION_KEY = "##options##";
		const string ID_CONTAINER_KEY = "peopleSelect_##ID##_container";
		const string ID_ITEMCONTAINER_KEY = "peopleSelect_##ID##_itemcontainer";

		static readonly string optionTemplate = "<option value=\"${" + REPLACE_KEY + ".id}\">${" + REPLACE_KEY + ".displayName}</option>";
        static readonly string selectTemplate = "<select onchange=\"MyOpenSpace.Widgets.PeopleSelector.OnSingleSelectSelectionChange(this,'{0}',{1},'{2}')\" id=\"" + ID_KEY + "\">" + OPTION_KEY + "</select>";

		static readonly string multiSelectContainerTemplate = "<span id=\"" + ID_CONTAINER_KEY + "\" class=\"myspace-peopleSelectorchecklist\" ></span>";
		static readonly string multiSelectItemTemplate = "<div id=\"" + ID_ITEMCONTAINER_KEY + "\" class=\"myspace-peopleselectorchecklist-container\" style=\"display:none;\">" + OPTION_KEY + "</div>";
		static readonly string multiSelectItem = "<div style=\"white-space: nowrap;\"><input type=\"checkbox\" value=\"${"+ REPLACE_KEY + ".id}\" title=\"${" + REPLACE_KEY + ".displayName}\"><span style=\"width: 100%;\">${" + REPLACE_KEY + ".displayName}</span></div>";
    
		private bool multiSelect = false;
		private int max;
		private string group;
		
		/// <summary>
		/// Gets the variable name to use for the current item.
		/// Defaults to "Cur"
		/// </summary>
		/// <returns></returns>
		private string GetCurrentLoopItemVariable()
		{
			string var = null;
			if (this.HasAttribute("var"))
			{
				var = this.GetAttribute("var");
			}
			if (string.IsNullOrEmpty(var))
			{
				return "Cur";
			}
			else
			{
				return var;
			}
		}

		private string Group
		{
			get
			{
				this.group = GetAttribute("group");
				if (string.IsNullOrEmpty(group))
				{
					return string.Empty;
				}
				if (group.StartsWith(DataContext.VARIABLE_START))
				{
					group = DataContext.GetVariableExpression(group);
				}
				return group;
			}
		}

		private bool MultiSelect
		{
			get
			{
				string var = null;
				if (this.HasAttribute("multiple"))
				{
					var = this.GetAttribute("multiple");
				}
				if (string.IsNullOrEmpty(var))
				{
					multiSelect = false;
				}
				else
				{
					bool.TryParse(var,out multiSelect);
				}

				return multiSelect;
			}
		}

		private string InputName
		{
			get
			{
				return this.GetAttribute("inputName");
			}
		}

		private int Max
		{
			get
			{
				string var = null;
				if (this.HasAttribute("max"))
				{
					var = this.GetAttribute("max");
				}
				if (string.IsNullOrEmpty(var))
				{
					max = 0;
				}
				else
				{
					int.TryParse(var, out max);
				}
				return max;
			}
		}

		private string OnSelectMethod
		{
			get
			{
				return this.GetAttribute("onselect");
			}
		}
	
		public override void Render(System.IO.TextWriter writer)
		{
			if (string.IsNullOrEmpty(this.Group))
				return;

			IEnumerable friendList = MyDataContext.GetEnumerableVariableObject(this.Group);
			if (null == friendList)
			{
				return;
			}

			string output = string.Empty;
			if (!MultiSelect)
			{
				output = singleSelectDropDown(friendList);
			}
			else
			{
				output = multiSelectDropDown(friendList);
			}

			writer.Write(output);
		}

		private string multiSelectDropDown(IEnumerable friendList)
		{
			LoopContext context = GetLoopContext(friendList);
			StringBuilder selectOption = new StringBuilder();
			foreach (object item in friendList)
			{
				object realItem = item;
				if (item is DictionaryEntry)
				{
					realItem = ((DictionaryEntry)item).Value;
				}

				MyDataContext.RegisterLocalValue(GetCurrentLoopItemVariable(), realItem);

				if (MyDataContext.HasVariable("Context"))
				{
					MyDataContext.RemoveLocalValue("Context");
				}
				MyDataContext.RegisterLocalValue("Context", context);

				string newRaw = multiSelectItem.Replace(REPLACE_KEY, GetCurrentLoopItemVariable());

				selectOption.Append(ResolveDataContextVariables(newRaw, MyDataContext));

				context.Count++;
				MyDataContext.RemoveLocalValue(GetCurrentLoopItemVariable());
			}

			StringBuilder output = new StringBuilder(multiSelectContainerTemplate.Replace(ID_KEY, this.ID));
			output.Append( multiSelectItemTemplate.Replace(ID_KEY, this.ID));
			output = output.Replace(OPTION_KEY, selectOption.ToString());

			output.Append(string.Format(
										scriptTemplate.Replace(ID_KEY, this.ID), ID_CONTAINER_KEY.Replace(ID_KEY, this.ID),
										ID_ITEMCONTAINER_KEY.Replace(ID_KEY, this.ID), this.Max,
                                        string.IsNullOrEmpty(this.OnSelectMethod) ? "null" : string.Format("typeof {0} == 'undefined' ? null : {0}",this.OnSelectMethod), 
                                        this.InputName, this.Group)
										);

			return output.ToString();
		}

		private string singleSelectDropDown(IEnumerable friendList)
		{

			LoopContext context = GetLoopContext(friendList);
			StringBuilder selectOption = new StringBuilder();
			foreach (object item in friendList)
			{
				object realItem = item;
				if (item is DictionaryEntry)
				{
					realItem = ((DictionaryEntry)item).Value;
				}

				MyDataContext.RegisterLocalValue(GetCurrentLoopItemVariable(), realItem);

				if (MyDataContext.HasVariable("Context"))
				{
					MyDataContext.RemoveLocalValue("Context");
				}
				MyDataContext.RegisterLocalValue("Context", context);

				string newRaw = optionTemplate.Replace(REPLACE_KEY, GetCurrentLoopItemVariable());

				selectOption.Append(ResolveDataContextVariables(newRaw, MyDataContext));

				context.Count++;
				MyDataContext.RemoveLocalValue(GetCurrentLoopItemVariable());
			}

			//remove the final context variable
			if (MyDataContext.HasVariable("Context"))
			{
				MyDataContext.RemoveLocalValue("Context");
			}

            string output = string.Format(selectTemplate.Replace(ID_KEY, this.ID), this.InputName, 
                                                   string.IsNullOrEmpty(this.OnSelectMethod) ? "null" : string.Format("typeof {0} == 'undefined' ? null : {0}", this.OnSelectMethod),
                                                   this.Group);
			output = output.Replace(OPTION_KEY, selectOption.ToString());
			return output;
		}

		private LoopContext GetLoopContext(IEnumerable friendList)
		{
			//System.Collections.Hashtable
			LoopContext context = new LoopContext(0, 0);

			//resolve the count
			if (friendList is IList)
			{
				context.Count = ((IList)friendList).Count;
			}
			else
			{
				//enumerate to count items
				int count = 0;
				foreach (object item in friendList)
				{
					count++;
				}
				context.Count = count;
			}
			return context;
		}
		
	}
}
