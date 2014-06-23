namespace VixenModules.App.LipSyncMap
{
    partial class LipSyncColorEditor
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
            this.colorTypeEditorControl1 = new VixenModules.EffectEditor.ColorTypeEditor.ColorTypeEditorControl();
            this.levelTypeEditorControl1 = new VixenModules.EffectEditor.LevelTypeEditor.LevelTypeEditorControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorTypeEditorControl1
            // 
            this.colorTypeEditorControl1.ColorValue = System.Drawing.Color.Empty;
            this.colorTypeEditorControl1.EffectParameterValues = new object[] {
        ((object)(System.Drawing.Color.Empty))};
            this.colorTypeEditorControl1.Location = new System.Drawing.Point(20, 30);
            this.colorTypeEditorControl1.Name = "colorTypeEditorControl1";
            this.colorTypeEditorControl1.Size = new System.Drawing.Size(80, 40);
            this.colorTypeEditorControl1.TabIndex = 0;
            this.colorTypeEditorControl1.TargetEffect = null;
            // 
            // levelTypeEditorControl1
            // 
            this.levelTypeEditorControl1.EffectParameterValues = new object[] {
        ((object)(1D))};
            this.levelTypeEditorControl1.LevelValue = 1D;
            this.levelTypeEditorControl1.Location = new System.Drawing.Point(127, 31);
            this.levelTypeEditorControl1.Name = "levelTypeEditorControl1";
            this.levelTypeEditorControl1.Size = new System.Drawing.Size(90, 39);
            this.levelTypeEditorControl1.TabIndex = 1;
            this.levelTypeEditorControl1.TargetEffect = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.colorTypeEditorControl1);
            this.groupBox1.Controls.Add(this.levelTypeEditorControl1);
            this.groupBox1.Location = new System.Drawing.Point(12, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 87);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(84, 129);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(165, 129);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // LipSyncColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 164);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Name = "LipSyncColorEditor";
            this.Text = "Color Editor";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EffectEditor.ColorTypeEditor.ColorTypeEditorControl colorTypeEditorControl1;
        private EffectEditor.LevelTypeEditor.LevelTypeEditorControl levelTypeEditorControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;

    }
}