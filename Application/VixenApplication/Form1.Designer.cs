namespace VixenApplication
{
	partial class Form1
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
			this.diagramDisplay = new Dataweb.NShape.WinFormsUI.Display();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// diagramDisplay
			// 
			this.diagramDisplay.AllowDrop = true;
			this.diagramDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.diagramDisplay.BackColorGradient = System.Drawing.SystemColors.Control;
			this.diagramDisplay.DiagramSetController = null;
			this.diagramDisplay.GridColor = System.Drawing.Color.Gainsboro;
			this.diagramDisplay.GridSize = 19;
			this.diagramDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.diagramDisplay.Location = new System.Drawing.Point(12, 12);
			this.diagramDisplay.Name = "diagramDisplay";
			this.diagramDisplay.PropertyController = null;
			this.diagramDisplay.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.diagramDisplay.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.diagramDisplay.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.diagramDisplay.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.diagramDisplay.Size = new System.Drawing.Size(966, 560);
			this.diagramDisplay.SnapToGrid = false;
			this.diagramDisplay.TabIndex = 0;
			this.diagramDisplay.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.diagramDisplay.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
			this.diagramDisplay.ShapeDoubleClick += new System.EventHandler<Dataweb.NShape.Controllers.DiagramPresenterShapeClickEventArgs>(this.displayDiagram_ShapeDoubleClick);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(903, 678);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(990, 713);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.diagramDisplay);
			this.Name = "Form1";
			this.Text = "Output Filter Setup";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display diagramDisplay;
		private System.Windows.Forms.Button buttonOK;

	}
}