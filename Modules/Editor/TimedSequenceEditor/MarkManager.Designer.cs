namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class MarkManager
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
			if (disposing && (components != null)) {
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
			this.groupBoxMarkCollections = new System.Windows.Forms.GroupBox();
			this.buttonRemoveCollection = new System.Windows.Forms.Button();
			this.buttonAddCollection = new System.Windows.Forms.Button();
			this.listViewMarkCollections = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOK = new System.Windows.Forms.Button();
			this.groupBoxSelectedMarkCollection = new System.Windows.Forms.GroupBox();
			this.groupBoxDetails = new System.Windows.Forms.GroupBox();
			this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panelColor = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownWeight = new System.Windows.Forms.NumericUpDown();
			this.textBoxCollectionName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxOperations = new System.Windows.Forms.GroupBox();
			this.buttonPasteEffectsToMarks = new System.Windows.Forms.Button();
			this.buttonOffsetMarks = new System.Windows.Forms.Button();
			this.buttonGenerateSubmarks = new System.Windows.Forms.Button();
			this.buttonEvenlySpaceMarks = new System.Windows.Forms.Button();
			this.buttonTapNewMarks = new System.Windows.Forms.Button();
			this.groupBoxMarks = new System.Windows.Forms.GroupBox();
			this.listViewMarks = new System.Windows.Forms.ListView();
			this.Times = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonAddOrUpdateMark = new System.Windows.Forms.Button();
			this.textBoxTime = new System.Windows.Forms.TextBox();
			this.buttonSelectAllMarks = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonCopyAndOffsetMarks = new System.Windows.Forms.Button();
			this.buttonGenerateBeatMarks = new System.Windows.Forms.Button();
			this.groupBoxMarkCollections.SuspendLayout();
			this.groupBoxSelectedMarkCollection.SuspendLayout();
			this.groupBoxDetails.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).BeginInit();
			this.groupBoxOperations.SuspendLayout();
			this.groupBoxMarks.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxMarkCollections
			// 
			this.groupBoxMarkCollections.Controls.Add(this.buttonRemoveCollection);
			this.groupBoxMarkCollections.Controls.Add(this.buttonAddCollection);
			this.groupBoxMarkCollections.Controls.Add(this.listViewMarkCollections);
			this.groupBoxMarkCollections.Location = new System.Drawing.Point(12, 12);
			this.groupBoxMarkCollections.Name = "groupBoxMarkCollections";
			this.groupBoxMarkCollections.Size = new System.Drawing.Size(238, 263);
			this.groupBoxMarkCollections.TabIndex = 1;
			this.groupBoxMarkCollections.TabStop = false;
			this.groupBoxMarkCollections.Text = "Mark Collections";
			// 
			// buttonRemoveCollection
			// 
			this.buttonRemoveCollection.Enabled = false;
			this.buttonRemoveCollection.Location = new System.Drawing.Point(122, 232);
			this.buttonRemoveCollection.Name = "buttonRemoveCollection";
			this.buttonRemoveCollection.Size = new System.Drawing.Size(110, 25);
			this.buttonRemoveCollection.TabIndex = 3;
			this.buttonRemoveCollection.Text = "Remove Collection";
			this.buttonRemoveCollection.UseVisualStyleBackColor = true;
			this.buttonRemoveCollection.Click += new System.EventHandler(this.buttonRemoveCollection_Click);
			// 
			// buttonAddCollection
			// 
			this.buttonAddCollection.Location = new System.Drawing.Point(6, 232);
			this.buttonAddCollection.Name = "buttonAddCollection";
			this.buttonAddCollection.Size = new System.Drawing.Size(110, 25);
			this.buttonAddCollection.TabIndex = 2;
			this.buttonAddCollection.Text = "Add Collection";
			this.buttonAddCollection.UseVisualStyleBackColor = true;
			this.buttonAddCollection.Click += new System.EventHandler(this.buttonAddCollection_Click);
			// 
			// listViewMarkCollections
			// 
			this.listViewMarkCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
			this.listViewMarkCollections.FullRowSelect = true;
			this.listViewMarkCollections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewMarkCollections.HideSelection = false;
			this.listViewMarkCollections.Location = new System.Drawing.Point(6, 19);
			this.listViewMarkCollections.MultiSelect = false;
			this.listViewMarkCollections.Name = "listViewMarkCollections";
			this.listViewMarkCollections.Size = new System.Drawing.Size(226, 207);
			this.listViewMarkCollections.TabIndex = 1;
			this.listViewMarkCollections.UseCompatibleStateImageBehavior = false;
			this.listViewMarkCollections.View = System.Windows.Forms.View.Details;
			this.listViewMarkCollections.SelectedIndexChanged += new System.EventHandler(this.listViewMarkCollections_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 114;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Weight";
			this.columnHeader3.Width = 48;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Marks";
			this.columnHeader2.Width = 43;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(563, 286);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 25);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// groupBoxSelectedMarkCollection
			// 
			this.groupBoxSelectedMarkCollection.Controls.Add(this.groupBoxDetails);
			this.groupBoxSelectedMarkCollection.Controls.Add(this.groupBoxOperations);
			this.groupBoxSelectedMarkCollection.Controls.Add(this.groupBoxMarks);
			this.groupBoxSelectedMarkCollection.Location = new System.Drawing.Point(256, 12);
			this.groupBoxSelectedMarkCollection.Name = "groupBoxSelectedMarkCollection";
			this.groupBoxSelectedMarkCollection.Size = new System.Drawing.Size(473, 263);
			this.groupBoxSelectedMarkCollection.TabIndex = 3;
			this.groupBoxSelectedMarkCollection.TabStop = false;
			this.groupBoxSelectedMarkCollection.Text = "Selected Collection";
			// 
			// groupBoxDetails
			// 
			this.groupBoxDetails.Controls.Add(this.checkBoxEnabled);
			this.groupBoxDetails.Controls.Add(this.label3);
			this.groupBoxDetails.Controls.Add(this.panelColor);
			this.groupBoxDetails.Controls.Add(this.label2);
			this.groupBoxDetails.Controls.Add(this.numericUpDownWeight);
			this.groupBoxDetails.Controls.Add(this.textBoxCollectionName);
			this.groupBoxDetails.Controls.Add(this.label1);
			this.groupBoxDetails.Location = new System.Drawing.Point(6, 19);
			this.groupBoxDetails.Name = "groupBoxDetails";
			this.groupBoxDetails.Size = new System.Drawing.Size(154, 153);
			this.groupBoxDetails.TabIndex = 9;
			this.groupBoxDetails.TabStop = false;
			this.groupBoxDetails.Text = "Details";
			// 
			// checkBoxEnabled
			// 
			this.checkBoxEnabled.AutoSize = true;
			this.checkBoxEnabled.Location = new System.Drawing.Point(17, 27);
			this.checkBoxEnabled.Name = "checkBoxEnabled";
			this.checkBoxEnabled.Size = new System.Drawing.Size(65, 17);
			this.checkBoxEnabled.TabIndex = 14;
			this.checkBoxEnabled.Text = "Enabled";
			this.checkBoxEnabled.UseVisualStyleBackColor = true;
			this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 114);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Color:";
			// 
			// panelColor
			// 
			this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelColor.Location = new System.Drawing.Point(58, 109);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(60, 25);
			this.panelColor.TabIndex = 12;
			this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 85);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Weight:";
			// 
			// numericUpDownWeight
			// 
			this.numericUpDownWeight.Location = new System.Drawing.Point(58, 83);
			this.numericUpDownWeight.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numericUpDownWeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownWeight.Name = "numericUpDownWeight";
			this.numericUpDownWeight.Size = new System.Drawing.Size(46, 20);
			this.numericUpDownWeight.TabIndex = 10;
			this.numericUpDownWeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownWeight.ValueChanged += new System.EventHandler(this.numericUpDownWeight_ValueChanged);
			// 
			// textBoxCollectionName
			// 
			this.textBoxCollectionName.Location = new System.Drawing.Point(58, 57);
			this.textBoxCollectionName.Name = "textBoxCollectionName";
			this.textBoxCollectionName.Size = new System.Drawing.Size(87, 20);
			this.textBoxCollectionName.TabIndex = 9;
			this.textBoxCollectionName.TextChanged += new System.EventHandler(this.textBoxCollectionName_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Name:";
			// 
			// groupBoxOperations
			// 
			this.groupBoxOperations.Controls.Add(this.buttonGenerateBeatMarks);
			this.groupBoxOperations.Controls.Add(this.buttonCopyAndOffsetMarks);
			this.groupBoxOperations.Controls.Add(this.buttonPasteEffectsToMarks);
			this.groupBoxOperations.Controls.Add(this.buttonOffsetMarks);
			this.groupBoxOperations.Controls.Add(this.buttonGenerateSubmarks);
			this.groupBoxOperations.Controls.Add(this.buttonEvenlySpaceMarks);
			this.groupBoxOperations.Controls.Add(this.buttonTapNewMarks);
			this.groupBoxOperations.Location = new System.Drawing.Point(301, 19);
			this.groupBoxOperations.Name = "groupBoxOperations";
			this.groupBoxOperations.Size = new System.Drawing.Size(164, 238);
			this.groupBoxOperations.TabIndex = 8;
			this.groupBoxOperations.TabStop = false;
			this.groupBoxOperations.Text = "Operations";
			// 
			// buttonPasteEffectsToMarks
			// 
			this.buttonPasteEffectsToMarks.Location = new System.Drawing.Point(6, 143);
			this.buttonPasteEffectsToMarks.Name = "buttonPasteEffectsToMarks";
			this.buttonPasteEffectsToMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonPasteEffectsToMarks.TabIndex = 9;
			this.buttonPasteEffectsToMarks.Text = "Paste effect to marks";
			this.buttonPasteEffectsToMarks.UseVisualStyleBackColor = true;
			this.buttonPasteEffectsToMarks.Click += new System.EventHandler(this.buttonPasteEffectsToMarks_Click);
			// 
			// buttonOffsetMarks
			// 
			this.buttonOffsetMarks.Location = new System.Drawing.Point(6, 50);
			this.buttonOffsetMarks.Name = "buttonOffsetMarks";
			this.buttonOffsetMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonOffsetMarks.TabIndex = 8;
			this.buttonOffsetMarks.Text = "Offset marks";
			this.buttonOffsetMarks.UseVisualStyleBackColor = true;
			this.buttonOffsetMarks.Click += new System.EventHandler(this.buttonOffsetMarks_Click);
			// 
			// buttonGenerateSubmarks
			// 
			this.buttonGenerateSubmarks.Location = new System.Drawing.Point(6, 112);
			this.buttonGenerateSubmarks.Name = "buttonGenerateSubmarks";
			this.buttonGenerateSubmarks.Size = new System.Drawing.Size(151, 25);
			this.buttonGenerateSubmarks.TabIndex = 7;
			this.buttonGenerateSubmarks.Text = "Generate submarks";
			this.buttonGenerateSubmarks.UseVisualStyleBackColor = true;
			this.buttonGenerateSubmarks.Click += new System.EventHandler(this.buttonGenerateSubmarks_Click);
			// 
			// buttonEvenlySpaceMarks
			// 
			this.buttonEvenlySpaceMarks.Location = new System.Drawing.Point(6, 81);
			this.buttonEvenlySpaceMarks.Name = "buttonEvenlySpaceMarks";
			this.buttonEvenlySpaceMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonEvenlySpaceMarks.TabIndex = 6;
			this.buttonEvenlySpaceMarks.Text = "Evenly space marks";
			this.buttonEvenlySpaceMarks.UseVisualStyleBackColor = true;
			this.buttonEvenlySpaceMarks.Click += new System.EventHandler(this.buttonEvenlySpaceMarks_Click);
			// 
			// buttonTapNewMarks
			// 
			this.buttonTapNewMarks.Location = new System.Drawing.Point(6, 19);
			this.buttonTapNewMarks.Name = "buttonTapNewMarks";
			this.buttonTapNewMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonTapNewMarks.TabIndex = 5;
			this.buttonTapNewMarks.Text = "Tap new marks...";
			this.buttonTapNewMarks.UseVisualStyleBackColor = true;
			this.buttonTapNewMarks.Click += new System.EventHandler(this.buttonTapNewMarks_Click);
			// 
			// groupBoxMarks
			// 
			this.groupBoxMarks.Controls.Add(this.listViewMarks);
			this.groupBoxMarks.Controls.Add(this.buttonAddOrUpdateMark);
			this.groupBoxMarks.Controls.Add(this.textBoxTime);
			this.groupBoxMarks.Controls.Add(this.buttonSelectAllMarks);
			this.groupBoxMarks.Location = new System.Drawing.Point(166, 19);
			this.groupBoxMarks.Name = "groupBoxMarks";
			this.groupBoxMarks.Size = new System.Drawing.Size(129, 238);
			this.groupBoxMarks.TabIndex = 1;
			this.groupBoxMarks.TabStop = false;
			this.groupBoxMarks.Text = "Marks";
			// 
			// listViewMarks
			// 
			this.listViewMarks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Times});
			this.listViewMarks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewMarks.HideSelection = false;
			this.listViewMarks.Location = new System.Drawing.Point(6, 19);
			this.listViewMarks.Name = "listViewMarks";
			this.listViewMarks.Size = new System.Drawing.Size(116, 149);
			this.listViewMarks.TabIndex = 8;
			this.listViewMarks.UseCompatibleStateImageBehavior = false;
			this.listViewMarks.View = System.Windows.Forms.View.Details;
			this.listViewMarks.SelectedIndexChanged += new System.EventHandler(this.listViewMarks_SelectedIndexChanged);
			this.listViewMarks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewMarks_KeyDown);
			// 
			// Times
			// 
			this.Times.Width = 90;
			// 
			// buttonAddOrUpdateMark
			// 
			this.buttonAddOrUpdateMark.Location = new System.Drawing.Point(73, 174);
			this.buttonAddOrUpdateMark.Name = "buttonAddOrUpdateMark";
			this.buttonAddOrUpdateMark.Size = new System.Drawing.Size(50, 25);
			this.buttonAddOrUpdateMark.TabIndex = 7;
			this.buttonAddOrUpdateMark.Text = "Add";
			this.buttonAddOrUpdateMark.UseVisualStyleBackColor = true;
			this.buttonAddOrUpdateMark.Click += new System.EventHandler(this.buttonAddOrUpdateMark_Click);
			// 
			// textBoxTime
			// 
			this.textBoxTime.Location = new System.Drawing.Point(6, 177);
			this.textBoxTime.Name = "textBoxTime";
			this.textBoxTime.Size = new System.Drawing.Size(60, 20);
			this.textBoxTime.TabIndex = 6;
			// 
			// buttonSelectAllMarks
			// 
			this.buttonSelectAllMarks.Location = new System.Drawing.Point(6, 205);
			this.buttonSelectAllMarks.Name = "buttonSelectAllMarks";
			this.buttonSelectAllMarks.Size = new System.Drawing.Size(116, 25);
			this.buttonSelectAllMarks.TabIndex = 5;
			this.buttonSelectAllMarks.Text = "Select All";
			this.buttonSelectAllMarks.UseVisualStyleBackColor = true;
			this.buttonSelectAllMarks.Click += new System.EventHandler(this.buttonSelectAllMarks_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(649, 286);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 25);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonCopyAndOffsetMarks
			// 
			this.buttonCopyAndOffsetMarks.Location = new System.Drawing.Point(6, 172);
			this.buttonCopyAndOffsetMarks.Name = "buttonCopyAndOffsetMarks";
			this.buttonCopyAndOffsetMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonCopyAndOffsetMarks.TabIndex = 10;
			this.buttonCopyAndOffsetMarks.Text = "Copy && offset marks";
			this.buttonCopyAndOffsetMarks.UseVisualStyleBackColor = true;
			this.buttonCopyAndOffsetMarks.Click += new System.EventHandler(this.buttonCopyAndOffsetMarks_Click);
			// 
			// buttonGenerateBeatMarks
			// 
			this.buttonGenerateBeatMarks.Location = new System.Drawing.Point(6, 203);
			this.buttonGenerateBeatMarks.Name = "buttonGenerateBeatMarks";
			this.buttonGenerateBeatMarks.Size = new System.Drawing.Size(151, 25);
			this.buttonGenerateBeatMarks.TabIndex = 11;
			this.buttonGenerateBeatMarks.Text = "Generate beat marks";
			this.buttonGenerateBeatMarks.UseVisualStyleBackColor = true;
			this.buttonGenerateBeatMarks.Click += new System.EventHandler(this.buttonGenerateBeatMarks_Click);
			// 
			// MarkManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(740, 323);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBoxSelectedMarkCollection);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxMarkCollections);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(746, 351);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(746, 351);
			this.Name = "MarkManager";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mark Collections Manager";
			this.Load += new System.EventHandler(this.MarkManager_Load);
			this.groupBoxMarkCollections.ResumeLayout(false);
			this.groupBoxSelectedMarkCollection.ResumeLayout(false);
			this.groupBoxDetails.ResumeLayout(false);
			this.groupBoxDetails.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).EndInit();
			this.groupBoxOperations.ResumeLayout(false);
			this.groupBoxMarks.ResumeLayout(false);
			this.groupBoxMarks.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxMarkCollections;
		private System.Windows.Forms.Button buttonRemoveCollection;
		private System.Windows.Forms.Button buttonAddCollection;
		private System.Windows.Forms.ListView listViewMarkCollections;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBoxSelectedMarkCollection;
		private System.Windows.Forms.GroupBox groupBoxMarks;
		private System.Windows.Forms.Button buttonSelectAllMarks;
		private System.Windows.Forms.GroupBox groupBoxOperations;
		private System.Windows.Forms.Button buttonTapNewMarks;
		private System.Windows.Forms.Button buttonAddOrUpdateMark;
		private System.Windows.Forms.TextBox textBoxTime;
		private System.Windows.Forms.Button buttonOffsetMarks;
		private System.Windows.Forms.Button buttonGenerateSubmarks;
		private System.Windows.Forms.Button buttonEvenlySpaceMarks;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ListView listViewMarks;
		private System.Windows.Forms.ColumnHeader Times;
		private System.Windows.Forms.GroupBox groupBoxDetails;
		private System.Windows.Forms.CheckBox checkBoxEnabled;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panelColor;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownWeight;
		private System.Windows.Forms.TextBox textBoxCollectionName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonPasteEffectsToMarks;
		private System.Windows.Forms.Button buttonCopyAndOffsetMarks;
		private System.Windows.Forms.Button buttonGenerateBeatMarks;

	}
}