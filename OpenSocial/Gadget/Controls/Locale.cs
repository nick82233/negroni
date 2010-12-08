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
using System.IO;
using Negroni.DataPipeline;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// A control that represents the &lt;Locale /&gt; element in an internationalized gadget
	/// This appears in the ModulePrefs section and wraps or references a message bundle
	/// </summary>
    [MarkupTag("Locale")]
    [ContextGroup(typeof(ModulePrefs))]
    public class Locale : BaseContainerControl
    {
        public Locale()
        {
            this.MyParseContext = new ParseContext(typeof(ModulePrefs));
        }

        public Locale(string markup)
            : this()
        {
            LoadTag(markup);
        }

        public Locale(string markup, GadgetMaster master)
            : this()
        {
            base.MyRootMaster = master;
            LoadTag(markup);
        }

        public Locale(string markup, OffsetItem offset)
            : this()
        {
            this.MyOffset = offset;
            LoadTag(markup);
        }



        private MessageBundle _myMessageBundle = null;

        /// <summary>
        /// Accessor for myMessageBundle.
        /// Performs lazy load upon first request
        /// </summary>
        public MessageBundle MyMessageBundle
        {
            get
            {
                if (_myMessageBundle == null)
                {
                    _myMessageBundle = new MessageBundle();
					_myMessageBundle.MyRootMaster = MyRootMaster;
					UpdateCultureCode();
                }
                return _myMessageBundle;
            }
            private set
            {
				if (_myMessageBundle != null)
				{
					value.MyRootMaster = MyRootMaster;
					if (_myMessageBundle.Messages.HasValues())
					{
						Dictionary<string, string> tmp = _myMessageBundle.Messages.GetDefinedMessages();
						foreach (KeyValuePair<string, string> keyset in tmp)
						{
							value.Messages.AddString(keyset.Key, keyset.Value);
						}
					}
				}
                _myMessageBundle = value;
                UpdateCultureCode();

            }
        }

		/// <summary>
		/// Loads an external messagebundle
		/// </summary>
		/// <param name="messageBundle"></param>
		public void LoadMessageBundle(string messageBundle)
		{
			if (string.IsNullOrEmpty(messageBundle))
			{
				return;
			}
			// Trim before loading.  
			// We don't care about offsets in this case, so trim is OK
			string testStr = "<messagebundle";
			if (!messageBundle.StartsWith(testStr, StringComparison.InvariantCultureIgnoreCase))
			{
				int pos = messageBundle.IndexOf(testStr);
				if (pos == -1)
				{
					return;
				}
				else
				{
					messageBundle = messageBundle.Substring(pos);
				}
			}
			
			MyMessageBundle.LoadTag(messageBundle);

			if (MyMessageBundle.Messages.HasValues())
			{
				try
				{
					MyDataContext.AddResourceBundle(MyMessageBundle.Messages);
				}
				catch (Exception ex)
				{
					if(MyRootMaster is GadgetMaster){
						GadgetMaster errRoot = (GadgetMaster)MyRootMaster;
						errRoot.Errors.MessageBundleErrors.Add(new MessageBundleItemError("Error adding bundle"));
					}
				}
			}

		}

        public override void LoadTag(string markup)
        {
            base.LoadTag(markup);

            if (this.HasAttributes())
            {
                this.Lang = GetAttribute("lang");
                this.Country = GetAttribute("country");
				this.MessageSrc = GetAttribute("messages");

                UpdateCultureCode();
            }
            if (MyMessageBundle.Messages.HasValues())
            {
                MyDataContext.AddResourceBundle(MyMessageBundle.Messages);
            }
        }

        private void UpdateCultureCode()
        {
            if (!string.IsNullOrEmpty(this.Lang))
            {
                if (!string.IsNullOrEmpty(this.Country))
                {
                    MyMessageBundle.Messages.CultureCode = Lang.ToLowerInvariant() + "-" + Country.ToUpperInvariant();
                }
                else
                {
                    MyMessageBundle.Messages.CultureCode = Lang.ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Adds a control to the internal collections.
        /// If the control is a special control - ex: Link
        /// it will be added to the correct special List.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public override BaseGadgetControl AddControl(BaseGadgetControl control)
        {
            if (null == control)
            {
                return null;
            }

            base.AddControl(control);
            if (control is MessageBundle)
            {
                MyMessageBundle = (MessageBundle)control;
            }
            else if (control is Message) //hackish - this is an illegal structure
            {
                MyMessageBundle.Messages.AddString(((Message)control).Name, ((Message)control).Value);
            }

            return control;
        }

        public override void Render(TextWriter writer)
        {
            return;
        }

		private string _country = null;
		/// <summary>
		/// Two letter country code
		/// </summary>
		public string Country
		{
			get
			{
				return _country;
			}
			set
			{
				_country = value;
				UpdateCultureCode();
			}
		}

		private string _lang = null;
		/// <summary>
		/// Two letter language code
		/// </summary>
		public string Lang
		{
			get
			{
				return _lang;
			}
			set
			{
				_lang = value;
				UpdateCultureCode();
			}
		}

		/// <summary>
		/// Source URI of an external message bundle.
		/// This is specified with the @messages attribute
		/// </summary>
		public string MessageSrc
		{
			get;
			set;
		}

		/// <summary>
		/// Source URI of the locally stored messages.
		/// This is typically on DFS
		/// </summary>
		public string MessageLocalSourceURI { get; set; }

    }
}
