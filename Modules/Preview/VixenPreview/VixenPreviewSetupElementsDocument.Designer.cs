namespace VixenModules.Preview.VixenPreview
{
    partial class VixenPreviewSetupElementsDocument
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VixenPreviewSetupElementsDocument));
            this.treeElements = new Common.Controls.MultiSelectTreeview();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeElements
            // 
            this.treeElements.AllowDrop = true;
            this.treeElements.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeElements.CustomDragCursor = null;
            this.treeElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeElements.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
            this.treeElements.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
            this.treeElements.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
            this.treeElements.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
            this.treeElements.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
            this.treeElements.HideSelection = false;
            this.treeElements.Location = new System.Drawing.Point(0, 0);
            this.treeElements.Name = "treeElements";
            this.treeElements.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeElements.SelectedNodes")));
            this.treeElements.Size = new System.Drawing.Size(227, 382);
            this.treeElements.TabIndex = 1;
            this.treeElements.UsingCustomDragCursor = false;
            this.treeElements.DragFinishing += new Common.Controls.DragFinishingEventHandler(this.treeElements_DragFinishing);
            this.treeElements.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeElements_AfterSelect);
            this.treeElements.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeElements_DragDrop);
            this.treeElements.DragOver += new System.Windows.Forms.DragEventHandler(this.treeElements_DragOver);
            this.treeElements.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeElements_MouseClick);
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "bullet_green.png");
            this.imageListStatus.Images.SetKeyName(1, "bullet_white.png");
            this.imageListStatus.Images.SetKeyName(2, "bullet_yellow.png");
            // 
            // VixenPreviewSetupElementsDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 382);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.treeElements);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "VixenPreviewSetupElementsDocument";
            this.Text = "Elements";
            this.Load += new System.EventHandler(this.VixenPreviewSetupElementsDocument_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Common.Controls.MultiSelectTreeview treeElements;
        private System.Windows.Forms.ImageList imageListStatus;
    }
}