namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    partial class VerticalMeterEffectEditorControl
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
            VixenModules.App.ColorGradients.ColorGradient colorGradient1 = new VixenModules.App.ColorGradients.ColorGradient();
            this.ReverseCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ReverseCheckbox
            // 
            this.ReverseCheckbox.AutoSize = true;
            this.ReverseCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.ReverseCheckbox.Location = new System.Drawing.Point(638, 318);
            this.ReverseCheckbox.Name = "ReverseCheckbox";
            this.ReverseCheckbox.Size = new System.Drawing.Size(164, 28);
            this.ReverseCheckbox.TabIndex = 92;
            this.ReverseCheckbox.Text = "Reverse Effect";
            this.ReverseCheckbox.UseVisualStyleBackColor = true;
            // 
            // VerticalMeterEffectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            colorGradient1.Gammacorrected = false;
            colorGradient1.IsCurrentLibraryGradient = false;
            colorGradient1.LibraryReferenceName = "";
            colorGradient1.Title = null;
            this.ColorGradientValue = colorGradient1;
            this.Controls.Add(this.ReverseCheckbox);
            this.EffectParameterValues = new object[] {
        ((object)(false)),
        ((object)(0)),
        ((object)(0)),
        ((object)(false)),
        ((object)(0)),
        ((object)(0)),
        ((object)(50)),
        ((object)(99)),
        ((object)(colorGradient1)),
        ((object)(VixenModules.Effect.AudioHelp.MeterColorTypes.Linear))};
            this.Name = "VerticalMeterEffectEditorControl";
            this.Size = new System.Drawing.Size(827, 397);
            this.Controls.SetChildIndex(this.ReverseCheckbox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ReverseCheckbox;
    }
}
