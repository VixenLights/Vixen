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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.stringMappingRadio = new System.Windows.Forms.RadioButton();
            this.matrixMappingRadio = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stringsUpDown = new System.Windows.Forms.NumericUpDown();
            this.pixelsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stringsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pixelsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(40, 154);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(59, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(101, 154);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(59, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // stringMappingRadio
            // 
            this.stringMappingRadio.AutoSize = true;
            this.stringMappingRadio.Checked = true;
            this.stringMappingRadio.Location = new System.Drawing.Point(6, 19);
            this.stringMappingRadio.Name = "stringMappingRadio";
            this.stringMappingRadio.Size = new System.Drawing.Size(96, 17);
            this.stringMappingRadio.TabIndex = 2;
            this.stringMappingRadio.TabStop = true;
            this.stringMappingRadio.Text = "String Mapping";
            this.stringMappingRadio.UseVisualStyleBackColor = true;
            // 
            // matrixMappingRadio
            // 
            this.matrixMappingRadio.AutoSize = true;
            this.matrixMappingRadio.Location = new System.Drawing.Point(6, 42);
            this.matrixMappingRadio.Name = "matrixMappingRadio";
            this.matrixMappingRadio.Size = new System.Drawing.Size(134, 17);
            this.matrixMappingRadio.TabIndex = 3;
            this.matrixMappingRadio.Text = "Pixel Matrix / Megatree";
            this.matrixMappingRadio.UseVisualStyleBackColor = true;
            this.matrixMappingRadio.CheckedChanged += new System.EventHandler(this.matrixMappingRadio_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stringsUpDown);
            this.groupBox1.Controls.Add(this.pixelsUpDown);
            this.groupBox1.Controls.Add(this.matrixMappingRadio);
            this.groupBox1.Controls.Add(this.stringMappingRadio);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(143, 131);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Type";
            // 
            // stringsUpDown
            // 
            this.stringsUpDown.Location = new System.Drawing.Point(76, 65);
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
            this.stringsUpDown.Size = new System.Drawing.Size(51, 20);
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
            this.pixelsUpDown.Location = new System.Drawing.Point(76, 91);
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
            this.pixelsUpDown.Size = new System.Drawing.Size(51, 20);
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
            this.label3.Location = new System.Drawing.Point(25, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Pixels";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 67);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Strings";
            // 
            // LipSyncNewMapType
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(172, 189);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "LipSyncNewMapType";
            this.Text = "LipSyncNewMapType";
            this.Load += new System.EventHandler(this.LipSyncNewMapType_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stringsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pixelsUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.RadioButton stringMappingRadio;
        public System.Windows.Forms.RadioButton matrixMappingRadio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown stringsUpDown;
        private System.Windows.Forms.NumericUpDown pixelsUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}