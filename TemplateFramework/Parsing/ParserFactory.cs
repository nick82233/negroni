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
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Negroni.TemplateFramework.Parsing
{
	/// <summary>
	/// Creates parsers.
	/// This could be eliminated since the TraceParser is the only valid parser
	/// </summary>
	public class ParserFactory
	{
		public static IOffsetParser GetOffsetParser()
		{
			return GetTraceOffsetParser(null);
			//return GetODOMOffsetParser();
		}
		public static IOffsetParser GetOffsetParser(ControlFactory controlFactory)
		{
			return GetTraceOffsetParser(controlFactory);
		}


		public static IOffsetParser GetTraceOffsetParser(ControlFactory controlFactory)
		{
			TraceOffsetParser parser = new TraceOffsetParser();
			parser.MyControlFactory = controlFactory;
			return parser;
		}

		/// <summary>
		/// Initialize a stream around the content
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		internal static MemoryStream CreateContentStream(string content)
		{
			MemoryStream stream = new MemoryStream(content.Length);
			StreamWriter w = new StreamWriter(stream);
			w.Write(content);
			w.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

		internal static XmlTextReader InitializeTagReader(string content)
		{
			return InitializeTagReader(content, null);
		}

		/// <summary>
		/// Creates an instance of a XmlReader initialized to process OSML markup
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		internal static XmlTextReader InitializeTagReader(string content, string[] customNamespaces)
		{
			MemoryStream stream = CreateContentStream(content);

			return InitializeTagReader(stream, customNamespaces);
		}

		/// <summary>
		/// Creates an instance of a XmlTextReader initialized to process markup.
		/// </summary>
		/// <remarks>Modify this method to set up namespacing for the xml</remarks>
		/// <param name="stream"></param>
		/// <returns></returns>
		internal static XmlTextReader InitializeTagReader(Stream stream, string[] customNamespaces)
		{

			XmlNamespaceManager nsManager = new XmlNamespaceManager(OsmlReaderSettings.NameTable);
			/*
			nsManager.AddNamespace("os", MySpace.OpenSocial.XmlNamespaces.OS_NAMESPACE_URL);
			if (null != customNamespaces)
			{
				for (int i = 0; i < customNamespaces.Length; i++)
				{
					if (!nsManager.HasNamespace(customNamespaces[i]))
					{
						nsManager.AddNamespace(customNamespaces[i], MySpace.OpenSocial.XmlNamespaces.OS_NAMESPACE_URL);
					}
				}
			}
			 * */
			XmlParserContext xContext = new XmlParserContext(OsmlReaderSettings.NameTable, nsManager, "elem", XmlSpace.None);
			
			XmlTextReader reader = new XmlTextReader(stream, OsmlReaderSettings.NameTable);
			reader.Namespaces = false;
			//LoadReaderSettings(reader); 
			return reader;
		}


		/// <summary>
		/// Initializes an XmlTextReader's settings
		/// </summary>
		/// <param name="reader"></param>
		static void LoadReaderSettings(XmlTextReader reader)
		{
			reader.Settings.ConformanceLevel = ConformanceLevel.Fragment;
			reader.Settings.IgnoreComments = false;
			reader.Settings.IgnoreWhitespace = false;
			reader.Settings.ValidationFlags = XmlSchemaValidationFlags.None;
			reader.Settings.ValidationType = ValidationType.None;
			reader.Settings.NameTable = OsmlReaderSettings.NameTable;
		}

		private static XmlReaderSettings _osmlReaderSettings = null;
		static XmlReaderSettings OsmlReaderSettings
		{
			get
			{
				if (null == _osmlReaderSettings)
				{
					NameTable nt = new NameTable();
					nt.Add("os");

					XmlReaderSettings settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					settings.IgnoreComments = false;
					settings.IgnoreWhitespace = false;
					settings.ValidationFlags = XmlSchemaValidationFlags.None;
					settings.ValidationType = ValidationType.None;
					settings.NameTable = nt;

					_osmlReaderSettings = settings;
				}
				return _osmlReaderSettings;
			}
		}
	}
}
