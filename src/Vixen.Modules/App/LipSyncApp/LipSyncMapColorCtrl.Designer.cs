﻿namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncMapColorCtrl
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
			this.intensityUpDown = new System.Windows.Forms.DomainUpDown();
			this.intensityTrackBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panelColor = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.intensityTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// intensityUpDown
			// 
			this.intensityUpDown.BackColor = System.Drawing.SystemColors.Control;
			this.intensityUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intensityUpDown.ForeColor = System.Drawing.SystemColors.ControlText;
			this.intensityUpDown.Location = new System.Drawing.Point(210, 12);
			this.intensityUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.intensityUpDown.Name = "intensityUpDown";
			this.intensityUpDown.ReadOnly = true;
			this.intensityUpDown.Size = new System.Drawing.Size(69, 26);
			this.intensityUpDown.TabIndex = 11;
			this.intensityUpDown.Text = "0";
			this.intensityUpDown.SelectedItemChanged += new System.EventHandler(this.intensityUpDown_SelectedItemChanged);
			// 
			// intensityTrackBar
			// 
			this.intensityTrackBar.Location = new System.Drawing.Point(82, 5);
			this.intensityTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.intensityTrackBar.Maximum = 100;
			this.intensityTrackBar.Name = "intensityTrackBar";
			this.intensityTrackBar.Size = new System.Drawing.Size(118, 69);
			this.intensityTrackBar.TabIndex = 10;
			this.intensityTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.intensityTrackBar.ValueChanged += new System.EventHandler(this.intensityTrackBar_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 15);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "Intensity";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 49);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 20);
			this.label1.TabIndex = 8;
			this.label1.Text = "Color";
			// 
			// panelColor
			// 
			this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelColor.Location = new System.Drawing.Point(82, 49);
			this.panelColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(94, 36);
			this.panelColor.TabIndex = 7;
			this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
			// 
			// LipSyncMapColorCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Turquoise;
			this.Controls.Add(this.intensityUpDown);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.intensityTrackBar);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "LipSyncMapColorCtrl";
			this.Size = new System.Drawing.Size(284, 103);
			((System.ComponentModel.ISupportInitialize)(this.intensityTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DomainUpDown intensityUpDown;
        private System.Windows.Forms.TrackBar intensityTrackBar;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.Panel panelColor;
    }
}
