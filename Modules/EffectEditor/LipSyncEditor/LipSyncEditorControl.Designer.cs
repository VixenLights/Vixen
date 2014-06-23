namespace VixenModules.EffectEditor.LipSyncEditor
{
    partial class LipSyncEditorControl
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
            this.components = new System.ComponentModel.Container();
            this.staticPhoneMeCombo = new System.Windows.Forms.ComboBox();
            this.imageListView = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mappingComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // staticPhoneMeCombo
            // 
            this.staticPhoneMeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.staticPhoneMeCombo.FormattingEnabled = true;
            this.staticPhoneMeCombo.Location = new System.Drawing.Point(69, 13);
            this.staticPhoneMeCombo.MaxDropDownItems = 12;
            this.staticPhoneMeCombo.Name = "staticPhoneMeCombo";
            this.staticPhoneMeCombo.Size = new System.Drawing.Size(64, 21);
            this.staticPhoneMeCombo.TabIndex = 6;
            this.staticPhoneMeCombo.SelectedIndexChanged += new System.EventHandler(this.staticPhoneMeCombo_SelectedIndexChanged);
            // 
            // imageListView
            // 
            this.imageListView.LargeImageList = this.imageList1;
            this.imageListView.Location = new System.Drawing.Point(6, 37);
            this.imageListView.Name = "imageListView";
            this.imageListView.Size = new System.Drawing.Size(494, 170);
            this.imageListView.SmallImageList = this.imageList1;
            this.imageListView.TabIndex = 7;
            this.imageListView.UseCompatibleStateImageBehavior = false;
            this.imageListView.SelectedIndexChanged += new System.EventHandler(this.imageListView_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mappingComboBox
            // 
            this.mappingComboBox.FormattingEnabled = true;
            this.mappingComboBox.Location = new System.Drawing.Point(270, 12);
            this.mappingComboBox.Name = "mappingComboBox";
            this.mappingComboBox.Size = new System.Drawing.Size(121, 21);
            this.mappingComboBox.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Mapping:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Phoneme";
            // 
            // LipSyncEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mappingComboBox);
            this.Controls.Add(this.imageListView);
            this.Controls.Add(this.staticPhoneMeCombo);
            this.Name = "LipSyncEditorControl";
            this.Size = new System.Drawing.Size(513, 233);
            this.Load += new System.EventHandler(this.LipSyncEditorControl_Load);
            this.Leave += new System.EventHandler(this.LipSyncEditorControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox staticPhoneMeCombo;
        private System.Windows.Forms.ListView imageListView;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ComboBox mappingComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
