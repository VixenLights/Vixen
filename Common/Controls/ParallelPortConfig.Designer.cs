using System.Windows.Forms;
namespace CommonElements
{
    partial class ParallelPortConfig
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
            this.parallelPortGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.portComboBox = new System.Windows.Forms.ComboBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.canceButton = new System.Windows.Forms.Button();
            this.parallelPortGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // parallelPortGroupBox
            // 
            this.parallelPortGroupBox.Controls.Add(this.portTextBox);
            this.parallelPortGroupBox.Controls.Add(this.portComboBox);
            this.parallelPortGroupBox.Controls.Add(this.label1);
            this.parallelPortGroupBox.Location = new System.Drawing.Point(12, 12);
            this.parallelPortGroupBox.Name = "parallelPortGroupBox";
            this.parallelPortGroupBox.Size = new System.Drawing.Size(296, 100);
            this.parallelPortGroupBox.TabIndex = 0;
            this.parallelPortGroupBox.TabStop = false;
            this.parallelPortGroupBox.Text = "Parallel Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select the port address";
            // 
            // portComboBox
            // 
            this.portComboBox.FormattingEnabled = true;
            this.portComboBox.Items.AddRange(new object[] {
            "Standard port 1 (0378)",
            "Standard port 2 (0278)",
            "Standard port 3 (03bc)",
            "Other...."});
            this.portComboBox.Location = new System.Drawing.Point(9, 45);
            this.portComboBox.Name = "portComboBox";
            this.portComboBox.Size = new System.Drawing.Size(187, 21);
            this.portComboBox.TabIndex = 1;
            this.portComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // portTextBox
            // 
            this.portTextBox.Enabled = false;
            this.portTextBox.Location = new System.Drawing.Point(210, 46);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(70, 20);
            this.portTextBox.TabIndex = 2;
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(152, 118);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // canceButton
            // 
            this.canceButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.canceButton.Location = new System.Drawing.Point(233, 118);
            this.canceButton.Name = "canceButton";
            this.canceButton.Size = new System.Drawing.Size(75, 23);
            this.canceButton.TabIndex = 2;
            this.canceButton.Text = "Cancel";
            this.canceButton.UseVisualStyleBackColor = true;
            // 
            // ParallelPortConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 156);
            this.Controls.Add(this.canceButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.parallelPortGroupBox);
            this.Name = "ParallelPortConfig";
            this.Text = "Setup";
            this.parallelPortGroupBox.ResumeLayout(false);
            this.parallelPortGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox parallelPortGroupBox;
        private Label label1;
        private TextBox portTextBox;
        private ComboBox portComboBox;
        private Button OkButton;
        private Button canceButton;
    }
}