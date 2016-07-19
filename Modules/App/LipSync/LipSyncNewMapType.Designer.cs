namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncNewMapType
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
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.stringMappingRadio = new System.Windows.Forms.RadioButton();
			this.matrixMappingRadio = new System.Windows.Forms.RadioButton();
			this.groupBoxMapType = new System.Windows.Forms.GroupBox();
			this.stringsUpDown = new System.Windows.Forms.NumericUpDown();
			this.pixelsUpDown = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBoxMapType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.stringsUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pixelsUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOk.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonOk.Location = new System.Drawing.Point(202, 141);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(87, 27);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonCancel.Location = new System.Drawing.Point(295, 141);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// stringMappingRadio
			// 
			this.stringMappingRadio.AutoSize = true;
			this.stringMappingRadio.Checked = true;
			this.stringMappingRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.stringMappingRadio.Location = new System.Drawing.Point(29, 22);
			this.stringMappingRadio.Name = "stringMappingRadio";
			this.stringMappingRadio.Size = new System.Drawing.Size(107, 19);
			this.stringMappingRadio.TabIndex = 2;
			this.stringMappingRadio.TabStop = true;
			this.stringMappingRadio.Text = "String Mapping";
			this.stringMappingRadio.UseVisualStyleBackColor = true;
			// 
			// matrixMappingRadio
			// 
			this.matrixMappingRadio.AutoSize = true;
			this.matrixMappingRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.matrixMappingRadio.Location = new System.Drawing.Point(183, 22);
			this.matrixMappingRadio.Name = "matrixMappingRadio";
			this.matrixMappingRadio.Size = new System.Drawing.Size(146, 19);
			this.matrixMappingRadio.TabIndex = 3;
			this.matrixMappingRadio.Text = "Pixel Matrix / Megatree";
			this.matrixMappingRadio.UseVisualStyleBackColor = true;
			this.matrixMappingRadio.CheckedChanged += new System.EventHandler(this.matrixMappingRadio_CheckedChanged);
			// 
			// groupBoxMapType
			// 
			this.groupBoxMapType.Controls.Add(this.stringsUpDown);
			this.groupBoxMapType.Controls.Add(this.pixelsUpDown);
			this.groupBoxMapType.Controls.Add(this.matrixMappingRadio);
			this.groupBoxMapType.Controls.Add(this.stringMappingRadio);
			this.groupBoxMapType.Controls.Add(this.label3);
			this.groupBoxMapType.Controls.Add(this.label2);
			this.groupBoxMapType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.groupBoxMapType.Location = new System.Drawing.Point(14, 14);
			this.groupBoxMapType.Name = "groupBoxMapType";
			this.groupBoxMapType.Size = new System.Drawing.Size(366, 121);
			this.groupBoxMapType.TabIndex = 4;
			this.groupBoxMapType.TabStop = false;
			this.groupBoxMapType.Text = "Map Type";
			this.groupBoxMapType.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// stringsUpDown
			// 
			this.stringsUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.stringsUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.stringsUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.stringsUpDown.Location = new System.Drawing.Point(265, 48);
			this.stringsUpDown.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.stringsUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.stringsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.stringsUpDown.Name = "stringsUpDown";
			this.stringsUpDown.Size = new System.Drawing.Size(59, 23);
			this.stringsUpDown.TabIndex = 29;
			this.stringsUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.stringsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.stringsUpDown.ValueChanged += new System.EventHandler(this.stringsUpDown_ValueChanged);
			// 
			// pixelsUpDown
			// 
			this.pixelsUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.pixelsUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pixelsUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.pixelsUpDown.Location = new System.Drawing.Point(265, 78);
			this.pixelsUpDown.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.pixelsUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.pixelsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.pixelsUpDown.Name = "pixelsUpDown";
			this.pixelsUpDown.Size = new System.Drawing.Size(59, 23);
			this.pixelsUpDown.TabIndex = 28;
			this.pixelsUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.pixelsUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.pixelsUpDown.ValueChanged += new System.EventHandler(this.pixelsUpDown_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label3.Location = new System.Drawing.Point(206, 82);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(36, 15);
			this.label3.TabIndex = 27;
			this.label3.Text = "Pixels";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.label2.Location = new System.Drawing.Point(206, 51);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 15);
			this.label2.TabIndex = 26;
			this.label2.Text = "Strings";
			// 
			// LipSyncNewMapType
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(395, 183);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.groupBoxMapType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LipSyncNewMapType";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "LipSyncNewMapType";
			this.Load += new System.EventHandler(this.LipSyncNewMapType_Load);
			this.groupBoxMapType.ResumeLayout(false);
			this.groupBoxMapType.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.stringsUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pixelsUpDown)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.RadioButton stringMappingRadio;
        public System.Windows.Forms.RadioButton matrixMappingRadio;
        private System.Windows.Forms.GroupBox groupBoxMapType;
        private System.Windows.Forms.NumericUpDown stringsUpDown;
        private System.Windows.Forms.NumericUpDown pixelsUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}