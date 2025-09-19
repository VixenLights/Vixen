namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncMapSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonOK = new Button();
            buttonEditMap = new Button();
            buttonDeleteMap = new Button();
            buttonNewMap = new Button();
            buttonCloneMap = new Button();
            mappingsListView = new ListView();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(357, 315);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(93, 29);
            buttonOK.TabIndex = 4;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = false;
            // 
            // buttonEditMap
            // 
            buttonEditMap.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonEditMap.Enabled = false;
            buttonEditMap.Location = new Point(380, 45);
            buttonEditMap.Name = "buttonEditMap";
            buttonEditMap.Size = new Size(68, 27);
            buttonEditMap.TabIndex = 7;
            buttonEditMap.Text = "Edit";
            buttonEditMap.UseVisualStyleBackColor = false;
            buttonEditMap.Click += buttonEditMap_Click;
            // 
            // buttonDeleteMap
            // 
            buttonDeleteMap.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDeleteMap.Enabled = false;
            buttonDeleteMap.Location = new Point(380, 78);
            buttonDeleteMap.Name = "buttonDeleteMap";
            buttonDeleteMap.Size = new Size(68, 27);
            buttonDeleteMap.TabIndex = 8;
            buttonDeleteMap.Text = "Remove";
            buttonDeleteMap.UseVisualStyleBackColor = false;
            buttonDeleteMap.Click += buttonDeleteMapping_Click;
            // 
            // buttonNewMap
            // 
            buttonNewMap.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonNewMap.Location = new Point(380, 12);
            buttonNewMap.Name = "buttonNewMap";
            buttonNewMap.Size = new Size(68, 27);
            buttonNewMap.TabIndex = 9;
            buttonNewMap.Text = "New";
            buttonNewMap.UseVisualStyleBackColor = false;
            buttonNewMap.Click += buttonNewMap_Click;
            // 
            // buttonCloneMap
            // 
            buttonCloneMap.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonCloneMap.Enabled = false;
            buttonCloneMap.Location = new Point(380, 111);
            buttonCloneMap.Name = "buttonCloneMap";
            buttonCloneMap.Size = new Size(68, 27);
            buttonCloneMap.TabIndex = 10;
            buttonCloneMap.Text = "Clone";
            buttonCloneMap.UseVisualStyleBackColor = false;
            buttonCloneMap.Click += buttonCloneMap_Click;
            // 
            // mappingsListView
            // 
            mappingsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mappingsListView.FullRowSelect = true;
            mappingsListView.LabelEdit = true;
            mappingsListView.Location = new Point(26, 12);
            mappingsListView.MaximumSize = new Size(524, 377);
            mappingsListView.Name = "mappingsListView";
            mappingsListView.Size = new Size(348, 273);
            mappingsListView.TabIndex = 12;
            mappingsListView.UseCompatibleStateImageBehavior = false;
            mappingsListView.View = View.Details;
            mappingsListView.AfterLabelEdit += mappingsListView_AfterLabelEdit;
            mappingsListView.BeforeLabelEdit += mappingsListView_BeforeLabelEdit;
            mappingsListView.SelectedIndexChanged += mappingsListView_SelectedIndexChanged;
            mappingsListView.DoubleClick += mappingsListView_DoubleClick;
            mappingsListView.KeyDown += mappingsListView_KeyDown;
            // 
            // LipSyncMapSelector
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(68, 68, 68);
            ClientSize = new Size(462, 356);
            Controls.Add(mappingsListView);
            Controls.Add(buttonCloneMap);
            Controls.Add(buttonNewMap);
            Controls.Add(buttonDeleteMap);
            Controls.Add(buttonEditMap);
            Controls.Add(buttonOK);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(221, 221, 221);
            KeyPreview = true;
            MaximumSize = new Size(640, 480);
            MinimumSize = new Size(375, 300);
            Name = "LipSyncMapSelector";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Image Maps";
            FormClosing += LipSyncMapSelector_FormClosing;
            Load += LipSyncMapSelector_Load;
            KeyDown += LipSyncMapSelector_KeyDown;
            Resize += LipSyncMapSelector_Resize;
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonEditMap;
        private System.Windows.Forms.Button buttonDeleteMap;
        private System.Windows.Forms.Button buttonNewMap;
        private System.Windows.Forms.Button buttonCloneMap;
        private System.Windows.Forms.ListView mappingsListView;
    }
}