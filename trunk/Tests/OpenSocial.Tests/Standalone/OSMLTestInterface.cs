using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Negroni.DataPipeline;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Test.TestData;

namespace OpenSocial.Test.Standalone
{
	public partial class OSMLTestInterface : Form
	{
		const string OSML_CONTROL_FACTORY_KEY = "osml09Sandbox";

		public OSMLTestInterface()
		{
			InitializeComponent();
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			RenderResult();

		}

		/// <summary>
		/// Enum for rendering options
		/// </summary>
		enum RenderAs
		{
			FullApp = 0,
			Partial = 1
		}

		enum RenderData
		{
			Live = 0,
			Sandbox = 1
		}

		/// <summary>
		/// Style of data to use when rendering
		/// </summary>
		RenderData RenderUsingData
		{
			get
			{
				if (radioDataLive.Checked)
				{
					return RenderData.Live;
				}
				else
				{
					return RenderData.Sandbox;
				}
			}
		}


		/// <summary>
		/// Describes how to instruct the control factory to render
		/// the source
		/// </summary>
		RenderAs RenderResultAs
		{
			get
			{
				if (radioRenderFull.Checked)
				{
					return RenderAs.FullApp;
				}
				else
				{
					return RenderAs.Partial;
				}
			}
		}

		/// <summary>
		/// Parsing context to use when parsing and rendering
		/// the source.
		/// </summary>
		ParseContext PartialRenderContext
		{
			get
			{
				if(RenderAs.Partial == RenderResultAs){
					object item = cmboPartial.SelectedItem;
					return null;
				}
				else{
					return new ParseContext(typeof(object));
				}
			}
		}


		string CurrentCulture
		{
			get
			{
				if (cmboCulture.SelectedIndex > -1)
				{
					return cmboCulture.SelectedItem.ToString();
				}
				else
				{
					return "en_US";
				}
			}
		}

		private Person _currentUser = null;

		/// <summary>
		/// Accessor for myProp.
		/// Performs lazy load upon first request
		/// </summary>
		public Person CurrentUser
		{
			get
			{
				if (_currentUser == null)
				{
					InitRenderUserData();
				}
				return _currentUser;
			}
		}

		int GetCurrentUserId()
		{
			return Int32.Parse(txtUserId.Text);
		}

		private void InitRenderUserData()
		{
			if (RenderUsingData == RenderData.Live)
			{
				// removing live implementation
			}
			else
			{
				_currentUser = GadgetTestData.Viewer;
				_currentUserFriends = GadgetTestData.GetViewerFriendsList();
			}
		}

		private List<Person> _currentUserFriends = null;

		/// <summary>
		/// Accessor for currentUserFriends.
		/// Performs lazy load upon first request
		/// </summary>
		public List<Person> CurrentUserFriends
		{
			get
			{
				if (_currentUserFriends == null)
				{
					//InitRenderUserData();
				}
				return _currentUserFriends;
			}
		}

		public string TemplateContent {
			get
			{
				if (isComboTemplate)
				{
					return LoadTemplate();
				}
				else
				{
					return txtMarkup.Text;
				}
			}
			set
			{
				txtMarkup.Text = value;
			}
		}

		private ControlFactory _currentControlFactory = null;

		/// <summary>
		/// Accessor for CurrentControlFactory.
		/// Performs lazy load upon first request
		/// </summary>
		public ControlFactory CurrentControlFactory
		{
			get
			{
				if (_currentControlFactory == null)
				{
					string cur = cmboControlFactory.SelectedItem.ToString();
					if (string.IsNullOrEmpty(cur))
					{
						if (cmboControlFactory.Items.Count > 0)
						{
							cur = cmboControlFactory.Items[0].ToString();
						}
					}
					if (!string.IsNullOrEmpty(cur))
					{
						_currentControlFactory = ControlFactory.GetControlFactory(cur);
					}
				}
				return _currentControlFactory;
			}
			set
			{
				cmboControlFactory.SelectedText = value.FactoryKey;
			}
		}




