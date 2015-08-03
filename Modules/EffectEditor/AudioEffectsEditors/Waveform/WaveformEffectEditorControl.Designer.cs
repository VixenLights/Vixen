namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    partial class WaveformEffectEditorControl
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
            this.ReverseCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.ScroolSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ScroolSpeedTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ReverseCheckbox
            // 
            this.ReverseCheckbox.AutoSize = true;
            this.ReverseCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.ReverseCheckbox.Location = new System.Drawing.Point(639, 309);
            this.ReverseCheckbox.Name = "ReverseCheckbox";
            this.ReverseCheckbox.Size = new System.Drawing.Size(164, 28);
            this.ReverseCheckbox.TabIndex = 94;
            this.ReverseCheckbox.Text = "Reverse Effect";
            this.ReverseCheckbox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(855, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 24);
            this.label1.TabIndex = 95;
            this.label1.Text = "Scroll";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(853, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 24);
            this.label12.TabIndex = 96;
            this.label12.Text = "Speed";
            // 
            // ScroolSpeedTrackBar
            // 
            this.ScroolSpeedTrackBar.Location = new System.Drawing.Point(847, 71);
            this.ScroolSpeedTrackBar.Maximum = 200;
            this.ScroolSpeedTrackBar.Minimum = 1;
            this.ScroolSpeedTrackBar.Name = "ScroolSpeedTrackBar";
            this.ScroolSpeedTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.ScroolSpeedTrackBar.Size = new System.Drawing.Size(45, 276);
            this.ScroolSpeedTrackBar.TabIndex = 97;
            this.ScroolSpeedTrackBar.TickFrequency = 10;
            this.ScroolSpeedTrackBar.Value = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(885, 323);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 24);
            this.label13.TabIndex = 98;
            this.label13.Text = "Fast";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(885, 70);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 24);
            this.label15.TabIndex = 99;
            this.label15.Text = "Slow";
            // 
            // WaveformEffectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.ScroolSpeedTrackBar);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ReverseCheckbox);
            this.Name = "WaveformEffectEditorControl";
            this.Size = new System.Drawing.Size(957, 379);
            this.Controls.SetChildIndex(this.ReverseCheckbox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label12, 0);
            this.Controls.SetChildIndex(this.ScroolSpeedTrackBar, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.label15, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ScroolSpeedTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ReverseCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar ScroolSpeedTrackBar;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
    }
}
