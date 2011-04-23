namespace TestClient {
    partial class ManualPatchDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.comboBoxOutput = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxChannel = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonPatch = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(266, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Controller";
			// 
			// comboBoxController
			// 
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(332, 23);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(121, 21);
			this.comboBoxController.TabIndex = 3;
			this.comboBoxController.SelectedIndexChanged += new System.EventHandler(this.comboBoxController_SelectedIndexChanged);
			// 
			// comboBoxOutput
			// 
			this.comboBoxOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutput.FormattingEnabled = true;
			this.comboBoxOutput.Location = new System.Drawing.Point(332, 50);
			this.comboBoxOutput.Name = "comboBoxOutput";
			this.comboBoxOutput.Size = new System.Drawing.Size(121, 21);
			this.comboBoxOutput.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(266, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Output";
			// 
			// comboBoxChannel
			// 
			this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannel.FormattingEnabled = true;
			this.comboBoxChannel.Location = new System.Drawing.Point(86, 35);
			this.comboBoxChannel.Name = "comboBoxChannel";
			this.comboBoxChannel.Size = new System.Drawing.Size(121, 21);
			this.comboBoxChannel.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(20, 38);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Channel";
			// 
			// buttonPatch
			// 
			this.buttonPatch.Location = new System.Drawing.Point(196, 95);
			this.buttonPatch.Name = "buttonPatch";
			this.buttonPatch.Size = new System.Drawing.Size(75, 23);
			this.buttonPatch.TabIndex = 6;
			this.buttonPatch.Text = "Patch";
			this.buttonPatch.UseVisualStyleBackColor = true;
			this.buttonPatch.Click += new System.EventHandler(this.buttonPatch_Click);
			// 
			// ManualPatchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(476, 134);
			this.Controls.Add(this.buttonPatch);
			this.Controls.Add(this.comboBoxChannel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxOutput);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxController);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ManualPatchDialog";
			this.Text = "ManualPatch";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxController;
        private System.Windows.Forms.ComboBox comboBoxOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxChannel;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonPatch;
    }
}