		private void RenderResult()
		{
			this.Cursor = Cursors.WaitCursor;
			Application.DoEvents();

			if (String.IsNullOrEmpty(TemplateContent))
			{
				TemplateContent = LoadTemplate();
				if (String.IsNullOrEmpty(TemplateContent))
				{
					this.Cursor = Cursors.Default;
					return;
				}
			}


			MemoryStream output = new MemoryStream(1024);
			StreamWriter w = new StreamWriter(output);

			//int ticksPerSecond = 10000000;

			long startTicks = DateTime.UtcNow.Ticks;

			try
			{
				this.Cursor = Cursors.WaitCursor;
				string surface = cmboSurface.Text;

				if (radioSingleRender.Checked)
				{
					RenderOnce(w, surface);
					w.Flush();
					output.Seek(0, SeekOrigin.Begin);

					using (StreamReader r = new StreamReader(output))
					{
						txtResult.Text = r.ReadToEnd();
					}

					TimeSpan time = new TimeSpan(DateTime.UtcNow.Ticks - startTicks);

					txtMessage.Text = "Time: " + time.Milliseconds.ToString() + " milliseconds";

				}
				else
				{
					txtMessage.Text = "";
					int numSeconds = 0;
					if (!Int32.TryParse(txtSeconds.Text, out numSeconds))
					{
						txtMessage.Text = "Invalid seconds";
						return;
					}

					long mark = DateTime.Now.Ticks;
					long targetTicks = mark + ( numSeconds * TimeSpan.TicksPerSecond);
					string xml = TemplateContent;

					int cnt = 0;
					while (DateTime.Now.Ticks < targetTicks)
					{
						RenderOnce(w, xml, surface, null);
						w.Flush();
						output.Seek(0, SeekOrigin.Begin);
						cnt++;
					}
					long finish = DateTime.Now.Ticks;

					string msg = String.Format("Plain Render Count: {0}", cnt);

					txtMessage.Text = msg;
					Application.DoEvents();
					Thread.Sleep(100);
					Application.DoEvents();

					// ==================
					// now the offset render

					GadgetMaster content = GadgetMaster.CreateGadget(OSML_CONTROL_FACTORY_KEY, TemplateContent);
					content.RenderContent(w);

					OffsetItem offsets = content.MyOffset;
					string offsetStr = offsets.ToString();

					mark = DateTime.Now.Ticks;
					targetTicks = mark + (numSeconds * TimeSpan.TicksPerSecond);

					cnt = 0;
					
					while (DateTime.Now.Ticks < targetTicks)
					{
						RenderOnce(w, xml, surface, offsetStr);
						w.Flush();
						output.Seek(0, SeekOrigin.Begin);
						cnt++;
					}
					finish = DateTime.Now.Ticks;

					msg = String.Format("\r\n\r\n Offset Render Count: {0}", cnt);
					txtMessage.Text += msg;


				}
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}

		}



		/// <summary>
		/// Renders from offsets
		/// </summary>
		/// <param name="w"></param>
		/// <param name="offsets"></param>
		private void RenderOnce(StreamWriter w, string gadgetXml, string surface, string offsets)
		{
			InitRenderUserData();


			OffsetItem offsetObj = null;
			if (!string.IsNullOrEmpty(offsets))
			{
				offsetObj = new OffsetItem(offsets);
			}
			GadgetMaster gadget = GadgetMaster.CreateGadget(CurrentControlFactory.FactoryKey, gadgetXml, offsetObj);
			gadget.MyDataContext.Culture = CurrentCulture;
			if (RenderUsingData == RenderData.Sandbox)
			{
				AccountTestData.ResolveDataControlValues(gadget.MyDataContext, CurrentUser, CurrentUser, CurrentUserFriends);
			}
			else
			{
				//gadget.MyDataContext.ResolveDataValues(GetCurrentUserId());
			}
			gadget.RenderContent(w, surface);
			if (chkDisposeGadget.Checked)
			{
				gadget.Dispose();
			}
		}


