namespace VixenModules.App.ExportWizard
{
	partial class BulkExportSummaryStage
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			taskProgress = new System.Windows.Forms.ProgressBar();
			lblTaskProgress = new System.Windows.Forms.Label();
			lblOverallProgress = new System.Windows.Forms.Label();
			mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			lblAudioOutputFolder = new System.Windows.Forms.Label();
			lblAudioOption = new System.Windows.Forms.Label();
			lblOutputFolder = new System.Windows.Forms.Label();
			lblTimingValue = new System.Windows.Forms.Label();
			lblFormatName = new System.Windows.Forms.Label();
			lblSummary = new System.Windows.Forms.Label();
			lblSequences = new System.Windows.Forms.Label();
			lblFormat = new System.Windows.Forms.Label();
			lblTiming = new System.Windows.Forms.Label();
			lblDestination = new System.Windows.Forms.Label();
			lblAudio = new System.Windows.Forms.Label();
			lblAudioDestination = new System.Windows.Forms.Label();
			lblSequenceCount = new System.Windows.Forms.Label();
			overallProgress = new System.Windows.Forms.ProgressBar();
			chkSaveConfig = new System.Windows.Forms.CheckBox();
			comboConfigName = new System.Windows.Forms.ComboBox();
			lblUniverse = new System.Windows.Forms.Label();
			lblUniverseFolder = new System.Windows.Forms.Label();
			lblUniverseFileWarning = new System.Windows.Forms.Label();
			lblFppInfo = new System.Windows.Forms.Label();
			lblFppHostName = new System.Windows.Forms.Label();
			lblFppHostNameValue = new System.Windows.Forms.Label();
			lblFppDescription = new System.Windows.Forms.Label();
			lblFppDescriptionValue = new System.Windows.Forms.Label();
			lblFppPlatform = new System.Windows.Forms.Label();
			lblFppPlatformValue = new System.Windows.Forms.Label();
			lblFppVariant = new System.Windows.Forms.Label();
			lblFppVariantValue = new System.Windows.Forms.Label();
			mainLayoutPanel.SuspendLayout();
			SuspendLayout();
			// 
			// taskProgress
			// 
			taskProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			mainLayoutPanel.SetColumnSpan(taskProgress, 2);
			taskProgress.Location = new System.Drawing.Point(3, 287);
			taskProgress.Name = "taskProgress";
			taskProgress.Size = new System.Drawing.Size(607, 23);
			taskProgress.TabIndex = 1;
			// 
			// lblTaskProgress
			// 
			lblTaskProgress.AutoSize = true;
			mainLayoutPanel.SetColumnSpan(lblTaskProgress, 2);
			lblTaskProgress.Location = new System.Drawing.Point(3, 269);
			lblTaskProgress.Name = "lblTaskProgress";
			lblTaskProgress.Size = new System.Drawing.Size(39, 15);
			lblTaskProgress.TabIndex = 3;
			lblTaskProgress.Text = "Ready";
			// 
			// lblOverallProgress
			// 
			lblOverallProgress.AutoSize = true;
			mainLayoutPanel.SetColumnSpan(lblOverallProgress, 2);
			lblOverallProgress.Location = new System.Drawing.Point(3, 313);
			lblOverallProgress.Name = "lblOverallProgress";
			lblOverallProgress.Size = new System.Drawing.Size(39, 15);
			lblOverallProgress.TabIndex = 5;
			lblOverallProgress.Text = "Ready";
			// 
			// mainLayoutPanel
			// 
			mainLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			mainLayoutPanel.AutoSize = false;
			mainLayoutPanel.ColumnCount = 2;
			mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			mainLayoutPanel.Controls.Add(lblAudioOutputFolder, 1, 7);
			mainLayoutPanel.Controls.Add(lblAudioOption, 1, 6);
			mainLayoutPanel.Controls.Add(lblOutputFolder, 1, 5);
			mainLayoutPanel.Controls.Add(lblTimingValue, 1, 4);
			mainLayoutPanel.Controls.Add(lblFormatName, 1, 3);
			mainLayoutPanel.Controls.Add(lblSummary, 0, 0);
			mainLayoutPanel.Controls.Add(lblSequences, 0, 2);
			mainLayoutPanel.Controls.Add(lblTaskProgress, 0, 18);
			mainLayoutPanel.Controls.Add(taskProgress, 0, 19);
			mainLayoutPanel.Controls.Add(lblFormat, 0, 3);
			mainLayoutPanel.Controls.Add(lblTiming, 0, 4);
			mainLayoutPanel.Controls.Add(lblDestination, 0, 5);
			mainLayoutPanel.Controls.Add(lblAudio, 0, 6);
			mainLayoutPanel.Controls.Add(lblAudioDestination, 0, 7);
			mainLayoutPanel.Controls.Add(lblOverallProgress, 0, 20);
			mainLayoutPanel.Controls.Add(lblSequenceCount, 1, 2);
			mainLayoutPanel.Controls.Add(overallProgress, 0, 22);
			mainLayoutPanel.Controls.Add(chkSaveConfig, 0, 16);
			mainLayoutPanel.Controls.Add(comboConfigName, 1, 16);
			mainLayoutPanel.Controls.Add(lblUniverse, 0, 8);
			mainLayoutPanel.Controls.Add(lblUniverseFolder, 1, 8);
			mainLayoutPanel.Controls.Add(lblUniverseFileWarning, 1, 9);
			mainLayoutPanel.Controls.Add(lblFppInfo, 0, 10);
			mainLayoutPanel.Controls.Add(lblFppHostName, 0, 11);
			mainLayoutPanel.Controls.Add(lblFppHostNameValue, 1, 11);
			mainLayoutPanel.Controls.Add(lblFppDescription, 0, 12);
			mainLayoutPanel.Controls.Add(lblFppDescriptionValue, 1, 12);
			mainLayoutPanel.Controls.Add(lblFppPlatform, 0, 13);
			mainLayoutPanel.Controls.Add(lblFppPlatformValue, 1, 13);
			mainLayoutPanel.Controls.Add(lblFppVariant, 0, 14);
			mainLayoutPanel.Controls.Add(lblFppVariantValue, 1, 14);
			mainLayoutPanel.Location = new System.Drawing.Point(3, 3);
			mainLayoutPanel.Name = "mainLayoutPanel";
			mainLayoutPanel.RowCount = 23;
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			mainLayoutPanel.Size = new System.Drawing.Size(613, 356);
			mainLayoutPanel.TabIndex = 6;
			mainLayoutPanel.Paint += mainLayoutPanel_Paint;
			// 
			// lblAudioOutputFolder
			// 
			lblAudioOutputFolder.AutoSize = true;
			lblAudioOutputFolder.Location = new System.Drawing.Point(145, 110);
			lblAudioOutputFolder.Name = "lblAudioOutputFolder";
			lblAudioOutputFolder.Size = new System.Drawing.Size(182, 15);
			lblAudioOutputFolder.TabIndex = 13;
			lblAudioOutputFolder.Text = "c:\\My Documents\\Vixen 3\\Export";
			// 
			// lblAudioOption
			// 
			lblAudioOption.AutoSize = true;
			lblAudioOption.Location = new System.Drawing.Point(145, 95);
			lblAudioOption.Name = "lblAudioOption";
			lblAudioOption.Size = new System.Drawing.Size(161, 15);
			lblAudioOption.TabIndex = 12;
			lblAudioOption.Text = "Renamed to match sequence";
			// 
			// lblOutputFolder
			// 
			lblOutputFolder.AutoSize = true;
			lblOutputFolder.Location = new System.Drawing.Point(145, 80);
			lblOutputFolder.Name = "lblOutputFolder";
			lblOutputFolder.Size = new System.Drawing.Size(182, 15);
			lblOutputFolder.TabIndex = 11;
			lblOutputFolder.Text = "c:\\My Documents\\Vixen 3\\Export";
			// 
			// lblTimingValue
			// 
			lblTimingValue.AutoSize = true;
			lblTimingValue.Location = new System.Drawing.Point(145, 65);
			lblTimingValue.Name = "lblTimingValue";
			lblTimingValue.Size = new System.Drawing.Size(38, 15);
			lblTimingValue.TabIndex = 10;
			lblTimingValue.Text = "50 ms";
			// 
			// lblFormatName
			// 
			lblFormatName.AutoSize = true;
			lblFormatName.Location = new System.Drawing.Point(145, 50);
			lblFormatName.Name = "lblFormatName";
			lblFormatName.Size = new System.Drawing.Size(131, 15);
			lblFormatName.TabIndex = 9;
			lblFormatName.Text = "Falcon Sequence Player";
			// 
			// lblSummary
			// 
			lblSummary.AutoSize = true;
			mainLayoutPanel.SetColumnSpan(lblSummary, 2);
			lblSummary.Location = new System.Drawing.Point(3, 0);
			lblSummary.Name = "lblSummary";
			lblSummary.Size = new System.Drawing.Size(96, 15);
			lblSummary.TabIndex = 0;
			lblSummary.Text = "Action Summary";
			// 
			// lblSequences
			// 
			lblSequences.AutoSize = true;
			lblSequences.Location = new System.Drawing.Point(3, 35);
			lblSequences.Name = "lblSequences";
			lblSequences.Size = new System.Drawing.Size(63, 15);
			lblSequences.TabIndex = 1;
			lblSequences.Text = "Sequences";
			// 
			// lblFormat
			// 
			lblFormat.AutoSize = true;
			lblFormat.Location = new System.Drawing.Point(3, 50);
			lblFormat.Name = "lblFormat";
			lblFormat.Size = new System.Drawing.Size(45, 15);
			lblFormat.TabIndex = 2;
			lblFormat.Text = "Format";
			// 
			// lblTiming
			// 
			lblTiming.AutoSize = true;
			lblTiming.Location = new System.Drawing.Point(3, 65);
			lblTiming.Name = "lblTiming";
			lblTiming.Size = new System.Drawing.Size(45, 15);
			lblTiming.TabIndex = 3;
			lblTiming.Text = "Timing";
			// 
			// lblDestination
			// 
			lblDestination.AutoSize = true;
			lblDestination.Location = new System.Drawing.Point(3, 80);
			lblDestination.Name = "lblDestination";
			lblDestination.Size = new System.Drawing.Size(67, 15);
			lblDestination.TabIndex = 4;
			lblDestination.Text = "Destination";
			// 
			// lblAudio
			// 
			lblAudio.AutoSize = true;
			lblAudio.Location = new System.Drawing.Point(3, 95);
			lblAudio.Name = "lblAudio";
			lblAudio.Size = new System.Drawing.Size(39, 15);
			lblAudio.TabIndex = 5;
			lblAudio.Text = "Audio";
			// 
			// lblAudioDestination
			// 
			lblAudioDestination.AutoSize = true;
			lblAudioDestination.Location = new System.Drawing.Point(3, 110);
			lblAudioDestination.Name = "lblAudioDestination";
			lblAudioDestination.Size = new System.Drawing.Size(102, 15);
			lblAudioDestination.TabIndex = 6;
			lblAudioDestination.Text = "Audio Destination";
			// 
			// lblSequenceCount
			// 
			lblSequenceCount.AutoSize = true;
			lblSequenceCount.Location = new System.Drawing.Point(145, 35);
			lblSequenceCount.Name = "lblSequenceCount";
			lblSequenceCount.Size = new System.Drawing.Size(13, 15);
			lblSequenceCount.TabIndex = 7;
			lblSequenceCount.Text = "5";
			// 
			// overallProgress
			// 
			overallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			mainLayoutPanel.SetColumnSpan(overallProgress, 2);
			overallProgress.Location = new System.Drawing.Point(3, 331);
			overallProgress.Name = "overallProgress";
			overallProgress.Size = new System.Drawing.Size(607, 22);
			overallProgress.TabIndex = 2;
			// 
			// chkSaveConfig
			// 
			chkSaveConfig.Anchor = System.Windows.Forms.AnchorStyles.Left;
			chkSaveConfig.AutoSize = true;
			chkSaveConfig.Location = new System.Drawing.Point(3, 280);
			chkSaveConfig.Name = "chkSaveConfig";
			chkSaveConfig.Size = new System.Drawing.Size(130, 19);
			chkSaveConfig.TabIndex = 18;
			chkSaveConfig.Text = "Save export settings";
			chkSaveConfig.UseVisualStyleBackColor = true;
			chkSaveConfig.CheckedChanged += chkSaveConfig_CheckedChanged;
			// 
			// comboConfigName
			// 
			comboConfigName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			comboConfigName.FormattingEnabled = true;
			comboConfigName.Location = new System.Drawing.Point(145, 278);
			comboConfigName.Name = "comboConfigName";
			comboConfigName.Size = new System.Drawing.Size(332, 23);
			comboConfigName.TabIndex = 19;
			comboConfigName.TextUpdate += comboConfigName_TextUpdate;
			comboConfigName.TextChanged += comboConfigName_TextChanged;
			// 
			// lblUniverse
			// 
			lblUniverse.AutoSize = true;
			lblUniverse.Location = new System.Drawing.Point(3, 125);
			lblUniverse.Name = "lblUniverse";
			lblUniverse.Size = new System.Drawing.Size(136, 15);
			lblUniverse.TabIndex = 20;
			lblUniverse.Text = "Universe File Destination";
			// 
			// lblUniverseFolder
			// 
			lblUniverseFolder.AutoSize = true;
			lblUniverseFolder.Location = new System.Drawing.Point(145, 125);
			lblUniverseFolder.Name = "lblUniverseFolder";
			lblUniverseFolder.Size = new System.Drawing.Size(182, 15);
			lblUniverseFolder.TabIndex = 21;
			lblUniverseFolder.Text = "c:\\My Documents\\Vixen 3\\Export";
			lblUniverseFolder.Click += lblUniverseFolder_Click;
			// 
			// lblUniverseFileWarning
			// 
			lblUniverseFileWarning.AutoSize = true;
			lblUniverseFileWarning.ForeColor = System.Drawing.Color.Yellow;
			lblUniverseFileWarning.Location = new System.Drawing.Point(145, 140);
			lblUniverseFileWarning.Name = "lblUniverseFileWarning";
			lblUniverseFileWarning.Size = new System.Drawing.Size(100, 15);
			lblUniverseFileWarning.TabIndex = 22;
			lblUniverseFileWarning.Text = "Universe Warning";
			lblUniverseFileWarning.Visible = false;
			// 
			// lblFppInfo
			// 
			lblFppInfo.AutoSize = true;
			mainLayoutPanel.SetColumnSpan(lblFppInfo, 2);
			lblFppInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblFppInfo.Location = new System.Drawing.Point(3, 175);
			lblFppInfo.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
			lblFppInfo.Name = "lblFppInfo";
			lblFppInfo.Size = new System.Drawing.Size(95, 15);
			lblFppInfo.TabIndex = 30;
			lblFppInfo.Text = "FPP Device Info";
			lblFppInfo.Visible = false;
			// 
			// lblFppHostName
			// 
			lblFppHostName.AutoSize = true;
			lblFppHostName.Location = new System.Drawing.Point(3, 193);
			lblFppHostName.Name = "lblFppHostName";
			lblFppHostName.Size = new System.Drawing.Size(67, 15);
			lblFppHostName.TabIndex = 31;
			lblFppHostName.Text = "Host Name";
			lblFppHostName.Visible = false;
			// 
			// lblFppHostNameValue
			// 
			lblFppHostNameValue.AutoSize = true;
			lblFppHostNameValue.Location = new System.Drawing.Point(145, 193);
			lblFppHostNameValue.Name = "lblFppHostNameValue";
			lblFppHostNameValue.Size = new System.Drawing.Size(0, 15);
			lblFppHostNameValue.TabIndex = 32;
			lblFppHostNameValue.Visible = false;
			// 
			// lblFppDescription
			// 
			lblFppDescription.AutoSize = true;
			lblFppDescription.Location = new System.Drawing.Point(3, 208);
			lblFppDescription.Name = "lblFppDescription";
			lblFppDescription.Size = new System.Drawing.Size(67, 15);
			lblFppDescription.TabIndex = 33;
			lblFppDescription.Text = "Description";
			lblFppDescription.Visible = false;
			// 
			// lblFppDescriptionValue
			// 
			lblFppDescriptionValue.AutoSize = true;
			lblFppDescriptionValue.Location = new System.Drawing.Point(145, 208);
			lblFppDescriptionValue.Name = "lblFppDescriptionValue";
			lblFppDescriptionValue.Size = new System.Drawing.Size(0, 15);
			lblFppDescriptionValue.TabIndex = 34;
			lblFppDescriptionValue.Visible = false;
			// 
			// lblFppPlatform
			// 
			lblFppPlatform.AutoSize = true;
			lblFppPlatform.Location = new System.Drawing.Point(3, 223);
			lblFppPlatform.Name = "lblFppPlatform";
			lblFppPlatform.Size = new System.Drawing.Size(53, 15);
			lblFppPlatform.TabIndex = 35;
			lblFppPlatform.Text = "Platform";
			lblFppPlatform.Visible = false;
			// 
			// lblFppPlatformValue
			// 
			lblFppPlatformValue.AutoSize = true;
			lblFppPlatformValue.Location = new System.Drawing.Point(145, 223);
			lblFppPlatformValue.Name = "lblFppPlatformValue";
			lblFppPlatformValue.Size = new System.Drawing.Size(0, 15);
			lblFppPlatformValue.TabIndex = 36;
			lblFppPlatformValue.Visible = false;
			// 
			// lblFppVariant
			// 
			lblFppVariant.AutoSize = true;
			lblFppVariant.Location = new System.Drawing.Point(3, 238);
			lblFppVariant.Name = "lblFppVariant";
			lblFppVariant.Size = new System.Drawing.Size(43, 15);
			lblFppVariant.TabIndex = 37;
			lblFppVariant.Text = "Variant";
			lblFppVariant.Visible = false;
			// 
			// lblFppVariantValue
			// 
			lblFppVariantValue.AutoSize = true;
			lblFppVariantValue.Location = new System.Drawing.Point(145, 238);
			lblFppVariantValue.Name = "lblFppVariantValue";
			lblFppVariantValue.Size = new System.Drawing.Size(0, 15);
			lblFppVariantValue.TabIndex = 38;
			lblFppVariantValue.Visible = false;
			// 
			// BulkExportSummaryStage
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(mainLayoutPanel);
			Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
			Size = new System.Drawing.Size(616, 377);
			mainLayoutPanel.ResumeLayout(false);
			mainLayoutPanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ProgressBar taskProgress;
		private System.Windows.Forms.Label lblTaskProgress;
		private System.Windows.Forms.Label lblOverallProgress;
		private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
		private System.Windows.Forms.Label lblAudioOutputFolder;
		private System.Windows.Forms.Label lblAudioOption;
		private System.Windows.Forms.Label lblOutputFolder;
		private System.Windows.Forms.Label lblTimingValue;
		private System.Windows.Forms.Label lblFormatName;
		private System.Windows.Forms.Label lblSummary;
		private System.Windows.Forms.Label lblSequences;
		private System.Windows.Forms.Label lblFormat;
		private System.Windows.Forms.Label lblTiming;
		private System.Windows.Forms.Label lblDestination;
		private System.Windows.Forms.Label lblAudio;
		private System.Windows.Forms.Label lblAudioDestination;
		private System.Windows.Forms.Label lblSequenceCount;
		private System.Windows.Forms.ProgressBar overallProgress;
		private System.Windows.Forms.CheckBox chkSaveConfig;
		private System.Windows.Forms.ComboBox comboConfigName;
		private System.Windows.Forms.Label lblUniverse;
		private System.Windows.Forms.Label lblUniverseFolder;
		private System.Windows.Forms.Label lblUniverseFileWarning;
		private System.Windows.Forms.Label lblFppInfo;
		private System.Windows.Forms.Label lblFppHostName;
		private System.Windows.Forms.Label lblFppHostNameValue;
		private System.Windows.Forms.Label lblFppDescription;
		private System.Windows.Forms.Label lblFppDescriptionValue;
		private System.Windows.Forms.Label lblFppPlatform;
		private System.Windows.Forms.Label lblFppPlatformValue;
		private System.Windows.Forms.Label lblFppVariant;
		private System.Windows.Forms.Label lblFppVariantValue;
	}
}
