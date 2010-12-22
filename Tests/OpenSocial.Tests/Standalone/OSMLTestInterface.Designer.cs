namespace OpenSocial.Test.Standalone
{
	partial class OSMLTestInterface
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.pnlControls = new System.Windows.Forms.Panel();
			this.cmboCulture = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioDataSandbox = new System.Windows.Forms.RadioButton();
			this.txtUserId = new System.Windows.Forms.TextBox();
			this.radioDataLive = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.cmboControlFactory = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cmboPartial = new System.Windows.Forms.ComboBox();
			this.radioRenderPart = new System.Windows.Forms.RadioButton();
			this.radioRenderFull = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmboSurface = new System.Windows.Forms.ComboBox();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtSeconds = new System.Windows.Forms.TextBox();
			this.radioRenderSpeed = new System.Windows.Forms.RadioButton();
			this.radioSingleRender = new System.Windows.Forms.RadioButton();
			this.cmboTemplate = new System.Windows.Forms.ComboBox();
			this.btnRender = new System.Windows.Forms.Button();
			this.splitterCode = new System.Windows.Forms.SplitContainer();
			this.txtMarkup = new System.Windows.Forms.TextBox();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.chkDisposeGadget = new System.Windows.Forms.CheckBox();
			this.chkCloneRender = new System.Windows.Forms.CheckBox();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.pnlControls.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.splitterCode.Panel1.SuspendLayout();
			this.splitterCode.Panel2.SuspendLayout();
			this.splitterCode.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMain.Name = "splitContainerMain";
			this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.pnlControls);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.Controls.Add(this.splitterCode);
			this.splitContainerMain.Size = new System.Drawing.Size(609, 554);
			this.splitContainerMain.SplitterDistance = 329;
			this.splitContainerMain.TabIndex = 31;
			// 
			// pnlControls
			// 
			this.pnlControls.Controls.Add(this.chkCloneRender);
			this.pnlControls.Controls.Add(this.chkDisposeGadget);
			this.pnlControls.Controls.Add(this.cmboCulture);
			this.pnlControls.Controls.Add(this.groupBox3);
			this.pnlControls.Controls.Add(this.label5);
			this.pnlControls.Controls.Add(this.cmboControlFactory);
			this.pnlControls.Controls.Add(this.label4);
			this.pnlControls.Controls.Add(this.groupBox2);
			this.pnlControls.Controls.Add(this.label3);
			this.pnlControls.Controls.Add(this.label2);
			this.pnlControls.Controls.Add(this.cmboSurface);
			this.pnlControls.Controls.Add(this.txtMessage);
			this.pnlControls.Controls.Add(this.groupBox1);
			this.pnlControls.Controls.Add(this.cmboTemplate);
			this.pnlControls.Controls.Add(this.btnRender);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlControls.Location = new System.Drawing.Point(0, 0);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(609, 329);
			this.pnlControls.TabIndex = 31;
			// 
			// cmboCulture
			// 
			this.cmboCulture.FormattingEnabled = true;
			this.cmboCulture.Location = new System.Drawing.Point(258, 25);
			this.cmboCulture.Name = "cmboCulture";
			this.cmboCulture.Size = new System.Drawing.Size(80, 21);
			this.cmboCulture.TabIndex = 46;
			this.cmboCulture.Text = "en-US";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioDataSandbox);
			this.groupBox3.Controls.Add(this.txtUserId);
			this.groupBox3.Controls.Add(this.radioDataLive);
			this.groupBox3.Location = new System.Drawing.Point(13, 13);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(239, 61);
			this.groupBox3.TabIndex = 45;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Data Source";
			// 
			// radioDataSandbox
			// 
			this.radioDataSandbox.AutoSize = true;
			this.radioDataSandbox.Checked = true;
			this.radioDataSandbox.Location = new System.Drawing.Point(3, 39);
			this.radioDataSandbox.Name = "radioDataSandbox";
			this.radioDataSandbox.Size = new System.Drawing.Size(93, 17);
			this.radioDataSandbox.TabIndex = 32;
			this.radioDataSandbox.TabStop = true;
			this.radioDataSandbox.Text = "Sandbox Data";
			this.radioDataSandbox.UseVisualStyleBackColor = true;
			this.radioDataSandbox.CheckedChanged += new System.EventHandler(this.radioData_CheckedChanged);
			// 
			// txtUserId
			// 
			this.txtUserId.Location = new System.Drawing.Point(122, 15);
			this.txtUserId.Name = "txtUserId";
			this.txtUserId.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.txtUserId.Size = new System.Drawing.Size(100, 20);
			this.txtUserId.TabIndex = 31;
			this.txtUserId.Text = "26000001";
			// 
			// radioDataLive
			// 
			this.radioDataLive.AutoSize = true;
			this.radioDataLive.Location = new System.Drawing.Point(3, 16);
			this.radioDataLive.Name = "radioDataLive";
			this.radioDataLive.Size = new System.Drawing.Size(107, 17);
			this.radioDataLive.TabIndex = 0;
			this.radioDataLive.Text = "Live Data UserID";
			this.radioDataLive.UseVisualStyleBackColor = true;
			this.radioDataLive.CheckedChanged += new System.EventHandler(this.radioData_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(458, 43);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 44;
			this.label5.Text = "Messages";
			// 
			// cmboControlFactory
			// 
			this.cmboControlFactory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmboControlFactory.FormattingEnabled = true;
			this.cmboControlFactory.Location = new System.Drawing.Point(98, 136);
			this.cmboControlFactory.Name = "cmboControlFactory";
			this.cmboControlFactory.Size = new System.Drawing.Size(114, 21);
			this.cmboControlFactory.TabIndex = 43;
			this.cmboControlFactory.SelectedIndexChanged += new System.EventHandler(this.cmboControlFactory_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 139);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 13);
			this.label4.TabIndex = 42;
			this.label4.Text = "Control Factory";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cmboPartial);
			this.groupBox2.Controls.Add(this.radioRenderPart);
			this.groupBox2.Controls.Add(this.radioRenderFull);
			this.groupBox2.Location = new System.Drawing.Point(12, 163);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(326, 40);
			this.groupBox2.TabIndex = 40;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Render &As";
			// 
			// cmboPartial
			// 
			this.cmboPartial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmboPartial.Enabled = false;
			this.cmboPartial.FormattingEnabled = true;
			this.cmboPartial.Location = new System.Drawing.Point(183, 13);
			this.cmboPartial.Name = "cmboPartial";
			this.cmboPartial.Size = new System.Drawing.Size(121, 21);
			this.cmboPartial.TabIndex = 2;
			// 
			// radioRenderPart
			// 
			this.radioRenderPart.AutoSize = true;
			this.radioRenderPart.Location = new System.Drawing.Point(123, 17);
			this.radioRenderPart.Name = "radioRenderPart";
			this.radioRenderPart.Size = new System.Drawing.Size(54, 17);
			this.radioRenderPart.TabIndex = 1;
			this.radioRenderPart.Text = "Partial";
			this.radioRenderPart.UseVisualStyleBackColor = true;
			this.radioRenderPart.CheckedChanged += new System.EventHandler(this.radioRenderPart_CheckedChanged);
			// 
			// radioRenderFull
			// 
			this.radioRenderFull.AutoSize = true;
			this.radioRenderFull.Checked = true;
			this.radioRenderFull.Location = new System.Drawing.Point(14, 17);
			this.radioRenderFull.Name = "radioRenderFull";
			this.radioRenderFull.Size = new System.Drawing.Size(101, 17);
			this.radioRenderFull.TabIndex = 0;
			this.radioRenderFull.TabStop = true;
			this.radioRenderFull.Text = "Full App Gadget";
			this.radioRenderFull.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 39;
			this.label3.Text = "Surface";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 13);
			this.label2.TabIndex = 38;
			this.label2.Text = "Template";
			// 
			// cmboSurface
			// 
			this.cmboSurface.FormattingEnabled = true;
			this.cmboSurface.Items.AddRange(new object[] {
            "home",
            "profile",
            "canvas"});
			this.cmboSurface.Location = new System.Drawing.Point(98, 107);
			this.cmboSurface.Name = "cmboSurface";
			this.cmboSurface.Size = new System.Drawing.Size(114, 21);
			this.cmboSurface.TabIndex = 35;
			this.cmboSurface.Text = "canvas";
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(344, 65);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.Size = new System.Drawing.Size(253, 180);
			this.txtMessage.TabIndex = 34;
			this.txtMessage.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtSeconds);
			this.groupBox1.Controls.Add(this.radioRenderSpeed);
			this.groupBox1.Controls.Add(this.radioSingleRender);
			this.groupBox1.Location = new System.Drawing.Point(12, 209);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 85);
			this.groupBox1.TabIndex = 33;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Render Exercise";
			// 
			// txtSeconds
			// 
			this.txtSeconds.Location = new System.Drawing.Point(139, 53);
			this.txtSeconds.Name = "txtSeconds";
			this.txtSeconds.Size = new System.Drawing.Size(38, 20);
			this.txtSeconds.TabIndex = 2;
			this.txtSeconds.Text = "5";
			// 
			// radioRenderSpeed
			// 
			this.radioRenderSpeed.AutoSize = true;
			this.radioRenderSpeed.Location = new System.Drawing.Point(6, 53);
			this.radioRenderSpeed.Name = "radioRenderSpeed";
			this.radioRenderSpeed.Size = new System.Drawing.Size(112, 17);
			this.radioRenderSpeed.TabIndex = 1;
			this.radioRenderSpeed.TabStop = true;
			this.radioRenderSpeed.Text = "Speed Test Timed";
			this.radioRenderSpeed.UseVisualStyleBackColor = true;
			// 
			// radioSingleRender
			// 
			this.radioSingleRender.AutoSize = true;
			this.radioSingleRender.Checked = true;
			this.radioSingleRender.Location = new System.Drawing.Point(7, 20);
			this.radioSingleRender.Name = "radioSingleRender";
			this.radioSingleRender.Size = new System.Drawing.Size(92, 17);
			this.radioSingleRender.TabIndex = 0;
			this.radioSingleRender.TabStop = true;
			this.radioSingleRender.Text = "Single Render";
			this.radioSingleRender.UseVisualStyleBackColor = true;
			// 
			// cmboTemplate
			// 
			this.cmboTemplate.FormattingEnabled = true;
			this.cmboTemplate.Location = new System.Drawing.Point(98, 80);
			this.cmboTemplate.Name = "cmboTemplate";
			this.cmboTemplate.Size = new System.Drawing.Size(114, 21);
			this.cmboTemplate.TabIndex = 32;
			this.cmboTemplate.SelectedIndexChanged += new System.EventHandler(this.cmboTemplate_SelectedIndexChanged);
			// 
			// btnRender
			// 
			this.btnRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRender.Location = new System.Drawing.Point(512, 9);
			this.btnRender.Name = "btnRender";
			this.btnRender.Size = new System.Drawing.Size(75, 23);
			this.btnRender.TabIndex = 31;
			this.btnRender.Text = "Render";
			this.btnRender.UseVisualStyleBackColor = true;
			this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
			// 
			// splitterCode
			// 
			this.splitterCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitterCode.Location = new System.Drawing.Point(0, 0);
			this.splitterCode.Name = "splitterCode";
			this.splitterCode.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitterCode.Panel1
			// 
			this.splitterCode.Panel1.Controls.Add(this.txtMarkup);
			// 
			// splitterCode.Panel2
			// 
			this.splitterCode.Panel2.Controls.Add(this.txtResult);
			this.splitterCode.Size = new System.Drawing.Size(609, 221);
			this.splitterCode.SplitterDistance = 99;
			this.splitterCode.TabIndex = 24;
			// 
			// txtMarkup
			// 
			this.txtMarkup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtMarkup.Location = new System.Drawing.Point(0, 0);
			this.txtMarkup.Multiline = true;
			this.txtMarkup.Name = "txtMarkup";
			this.txtMarkup.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMarkup.Size = new System.Drawing.Size(609, 99);
			this.txtMarkup.TabIndex = 25;
			this.txtMarkup.TabStop = false;
			this.txtMarkup.TextChanged += new System.EventHandler(this.txtMarkup_TextChanged);
			// 
			// txtResult
			// 
			this.txtResult.BackColor = System.Drawing.Color.Beige;
			this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtResult.Location = new System.Drawing.Point(0, 0);
			this.txtResult.Multiline = true;
			this.txtResult.Name = "txtResult";
			this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtResult.Size = new System.Drawing.Size(609, 118);
			this.txtResult.TabIndex = 23;
			this.txtResult.TabStop = false;
			// 
			// chkDisposeGadget
			// 
			this.chkDisposeGadget.AutoSize = true;
			this.chkDisposeGadget.Location = new System.Drawing.Point(236, 277);
			this.chkDisposeGadget.Name = "chkDisposeGadget";
			this.chkDisposeGadget.Size = new System.Drawing.Size(102, 17);
			this.chkDisposeGadget.TabIndex = 47;
			this.chkDisposeGadget.Text = "&Dispose Gadget";
			this.chkDisposeGadget.UseVisualStyleBackColor = true;
			// 
			// chkCloneRender
			// 
			this.chkCloneRender.AutoSize = true;
			this.chkCloneRender.Location = new System.Drawing.Point(236, 254);
			this.chkCloneRender.Name = "chkCloneRender";
			this.chkCloneRender.Size = new System.Drawing.Size(91, 17);
			this.chkCloneRender.TabIndex = 48;
			this.chkCloneRender.Text = "&Clone Render";
			this.chkCloneRender.UseVisualStyleBackColor = true;
			// 
			// OSMLTestInterface
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(609, 554);
			this.Controls.Add(this.splitContainerMain);
			this.Name = "OSMLTestInterface";
			this.Text = "Test OSML Render";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel2.ResumeLayout(false);
			this.splitContainerMain.ResumeLayout(false);
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.splitterCode.Panel1.ResumeLayout(false);
			this.splitterCode.Panel1.PerformLayout();
			this.splitterCode.Panel2.ResumeLayout(false);
			this.splitterCode.Panel2.PerformLayout();
			this.splitterCode.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.SplitContainer splitterCode;
		private System.Windows.Forms.TextBox txtMarkup;
		private System.Windows.Forms.TextBox txtResult;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox cmboPartial;
		private System.Windows.Forms.RadioButton radioRenderPart;
		private System.Windows.Forms.RadioButton radioRenderFull;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmboSurface;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtSeconds;
		private System.Windows.Forms.RadioButton radioRenderSpeed;
		private System.Windows.Forms.RadioButton radioSingleRender;
		private System.Windows.Forms.ComboBox cmboTemplate;
		private System.Windows.Forms.Button btnRender;
		private System.Windows.Forms.ComboBox cmboControlFactory;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtUserId;
		private System.Windows.Forms.RadioButton radioDataLive;
		private System.Windows.Forms.RadioButton radioDataSandbox;
		private System.Windows.Forms.ComboBox cmboCulture;
		private System.Windows.Forms.CheckBox chkDisposeGadget;
		private System.Windows.Forms.CheckBox chkCloneRender;
	}
}

