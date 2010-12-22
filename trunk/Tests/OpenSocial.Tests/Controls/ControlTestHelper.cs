using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Gadget;

namespace Negroni.OpenSocial.Test.Controls
{
	class ControlTestHelper
	{
		static public Person CreatePerson(int id, string displayName, string thumbnailUrl)
		{
			Person p = new Person
			{
				Id = id.ToString(),
				DisplayName = displayName,
				ThumbnailUrl = thumbnailUrl
			};
			return p;
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

		internal static string GetStreamContent(Stream stream)
		{
			string val = null;
			stream.Seek(0, SeekOrigin.Begin);
			using (StreamReader reader = new StreamReader(stream))
			{
				val = reader.ReadToEnd();
			}
			return val;
		}


		/// <summary>
		/// Normalizes results by removing all newlines
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		internal static string NormalizeRenderResult(string result)
		{
			if(string.IsNullOrEmpty(result)){
				return result;
			}

			result = result.Replace("\r\n", "\n");
			result = result.Replace("\n", "");
			result = result.Replace("\t", " ");
			while (result.Contains("  "))
			{
				result = result.Replace("  ", " ");
			}

			return result.Trim();
		}


		internal static string GetRenderedContents(BaseGadgetControl control)
		{
			if (control == null) throw new ArgumentNullException("Null control");

			MemoryStream output = new MemoryStream();
			TextWriter w = new StreamWriter(output);

			control.Render(w);
			w.Flush();
			string result = ControlTestHelper.GetStreamContent(output);

			return result;
		}

		internal static string GetRenderedContents(GadgetMaster master, string surface)
		{
			if (master == null) throw new ArgumentNullException("Null control");

			MemoryStream output = new MemoryStream();
			TextWriter w = new StreamWriter(output);

			master.RenderContent(w, surface);
			w.Flush();
			string result = ControlTestHelper.GetStreamContent(output);

			return result;
		}

	}
}
