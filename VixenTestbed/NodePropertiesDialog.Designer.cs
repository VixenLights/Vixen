namespace VixenTestbed {
	partial class NodePropertiesDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelNode = new System.Windows.Forms.Label();
			this.buttonSetup = new System.Windows.Forms.Button();
			this.buttonDone = new System.Windows.Forms.Button();
			this.listBoxAvailable = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxUsed = new System.Windows.Forms.ListBox();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelNode
			// 
			this.labelNode.AutoSize = true;
			this.labelNode.Location = new System.Drawing.Point(18, 18);
			this.labelNode.Name = "labelNode";
			this.labelNode.Size = new System.Drawing.Size(36, 13);
			this.labelNode.TabIndex = 0;
			this.labelNode.Text = "Node:";
			// 
			// buttonSetup
			// 
			this.buttonSetup.Enabled = false;
			this.buttonSetup.Location = new System.Drawing.Point(231, 219);
			this.buttonSetup.Name = "buttonSetup";
			this.buttonSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonSetup.TabIndex = 3;
			this.buttonSetup.Text = "Setup";
			this.buttonSetup.UseVisualStyleBackColor = true;
			this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
			// 
			// buttonDone
			// 
			this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(331, 266);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 4;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			// 
			// listBoxAvailable
			// 
			this.listBoxAvailable.FormattingEnabled = true;
			this.listBoxAvailable.Location = new System.Drawing.Point(21, 66);
			this.listBoxAvailable.Name = "listBoxAvailable";
			this.listBoxAvailable.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxAvailable.Size = new System.Drawing.Size(170, 147);
			this.listBoxAvailable.TabIndex = 5;
			this.listBoxAvailable.SelectedIndexChanged += new System.EventHandler(this.listBoxAvailable_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Available properties:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(228, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(149, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Properties already being used:";
			// 
			// listBoxUsed
			// 
			this.listBoxUsed.FormattingEnabled = true;
			this.listBoxUsed.Location = new System.Drawing.Point(231, 66);
			this.listBoxUsed.Name = "listBoxUsed";
			this.listBoxUsed.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxUsed.Size = new System.Drawing.Size(170, 147);
			this.listBoxUsed.TabIndex = 7;
			this.listBoxUsed.SelectedIndexChanged += new System.EventHandler(this.listBoxUsed_SelectedIndexChanged);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(197, 66);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(28, 23);
			this.buttonRemove.TabIndex = 9;
			this.buttonRemove.Text = "<";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Enabled = false;
			this.buttonAdd.Location = new System.Drawing.Point(197, 95);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(28, 23);
			this.buttonAdd.TabIndex = 10;
			this.buttonAdd.Text = ">";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// NodePropertiesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonDone;
			this.ClientSize = new System.Drawing.Size(418, 301);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listBoxUsed);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxAvailable);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.buttonSetup);
			this.Controls.Add(this.labelNode);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NodePropertiesDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Properties";
			this.Load += new System.EventHandler(this.NodePropertiesDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelNode;
		private System.Windows.Forms.Button buttonSetup;
		private System.Windows.Forms.Button buttonDone;
		private System.Windows.Forms.ListBox listBoxAvailable;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxUsed;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonAdd;
	}
}