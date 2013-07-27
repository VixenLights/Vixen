namespace VixenApplication
{
	partial class ConfigElements
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigElements));
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxSelectedNode = new System.Windows.Forms.GroupBox();
			this.labelProperties = new System.Windows.Forms.Label();
			this.buttonConfigureProperty = new System.Windows.Forms.Button();
			this.buttonDeleteProperty = new System.Windows.Forms.Button();
			this.buttonAddProperty = new System.Windows.Forms.Button();
			this.listViewProperties = new System.Windows.Forms.ListView();
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelParents = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.groupBoxOperations = new System.Windows.Forms.GroupBox();
			this.buttonAddMultipleElements = new System.Windows.Forms.Button();
			this.buttonDeleteElement = new System.Windows.Forms.Button();
			this.buttonRename = new System.Windows.Forms.Button();
			this.buttonCreateGroup = new System.Windows.Forms.Button();
			this.buttonAddElement = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.elementTree = new Common.Controls.ElementTree();
			this.groupBoxSelectedNode.SuspendLayout();
			this.groupBoxOperations.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(336, 478);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 25);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// groupBoxSelectedNode
			// 
			this.groupBoxSelectedNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedNode.Controls.Add(this.labelProperties);
			this.groupBoxSelectedNode.Controls.Add(this.buttonConfigureProperty);
			this.groupBoxSelectedNode.Controls.Add(this.buttonDeleteProperty);
			this.groupBoxSelectedNode.Controls.Add(this.buttonAddProperty);
			this.groupBoxSelectedNode.Controls.Add(this.listViewProperties);
			this.groupBoxSelectedNode.Controls.Add(this.labelParents);
			this.groupBoxSelectedNode.Controls.Add(this.label2);
			this.groupBoxSelectedNode.Controls.Add(this.textBoxName);
			this.groupBoxSelectedNode.Location = new System.Drawing.Point(310, 219);
			this.groupBoxSelectedNode.Name = "groupBoxSelectedNode";
			this.groupBoxSelectedNode.Size = new System.Drawing.Size(212, 242);
			this.groupBoxSelectedNode.TabIndex = 11;
			this.groupBoxSelectedNode.TabStop = false;
			this.groupBoxSelectedNode.Text = "Selected Item";
			// 
			// labelProperties
			// 
			this.labelProperties.AutoSize = true;
			this.labelProperties.Location = new System.Drawing.Point(12, 89);
			this.labelProperties.Name = "labelProperties";
			this.labelProperties.Size = new System.Drawing.Size(57, 13);
			this.labelProperties.TabIndex = 29;
			this.labelProperties.Text = "Properties:";
			// 
			// buttonConfigureProperty
			// 
			this.buttonConfigureProperty.Location = new System.Drawing.Point(76, 200);
			this.buttonConfigureProperty.Name = "buttonConfigureProperty";
			this.buttonConfigureProperty.Size = new System.Drawing.Size(60, 25);
			this.buttonConfigureProperty.TabIndex = 28;
			this.buttonConfigureProperty.Text = "Configure";
			this.buttonConfigureProperty.UseVisualStyleBackColor = true;
			this.buttonConfigureProperty.Click += new System.EventHandler(this.buttonConfigureProperty_Click);
			// 
			// buttonDeleteProperty
			// 
			this.buttonDeleteProperty.Location = new System.Drawing.Point(140, 200);
			this.buttonDeleteProperty.Name = "buttonDeleteProperty";
			this.buttonDeleteProperty.Size = new System.Drawing.Size(60, 25);
			this.buttonDeleteProperty.TabIndex = 27;
			this.buttonDeleteProperty.Text = "Delete";
			this.buttonDeleteProperty.UseVisualStyleBackColor = true;
			this.buttonDeleteProperty.Click += new System.EventHandler(this.buttonDeleteProperty_Click);
			// 
			// buttonAddProperty
			// 
			this.buttonAddProperty.Location = new System.Drawing.Point(12, 200);
			this.buttonAddProperty.Name = "buttonAddProperty";
			this.buttonAddProperty.Size = new System.Drawing.Size(60, 25);
			this.buttonAddProperty.TabIndex = 26;
			this.buttonAddProperty.Text = "Add";
			this.buttonAddProperty.UseVisualStyleBackColor = true;
			this.buttonAddProperty.Click += new System.EventHandler(this.buttonAddProperty_Click);
			// 
			// listViewProperties
			// 
			this.listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
			this.listViewProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewProperties.HideSelection = false;
			this.listViewProperties.Location = new System.Drawing.Point(15, 110);
			this.listViewProperties.Name = "listViewProperties";
			this.listViewProperties.Size = new System.Drawing.Size(180, 75);
			this.listViewProperties.TabIndex = 25;
			this.listViewProperties.UseCompatibleStateImageBehavior = false;
			this.listViewProperties.View = System.Windows.Forms.View.Details;
			this.listViewProperties.SelectedIndexChanged += new System.EventHandler(this.listViewProperties_SelectedIndexChanged);
			this.listViewProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewProperties_MouseDoubleClick);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 155;
			// 
			// labelParents
			// 
			this.labelParents.AutoEllipsis = true;
			this.labelParents.Location = new System.Drawing.Point(12, 53);
			this.labelParents.Name = "labelParents";
			this.labelParents.Size = new System.Drawing.Size(183, 28);
			this.labelParents.TabIndex = 24;
			this.labelParents.Text = "This item is in <x> groups:\r\nasdfasdfasdfasdfasdfasdfasdfasdfqwerqwerqwerqwer\r\n23" +
    "4123412341234123412341234";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(53, 25);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(142, 20);
			this.textBoxName.TabIndex = 11;
			this.textBoxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxName_KeyDown);
			// 
			// groupBoxOperations
			// 
			this.groupBoxOperations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOperations.Controls.Add(this.buttonAddMultipleElements);
			this.groupBoxOperations.Controls.Add(this.buttonDeleteElement);
			this.groupBoxOperations.Controls.Add(this.buttonRename);
			this.groupBoxOperations.Controls.Add(this.buttonCreateGroup);
			this.groupBoxOperations.Controls.Add(this.buttonAddElement);
			this.groupBoxOperations.Location = new System.Drawing.Point(310, 12);
			this.groupBoxOperations.Name = "groupBoxOperations";
			this.groupBoxOperations.Size = new System.Drawing.Size(212, 201);
			this.groupBoxOperations.TabIndex = 23;
			this.groupBoxOperations.TabStop = false;
			this.groupBoxOperations.Text = "Operations";
			// 
			// buttonAddMultipleElements
			// 
			this.buttonAddMultipleElements.Location = new System.Drawing.Point(35, 59);
			this.buttonAddMultipleElements.Name = "buttonAddMultipleElements";
			this.buttonAddMultipleElements.Size = new System.Drawing.Size(140, 25);
			this.buttonAddMultipleElements.TabIndex = 27;
			this.buttonAddMultipleElements.Text = "Add Multiple";
			this.buttonAddMultipleElements.UseVisualStyleBackColor = true;
			this.buttonAddMultipleElements.Click += new System.EventHandler(this.buttonAddMultipleElements_Click);
			// 
			// buttonDeleteElement
			// 
			this.buttonDeleteElement.Location = new System.Drawing.Point(35, 93);
			this.buttonDeleteElement.Name = "buttonDeleteElement";
			this.buttonDeleteElement.Size = new System.Drawing.Size(140, 25);
			this.buttonDeleteElement.TabIndex = 24;
			this.buttonDeleteElement.Text = "Delete";
			this.buttonDeleteElement.UseVisualStyleBackColor = true;
			this.buttonDeleteElement.Click += new System.EventHandler(this.buttonDeleteElement_Click);
			// 
			// buttonRename
			// 
			this.buttonRename.Location = new System.Drawing.Point(35, 161);
			this.buttonRename.Name = "buttonRename";
			this.buttonRename.Size = new System.Drawing.Size(140, 25);
			this.buttonRename.TabIndex = 26;
			this.buttonRename.Text = "Rename";
			this.buttonRename.UseVisualStyleBackColor = true;
			this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
			// 
			// buttonCreateGroup
			// 
			this.buttonCreateGroup.Location = new System.Drawing.Point(35, 127);
			this.buttonCreateGroup.Name = "buttonCreateGroup";
			this.buttonCreateGroup.Size = new System.Drawing.Size(140, 25);
			this.buttonCreateGroup.TabIndex = 25;
			this.buttonCreateGroup.Text = "Create Group";
			this.buttonCreateGroup.UseVisualStyleBackColor = true;
			this.buttonCreateGroup.Click += new System.EventHandler(this.buttonCreateGroup_Click);
			// 
			// buttonAddElement
			// 
			this.buttonAddElement.Location = new System.Drawing.Point(35, 25);
			this.buttonAddElement.Name = "buttonAddElement";
			this.buttonAddElement.Size = new System.Drawing.Size(140, 25);
			this.buttonAddElement.TabIndex = 23;
			this.buttonAddElement.Text = "Add";
			this.buttonAddElement.UseVisualStyleBackColor = true;
			this.buttonAddElement.Click += new System.EventHandler(this.buttonAddElement_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 471);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(239, 13);
			this.label4.TabIndex = 24;
			this.label4.Text = "Click and drag to move items in the configuration.";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 488);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(294, 13);
			this.label5.TabIndex = 25;
			this.label5.Text = "Hold down CTRL while dragging to copy items to destination.";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(432, 478);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 25);
			this.buttonCancel.TabIndex = 26;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// elementTree
			// 
			this.elementTree.AllowDragging = true;
			this.elementTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.elementTree.Location = new System.Drawing.Point(15, 12);
			this.elementTree.Name = "elementTree";
			this.elementTree.Size = new System.Drawing.Size(280, 456);
			this.elementTree.TabIndex = 27;
			// 
			// ConfigElements
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(534, 512);
			this.Controls.Add(this.elementTree);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.groupBoxOperations);
			this.Controls.Add(this.groupBoxSelectedNode);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(550, 550);
			this.Name = "ConfigElements";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Display Elements & Group Configuration";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigElements_FormClosing);
			this.Load += new System.EventHandler(this.ConfigElements_Load);
			this.groupBoxSelectedNode.ResumeLayout(false);
			this.groupBoxSelectedNode.PerformLayout();
			this.groupBoxOperations.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBoxSelectedNode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label labelParents;
		private System.Windows.Forms.GroupBox groupBoxOperations;
		private System.Windows.Forms.Button buttonDeleteElement;
		private System.Windows.Forms.Button buttonRename;
		private System.Windows.Forms.Button buttonCreateGroup;
		private System.Windows.Forms.Button buttonAddElement;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelProperties;
		private System.Windows.Forms.Button buttonConfigureProperty;
		private System.Windows.Forms.Button buttonDeleteProperty;
		private System.Windows.Forms.Button buttonAddProperty;
		private System.Windows.Forms.ListView listViewProperties;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonAddMultipleElements;
		private Common.Controls.ElementTree elementTree;
	}
}