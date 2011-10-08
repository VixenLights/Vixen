namespace VixenApplication
{
	partial class VixenApplication
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.contextMenuStripNewSequence = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tabPageAdministration = new System.Windows.Forms.TabPage();
			this.tabPageDisplaySetup = new System.Windows.Forms.TabPage();
			this.buttonSetupPatches = new System.Windows.Forms.Button();
			this.buttonSetupOutputControllers = new System.Windows.Forms.Button();
			this.buttonSetupChannels = new System.Windows.Forms.Button();
			this.tabPageSequences = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxRecentSequences = new System.Windows.Forms.ListBox();
			this.buttonOpenSequence = new System.Windows.Forms.Button();
			this.buttonNewSequence = new System.Windows.Forms.Button();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageTest = new System.Windows.Forms.TabPage();
			this.button1 = new System.Windows.Forms.Button();
			this.tabPageDisplaySetup.SuspendLayout();
			this.tabPageSequences.SuspendLayout();
			this.tabControlMain.SuspendLayout();
			this.tabPageTest.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(187, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "<fancy vixen logo here>";
			// 
			// contextMenuStripNewSequence
			// 
			this.contextMenuStripNewSequence.Name = "contextMenuStripNewSequence";
			this.contextMenuStripNewSequence.ShowImageMargin = false;
			this.contextMenuStripNewSequence.Size = new System.Drawing.Size(36, 4);
			// 
			// tabPageAdministration
			// 
			this.tabPageAdministration.Location = new System.Drawing.Point(4, 22);
			this.tabPageAdministration.Name = "tabPageAdministration";
			this.tabPageAdministration.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAdministration.Size = new System.Drawing.Size(452, 387);
			this.tabPageAdministration.TabIndex = 2;
			this.tabPageAdministration.Text = "Administration";
			this.tabPageAdministration.UseVisualStyleBackColor = true;
			// 
			// tabPageDisplaySetup
			// 
			this.tabPageDisplaySetup.Controls.Add(this.buttonSetupPatches);
			this.tabPageDisplaySetup.Controls.Add(this.buttonSetupOutputControllers);
			this.tabPageDisplaySetup.Controls.Add(this.buttonSetupChannels);
			this.tabPageDisplaySetup.Location = new System.Drawing.Point(4, 22);
			this.tabPageDisplaySetup.Name = "tabPageDisplaySetup";
			this.tabPageDisplaySetup.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDisplaySetup.Size = new System.Drawing.Size(452, 387);
			this.tabPageDisplaySetup.TabIndex = 1;
			this.tabPageDisplaySetup.Text = "Display Setup";
			this.tabPageDisplaySetup.UseVisualStyleBackColor = true;
			// 
			// buttonSetupPatches
			// 
			this.buttonSetupPatches.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonSetupPatches.Location = new System.Drawing.Point(106, 226);
			this.buttonSetupPatches.Name = "buttonSetupPatches";
			this.buttonSetupPatches.Size = new System.Drawing.Size(233, 34);
			this.buttonSetupPatches.TabIndex = 3;
			this.buttonSetupPatches.Text = "Configure Patches";
			this.buttonSetupPatches.UseVisualStyleBackColor = true;
			this.buttonSetupPatches.Click += new System.EventHandler(this.buttonSetupPatches_Click);
			// 
			// buttonSetupOutputControllers
			// 
			this.buttonSetupOutputControllers.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonSetupOutputControllers.Location = new System.Drawing.Point(106, 140);
			this.buttonSetupOutputControllers.Name = "buttonSetupOutputControllers";
			this.buttonSetupOutputControllers.Size = new System.Drawing.Size(233, 34);
			this.buttonSetupOutputControllers.TabIndex = 2;
			this.buttonSetupOutputControllers.Text = "Configure Output Controllers";
			this.buttonSetupOutputControllers.UseVisualStyleBackColor = true;
			this.buttonSetupOutputControllers.Click += new System.EventHandler(this.buttonSetupOutputControllers_Click);
			// 
			// buttonSetupChannels
			// 
			this.buttonSetupChannels.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonSetupChannels.Location = new System.Drawing.Point(106, 56);
			this.buttonSetupChannels.Name = "buttonSetupChannels";
			this.buttonSetupChannels.Size = new System.Drawing.Size(233, 34);
			this.buttonSetupChannels.TabIndex = 1;
			this.buttonSetupChannels.Text = "Configure Channels && Groups";
			this.buttonSetupChannels.UseVisualStyleBackColor = true;
			this.buttonSetupChannels.Click += new System.EventHandler(this.buttonSetupChannels_Click);
			// 
			// tabPageSequences
			// 
			this.tabPageSequences.Controls.Add(this.label2);
			this.tabPageSequences.Controls.Add(this.listBoxRecentSequences);
			this.tabPageSequences.Controls.Add(this.buttonOpenSequence);
			this.tabPageSequences.Controls.Add(this.buttonNewSequence);
			this.tabPageSequences.Location = new System.Drawing.Point(4, 22);
			this.tabPageSequences.Name = "tabPageSequences";
			this.tabPageSequences.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSequences.Size = new System.Drawing.Size(452, 387);
			this.tabPageSequences.TabIndex = 0;
			this.tabPageSequences.Text = "Sequences";
			this.tabPageSequences.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(104, 157);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Recent Sequences:";
			// 
			// listBoxRecentSequences
			// 
			this.listBoxRecentSequences.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.listBoxRecentSequences.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listBoxRecentSequences.FormattingEnabled = true;
			this.listBoxRecentSequences.Items.AddRange(new object[] {
            "dummy sequence 1",
            "dummy sequence 2",
            "dummy sequence 3",
            ""});
			this.listBoxRecentSequences.Location = new System.Drawing.Point(107, 173);
			this.listBoxRecentSequences.Name = "listBoxRecentSequences";
			this.listBoxRecentSequences.Size = new System.Drawing.Size(233, 197);
			this.listBoxRecentSequences.TabIndex = 2;
			// 
			// buttonOpenSequence
			// 
			this.buttonOpenSequence.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonOpenSequence.Location = new System.Drawing.Point(107, 94);
			this.buttonOpenSequence.Name = "buttonOpenSequence";
			this.buttonOpenSequence.Size = new System.Drawing.Size(233, 34);
			this.buttonOpenSequence.TabIndex = 1;
			this.buttonOpenSequence.Text = "Open Sequence...";
			this.buttonOpenSequence.UseVisualStyleBackColor = true;
			this.buttonOpenSequence.Click += new System.EventHandler(this.buttonOpenSequence_Click);
			// 
			// buttonNewSequence
			// 
			this.buttonNewSequence.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonNewSequence.Location = new System.Drawing.Point(107, 39);
			this.buttonNewSequence.Name = "buttonNewSequence";
			this.buttonNewSequence.Size = new System.Drawing.Size(233, 34);
			this.buttonNewSequence.TabIndex = 0;
			this.buttonNewSequence.Text = "New Sequence...";
			this.buttonNewSequence.UseVisualStyleBackColor = true;
			this.buttonNewSequence.Click += new System.EventHandler(this.buttonNewSequence_Click);
			// 
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabPageSequences);
			this.tabControlMain.Controls.Add(this.tabPageDisplaySetup);
			this.tabControlMain.Controls.Add(this.tabPageAdministration);
			this.tabControlMain.Controls.Add(this.tabPageTest);
			this.tabControlMain.HotTrack = true;
			this.tabControlMain.Location = new System.Drawing.Point(12, 93);
			this.tabControlMain.Multiline = true;
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(460, 413);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageTest
			// 
			this.tabPageTest.Controls.Add(this.button1);
			this.tabPageTest.Location = new System.Drawing.Point(4, 22);
			this.tabPageTest.Name = "tabPageTest";
			this.tabPageTest.Size = new System.Drawing.Size(452, 387);
			this.tabPageTest.TabIndex = 3;
			this.tabPageTest.Text = "Test";
			this.tabPageTest.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(92, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "picker";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// VixenApplication
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 518);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.label1);
			this.Name = "VixenApplication";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Vixen";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VixenApp_FormClosing);
			this.tabPageDisplaySetup.ResumeLayout(false);
			this.tabPageSequences.ResumeLayout(false);
			this.tabPageSequences.PerformLayout();
			this.tabControlMain.ResumeLayout(false);
			this.tabPageTest.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripNewSequence;
		private System.Windows.Forms.TabPage tabPageAdministration;
		private System.Windows.Forms.TabPage tabPageDisplaySetup;
		private System.Windows.Forms.TabPage tabPageSequences;
		private System.Windows.Forms.Button buttonNewSequence;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.Button buttonOpenSequence;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxRecentSequences;
		private System.Windows.Forms.Button buttonSetupPatches;
		private System.Windows.Forms.Button buttonSetupOutputControllers;
		private System.Windows.Forms.Button buttonSetupChannels;
		private System.Windows.Forms.TabPage tabPageTest;
		private System.Windows.Forms.Button button1;
	}
}

