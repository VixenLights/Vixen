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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VixenPreviewSetupElementsDocument));
            this.treeElements = new Common.Controls.MultiSelectTreeview();
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
            this.treeElements.Location = new System.Drawing.Point(0, 0);
            this.treeElements.Name = "treeElements";
            this.treeElements.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeElements.SelectedNodes")));
            this.treeElements.Size = new System.Drawing.Size(227, 382);
            this.treeElements.TabIndex = 1;
            this.treeElements.UsingCustomDragCursor = false;
            this.treeElements.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeElements_AfterSelect);
            // 
            // VixenPreviewSetupElementsDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 382);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.treeElements);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VixenPreviewSetupElementsDocument";
            this.Text = "Elements";
            this.Load += new System.EventHandler(this.VixenPreviewSetupElementsDocument_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Common.Controls.MultiSelectTreeview treeElements;
    }
}