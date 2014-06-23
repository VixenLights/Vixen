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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.listViewMappings = new System.Windows.Forms.ListView();
            this.buttonEditMap = new System.Windows.Forms.Button();
            this.buttonDeleteMap = new System.Windows.Forms.Button();
            this.buttonNewMap = new System.Windows.Forms.Button();
            this.buttonCloneMap = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(466, 273);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 25);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(380, 275);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 25);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // listViewMappings
            // 
            this.listViewMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMappings.Location = new System.Drawing.Point(12, 12);
            this.listViewMappings.Name = "listViewMappings";
            this.listViewMappings.Size = new System.Drawing.Size(534, 249);
            this.listViewMappings.TabIndex = 6;
            this.listViewMappings.UseCompatibleStateImageBehavior = false;
            this.listViewMappings.ItemActivate += new System.EventHandler(this.listViewMappings_ItemActivate);
            this.listViewMappings.SelectedIndexChanged += new System.EventHandler(this.listViewMappings_SelectedIndexChanged);
            this.listViewMappings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewMappings_KeyDown);
            this.listViewMappings.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewMappings_MouseDoubleClick);
            // 
            // buttonEditMap
            // 
            this.buttonEditMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditMap.Enabled = false;
            this.buttonEditMap.Location = new System.Drawing.Point(85, 275);
            this.buttonEditMap.Name = "buttonEditMap";
            this.buttonEditMap.Size = new System.Drawing.Size(58, 23);
            this.buttonEditMap.TabIndex = 7;
            this.buttonEditMap.Text = "Edit";
            this.buttonEditMap.UseVisualStyleBackColor = true;
            this.buttonEditMap.Click += new System.EventHandler(this.buttonEditMap_Click);
            // 
            // buttonDeleteMap
            // 
            this.buttonDeleteMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteMap.Enabled = false;
            this.buttonDeleteMap.Location = new System.Drawing.Point(149, 275);
            this.buttonDeleteMap.Name = "buttonDeleteMap";
            this.buttonDeleteMap.Size = new System.Drawing.Size(58, 23);
            this.buttonDeleteMap.TabIndex = 8;
            this.buttonDeleteMap.Text = "Remove";
            this.buttonDeleteMap.UseVisualStyleBackColor = true;
            this.buttonDeleteMap.Click += new System.EventHandler(this.buttonDeleteMapping_Click);
            // 
            // buttonNewMap
            // 
            this.buttonNewMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNewMap.Location = new System.Drawing.Point(21, 275);
            this.buttonNewMap.Name = "buttonNewMap";
            this.buttonNewMap.Size = new System.Drawing.Size(58, 23);
            this.buttonNewMap.TabIndex = 9;
            this.buttonNewMap.Text = "New";
            this.buttonNewMap.UseVisualStyleBackColor = true;
            this.buttonNewMap.Click += new System.EventHandler(this.buttonNewMap_Click);
            // 
            // buttonCloneMap
            // 
            this.buttonCloneMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCloneMap.Enabled = false;
            this.buttonCloneMap.Location = new System.Drawing.Point(213, 275);
            this.buttonCloneMap.Name = "buttonCloneMap";
            this.buttonCloneMap.Size = new System.Drawing.Size(58, 23);
            this.buttonCloneMap.TabIndex = 10;
            this.buttonCloneMap.Text = "Clone";
            this.buttonCloneMap.UseVisualStyleBackColor = true;
            this.buttonCloneMap.Click += new System.EventHandler(this.buttonCloneMap_Click);
            // 
            // LipSyncMapSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 310);
            this.Controls.Add(this.buttonCloneMap);
            this.Controls.Add(this.buttonNewMap);
            this.Controls.Add(this.buttonDeleteMap);
            this.Controls.Add(this.buttonEditMap);
            this.Controls.Add(this.listViewMappings);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "LipSyncMapSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LipSync Maps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LipSyncMapSelector_FormClosing);
            this.Load += new System.EventHandler(this.LipSyncMapSelector_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LipSyncMapSelector_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListView listViewMappings;
        private System.Windows.Forms.Button buttonEditMap;
        private System.Windows.Forms.Button buttonDeleteMap;
        private System.Windows.Forms.Button buttonNewMap;
        private System.Windows.Forms.Button buttonCloneMap;
    }
}