		private void RenderOnce(StreamWriter w, string surface)
		{
			RenderOnce(w, TemplateContent, surface, null);
		}


		bool isComboTemplate = false;

		private string LoadTemplate()
		{
			string s = null;
			TemplateItem cur = (TemplateItem)cmboTemplate.SelectedItem;
			if (null == cur)
			{
				cmboTemplate.Focus();
				MessageBox.Show("Select a template");
				return string.Empty;
			}
			using (StreamReader r = new StreamReader(cur.FilePath))
			{
				s = r.ReadToEnd();
			}
			return s;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadSampleTemplates();

			LoadControlFactories();

			LoadCultureDropdown();
		}

		private void LoadCultureDropdown()
		{
			List<string> cultures = GetValidCultures();
			cmboCulture.Items.Clear();
			foreach (string item in cultures)
			{
				cmboCulture.Items.Add(item);				
			}
			cmboCulture.SelectedItem = "en-US";
		}

		/// <summary>
		/// Gets all culture codes as a list of strings.
		/// </summary>
		/// <returns></returns>
		List<string> GetValidCultures()
		{
			List<string> validCultures = new List<string>();

			validCultures.Add("en-US");
			validCultures.Add("de-DE");
			validCultures.Add("fr-FR");
			validCultures.Add("es-ES");

			return validCultures;
		}


		private void LoadControlFactories()
		{
			//ControlFactory.
			List<string> factoryKeys = ControlFactory.GetControlFactoryKeys();
			foreach (string key in factoryKeys)
			{
				cmboControlFactory.Items.Add(key);
				//test factory load
				ControlFactory cf = ControlFactory.GetControlFactory(key);
				if (cf.LoadErrors.Count > 0)
				{
					AppendMessage("Factory Load errs: " + cf.FactoryKey);
					foreach (string err in cf.LoadErrors)
					{
						AppendMessage(err);
					}
					AppendMessage("");
				}
			}
			if (cmboControlFactory.Items.Count > 0)
			{
				cmboControlFactory.SelectedIndex = 0;
			}



		}

		private void LoadSampleTemplates()
		{
			string path = AppDomain.CurrentDomain.BaseDirectory;
			path = Path.Combine(path, "Samples");

			string[] files = Directory.GetFiles(path, "*.xml");

			for (int i = 0; i < files.Length; i++)
			{
				string name = Path.GetFileName(files[i]);
				TemplateItem item = new TemplateItem(name, files[i]);
				cmboTemplate.Items.Add(item);
			}
		}

		void AppendMessage(string message)
		{
			txtMessage.Text += message + "\n";
		}

		void ClearMessages()
		{
			txtMessage.Text = "";
		}


		private void cmboTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			isComboTemplate = true;
			TemplateContent = LoadTemplate();
		}

		private void txtUserId_TextChanged(object sender, EventArgs e)
		{
			InitRenderUserData();
		}

		private void radioRenderPart_CheckedChanged(object sender, EventArgs e)
		{
			cmboPartial.Enabled = radioRenderPart.Checked;
		}

		private void cmboControlFactory_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControlFactorySettings();
		}

		private void UpdateControlFactorySettings()
		{
			cmboPartial.Items.Clear();
			_currentControlFactory = null;
			List<ParseContext> contexts = CurrentControlFactory.GetAvailableContextGroups();

			foreach (ParseContext item in contexts)
			{
				cmboPartial.Items.Add(item);
			}


		}


		private void radioData_CheckedChanged(object sender, EventArgs e)
		{
			txtUserId.Enabled = (radioDataLive.Checked);
		}

		private void txtMarkup_TextChanged(object sender, EventArgs e)
		{
			isComboTemplate = false;
		}

	}


	class TemplateItem
	{
		public TemplateItem() { }
		public TemplateItem(string name, string fileName)
		{
			Name = name;
			FilePath = fileName;
		}

		public string FilePath { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

}
