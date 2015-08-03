namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    public partial class AudioMeterEffectEditorControlBase
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
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.AttackBar = new System.Windows.Forms.TrackBar();
            this.label31 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.RangeBar = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.GainBar = new System.Windows.Forms.TrackBar();
            this.NormalizeCheckbox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DecayBar = new System.Windows.Forms.TrackBar();
            this.VolumeGradientPanel = new System.Windows.Forms.Panel();
            this.RedBar = new System.Windows.Forms.TrackBar();
            this.GreenBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CustomRadio = new System.Windows.Forms.RadioButton();
            this.DiscreteRadio = new System.Windows.Forms.RadioButton();
            this.LinearRadio = new System.Windows.Forms.RadioButton();
            this.label32 = new System.Windows.Forms.Label();
            this.previewButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.VolumeMeter = new VixenModules.EffectEditor.AudioMeterEffectEditor.VerticalMeterControl();
            ((System.ComponentModel.ISupportInitialize)(this.AttackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RangeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DecayBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RedBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(18, 30);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(119, 24);
            this.label21.TabIndex = 86;
            this.label21.Text = "Attack Time";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(105, 354);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(16, 16);
            this.label22.TabIndex = 85;
            this.label22.Text = "0";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(105, 183);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(16, 16);
            this.label23.TabIndex = 84;
            this.label23.Text = "6";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(101, 70);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(24, 16);
            this.label24.TabIndex = 83;
            this.label24.Text = "10";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(105, 127);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(16, 16);
            this.label25.TabIndex = 82;
            this.label25.Text = "8";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(105, 240);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(16, 16);
            this.label26.TabIndex = 81;
            this.label26.Text = "4";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(105, 296);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(16, 16);
            this.label27.TabIndex = 80;
            this.label27.Text = "2";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(26, 63);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(47, 20);
            this.label28.TabIndex = 79;
            this.label28.Text = "Slow";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(26, 351);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(45, 20);
            this.label29.TabIndex = 78;
            this.label29.Text = "Fast";
            // 
            // AttackBar
            // 
            this.AttackBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.AttackBar.LargeChange = 50;
            this.AttackBar.Location = new System.Drawing.Point(77, 65);
            this.AttackBar.Maximum = 300;
            this.AttackBar.Name = "AttackBar";
            this.AttackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.AttackBar.Size = new System.Drawing.Size(45, 310);
            this.AttackBar.SmallChange = 25;
            this.AttackBar.TabIndex = 77;
            this.AttackBar.TickFrequency = 2000;
            this.AttackBar.Scroll += new System.EventHandler(this.AttackBar_Scroll);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(172, 30);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(121, 24);
            this.label31.TabIndex = 75;
            this.label31.Text = "Decay Time";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(427, 33);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(44, 24);
            this.label14.TabIndex = 68;
            this.label14.Text = "Min";
            // 
            // RangeBar
            // 
            this.RangeBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.RangeBar.LargeChange = 10;
            this.RangeBar.Location = new System.Drawing.Point(427, 51);
            this.RangeBar.Maximum = 50;
            this.RangeBar.Name = "RangeBar";
            this.RangeBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.RangeBar.Size = new System.Drawing.Size(45, 285);
            this.RangeBar.TabIndex = 67;
            this.RangeBar.TickFrequency = 10;
            this.RangeBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.RangeBar.Value = 10;
            this.RangeBar.Scroll += new System.EventHandler(this.RangeBar_Scroll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(343, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 24);
            this.label10.TabIndex = 63;
            this.label10.Text = "Max";
            // 
            // GainBar
            // 
            this.GainBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.GainBar.LargeChange = 10;
            this.GainBar.Location = new System.Drawing.Point(358, 51);
            this.GainBar.Maximum = 50;
            this.GainBar.Name = "GainBar";
            this.GainBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.GainBar.Size = new System.Drawing.Size(45, 285);
            this.GainBar.TabIndex = 62;
            this.GainBar.TickFrequency = 10;
            this.GainBar.Value = 10;
            this.GainBar.Scroll += new System.EventHandler(this.GainBar_Scroll);
            // 
            // NormalizeCheckbox
            // 
            this.NormalizeCheckbox.AutoSize = true;
            this.NormalizeCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NormalizeCheckbox.Location = new System.Drawing.Point(638, 284);
            this.NormalizeCheckbox.Name = "NormalizeCheckbox";
            this.NormalizeCheckbox.Size = new System.Drawing.Size(127, 28);
            this.NormalizeCheckbox.TabIndex = 61;
            this.NormalizeCheckbox.Text = "Auto. Gain";
            this.NormalizeCheckbox.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(259, 354);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 16);
            this.label9.TabIndex = 60;
            this.label9.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(259, 183);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 16);
            this.label8.TabIndex = 59;
            this.label8.Text = "6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(255, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 16);
            this.label7.TabIndex = 58;
            this.label7.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(259, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 16);
            this.label6.TabIndex = 57;
            this.label6.Text = "8";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(259, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 56;
            this.label5.Text = "4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(259, 296);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 16);
            this.label4.TabIndex = 55;
            this.label4.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(180, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 54;
            this.label3.Text = "Slow";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(180, 351);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 20);
            this.label2.TabIndex = 53;
            this.label2.Text = "Fast";
            // 
            // DecayBar
            // 
            this.DecayBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.DecayBar.LargeChange = 1000;
            this.DecayBar.Location = new System.Drawing.Point(231, 65);
            this.DecayBar.Maximum = 10000;
            this.DecayBar.Name = "DecayBar";
            this.DecayBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.DecayBar.Size = new System.Drawing.Size(45, 310);
            this.DecayBar.SmallChange = 250;
            this.DecayBar.TabIndex = 52;
            this.DecayBar.TickFrequency = 2000;
            // 
            // VolumeGradientPanel
            // 
            this.VolumeGradientPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VolumeGradientPanel.Location = new System.Drawing.Point(542, 70);
            this.VolumeGradientPanel.Name = "VolumeGradientPanel";
            this.VolumeGradientPanel.Size = new System.Drawing.Size(36, 300);
            this.VolumeGradientPanel.TabIndex = 81;
            this.VolumeGradientPanel.Click += new System.EventHandler(this.VolumeGradientPanel_Click);
            // 
            // RedBar
            // 
            this.RedBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.RedBar.LargeChange = 10;
            this.RedBar.Location = new System.Drawing.Point(516, 60);
            this.RedBar.Maximum = 99;
            this.RedBar.Minimum = 1;
            this.RedBar.Name = "RedBar";
            this.RedBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.RedBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RedBar.Size = new System.Drawing.Size(45, 320);
            this.RedBar.TabIndex = 80;
            this.RedBar.TickFrequency = 100;
            this.RedBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.RedBar.Value = 99;
            this.RedBar.Scroll += new System.EventHandler(this.RedBar_Scroll);
            // 
            // GreenBar
            // 
            this.GreenBar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.GreenBar.LargeChange = 10;
            this.GreenBar.Location = new System.Drawing.Point(581, 59);
            this.GreenBar.Maximum = 99;
            this.GreenBar.Minimum = 1;
            this.GreenBar.Name = "GreenBar";
            this.GreenBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.GreenBar.Size = new System.Drawing.Size(45, 322);
            this.GreenBar.TabIndex = 79;
            this.GreenBar.TickFrequency = 200;
            this.GreenBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.GreenBar.Value = 50;
            this.GreenBar.Scroll += new System.EventHandler(this.GreenBar_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CustomRadio);
            this.groupBox1.Controls.Add(this.DiscreteRadio);
            this.groupBox1.Controls.Add(this.LinearRadio);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(641, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 139);
            this.groupBox1.TabIndex = 78;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Coloring";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter_1);
            // 
            // CustomRadio
            // 
            this.CustomRadio.AutoSize = true;
            this.CustomRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.CustomRadio.Location = new System.Drawing.Point(19, 96);
            this.CustomRadio.Name = "CustomRadio";
            this.CustomRadio.Size = new System.Drawing.Size(88, 24);
            this.CustomRadio.TabIndex = 83;
            this.CustomRadio.TabStop = true;
            this.CustomRadio.Text = "Custom";
            this.CustomRadio.UseVisualStyleBackColor = true;
            this.CustomRadio.Click += new System.EventHandler(this.CustomRadio_Click);
            // 
            // DiscreteRadio
            // 
            this.DiscreteRadio.AutoSize = true;
            this.DiscreteRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.DiscreteRadio.Location = new System.Drawing.Point(19, 66);
            this.DiscreteRadio.Name = "DiscreteRadio";
            this.DiscreteRadio.Size = new System.Drawing.Size(94, 24);
            this.DiscreteRadio.TabIndex = 82;
            this.DiscreteRadio.TabStop = true;
            this.DiscreteRadio.Text = "Discrete";
            this.DiscreteRadio.UseVisualStyleBackColor = true;
            this.DiscreteRadio.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DiscreteRadio_MouseClick);
            // 
            // LinearRadio
            // 
            this.LinearRadio.AutoSize = true;
            this.LinearRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.LinearRadio.Location = new System.Drawing.Point(19, 36);
            this.LinearRadio.Name = "LinearRadio";
            this.LinearRadio.Size = new System.Drawing.Size(77, 24);
            this.LinearRadio.TabIndex = 81;
            this.LinearRadio.TabStop = true;
            this.LinearRadio.Text = "Linear";
            this.LinearRadio.UseVisualStyleBackColor = true;
            this.LinearRadio.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LinearRadio_MouseClick);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(520, 32);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(89, 24);
            this.label32.TabIndex = 87;
            this.label32.Text = "Coloring";
            // 
            // previewButton
            // 
            this.previewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewButton.Location = new System.Drawing.Point(352, 334);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(104, 40);
            this.previewButton.TabIndex = 88;
            this.previewButton.Text = "Play";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(372, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 24);
            this.label11.TabIndex = 90;
            this.label11.Text = "Range";
            // 
            // VolumeMeter
            // 
            this.VolumeMeter.Location = new System.Drawing.Point(392, 61);
            this.VolumeMeter.Name = "VolumeMeter";
            this.VolumeMeter.Size = new System.Drawing.Size(33, 264);
            this.VolumeMeter.TabIndex = 91;
            // 
            // AudioMeterEffectEditorControlBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VolumeMeter);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.previewButton);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.VolumeGradientPanel);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.RedBar);
            this.Controls.Add(this.GreenBar);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AttackBar);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.NormalizeCheckbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.RangeBar);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.GainBar);
            this.Controls.Add(this.DecayBar);
            this.Name = "AudioMeterEffectEditorControlBase";
            this.Size = new System.Drawing.Size(823, 379);
            this.Load += new System.EventHandler(this.VerticalMeterEffectEditorControl_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.VerticalMeterEffectEditorControl_Validating);
            this.Validated += new System.EventHandler(this.VerticalMeterEffectEditorControl_Validated);
            ((System.ComponentModel.ISupportInitialize)(this.AttackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RangeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DecayBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RedBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TrackBar AttackBar;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TrackBar RangeBar;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar GainBar;
        private System.Windows.Forms.CheckBox NormalizeCheckbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar DecayBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton CustomRadio;
        private System.Windows.Forms.RadioButton DiscreteRadio;
        private System.Windows.Forms.RadioButton LinearRadio;
        private System.Windows.Forms.TrackBar RedBar;
        private System.Windows.Forms.TrackBar GreenBar;
        private System.Windows.Forms.Panel VolumeGradientPanel;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.Label label11;
        private AudioMeterEffectEditor.VerticalMeterControl VolumeMeter;


    }
}
