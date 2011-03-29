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
using System.IO;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Factory class for managing custom tags as defined by templates.
	/// </summary>
	public class CustomTagFactory
	{
		/// <summary>
		/// Prefix ID value used to identify custom tag templates
		/// </summary>
		public const string CLIENT_TEMPLATE_ID_PREFIX = "CLIENT_CUSTOM_TAG__";
		public const string CLIENT_TEMPLATE_TAG_ATTRIBUTE = "xmysp-template-tag";
		private const string CLIENT_TEMPLATE = @"<Template xmlns:os=\""http://opensocial.org/templates\"" xmlns:{0}=\""http://opensocial.org/templates\"">{1}</Template>";

		static readonly string clientTemplateDivWrapper = "<div style=\"display:none;\" id=\"" + CLIENT_TEMPLATE_ID_PREFIX + "{0}\" " + CLIENT_TEMPLATE_TAG_ATTRIBUTE + "=\"{0}\" >";

#if DEBUG
		public string myId = Guid.NewGuid().ToString();
#endif

		public CustomTagFactory()
		{
			CustomTags = new Dictionary<string, CustomTagTemplate>();
		}
		public CustomTagFactory(ControlFactory controlFactory)
			: this()
		{
			MyControlFactory = controlFactory;
		}



		private RootElementMaster _myRootMaster = null;

		/// <summary>
		/// Internal RootElementMaster to hang references to Parser and ControlFactory off of.
		/// </summary>
		internal RootElementMaster MyRootMaster
		{
			get
			{
				if (_myRootMaster == null)
				{
					_myRootMaster = new RootElementMaster(MyControlFactory);
				}
				return _myRootMaster;
			}
		}


		/// <summary>
		/// Initializes a RootElementMaster object for this factory to attach CustomTag definitions to.
		/// This object is required for the control parsing to function properly
		/// </summary>
		/// <param name="controlFactory"></param>
		private void InitializeRootElementMaster(ControlFactory controlFactory)
		{
			_myRootMaster = new RootElementMaster(controlFactory);
		}

		/// <summary>
		/// Custom tags registered with this factory for use within the 
		/// processing gadget
		/// </summary>
		public Dictionary<string, CustomTagTemplate> CustomTags { get; private set; }

		private ControlFactory _myControlFactory = null;

		/// <summary>
		/// The ControlFactory to use when parsing contents of a custom tag
		/// </summary>
		public ControlFactory MyControlFactory
		{
			get
			{
				return _myControlFactory;
			}
			set
			{
				_myControlFactory = value;
			}
		}



		/// <summary>
		/// Looks up the OsTagTemplate defined for the tag.
		/// Returns null if tag is not defined.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		internal CustomTagTemplate GetTagTemplate(string tag)
		{
			if (CustomTags.ContainsKey(tag))
			{
				return CustomTags[tag];
			}
			return null;
		}


		/// <summary>
		/// Registers a new custom tag to be available for use within a gadget.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="tagTemplate"></param>
		/// <returns></returns>
		public CustomTagTemplate RegisterCustomTag(string tag, string tagTemplate)
		{
			CustomTagTemplate newTagDef = new CustomTagTemplate(tag, tagTemplate, MyRootMaster);
			return RegisterCustomTag(newTagDef);
		}

		/// <summary>
		/// Registers a new custom tag to be available for use within a gadget.
		/// </summary>
		/// <param name="tagTemplate"></param>
		/// <returns></returns>
		public CustomTagTemplate RegisterCustomTag(CustomTagTemplate tagTemplate)
		{
			if (null == tagTemplate)
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(tagTemplate.Tag))
			{
				throw new ArgumentException("Custom template does not define a tag");
			}
			CustomTags.Add(tagTemplate.Tag, tagTemplate);
			if (!tagTemplate.IsParsed)
			{
				tagTemplate.Parse();
			}
			return tagTemplate;
		}

		/// <summary>
		/// Registers a collection of templates
		/// </summary>
		/// <param name="templates"></param>
		/// <returns></returns>
		public int RegisterCustomTags(List<CustomTagTemplate> templates)
		{
			int count = 0;
			foreach (CustomTagTemplate template in templates)
			{
				RegisterCustomTag(template);
				count++;				
			}
			return count;
		}


		public CustomTag CreateTagInstance(string tag)
		{
			return CreateTagInstance(tag, null);
		}

		public CustomTag CreateTagInstance(string tag, string markup)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentNullException("Must specify tag");
			}

			if (!CustomTags.ContainsKey(tag))
			{
				return null;
			}

			return new CustomTag(tag, CustomTags[tag], markup);
		}

		/// <summary>
		/// Tests to see if the given tag is a defined custom tag.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool IsCustomTag(string tag)
		{
			return CustomTags.ContainsKey(tag);
		}


		/// <summary>
		/// Render all registered templates as client-side templates.
		/// This call renders them as DOM elements
		/// </summary>
		/// <param name="writer"></param>
		public void RenderClientTemplates(TextWriter writer)
		{
			RenderClientTemplates(writer, false);
		}
		/// <summary>
		/// Render all registered templates as client-side javascript templates.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="asDomElements">If true as DOM elements, if false as script</param>
		public void RenderClientTemplates(TextWriter writer, bool asDomElements)
		{
			RenderClientTemplates(writer, null, false);
		}

		/// <summary>
		/// Render all registered templates as client-side templates
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="registeredTags">Array of registered tags to include in rendering</param>
		public void RenderClientTemplates(TextWriter writer, string[] registeredTags, bool asDomElementsa)
		{
			bool asDomElements = false;
			string delim = "||";
			bool includeAll = (registeredTags == null);
			string matchStr = null;
			if (!includeAll)
			{
				matchStr = String.Join(delim, registeredTags);
			}

			if (!asDomElements)
			{
				writer.WriteLine(@"opensocial.template._rawTemplates = [];");
			}
			
			foreach (KeyValuePair<string, CustomTagTemplate> keyset in CustomTags)
			{
				if (!includeAll && (matchStr.IndexOf(delim + keyset.Key + delim) == -1))
				{
					continue;
				}
				//also check to see if this is supposed to render
				if (keyset.Value.ClientRegister)
				{
					if (asDomElements)
					{
						writer.WriteLine(string.Format(clientTemplateDivWrapper, keyset.Key));
						writer.WriteLine(keyset.Value.InnerMarkup);
						writer.WriteLine("</div>");
					}
					else
					{
						writer.Write(@"MyOpenSpace.template.TemplateProcessor.templatesXml[");
						writer.Write(JsonData.JSSafeQuote(keyset.Key));
						writer.Write(@"] = ");
						string sufix = keyset.Key.Split(':')[0];
						string template = string.Format(CLIENT_TEMPLATE, sufix, keyset.Value.InnerMarkup);
						string content = JsonData.JSSafeQuote(template);
						writer.Write(content);
						writer.Write(";\n");
					}
				}
			}


		}


	}
}
