
namespace VixenModules.App.Curves
{
	partial class CurveEditor
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
			this.components = new System.ComponentModel.Container();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelInstructions1 = new System.Windows.Forms.Label();
			this.labelInstructions2 = new System.Windows.Forms.Label();
			this.groupBoxLibrary = new System.Windows.Forms.GroupBox();
			this.buttonEditLibraryCurve = new System.Windows.Forms.Button();
			this.buttonUnlinkCurve = new System.Windows.Forms.Button();
			this.labelCurve = new System.Windows.Forms.Label();
			this.buttonSaveCurveToLibrary = new System.Windows.Forms.Button();
			this.buttonLoadCurveFromLibrary = new System.Windows.Forms.Button();
			this.yLabel = new System.Windows.Forms.Label();
			this.xLabel = new System.Windows.Forms.Label();
			this.grpCurve = new System.Windows.Forms.GroupBox();
			this.btnUpdateCoordinates = new System.Windows.Forms.Button();
			this.lblSelectedPoint = new System.Windows.Forms.Label();
			this.txtYValue = new Common.Controls.NumericTextBox();
			this.txtXValue = new Common.Controls.NumericTextBox();
			this.btnInvert = new System.Windows.Forms.Button();
			this.btnReverse = new System.Windows.Forms.Button();
			this.zedGraphControl = new ZedGraph.ZedGraphControl();
			this.groupBoxLibrary.SuspendLayout();
			this.grpCurve.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(317, 622);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 25);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(403, 622);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 25);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelInstructions1
			// 
			this.labelInstructions1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInstructions1.AutoSize = true;
			this.labelInstructions1.Location = new System.Drawing.Point(21, 615);
			this.labelInstructions1.Name = "labelInstructions1";
			this.labelInstructions1.Size = new System.Drawing.Size(219, 13);
			this.labelInstructions1.TabIndex = 7;
			this.labelInstructions1.Text = "Hold down Ctrl to add new points on the grid.";
			// 
			// labelInstructions2
			// 
			this.labelInstructions2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInstructions2.AutoSize = true;
			this.labelInstructions2.Location = new System.Drawing.Point(21, 634);
			this.labelInstructions2.Name = "labelInstructions2";
			this.labelInstructions2.Size = new System.Drawing.Size(161, 13);
			this.labelInstructions2.TabIndex = 6;
			this.labelInstructions2.Text = "Hold down Alt to remove a point.";
			// 
			// groupBoxLibrary
			// 
			this.groupBoxLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxLibrary.Controls.Add(this.buttonEditLibraryCurve);
			this.groupBoxLibrary.Controls.Add(this.buttonUnlinkCurve);
			this.groupBoxLibrary.Controls.Add(this.labelCurve);
			this.groupBoxLibrary.Controls.Add(this.buttonSaveCurveToLibrary);
			this.groupBoxLibrary.Controls.Add(this.buttonLoadCurveFromLibrary);
			this.groupBoxLibrary.Location = new System.Drawing.Point(12, 522);
			this.groupBoxLibrary.Name = "groupBoxLibrary";
			this.groupBoxLibrary.Size = new System.Drawing.Size(471, 85);
			this.groupBoxLibrary.TabIndex = 8;
			this.groupBoxLibrary.TabStop = false;
			this.groupBoxLibrary.Text = "Library";
			// 
			// buttonEditLibraryCurve
			// 
			this.buttonEditLibraryCurve.Location = new System.Drawing.Point(254, 50);
			this.buttonEditLibraryCurve.Name = "buttonEditLibraryCurve";
			this.buttonEditLibraryCurve.Size = new System.Drawing.Size(120, 25);
			this.buttonEditLibraryCurve.TabIndex = 4;
			this.buttonEditLibraryCurve.Text = "Edit Library Curve";
			this.buttonEditLibraryCurve.UseVisualStyleBackColor = true;
			this.buttonEditLibraryCurve.Click += new System.EventHandler(this.buttonEditLibraryCurve_Click);
			// 
			// buttonUnlinkCurve
			// 
			this.buttonUnlinkCurve.Location = new System.Drawing.Point(128, 50);
			this.buttonUnlinkCurve.Name = "buttonUnlinkCurve";
			this.buttonUnlinkCurve.Size = new System.Drawing.Size(120, 25);
			this.buttonUnlinkCurve.TabIndex = 3;
			this.buttonUnlinkCurve.Text = "Unlink Curve";
			this.buttonUnlinkCurve.UseVisualStyleBackColor = true;
			this.buttonUnlinkCurve.Click += new System.EventHandler(this.buttonUnlinkCurve_Click);
			// 
			// labelCurve
			// 
			this.labelCurve.AutoSize = true;
			this.labelCurve.Location = new System.Drawing.Point(126, 25);
			this.labelCurve.Name = "labelCurve";
			this.labelCurve.Size = new System.Drawing.Size(254, 13);
			this.labelCurve.TabIndex = 2;
			this.labelCurve.Text = "This curve is linked to the library curve: \'ASDFASDF\'";
			// 
			// buttonSaveCurveToLibrary
			// 
			this.buttonSaveCurveToLibrary.Location = new System.Drawing.Point(12, 50);
			this.buttonSaveCurveToLibrary.Name = "buttonSaveCurveToLibrary";
			this.buttonSaveCurveToLibrary.Size = new System.Drawing.Size(100, 25);
			this.buttonSaveCurveToLibrary.TabIndex = 1;
			this.buttonSaveCurveToLibrary.Text = "Save Curve";
			this.buttonSaveCurveToLibrary.UseVisualStyleBackColor = true;
			this.buttonSaveCurveToLibrary.Click += new System.EventHandler(this.buttonSaveCurveToLibrary_Click);
			// 
			// buttonLoadCurveFromLibrary
			// 
			this.buttonLoadCurveFromLibrary.Location = new System.Drawing.Point(12, 19);
			this.buttonLoadCurveFromLibrary.Name = "buttonLoadCurveFromLibrary";
			this.buttonLoadCurveFromLibrary.Size = new System.Drawing.Size(100, 25);
			this.buttonLoadCurveFromLibrary.TabIndex = 0;
			this.buttonLoadCurveFromLibrary.Text = "Load Curve";
			this.buttonLoadCurveFromLibrary.UseVisualStyleBackColor = true;
			this.buttonLoadCurveFromLibrary.Click += new System.EventHandler(this.buttonLoadCurveFromLibrary_Click);
			// 
			// yLabel
			// 
			this.yLabel.AutoSize = true;
			this.yLabel.Location = new System.Drawing.Point(324, 49);
			this.yLabel.Name = "yLabel";
			this.yLabel.Size = new System.Drawing.Size(17, 13);
			this.yLabel.TabIndex = 6;
			this.yLabel.Text = "Y:";
			// 
			// xLabel
			// 
			this.xLabel.AutoSize = true;
			this.xLabel.Location = new System.Drawing.Point(323, 25);
			this.xLabel.Name = "xLabel";
			this.xLabel.Size = new System.Drawing.Size(17, 13);
			this.xLabel.TabIndex = 5;
			this.xLabel.Text = "X:";
			// 
			// grpCurve
			// 
			this.grpCurve.Controls.Add(this.btnUpdateCoordinates);
			this.grpCurve.Controls.Add(this.lblSelectedPoint);
			this.grpCurve.Controls.Add(this.txtYValue);
			this.grpCurve.Controls.Add(this.txtXValue);
			this.grpCurve.Controls.Add(this.btnInvert);
			this.grpCurve.Controls.Add(this.btnReverse);
			this.grpCurve.Controls.Add(this.xLabel);
			this.grpCurve.Controls.Add(this.yLabel);
			this.grpCurve.Location = new System.Drawing.Point(11, 436);
			this.grpCurve.Name = "grpCurve";
			this.grpCurve.Size = new System.Drawing.Size(472, 80);
			this.grpCurve.TabIndex = 9;
			this.grpCurve.TabStop = false;
			this.grpCurve.Text = "Curve";
			// 
			// btnUpdateCoordinates
			// 
			this.btnUpdateCoordinates.Enabled = false;
			this.btnUpdateCoordinates.Location = new System.Drawing.Point(203, 43);
			this.btnUpdateCoordinates.Name = "btnUpdateCoordinates";
			this.btnUpdateCoordinates.Size = new System.Drawing.Size(100, 25);
			this.btnUpdateCoordinates.TabIndex = 15;
			this.btnUpdateCoordinates.Text = "Update";
			this.btnUpdateCoordinates.UseVisualStyleBackColor = true;
			this.btnUpdateCoordinates.Click += new System.EventHandler(this.btnUpdateCoordinates_Click);
			// 
			// lblSelectedPoint
			// 
			this.lblSelectedPoint.AutoSize = true;
			this.lblSelectedPoint.Location = new System.Drawing.Point(216, 22);
			this.lblSelectedPoint.Name = "lblSelectedPoint";
			this.lblSelectedPoint.Size = new System.Drawing.Size(76, 13);
			this.lblSelectedPoint.TabIndex = 14;
			this.lblSelectedPoint.Text = "Selected Point";
			// 
			// txtYValue
			// 
			this.txtYValue.AllowSpace = false;
			this.txtYValue.Enabled = false;
			this.txtYValue.Location = new System.Drawing.Point(347, 49);
			this.txtYValue.Name = "txtYValue";
			this.txtYValue.Size = new System.Drawing.Size(119, 20);
			this.txtYValue.TabIndex = 13;
			// 
			// txtXValue
			// 
			this.txtXValue.AllowSpace = false;
			this.txtXValue.Enabled = false;
			this.txtXValue.Location = new System.Drawing.Point(347, 19);
			this.txtXValue.Name = "txtXValue";
			this.txtXValue.Size = new System.Drawing.Size(119, 20);
			this.txtXValue.TabIndex = 12;
			// 
			// btnInvert
			// 
			this.btnInvert.Location = new System.Drawing.Point(13, 49);
			this.btnInvert.Name = "btnInvert";
			this.btnInvert.Size = new System.Drawing.Size(100, 25);
			this.btnInvert.TabIndex = 9;
			this.btnInvert.Text = "Invert Curve";
			this.btnInvert.UseVisualStyleBackColor = true;
			this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
			// 
			// btnReverse
			// 
			this.btnReverse.Location = new System.Drawing.Point(13, 19);
			this.btnReverse.Name = "btnReverse";
			this.btnReverse.Size = new System.Drawing.Size(100, 25);
			this.btnReverse.TabIndex = 0;
			this.btnReverse.Text = "Reverse Curve";
			this.btnReverse.UseVisualStyleBackColor = true;
			this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
			// 
			// zedGraphControl
			// 
			this.zedGraphControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.zedGraphControl.IsAntiAlias = true;
			this.zedGraphControl.IsEnableHEdit = true;
			this.zedGraphControl.IsEnableHPan = false;
			this.zedGraphControl.IsEnableHZoom = false;
			this.zedGraphControl.IsEnableVEdit = true;
			this.zedGraphControl.IsEnableVPan = false;
			this.zedGraphControl.IsEnableVZoom = false;
			this.zedGraphControl.IsEnableWheelZoom = false;
			this.zedGraphControl.IsShowContextMenu = false;
			this.zedGraphControl.LinkModifierKeys = System.Windows.Forms.Keys.None;
			this.zedGraphControl.Location = new System.Drawing.Point(11, 12);
			this.zedGraphControl.Name = "zedGraphControl";
			this.zedGraphControl.PanModifierKeys = System.Windows.Forms.Keys.None;
			this.zedGraphControl.ScrollGrace = 0D;
			this.zedGraphControl.ScrollMaxX = 0D;
			this.zedGraphControl.ScrollMaxY = 0D;
			this.zedGraphControl.ScrollMaxY2 = 0D;
			this.zedGraphControl.ScrollMinX = 0D;
			this.zedGraphControl.ScrollMinY = 0D;
			this.zedGraphControl.ScrollMinY2 = 0D;
			this.zedGraphControl.Size = new System.Drawing.Size(471, 418);
			this.zedGraphControl.TabIndex = 0;
			this.zedGraphControl.MouseDownEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_MouseDownEvent);
			this.zedGraphControl.MouseUpEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_MouseUpEvent);
			this.zedGraphControl.PreMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_PreMouseMoveEvent);
			this.zedGraphControl.PostMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_PostMouseMoveEvent);
			// 
			// CurveEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(495, 656);
			this.Controls.Add(this.grpCurve);
			this.Controls.Add(this.groupBoxLibrary);
			this.Controls.Add(this.labelInstructions1);
			this.Controls.Add(this.labelInstructions2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.zedGraphControl);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(440, 440);
			this.Name = "CurveEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Curve Editor";
			this.groupBoxLibrary.ResumeLayout(false);
			this.groupBoxLibrary.PerformLayout();
			this.grpCurve.ResumeLayout(false);
			this.grpCurve.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZedGraph.ZedGraphControl zedGraphControl;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelInstructions1;
		private System.Windows.Forms.Label labelInstructions2;
		private System.Windows.Forms.GroupBox groupBoxLibrary;
		private System.Windows.Forms.Button buttonUnlinkCurve;
		private System.Windows.Forms.Label labelCurve;
		private System.Windows.Forms.Button buttonSaveCurveToLibrary;
		private System.Windows.Forms.Button buttonLoadCurveFromLibrary;
		private System.Windows.Forms.Button buttonEditLibraryCurve;
		private System.Windows.Forms.Label yLabel;
		private System.Windows.Forms.Label xLabel;
		private System.Windows.Forms.GroupBox grpCurve;
		private System.Windows.Forms.Button btnReverse;
		private System.Windows.Forms.Button btnInvert;
		private Common.Controls.NumericTextBox txtYValue;
		private Common.Controls.NumericTextBox txtXValue;
		private System.Windows.Forms.Label lblSelectedPoint;
		private System.Windows.Forms.Button btnUpdateCoordinates;
	}
}