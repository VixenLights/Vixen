using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Windows;

namespace TestCommandEditors {
    public class ByteValueEditorControl : CommandEditorControlBase {
        private System.Windows.Forms.NumericUpDown numericUpDownValue;
        private System.Windows.Forms.Label label1;

        public ByteValueEditorControl()
            : base() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value (0-255)";
            // 
            // numericUpDownValue
            // 
            this.numericUpDownValue.Location = new System.Drawing.Point(200, 0);
            this.numericUpDownValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownValue.Name = "numericUpDownValue";
            this.numericUpDownValue.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownValue.TabIndex = 0;
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
