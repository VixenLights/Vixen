namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class ColorCollectionLibrary_Form
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
			this.listViewColors = new System.Windows.Forms.ListView();
			this.lblLibraryName = new System.Windows.Forms.Label();
			this.comboBoxCollections = new System.Windows.Forms.ComboBox();
			this.btnDeleteCollection = new System.Windows.Forms.Button();
			this.btnAddColor = new System.Windows.Forms.Button();
			this.btnRemoveColor = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.buttonNewCollection = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listViewColors
			// 
			this.listViewColors.AllowDrop = true;
			this.listViewColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listViewColors.Location = new System.Drawing.Point(15, 66);
			this.listViewColors.Name = "listViewColors";
			this.listViewColors.ShowItemToolTips = true;
			this.listViewColors.Size = new System.Drawing.Size(542, 227);
			this.listViewColors.TabIndex = 0;
			this.listViewColors.UseCompatibleStateImageBehavior = false;
			this.listViewColors.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewColors_ItemDrag);
			this.listViewColors.SelectedIndexChanged += new System.EventHandler(this.listViewColors_SelectedIndexChanged);
			this.listViewColors.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragDrop);
			this.listViewColors.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewColors_DragEnter);
			// 
			// lblLibraryName
			// 
			this.lblLibraryName.AutoSize = true;
			this.lblLibraryName.Location = new System.Drawing.Point(12, 15);
			this.lblLibraryName.Name = "lblLibraryName";
			this.lblLibraryName.Size = new System.Drawing.Size(80, 13);
			this.lblLibraryName.TabIndex = 1;
			this.lblLibraryName.Text = "Color Collection";
			// 
			// comboBoxCollections
			// 
			this.comboBoxCollections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCollections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCollections.FormattingEnabled = true;
			this.comboBoxCollections.Location = new System.Drawing.Point(98, 12);
			this.comboBoxCollections.Name = "comboBoxCollections";
			this.comboBoxCollections.Size = new System.Drawing.Size(165, 21);
			this.comboBoxCollections.TabIndex = 2;
			this.comboBoxCollections.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollections_SelectedIndexChanged);
			// 
			// btnDeleteCollection
			// 
			this.btnDeleteCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeleteCollection.Enabled = false;
			this.btnDeleteCollection.Location = new System.Drawing.Point(120, 299);
			this.btnDeleteCollection.Name = "btnDeleteCollection";
			this.btnDeleteCollection.Size = new System.Drawing.Size(121, 23);
			this.btnDeleteCollection.TabIndex = 4;
			this.btnDeleteCollection.Text = "Delete Collection";
			this.btnDeleteCollection.UseVisualStyleBackColor = true;
			this.btnDeleteCollection.Click += new System.EventHandler(this.btnDeleteCollection_Click);
			// 
			// btnAddColor
			// 
			this.btnAddColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddColor.Enabled = false;
			this.btnAddColor.Location = new System.Drawing.Point(383, 38);
			this.btnAddColor.Name = "btnAddColor";
			this.btnAddColor.Size = new System.Drawing.Size(84, 23);
			this.btnAddColor.TabIndex = 6;
			this.btnAddColor.Text = "Add Color";
			this.btnAddColor.UseVisualStyleBackColor = true;
			this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
			// 
			// btnRemoveColor
			// 
			this.btnRemoveColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveColor.Enabled = false;
			this.btnRemoveColor.Location = new System.Drawing.Point(473, 38);
			this.btnRemoveColor.Name = "btnRemoveColor";
			this.btnRemoveColor.Size = new System.Drawing.Size(84, 23);
			this.btnRemoveColor.TabIndex = 7;
			this.btnRemoveColor.Text = "Remove Color";
			this.btnRemoveColor.UseVisualStyleBackColor = true;
			this.btnRemoveColor.Click += new System.EventHandler(this.btnRemoveColor_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(345, 299);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(103, 23);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(454, 299);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(103, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// buttonNewCollection
			// 
			this.buttonNewCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewCollection.Location = new System.Drawing.Point(15, 299);
			this.buttonNewCollection.Name = "buttonNewCollection";
			this.buttonNewCollection.Size = new System.Drawing.Size(99, 23);
			this.buttonNewCollection.TabIndex = 10;
			this.buttonNewCollection.Text = "New Collection";
			this.buttonNewCollection.UseVisualStyleBackColor = true;
			this.buttonNewCollection.Click += new System.EventHandler(this.buttonNewCollection_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.Enabled = false;
			this.textBoxDescription.Location = new System.Drawing.Point(99, 40);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(164, 20);
			this.textBoxDescription.TabIndex = 12;
			this.textBoxDescription.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxDescription_KeyUp);
			// 
			// ColorCollectionLibrary_Form
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(573, 334);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonNewCollection);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnRemoveColor);
			this.Controls.Add(this.btnAddColor);
			this.Controls.Add(this.btnDeleteCollection);
			this.Controls.Add(this.comboBoxCollections);
			this.Controls.Add(this.lblLibraryName);
			this.Controls.Add(this.listViewColors);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(487, 336);
			this.Name = "ColorCollectionLibrary_Form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Collection Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RandomColorLibrary_Form_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewColors;
		private System.Windows.Forms.Label lblLibraryName;
		private System.Windows.Forms.ComboBox comboBoxCollections;
		private System.Windows.Forms.Button btnDeleteCollection;
		private System.Windows.Forms.Button btnAddColor;
		private System.Windows.Forms.Button btnRemoveColor;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button buttonNewCollection;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxDescription;
	}
}