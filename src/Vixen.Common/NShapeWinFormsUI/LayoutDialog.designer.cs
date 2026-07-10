namespace Dataweb.NShape.WinFormsUI {

	partial class LayoutDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Panel expansionPanel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutDialog));
			this.keepAspectRationCheckBox = new System.Windows.Forms.CheckBox();
			this.expandDescriptionLabel = new System.Windows.Forms.Label();
			this.verticalCompressionLabel = new System.Windows.Forms.Label();
			this.horizontalCompressionLabel = new System.Windows.Forms.Label();
			this.verticalCompressionTrackBar = new System.Windows.Forms.TrackBar();
			this.horizontalCompressionTrackBar = new System.Windows.Forms.TrackBar();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.fastRadioButton = new System.Windows.Forms.RadioButton();
			this.animatedRadioButton = new System.Windows.Forms.RadioButton();
			this.previewGroupBox = new System.Windows.Forms.GroupBox();
			this.stepRadioButton = new System.Windows.Forms.RadioButton();
			this.immediateRadioButton = new System.Windows.Forms.RadioButton();
			this.applyButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.alignmentPanel = new System.Windows.Forms.Panel();
			this.gridDescriptionLabel = new System.Windows.Forms.Label();
			this.gridRowDistanceLabel = new System.Windows.Forms.Label();
			this.gridColumnDistanceLabel = new System.Windows.Forms.Label();
			this.rowDistanceTrackBar = new System.Windows.Forms.TrackBar();
			this.columnDistanceTrackBar = new System.Windows.Forms.TrackBar();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.centerButton = new System.Windows.Forms.Button();
			this.layoutTimer = new System.Windows.Forms.Timer(this.components);
			this.repulsionPanel = new System.Windows.Forms.Panel();
			this.repulsionRangeTrackBar = new System.Windows.Forms.TrackBar();
			this.repulsionRangeLabel = new System.Windows.Forms.Label();
			this.repulsionLabel4 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.attractionStrengthLabel = new System.Windows.Forms.Label();
			this.repulsionStrengthLabel = new System.Windows.Forms.Label();
			this.repulsionStrengthTrackBar = new System.Windows.Forms.TrackBar();
			this.attractionStrengthTrackBar = new System.Windows.Forms.TrackBar();
			this.repulsionLabel3 = new System.Windows.Forms.Label();
			this.repulsionLabel2 = new System.Windows.Forms.Label();
			this.flowPanel = new System.Windows.Forms.Panel();
			this.flowRowDistanceLabel = new System.Windows.Forms.Label();
			this.flowLayerDistanceLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.flowRowDistanceTrackBar = new System.Windows.Forms.TrackBar();
			this.flowLayerDistanceTrackBar = new System.Windows.Forms.TrackBar();
			this.flowDirectionGroupBox = new System.Windows.Forms.GroupBox();
			this.leftToRightRadioButton = new System.Windows.Forms.RadioButton();
			this.topDownRadioButton = new System.Windows.Forms.RadioButton();
			this.rightToLeftRadioButton = new System.Windows.Forms.RadioButton();
			this.bottomUpRadioButton = new System.Windows.Forms.RadioButton();
			this.flowDescriptionLabel = new System.Windows.Forms.Label();
			this.previewButton = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.undoButton = new System.Windows.Forms.Button();
			this.redoButton = new System.Windows.Forms.Button();
			this.algorithmListBox = new Dataweb.NShape.WinFormsUI.VerticalTabControl();
			expansionPanel = new System.Windows.Forms.Panel();
			expansionPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.verticalCompressionTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horizontalCompressionTrackBar)).BeginInit();
			this.previewGroupBox.SuspendLayout();
			this.alignmentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rowDistanceTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.columnDistanceTrackBar)).BeginInit();
			this.repulsionPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.repulsionRangeTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repulsionStrengthTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.attractionStrengthTrackBar)).BeginInit();
			this.flowPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.flowRowDistanceTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.flowLayerDistanceTrackBar)).BeginInit();
			this.flowDirectionGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// expansionPanel
			// 
			expansionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			expansionPanel.Controls.Add(this.keepAspectRationCheckBox);
			expansionPanel.Controls.Add(this.expandDescriptionLabel);
			expansionPanel.Controls.Add(this.verticalCompressionLabel);
			expansionPanel.Controls.Add(this.horizontalCompressionLabel);
			expansionPanel.Controls.Add(this.verticalCompressionTrackBar);
			expansionPanel.Controls.Add(this.horizontalCompressionTrackBar);
			expansionPanel.Controls.Add(this.label7);
			expansionPanel.Controls.Add(this.label8);
			expansionPanel.Location = new System.Drawing.Point(133, 5);
			expansionPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			expansionPanel.Name = "expansionPanel";
			expansionPanel.Size = new System.Drawing.Size(488, 338);
			expansionPanel.TabIndex = 10;
			expansionPanel.Tag = "Expansion";
			// 
			// keepAspectRationCheckBox
			// 
			this.keepAspectRationCheckBox.AutoSize = true;
			this.keepAspectRationCheckBox.Checked = true;
			this.keepAspectRationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.keepAspectRationCheckBox.Location = new System.Drawing.Point(11, 281);
			this.keepAspectRationCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.keepAspectRationCheckBox.Name = "keepAspectRationCheckBox";
			this.keepAspectRationCheckBox.Size = new System.Drawing.Size(109, 17);
			this.keepAspectRationCheckBox.TabIndex = 9;
			this.keepAspectRationCheckBox.Text = "Keep aspect ratio";
			this.keepAspectRationCheckBox.UseVisualStyleBackColor = true;
			this.keepAspectRationCheckBox.CheckedChanged += new System.EventHandler(this.keepAspectRationCheckBox_CheckedChanged);
			// 
			// expandDescriptionLabel
			// 
			this.expandDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.expandDescriptionLabel.Location = new System.Drawing.Point(5, 4);
			this.expandDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.expandDescriptionLabel.Name = "expandDescriptionLabel";
			this.expandDescriptionLabel.Size = new System.Drawing.Size(477, 85);
			this.expandDescriptionLabel.TabIndex = 8;
			this.expandDescriptionLabel.Text = "Track the sliders to compress or expand the selected shapes without modifying the" +
    "ir relative positions.\r\nUse this layouter to assign a larger or smaller area of " +
    "the diagram to the selected shapes.";
			// 
			// verticalCompressionLabel
			// 
			this.verticalCompressionLabel.AutoSize = true;
			this.verticalCompressionLabel.Location = new System.Drawing.Point(180, 199);
			this.verticalCompressionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.verticalCompressionLabel.Name = "verticalCompressionLabel";
			this.verticalCompressionLabel.Size = new System.Drawing.Size(25, 13);
			this.verticalCompressionLabel.TabIndex = 7;
			this.verticalCompressionLabel.Text = "100";
			// 
			// horizontalCompressionLabel
			// 
			this.horizontalCompressionLabel.AutoSize = true;
			this.horizontalCompressionLabel.Location = new System.Drawing.Point(200, 121);
			this.horizontalCompressionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.horizontalCompressionLabel.Name = "horizontalCompressionLabel";
			this.horizontalCompressionLabel.Size = new System.Drawing.Size(25, 13);
			this.horizontalCompressionLabel.TabIndex = 6;
			this.horizontalCompressionLabel.Text = "100";
			// 
			// verticalCompressionTrackBar
			// 
			this.verticalCompressionTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.verticalCompressionTrackBar.LargeChange = 20;
			this.verticalCompressionTrackBar.Location = new System.Drawing.Point(11, 219);
			this.verticalCompressionTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.verticalCompressionTrackBar.Maximum = 500;
			this.verticalCompressionTrackBar.Name = "verticalCompressionTrackBar";
			this.verticalCompressionTrackBar.Size = new System.Drawing.Size(457, 45);
			this.verticalCompressionTrackBar.TabIndex = 5;
			this.verticalCompressionTrackBar.TickFrequency = 25;
			this.verticalCompressionTrackBar.Value = 100;
			this.verticalCompressionTrackBar.ValueChanged += new System.EventHandler(this.verticalCompressionTrackBar_ValueChanged);
			// 
			// horizontalCompressionTrackBar
			// 
			this.horizontalCompressionTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.horizontalCompressionTrackBar.AutoSize = false;
			this.horizontalCompressionTrackBar.LargeChange = 20;
			this.horizontalCompressionTrackBar.Location = new System.Drawing.Point(11, 140);
			this.horizontalCompressionTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.horizontalCompressionTrackBar.Maximum = 500;
			this.horizontalCompressionTrackBar.Name = "horizontalCompressionTrackBar";
			this.horizontalCompressionTrackBar.Size = new System.Drawing.Size(457, 55);
			this.horizontalCompressionTrackBar.TabIndex = 4;
			this.horizontalCompressionTrackBar.TickFrequency = 25;
			this.horizontalCompressionTrackBar.Value = 100;
			this.horizontalCompressionTrackBar.ValueChanged += new System.EventHandler(this.horizontalCompressionTrackBar_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(5, 199);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(129, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "Vertical compression in %:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 121);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(141, 13);
			this.label8.TabIndex = 1;
			this.label8.Text = "Horizontal compression in %:";
			// 
			// fastRadioButton
			// 
			this.fastRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.fastRadioButton.AutoSize = true;
			this.fastRadioButton.Location = new System.Drawing.Point(113, 23);
			this.fastRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.fastRadioButton.Name = "fastRadioButton";
			this.fastRadioButton.Size = new System.Drawing.Size(60, 21);
			this.fastRadioButton.TabIndex = 1;
			this.fastRadioButton.Text = "Fast";
			this.fastRadioButton.UseVisualStyleBackColor = true;
			// 
			// animatedRadioButton
			// 
			this.animatedRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.animatedRadioButton.AutoSize = true;
			this.animatedRadioButton.Checked = true;
			this.animatedRadioButton.Location = new System.Drawing.Point(181, 23);
			this.animatedRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.animatedRadioButton.Name = "animatedRadioButton";
			this.animatedRadioButton.Size = new System.Drawing.Size(92, 21);
			this.animatedRadioButton.TabIndex = 2;
			this.animatedRadioButton.TabStop = true;
			this.animatedRadioButton.Text = "Animated";
			this.animatedRadioButton.UseVisualStyleBackColor = true;
			// 
			// previewGroupBox
			// 
			this.previewGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.previewGroupBox.Controls.Add(this.stepRadioButton);
			this.previewGroupBox.Controls.Add(this.immediateRadioButton);
			this.previewGroupBox.Controls.Add(this.fastRadioButton);
			this.previewGroupBox.Controls.Add(this.animatedRadioButton);
			this.previewGroupBox.Location = new System.Drawing.Point(133, 359);
			this.previewGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.previewGroupBox.Name = "previewGroupBox";
			this.previewGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.previewGroupBox.Size = new System.Drawing.Size(379, 60);
			this.previewGroupBox.TabIndex = 5;
			this.previewGroupBox.TabStop = false;
			this.previewGroupBox.Text = "Preview";
			// 
			// stepRadioButton
			// 
			this.stepRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.stepRadioButton.AutoSize = true;
			this.stepRadioButton.Location = new System.Drawing.Point(281, 23);
			this.stepRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.stepRadioButton.Name = "stepRadioButton";
			this.stepRadioButton.Size = new System.Drawing.Size(63, 21);
			this.stepRadioButton.TabIndex = 3;
			this.stepRadioButton.TabStop = true;
			this.stepRadioButton.Text = "Step";
			this.stepRadioButton.UseVisualStyleBackColor = true;
			// 
			// immediateRadioButton
			// 
			this.immediateRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.immediateRadioButton.AutoSize = true;
			this.immediateRadioButton.Location = new System.Drawing.Point(8, 23);
			this.immediateRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.immediateRadioButton.Name = "immediateRadioButton";
			this.immediateRadioButton.Size = new System.Drawing.Size(97, 21);
			this.immediateRadioButton.TabIndex = 0;
			this.immediateRadioButton.TabStop = true;
			this.immediateRadioButton.Text = "Immediate";
			this.immediateRadioButton.UseVisualStyleBackColor = true;
			this.immediateRadioButton.CheckedChanged += new System.EventHandler(this.immediateRadioButton_CheckedChanged);
			// 
			// applyButton
			// 
			this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.applyButton.Location = new System.Drawing.Point(216, 438);
			this.applyButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(68, 28);
			this.applyButton.TabIndex = 6;
			this.applyButton.Text = "Apply";
			this.toolTip.SetToolTip(this.applyButton, "When enabled (in immediate and in stepping mode) makes current layout operation p" +
        "ermanent.");
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(520, 441);
			this.closeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(100, 28);
			this.closeButton.TabIndex = 7;
			this.closeButton.Text = "Close";
			this.toolTip.SetToolTip(this.closeButton, "Closes the layout control.");
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// alignmentPanel
			// 
			this.alignmentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.alignmentPanel.Controls.Add(this.gridDescriptionLabel);
			this.alignmentPanel.Controls.Add(this.gridRowDistanceLabel);
			this.alignmentPanel.Controls.Add(this.gridColumnDistanceLabel);
			this.alignmentPanel.Controls.Add(this.rowDistanceTrackBar);
			this.alignmentPanel.Controls.Add(this.columnDistanceTrackBar);
			this.alignmentPanel.Controls.Add(this.label4);
			this.alignmentPanel.Controls.Add(this.label3);
			this.alignmentPanel.Location = new System.Drawing.Point(133, 5);
			this.alignmentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.alignmentPanel.Name = "alignmentPanel";
			this.alignmentPanel.Size = new System.Drawing.Size(488, 338);
			this.alignmentPanel.TabIndex = 5;
			this.alignmentPanel.Tag = "Alignment";
			// 
			// gridDescriptionLabel
			// 
			this.gridDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridDescriptionLabel.Location = new System.Drawing.Point(4, 0);
			this.gridDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.gridDescriptionLabel.Name = "gridDescriptionLabel";
			this.gridDescriptionLabel.Size = new System.Drawing.Size(477, 42);
			this.gridDescriptionLabel.TabIndex = 9;
			this.gridDescriptionLabel.Text = "Positions the shapes on the intersections of a rectangular grid.  Greater coarsen" +
    "ess  leads to larger grids.";
			// 
			// gridRowDistanceLabel
			// 
			this.gridRowDistanceLabel.AutoSize = true;
			this.gridRowDistanceLabel.Location = new System.Drawing.Point(244, 183);
			this.gridRowDistanceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.gridRowDistanceLabel.Name = "gridRowDistanceLabel";
			this.gridRowDistanceLabel.Size = new System.Drawing.Size(19, 13);
			this.gridRowDistanceLabel.TabIndex = 7;
			this.gridRowDistanceLabel.Text = "50";
			// 
			// gridColumnDistanceLabel
			// 
			this.gridColumnDistanceLabel.AutoSize = true;
			this.gridColumnDistanceLabel.Location = new System.Drawing.Point(255, 105);
			this.gridColumnDistanceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.gridColumnDistanceLabel.Name = "gridColumnDistanceLabel";
			this.gridColumnDistanceLabel.Size = new System.Drawing.Size(19, 13);
			this.gridColumnDistanceLabel.TabIndex = 6;
			this.gridColumnDistanceLabel.Text = "50";
			// 
			// rowDistanceTrackBar
			// 
			this.rowDistanceTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rowDistanceTrackBar.LargeChange = 10;
			this.rowDistanceTrackBar.Location = new System.Drawing.Point(8, 202);
			this.rowDistanceTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.rowDistanceTrackBar.Maximum = 100;
			this.rowDistanceTrackBar.Name = "rowDistanceTrackBar";
			this.rowDistanceTrackBar.Size = new System.Drawing.Size(473, 45);
			this.rowDistanceTrackBar.TabIndex = 5;
			this.rowDistanceTrackBar.TickFrequency = 5;
			this.rowDistanceTrackBar.Value = 50;
			this.rowDistanceTrackBar.ValueChanged += new System.EventHandler(this.rowDistanceTrackBar_ValueChanged);
			// 
			// columnDistanceTrackBar
			// 
			this.columnDistanceTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.columnDistanceTrackBar.AutoSize = false;
			this.columnDistanceTrackBar.LargeChange = 10;
			this.columnDistanceTrackBar.Location = new System.Drawing.Point(8, 124);
			this.columnDistanceTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.columnDistanceTrackBar.Maximum = 100;
			this.columnDistanceTrackBar.Name = "columnDistanceTrackBar";
			this.columnDistanceTrackBar.Size = new System.Drawing.Size(473, 55);
			this.columnDistanceTrackBar.TabIndex = 4;
			this.columnDistanceTrackBar.TickFrequency = 5;
			this.columnDistanceTrackBar.Value = 50;
			this.columnDistanceTrackBar.ValueChanged += new System.EventHandler(this.columnDistanceTrackBar_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 182);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(174, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Coarseness in the vertical direction:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 105);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(185, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Coarseness in the horizontal direction:";
			// 
			// centerButton
			// 
			this.centerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.centerButton.Location = new System.Drawing.Point(133, 438);
			this.centerButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.centerButton.Name = "centerButton";
			this.centerButton.Size = new System.Drawing.Size(75, 28);
			this.centerButton.TabIndex = 4;
			this.centerButton.Text = "Fit page";
			this.toolTip.SetToolTip(this.centerButton, "Fits the selected shapes (or all if nothing is selected) onto the page.");
			this.centerButton.UseVisualStyleBackColor = true;
			this.centerButton.Click += new System.EventHandler(this.centerButton_Click);
			// 
			// layoutTimer
			// 
			this.layoutTimer.Tick += new System.EventHandler(this.layoutTimer_Tick);
			// 
			// repulsionPanel
			// 
			this.repulsionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.repulsionPanel.Controls.Add(this.repulsionRangeTrackBar);
			this.repulsionPanel.Controls.Add(this.repulsionRangeLabel);
			this.repulsionPanel.Controls.Add(this.repulsionLabel4);
			this.repulsionPanel.Controls.Add(this.label14);
			this.repulsionPanel.Controls.Add(this.attractionStrengthLabel);
			this.repulsionPanel.Controls.Add(this.repulsionStrengthLabel);
			this.repulsionPanel.Controls.Add(this.repulsionStrengthTrackBar);
			this.repulsionPanel.Controls.Add(this.attractionStrengthTrackBar);
			this.repulsionPanel.Controls.Add(this.repulsionLabel3);
			this.repulsionPanel.Controls.Add(this.repulsionLabel2);
			this.repulsionPanel.Location = new System.Drawing.Point(133, 5);
			this.repulsionPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.repulsionPanel.Name = "repulsionPanel";
			this.repulsionPanel.Size = new System.Drawing.Size(488, 338);
			this.repulsionPanel.TabIndex = 9;
			this.repulsionPanel.Tag = "Clusters";
			// 
			// repulsionRangeTrackBar
			// 
			this.repulsionRangeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.repulsionRangeTrackBar.LargeChange = 50;
			this.repulsionRangeTrackBar.Location = new System.Drawing.Point(11, 249);
			this.repulsionRangeTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.repulsionRangeTrackBar.Maximum = 1000;
			this.repulsionRangeTrackBar.Minimum = 100;
			this.repulsionRangeTrackBar.Name = "repulsionRangeTrackBar";
			this.repulsionRangeTrackBar.Size = new System.Drawing.Size(457, 45);
			this.repulsionRangeTrackBar.TabIndex = 18;
			this.repulsionRangeTrackBar.TickFrequency = 50;
			this.repulsionRangeTrackBar.Value = 600;
			this.repulsionRangeTrackBar.ValueChanged += new System.EventHandler(this.repulsionRangeTrackBar_ValueChanged);
			// 
			// repulsionRangeLabel
			// 
			this.repulsionRangeLabel.AutoSize = true;
			this.repulsionRangeLabel.Location = new System.Drawing.Point(255, 226);
			this.repulsionRangeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.repulsionRangeLabel.Name = "repulsionRangeLabel";
			this.repulsionRangeLabel.Size = new System.Drawing.Size(25, 13);
			this.repulsionRangeLabel.TabIndex = 17;
			this.repulsionRangeLabel.Text = "120";
			// 
			// repulsionLabel4
			// 
			this.repulsionLabel4.AutoSize = true;
			this.repulsionLabel4.Location = new System.Drawing.Point(7, 226);
			this.repulsionLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.repulsionLabel4.Name = "repulsionLabel4";
			this.repulsionLabel4.Size = new System.Drawing.Size(180, 13);
			this.repulsionLabel4.TabIndex = 15;
			this.repulsionLabel4.Text = "Range of repulsion between shapes:";
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label14.Location = new System.Drawing.Point(4, 4);
			this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(477, 38);
			this.label14.TabIndex = 14;
			this.label14.Text = "Moves connected shapes nearer together and thrusts unconnected shapes further apa" +
    "rt.";
			// 
			// attractionStrengthLabel
			// 
			this.attractionStrengthLabel.AutoSize = true;
			this.attractionStrengthLabel.Location = new System.Drawing.Point(339, 69);
			this.attractionStrengthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.attractionStrengthLabel.Name = "attractionStrengthLabel";
			this.attractionStrengthLabel.Size = new System.Drawing.Size(25, 13);
			this.attractionStrengthLabel.TabIndex = 13;
			this.attractionStrengthLabel.Text = "120";
			// 
			// repulsionStrengthLabel
			// 
			this.repulsionStrengthLabel.AutoSize = true;
			this.repulsionStrengthLabel.Location = new System.Drawing.Point(264, 148);
			this.repulsionStrengthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.repulsionStrengthLabel.Name = "repulsionStrengthLabel";
			this.repulsionStrengthLabel.Size = new System.Drawing.Size(25, 13);
			this.repulsionStrengthLabel.TabIndex = 12;
			this.repulsionStrengthLabel.Text = "120";
			// 
			// repulsionStrengthTrackBar
			// 
			this.repulsionStrengthTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.repulsionStrengthTrackBar.Location = new System.Drawing.Point(11, 167);
			this.repulsionStrengthTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.repulsionStrengthTrackBar.Name = "repulsionStrengthTrackBar";
			this.repulsionStrengthTrackBar.Size = new System.Drawing.Size(457, 45);
			this.repulsionStrengthTrackBar.TabIndex = 11;
			this.repulsionStrengthTrackBar.Value = 1;
			this.repulsionStrengthTrackBar.ValueChanged += new System.EventHandler(this.repulsionStrengthTrackBar_ValueChanged);
			// 
			// attractionStrengthTrackBar
			// 
			this.attractionStrengthTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.attractionStrengthTrackBar.AutoSize = false;
			this.attractionStrengthTrackBar.Location = new System.Drawing.Point(8, 89);
			this.attractionStrengthTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.attractionStrengthTrackBar.Maximum = 20;
			this.attractionStrengthTrackBar.Name = "attractionStrengthTrackBar";
			this.attractionStrengthTrackBar.Size = new System.Drawing.Size(457, 55);
			this.attractionStrengthTrackBar.TabIndex = 10;
			this.attractionStrengthTrackBar.Value = 8;
			this.attractionStrengthTrackBar.ValueChanged += new System.EventHandler(this.attractionStrengthTrackBar_ValueChanged);
			// 
			// repulsionLabel3
			// 
			this.repulsionLabel3.AutoSize = true;
			this.repulsionLabel3.Location = new System.Drawing.Point(5, 148);
			this.repulsionLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.repulsionLabel3.Name = "repulsionLabel3";
			this.repulsionLabel3.Size = new System.Drawing.Size(188, 13);
			this.repulsionLabel3.TabIndex = 9;
			this.repulsionLabel3.Text = "Strength of repulsion between shapes:";
			// 
			// repulsionLabel2
			// 
			this.repulsionLabel2.AutoSize = true;
			this.repulsionLabel2.Location = new System.Drawing.Point(5, 69);
			this.repulsionLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.repulsionLabel2.Name = "repulsionLabel2";
			this.repulsionLabel2.Size = new System.Drawing.Size(244, 13);
			this.repulsionLabel2.TabIndex = 8;
			this.repulsionLabel2.Text = "Strength of attraction between connected shapes:";
			// 
			// flowPanel
			// 
			this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowPanel.Controls.Add(this.flowRowDistanceLabel);
			this.flowPanel.Controls.Add(this.flowLayerDistanceLabel);
			this.flowPanel.Controls.Add(this.label2);
			this.flowPanel.Controls.Add(this.label1);
			this.flowPanel.Controls.Add(this.flowRowDistanceTrackBar);
			this.flowPanel.Controls.Add(this.flowLayerDistanceTrackBar);
			this.flowPanel.Controls.Add(this.flowDirectionGroupBox);
			this.flowPanel.Controls.Add(this.flowDescriptionLabel);
			this.flowPanel.Location = new System.Drawing.Point(133, 5);
			this.flowPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.flowPanel.Name = "flowPanel";
			this.flowPanel.Size = new System.Drawing.Size(488, 338);
			this.flowPanel.TabIndex = 11;
			this.flowPanel.Tag = "Flow";
			// 
			// flowRowDistanceLabel
			// 
			this.flowRowDistanceLabel.AutoSize = true;
			this.flowRowDistanceLabel.Location = new System.Drawing.Point(240, 266);
			this.flowRowDistanceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.flowRowDistanceLabel.Name = "flowRowDistanceLabel";
			this.flowRowDistanceLabel.Size = new System.Drawing.Size(25, 13);
			this.flowRowDistanceLabel.TabIndex = 12;
			this.flowRowDistanceLabel.Text = "100";
			// 
			// flowLayerDistanceLabel
			// 
			this.flowLayerDistanceLabel.AutoSize = true;
			this.flowLayerDistanceLabel.Location = new System.Drawing.Point(183, 187);
			this.flowLayerDistanceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.flowLayerDistanceLabel.Name = "flowLayerDistanceLabel";
			this.flowLayerDistanceLabel.Size = new System.Drawing.Size(25, 13);
			this.flowLayerDistanceLabel.TabIndex = 11;
			this.flowLayerDistanceLabel.Text = "100";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 187);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(126, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Distance between layers:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 266);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(169, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Distance between shapes in layer:";
			// 
			// flowRowDistanceTrackBar
			// 
			this.flowRowDistanceTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowRowDistanceTrackBar.AutoSize = false;
			this.flowRowDistanceTrackBar.LargeChange = 10;
			this.flowRowDistanceTrackBar.Location = new System.Drawing.Point(11, 287);
			this.flowRowDistanceTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.flowRowDistanceTrackBar.Maximum = 1000;
			this.flowRowDistanceTrackBar.Name = "flowRowDistanceTrackBar";
			this.flowRowDistanceTrackBar.Size = new System.Drawing.Size(457, 42);
			this.flowRowDistanceTrackBar.TabIndex = 8;
			this.flowRowDistanceTrackBar.TickFrequency = 50;
			this.flowRowDistanceTrackBar.Value = 100;
			this.flowRowDistanceTrackBar.ValueChanged += new System.EventHandler(this.flowRowDistanceTrackBar_ValueChanged);
			// 
			// flowLayerDistanceTrackBar
			// 
			this.flowLayerDistanceTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayerDistanceTrackBar.AutoSize = false;
			this.flowLayerDistanceTrackBar.LargeChange = 10;
			this.flowLayerDistanceTrackBar.Location = new System.Drawing.Point(11, 208);
			this.flowLayerDistanceTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.flowLayerDistanceTrackBar.Maximum = 1000;
			this.flowLayerDistanceTrackBar.Name = "flowLayerDistanceTrackBar";
			this.flowLayerDistanceTrackBar.Size = new System.Drawing.Size(467, 41);
			this.flowLayerDistanceTrackBar.TabIndex = 7;
			this.flowLayerDistanceTrackBar.TickFrequency = 50;
			this.flowLayerDistanceTrackBar.Value = 100;
			this.flowLayerDistanceTrackBar.ValueChanged += new System.EventHandler(this.flowLayerDistanceTrackBar_ValueChanged);
			// 
			// flowDirectionGroupBox
			// 
			this.flowDirectionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowDirectionGroupBox.Controls.Add(this.leftToRightRadioButton);
			this.flowDirectionGroupBox.Controls.Add(this.topDownRadioButton);
			this.flowDirectionGroupBox.Controls.Add(this.rightToLeftRadioButton);
			this.flowDirectionGroupBox.Controls.Add(this.bottomUpRadioButton);
			this.flowDirectionGroupBox.Location = new System.Drawing.Point(11, 110);
			this.flowDirectionGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.flowDirectionGroupBox.Name = "flowDirectionGroupBox";
			this.flowDirectionGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.flowDirectionGroupBox.Size = new System.Drawing.Size(467, 60);
			this.flowDirectionGroupBox.TabIndex = 6;
			this.flowDirectionGroupBox.TabStop = false;
			this.flowDirectionGroupBox.Text = "Flow Direction";
			// 
			// leftToRightRadioButton
			// 
			this.leftToRightRadioButton.AutoSize = true;
			this.leftToRightRadioButton.Location = new System.Drawing.Point(125, 23);
			this.leftToRightRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.leftToRightRadioButton.Name = "leftToRightRadioButton";
			this.leftToRightRadioButton.Size = new System.Drawing.Size(99, 21);
			this.leftToRightRadioButton.TabIndex = 6;
			this.leftToRightRadioButton.Text = "left to right";
			this.leftToRightRadioButton.UseVisualStyleBackColor = true;
			// 
			// topDownRadioButton
			// 
			this.topDownRadioButton.AutoSize = true;
			this.topDownRadioButton.Checked = true;
			this.topDownRadioButton.Location = new System.Drawing.Point(25, 23);
			this.topDownRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.topDownRadioButton.Name = "topDownRadioButton";
			this.topDownRadioButton.Size = new System.Drawing.Size(92, 21);
			this.topDownRadioButton.TabIndex = 7;
			this.topDownRadioButton.TabStop = true;
			this.topDownRadioButton.Text = "top-down";
			this.topDownRadioButton.UseVisualStyleBackColor = true;
			// 
			// rightToLeftRadioButton
			// 
			this.rightToLeftRadioButton.AutoSize = true;
			this.rightToLeftRadioButton.Location = new System.Drawing.Point(336, 23);
			this.rightToLeftRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.rightToLeftRadioButton.Name = "rightToLeftRadioButton";
			this.rightToLeftRadioButton.Size = new System.Drawing.Size(99, 21);
			this.rightToLeftRadioButton.TabIndex = 8;
			this.rightToLeftRadioButton.Text = "right to left";
			this.rightToLeftRadioButton.UseVisualStyleBackColor = true;
			// 
			// bottomUpRadioButton
			// 
			this.bottomUpRadioButton.AutoSize = true;
			this.bottomUpRadioButton.Location = new System.Drawing.Point(232, 23);
			this.bottomUpRadioButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.bottomUpRadioButton.Name = "bottomUpRadioButton";
			this.bottomUpRadioButton.Size = new System.Drawing.Size(96, 21);
			this.bottomUpRadioButton.TabIndex = 5;
			this.bottomUpRadioButton.Text = "bottom-up";
			this.bottomUpRadioButton.UseVisualStyleBackColor = true;
			// 
			// flowDescriptionLabel
			// 
			this.flowDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowDescriptionLabel.Location = new System.Drawing.Point(7, 0);
			this.flowDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.flowDescriptionLabel.Name = "flowDescriptionLabel";
			this.flowDescriptionLabel.Size = new System.Drawing.Size(475, 97);
			this.flowDescriptionLabel.TabIndex = 0;
			this.flowDescriptionLabel.Text = resources.GetString("flowDescriptionLabel.Text");
			// 
			// previewButton
			// 
			this.previewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.previewButton.Location = new System.Drawing.Point(520, 379);
			this.previewButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.previewButton.Name = "previewButton";
			this.previewButton.Size = new System.Drawing.Size(100, 28);
			this.previewButton.TabIndex = 12;
			this.previewButton.Text = "Execute";
			this.previewButton.UseVisualStyleBackColor = true;
			this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
			// 
			// undoButton
			// 
			this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.undoButton.Location = new System.Drawing.Point(292, 438);
			this.undoButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.undoButton.Name = "undoButton";
			this.undoButton.Size = new System.Drawing.Size(75, 28);
			this.undoButton.TabIndex = 13;
			this.undoButton.Text = "Undo";
			this.undoButton.UseVisualStyleBackColor = true;
			this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
			// 
			// redoButton
			// 
			this.redoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.redoButton.Location = new System.Drawing.Point(375, 438);
			this.redoButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.redoButton.Name = "redoButton";
			this.redoButton.Size = new System.Drawing.Size(75, 28);
			this.redoButton.TabIndex = 14;
			this.redoButton.Text = "Redo";
			this.redoButton.UseVisualStyleBackColor = true;
			this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
			// 
			// algorithmListBox
			// 
			this.algorithmListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.algorithmListBox.BackColor = System.Drawing.SystemColors.Control;
			this.algorithmListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.algorithmListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.algorithmListBox.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(220)))));
			this.algorithmListBox.FocusedItemColor = System.Drawing.Color.Beige;
			this.algorithmListBox.FormattingEnabled = true;
			this.algorithmListBox.HighlightedItemColor = System.Drawing.SystemColors.ControlLightLight;
			this.algorithmListBox.InactiveItemBackgroundColor = System.Drawing.SystemColors.Control;
			this.algorithmListBox.InactiveItemBorderColor = System.Drawing.SystemColors.Window;
			this.algorithmListBox.InactiveItemTextColor = System.Drawing.SystemColors.ControlDarkDark;
			this.algorithmListBox.IntegralHeight = false;
			this.algorithmListBox.Items.AddRange(new object[] {
            "Clusters",
            "Flow",
            "Expansion",
            "Alignment"});
			this.algorithmListBox.Location = new System.Drawing.Point(0, 0);
			this.algorithmListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.algorithmListBox.Name = "algorithmListBox";
			this.algorithmListBox.SelectedItemColor = System.Drawing.SystemColors.Window;
			this.algorithmListBox.SelectedItemTextColor = System.Drawing.SystemColors.ControlText;
			this.algorithmListBox.Size = new System.Drawing.Size(124, 486);
			this.algorithmListBox.TabIndex = 0;
			this.algorithmListBox.SelectedIndexChanged += new System.EventHandler(this.algorithmListBox_SelectedIndexChanged);
			// 
			// LayoutDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(632, 482);
			this.Controls.Add(this.flowPanel);
			this.Controls.Add(this.repulsionPanel);
			this.Controls.Add(this.alignmentPanel);
			this.Controls.Add(expansionPanel);
			this.Controls.Add(this.redoButton);
			this.Controls.Add(this.undoButton);
			this.Controls.Add(this.previewButton);
			this.Controls.Add(this.centerButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.previewGroupBox);
			this.Controls.Add(this.algorithmListBox);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MinimumSize = new System.Drawing.Size(648, 520);
			this.Name = "LayoutDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Layout Control";
			this.Load += new System.EventHandler(this.LayoutControlForm_Load);
			this.Shown += new System.EventHandler(this.LayoutControlForm_Shown);
			expansionPanel.ResumeLayout(false);
			expansionPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.verticalCompressionTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horizontalCompressionTrackBar)).EndInit();
			this.previewGroupBox.ResumeLayout(false);
			this.previewGroupBox.PerformLayout();
			this.alignmentPanel.ResumeLayout(false);
			this.alignmentPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.rowDistanceTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.columnDistanceTrackBar)).EndInit();
			this.repulsionPanel.ResumeLayout(false);
			this.repulsionPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.repulsionRangeTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repulsionStrengthTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.attractionStrengthTrackBar)).EndInit();
			this.flowPanel.ResumeLayout(false);
			this.flowPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.flowRowDistanceTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.flowLayerDistanceTrackBar)).EndInit();
			this.flowDirectionGroupBox.ResumeLayout(false);
			this.flowDirectionGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.VerticalTabControl algorithmListBox;
		private System.Windows.Forms.RadioButton fastRadioButton;
		private System.Windows.Forms.RadioButton animatedRadioButton;
		private System.Windows.Forms.GroupBox previewGroupBox;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Panel alignmentPanel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button centerButton;
		private System.Windows.Forms.Timer layoutTimer;
		private System.Windows.Forms.TrackBar rowDistanceTrackBar;
		private System.Windows.Forms.TrackBar columnDistanceTrackBar;
		private System.Windows.Forms.Label gridRowDistanceLabel;
		private System.Windows.Forms.Label gridColumnDistanceLabel;
		private System.Windows.Forms.Panel repulsionPanel;
		private System.Windows.Forms.Label verticalCompressionLabel;
		private System.Windows.Forms.Label horizontalCompressionLabel;
		private System.Windows.Forms.TrackBar verticalCompressionTrackBar;
		private System.Windows.Forms.TrackBar horizontalCompressionTrackBar;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label expandDescriptionLabel;
		private System.Windows.Forms.Label gridDescriptionLabel;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label attractionStrengthLabel;
		private System.Windows.Forms.Label repulsionStrengthLabel;
		private System.Windows.Forms.TrackBar repulsionStrengthTrackBar;
		private System.Windows.Forms.TrackBar attractionStrengthTrackBar;
		private System.Windows.Forms.TrackBar repulsionRangeTrackBar;
		private System.Windows.Forms.Label repulsionLabel3;
		private System.Windows.Forms.Label repulsionLabel2;
		private System.Windows.Forms.Label repulsionRangeLabel;
		private System.Windows.Forms.Label repulsionLabel4;
		private System.Windows.Forms.CheckBox keepAspectRationCheckBox;
		private System.Windows.Forms.Panel flowPanel;
		private System.Windows.Forms.Label flowDescriptionLabel;
		private System.Windows.Forms.RadioButton immediateRadioButton;
		private System.Windows.Forms.GroupBox flowDirectionGroupBox;
		private System.Windows.Forms.RadioButton leftToRightRadioButton;
		private System.Windows.Forms.RadioButton topDownRadioButton;
		private System.Windows.Forms.RadioButton rightToLeftRadioButton;
		private System.Windows.Forms.RadioButton bottomUpRadioButton;
		private System.Windows.Forms.Button previewButton;
		private System.Windows.Forms.RadioButton stepRadioButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar flowRowDistanceTrackBar;
		private System.Windows.Forms.TrackBar flowLayerDistanceTrackBar;
		private System.Windows.Forms.Button undoButton;
		private System.Windows.Forms.Button redoButton;
		private System.Windows.Forms.Label flowRowDistanceLabel;
		private System.Windows.Forms.Label flowLayerDistanceLabel;
	}
}