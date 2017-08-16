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
			this.taskProgress = new System.Windows.Forms.ProgressBar();
			this.lblTaskProgress = new System.Windows.Forms.Label();
			this.lblOverallProgress = new System.Windows.Forms.Label();
			this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.lblAudioOutputFolder = new System.Windows.Forms.Label();
			this.lblAudioOption = new System.Windows.Forms.Label();
			this.lblOutputFolder = new System.Windows.Forms.Label();
			this.lblTimingValue = new System.Windows.Forms.Label();
			this.lblFormatName = new System.Windows.Forms.Label();
			this.lblSummary = new System.Windows.Forms.Label();
			this.lblSequences = new System.Windows.Forms.Label();
			this.lblFormat = new System.Windows.Forms.Label();
			this.lblTiming = new System.Windows.Forms.Label();
			this.lblDestination = new System.Windows.Forms.Label();
			this.lblAudio = new System.Windows.Forms.Label();
			this.lblAudioDestination = new System.Windows.Forms.Label();
			this.lblSequenceCount = new System.Windows.Forms.Label();
			this.overallProgress = new System.Windows.Forms.ProgressBar();
			this.chkSaveConfig = new System.Windows.Forms.CheckBox();
			this.comboConfigName = new System.Windows.Forms.ComboBox();
			this.mainLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// taskProgress
			// 
			this.taskProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainLayoutPanel.SetColumnSpan(this.taskProgress, 2);
			this.taskProgress.Location = new System.Drawing.Point(3, 287);
			this.taskProgress.Name = "taskProgress";
			this.taskProgress.Size = new System.Drawing.Size(607, 23);
			this.taskProgress.TabIndex = 1;
			// 
			// lblTaskProgress
			// 
			this.lblTaskProgress.AutoSize = true;
			this.mainLayoutPanel.SetColumnSpan(this.lblTaskProgress, 2);
			this.lblTaskProgress.Location = new System.Drawing.Point(3, 269);
			this.lblTaskProgress.Name = "lblTaskProgress";
			this.lblTaskProgress.Size = new System.Drawing.Size(39, 15);
			this.lblTaskProgress.TabIndex = 3;
			this.lblTaskProgress.Text = "Ready";
			// 
			// lblOverallProgress
			// 
			this.lblOverallProgress.AutoSize = true;
			this.mainLayoutPanel.SetColumnSpan(this.lblOverallProgress, 2);
			this.lblOverallProgress.Location = new System.Drawing.Point(3, 313);
			this.lblOverallProgress.Name = "lblOverallProgress";
			this.lblOverallProgress.Size = new System.Drawing.Size(39, 15);
			this.lblOverallProgress.TabIndex = 5;
			this.lblOverallProgress.Text = "Ready";
			// 
			// mainLayoutPanel
			// 
			this.mainLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainLayoutPanel.AutoSize = true;
			this.mainLayoutPanel.ColumnCount = 2;
			this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainLayoutPanel.Controls.Add(this.lblAudioOutputFolder, 1, 7);
			this.mainLayoutPanel.Controls.Add(this.lblAudioOption, 1, 6);
			this.mainLayoutPanel.Controls.Add(this.lblOutputFolder, 1, 5);
			this.mainLayoutPanel.Controls.Add(this.lblTimingValue, 1, 4);
			this.mainLayoutPanel.Controls.Add(this.lblFormatName, 1, 3);
			this.mainLayoutPanel.Controls.Add(this.lblSummary, 0, 0);
			this.mainLayoutPanel.Controls.Add(this.lblSequences, 0, 2);
			this.mainLayoutPanel.Controls.Add(this.lblTaskProgress, 0, 11);
			this.mainLayoutPanel.Controls.Add(this.taskProgress, 0, 12);
			this.mainLayoutPanel.Controls.Add(this.lblFormat, 0, 3);
			this.mainLayoutPanel.Controls.Add(this.lblTiming, 0, 4);
			this.mainLayoutPanel.Controls.Add(this.lblDestination, 0, 5);
			this.mainLayoutPanel.Controls.Add(this.lblAudio, 0, 6);
			this.mainLayoutPanel.Controls.Add(this.lblAudioDestination, 0, 7);
			this.mainLayoutPanel.Controls.Add(this.lblOverallProgress, 0, 13);
			this.mainLayoutPanel.Controls.Add(this.lblSequenceCount, 1, 2);
			this.mainLayoutPanel.Controls.Add(this.overallProgress, 0, 15);
			this.mainLayoutPanel.Controls.Add(this.chkSaveConfig, 0, 9);
			this.mainLayoutPanel.Controls.Add(this.comboConfigName, 1, 9);
			this.mainLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this.mainLayoutPanel.Name = "mainLayoutPanel";
			this.mainLayoutPanel.RowCount = 16;
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainLayoutPanel.Size = new System.Drawing.Size(613, 356);
			this.mainLayoutPanel.TabIndex = 6;
			// 
			// lblAudioOutputFolder
			// 
			this.lblAudioOutputFolder.AutoSize = true;
			this.lblAudioOutputFolder.Location = new System.Drawing.Point(139, 110);
			this.lblAudioOutputFolder.Name = "lblAudioOutputFolder";
			this.lblAudioOutputFolder.Size = new System.Drawing.Size(182, 15);
			this.lblAudioOutputFolder.TabIndex = 13;
			this.lblAudioOutputFolder.Text = "c:\\My Documents\\Vixen 3\\Export";
			// 
			// lblAudioOption
			// 
			this.lblAudioOption.AutoSize = true;
			this.lblAudioOption.Location = new System.Drawing.Point(139, 95);
			this.lblAudioOption.Name = "lblAudioOption";
			this.lblAudioOption.Size = new System.Drawing.Size(161, 15);
			this.lblAudioOption.TabIndex = 12;
			this.lblAudioOption.Text = "Renamed to match sequence";
			// 
			// lblOutputFolder
			// 
			this.lblOutputFolder.AutoSize = true;
			this.lblOutputFolder.Location = new System.Drawing.Point(139, 80);
			this.lblOutputFolder.Name = "lblOutputFolder";
			this.lblOutputFolder.Size = new System.Drawing.Size(182, 15);
			this.lblOutputFolder.TabIndex = 11;
			this.lblOutputFolder.Text = "c:\\My Documents\\Vixen 3\\Export";
			// 
			// lblTimingValue
			// 
			this.lblTimingValue.AutoSize = true;
			this.lblTimingValue.Location = new System.Drawing.Point(139, 65);
			this.lblTimingValue.Name = "lblTimingValue";
			this.lblTimingValue.Size = new System.Drawing.Size(38, 15);
			this.lblTimingValue.TabIndex = 10;
			this.lblTimingValue.Text = "50 ms";
			// 
			// lblFormatName
			// 
			this.lblFormatName.AutoSize = true;
			this.lblFormatName.Location = new System.Drawing.Point(139, 50);
			this.lblFormatName.Name = "lblFormatName";
			this.lblFormatName.Size = new System.Drawing.Size(131, 15);
			this.lblFormatName.TabIndex = 9;
			this.lblFormatName.Text = "Falcon Sequence Player";
			// 
			// lblSummary
			// 
			this.lblSummary.AutoSize = true;
			this.mainLayoutPanel.SetColumnSpan(this.lblSummary, 2);
			this.lblSummary.Location = new System.Drawing.Point(3, 0);
			this.lblSummary.Name = "lblSummary";
			this.lblSummary.Size = new System.Drawing.Size(96, 15);
			this.lblSummary.TabIndex = 0;
			this.lblSummary.Text = "Action Summary";
			// 
			// lblSequences
			// 
			this.lblSequences.AutoSize = true;
			this.lblSequences.Location = new System.Drawing.Point(3, 35);
			this.lblSequences.Name = "lblSequences";
			this.lblSequences.Size = new System.Drawing.Size(63, 15);
			this.lblSequences.TabIndex = 1;
			this.lblSequences.Text = "Sequences";
			// 
			// lblFormat
			// 
			this.lblFormat.AutoSize = true;
			this.lblFormat.Location = new System.Drawing.Point(3, 50);
			this.lblFormat.Name = "lblFormat";
			this.lblFormat.Size = new System.Drawing.Size(45, 15);
			this.lblFormat.TabIndex = 2;
			this.lblFormat.Text = "Format";
			// 
			// lblTiming
			// 
			this.lblTiming.AutoSize = true;
			this.lblTiming.Location = new System.Drawing.Point(3, 65);
			this.lblTiming.Name = "lblTiming";
			this.lblTiming.Size = new System.Drawing.Size(45, 15);
			this.lblTiming.TabIndex = 3;
			this.lblTiming.Text = "Timing";
			// 
			// lblDestination
			// 
			this.lblDestination.AutoSize = true;
			this.lblDestination.Location = new System.Drawing.Point(3, 80);
			this.lblDestination.Name = "lblDestination";
			this.lblDestination.Size = new System.Drawing.Size(67, 15);
			this.lblDestination.TabIndex = 4;
			this.lblDestination.Text = "Destination";
			// 
			// lblAudio
			// 
			this.lblAudio.AutoSize = true;
			this.lblAudio.Location = new System.Drawing.Point(3, 95);
			this.lblAudio.Name = "lblAudio";
			this.lblAudio.Size = new System.Drawing.Size(39, 15);
			this.lblAudio.TabIndex = 5;
			this.lblAudio.Text = "Audio";
			// 
			// lblAudioDestination
			// 
			this.lblAudioDestination.AutoSize = true;
			this.lblAudioDestination.Location = new System.Drawing.Point(3, 110);
			this.lblAudioDestination.Name = "lblAudioDestination";
			this.lblAudioDestination.Size = new System.Drawing.Size(102, 15);
			this.lblAudioDestination.TabIndex = 6;
			this.lblAudioDestination.Text = "Audio Destination";
			// 
			// lblSequenceCount
			// 
			this.lblSequenceCount.AutoSize = true;
			this.lblSequenceCount.Location = new System.Drawing.Point(139, 35);
			this.lblSequenceCount.Name = "lblSequenceCount";
			this.lblSequenceCount.Size = new System.Drawing.Size(13, 15);
			this.lblSequenceCount.TabIndex = 7;
			this.lblSequenceCount.Text = "5";
			// 
			// overallProgress
			// 
			this.overallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainLayoutPanel.SetColumnSpan(this.overallProgress, 2);
			this.overallProgress.Location = new System.Drawing.Point(3, 331);
			this.overallProgress.Name = "overallProgress";
			this.overallProgress.Size = new System.Drawing.Size(607, 22);
			this.overallProgress.TabIndex = 2;
			// 
			// chkSaveConfig
			// 
			this.chkSaveConfig.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.chkSaveConfig.AutoSize = true;
			this.chkSaveConfig.Location = new System.Drawing.Point(3, 150);
			this.chkSaveConfig.Name = "chkSaveConfig";
			this.chkSaveConfig.Size = new System.Drawing.Size(130, 19);
			this.chkSaveConfig.TabIndex = 18;
			this.chkSaveConfig.Text = "Save export settings";
			this.chkSaveConfig.UseVisualStyleBackColor = true;
			this.chkSaveConfig.CheckedChanged += new System.EventHandler(this.chkSaveConfig_CheckedChanged);
			// 
			// comboConfigName
			// 
			this.comboConfigName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.comboConfigName.FormattingEnabled = true;
			this.comboConfigName.Location = new System.Drawing.Point(139, 148);
			this.comboConfigName.Name = "comboConfigName";
			this.comboConfigName.Size = new System.Drawing.Size(332, 23);
			this.comboConfigName.TabIndex = 19;
			this.comboConfigName.TextUpdate += new System.EventHandler(this.comboConfigName_TextUpdate);
			this.comboConfigName.TextChanged += new System.EventHandler(this.comboConfigName_TextChanged);
			// 
			// BulkExportSummaryStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainLayoutPanel);
			this.Name = "BulkExportSummaryStage";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
			this.Size = new System.Drawing.Size(616, 377);
			this.mainLayoutPanel.ResumeLayout(false);
			this.mainLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}
