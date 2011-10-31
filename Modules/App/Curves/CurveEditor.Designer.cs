
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.zedGraphControl = new ZedGraph.ZedGraphControl();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(468, 442);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(387, 442);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
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
			this.zedGraphControl.Size = new System.Drawing.Size(531, 421);
			this.zedGraphControl.TabIndex = 0;
			this.zedGraphControl.MouseDownEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_MouseDownEvent);
			this.zedGraphControl.MouseUpEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_MouseUpEvent);
			this.zedGraphControl.PreMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_PreMouseMoveEvent);
			this.zedGraphControl.PostMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraphControl_PostMouseMoveEvent);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 454);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(243, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Hold down Alt to remove a point as you click on it.";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 438);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(219, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Hold down Ctrl to add new points on the grid.";
			// 
			// CurveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(555, 473);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.zedGraphControl);
			this.Name = "CurveEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Curve Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZedGraph.ZedGraphControl zedGraphControl;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}