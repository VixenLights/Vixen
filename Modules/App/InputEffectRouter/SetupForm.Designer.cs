namespace VixenModules.App.InputEffectRouter {
	partial class SetupForm {
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
			this.components = new System.ComponentModel.Container();
			this.groupBoxInputDetail = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.treeViewInputs = new System.Windows.Forms.TreeView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonAddInputModule = new System.Windows.Forms.Button();
			this.buttonRemoveInputModule = new System.Windows.Forms.Button();
			this.contextMenuStripInputModules = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.buttonSetup = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxEffect = new System.Windows.Forms.ComboBox();
			this.buttonSetupEffect = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.panelEffectParameters = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.checkedListBoxNodes = new System.Windows.Forms.CheckedListBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxAddEdit = new System.Windows.Forms.GroupBox();
			this.listViewInputEffectMap = new System.Windows.Forms.ListView();
			this.buttonRemoveInputEffectMap = new System.Windows.Forms.Button();
			this.buttonAddInputEffect = new System.Windows.Forms.Button();
			this.buttonUpdateInputEffect = new System.Windows.Forms.Button();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBoxInputDetail.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBoxAddEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxInputDetail
			// 
			this.groupBoxInputDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxInputDetail.Controls.Add(this.buttonRemoveInputEffectMap);
			this.groupBoxInputDetail.Controls.Add(this.listViewInputEffectMap);
			this.groupBoxInputDetail.Controls.Add(this.groupBoxAddEdit);
			this.groupBoxInputDetail.Enabled = false;
			this.groupBoxInputDetail.Location = new System.Drawing.Point(6, 3);
			this.groupBoxInputDetail.Name = "groupBoxInputDetail";
			this.groupBoxInputDetail.Size = new System.Drawing.Size(409, 487);
			this.groupBoxInputDetail.TabIndex = 0;
			this.groupBoxInputDetail.TabStop = false;
			this.groupBoxInputDetail.Text = "Input Causes...";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.buttonSetup);
			this.panel1.Controls.Add(this.buttonRemoveInputModule);
			this.panel1.Controls.Add(this.buttonAddInputModule);
			this.panel1.Controls.Add(this.treeViewInputs);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(255, 519);
			this.panel1.TabIndex = 0;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(547, 537);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// treeViewInputs
			// 
			this.treeViewInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeViewInputs.HideSelection = false;
			this.treeViewInputs.Location = new System.Drawing.Point(3, 31);
			this.treeViewInputs.Name = "treeViewInputs";
			this.treeViewInputs.Size = new System.Drawing.Size(249, 485);
			this.treeViewInputs.TabIndex = 3;
			this.treeViewInputs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewInputs_AfterSelect);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.Controls.Add(this.groupBoxInputDetail);
			this.panel2.Location = new System.Drawing.Point(273, 12);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(430, 519);
			this.panel2.TabIndex = 1;
			// 
			// buttonAddInputModule
			// 
			this.buttonAddInputModule.Enabled = false;
			this.buttonAddInputModule.Location = new System.Drawing.Point(3, 3);
			this.buttonAddInputModule.Name = "buttonAddInputModule";
			this.buttonAddInputModule.Size = new System.Drawing.Size(48, 23);
			this.buttonAddInputModule.TabIndex = 0;
			this.buttonAddInputModule.Text = "Add";
			this.buttonAddInputModule.UseVisualStyleBackColor = true;
			this.buttonAddInputModule.Click += new System.EventHandler(this.buttonAddInputModule_Click);
			// 
			// buttonRemoveInputModule
			// 
			this.buttonRemoveInputModule.Enabled = false;
			this.buttonRemoveInputModule.Location = new System.Drawing.Point(57, 3);
			this.buttonRemoveInputModule.Name = "buttonRemoveInputModule";
			this.buttonRemoveInputModule.Size = new System.Drawing.Size(75, 23);
			this.buttonRemoveInputModule.TabIndex = 1;
			this.buttonRemoveInputModule.Text = "Remove";
			this.buttonRemoveInputModule.UseVisualStyleBackColor = true;
			this.buttonRemoveInputModule.Click += new System.EventHandler(this.buttonRemoveInputModule_Click);
			// 
			// contextMenuStripInputModules
			// 
			this.contextMenuStripInputModules.Name = "contextMenuStripInputModules";
			this.contextMenuStripInputModules.Size = new System.Drawing.Size(61, 4);
			// 
			// buttonSetup
			// 
			this.buttonSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSetup.Enabled = false;
			this.buttonSetup.Location = new System.Drawing.Point(177, 3);
			this.buttonSetup.Name = "buttonSetup";
			this.buttonSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonSetup.TabIndex = 2;
			this.buttonSetup.Text = "Setup";
			this.buttonSetup.UseVisualStyleBackColor = true;
			this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Effect:";
			// 
			// comboBoxEffect
			// 
			this.comboBoxEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEffect.FormattingEnabled = true;
			this.comboBoxEffect.Location = new System.Drawing.Point(24, 41);
			this.comboBoxEffect.Name = "comboBoxEffect";
			this.comboBoxEffect.Size = new System.Drawing.Size(238, 21);
			this.comboBoxEffect.TabIndex = 1;
			this.comboBoxEffect.SelectedIndexChanged += new System.EventHandler(this.comboBoxEffect_SelectedIndexChanged);
			// 
			// buttonSetupEffect
			// 
			this.buttonSetupEffect.Enabled = false;
			this.buttonSetupEffect.Location = new System.Drawing.Point(268, 41);
			this.buttonSetupEffect.Name = "buttonSetupEffect";
			this.buttonSetupEffect.Size = new System.Drawing.Size(56, 21);
			this.buttonSetupEffect.TabIndex = 4;
			this.buttonSetupEffect.Text = "Setup";
			this.buttonSetupEffect.UseVisualStyleBackColor = true;
			this.buttonSetupEffect.Click += new System.EventHandler(this.buttonSetupEffect_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(166, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Of that effect, the input will affect:";
			// 
			// panelEffectParameters
			// 
			this.panelEffectParameters.AutoScroll = true;
			this.panelEffectParameters.Location = new System.Drawing.Point(24, 97);
			this.panelEffectParameters.Name = "panelEffectParameters";
			this.panelEffectParameters.Size = new System.Drawing.Size(164, 177);
			this.panelEffectParameters.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(206, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "On nodes:";
			// 
			// checkedListBoxNodes
			// 
			this.checkedListBoxNodes.CheckOnClick = true;
			this.checkedListBoxNodes.Location = new System.Drawing.Point(209, 93);
			this.checkedListBoxNodes.Name = "checkedListBoxNodes";
			this.checkedListBoxNodes.Size = new System.Drawing.Size(143, 184);
			this.checkedListBoxNodes.TabIndex = 9;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(628, 537);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// groupBoxAddEdit
			// 
			this.groupBoxAddEdit.Controls.Add(this.buttonUpdateInputEffect);
			this.groupBoxAddEdit.Controls.Add(this.buttonAddInputEffect);
			this.groupBoxAddEdit.Controls.Add(this.label1);
			this.groupBoxAddEdit.Controls.Add(this.checkedListBoxNodes);
			this.groupBoxAddEdit.Controls.Add(this.comboBoxEffect);
			this.groupBoxAddEdit.Controls.Add(this.label3);
			this.groupBoxAddEdit.Controls.Add(this.buttonSetupEffect);
			this.groupBoxAddEdit.Controls.Add(this.panelEffectParameters);
			this.groupBoxAddEdit.Controls.Add(this.label2);
			this.groupBoxAddEdit.Location = new System.Drawing.Point(19, 162);
			this.groupBoxAddEdit.Name = "groupBoxAddEdit";
			this.groupBoxAddEdit.Size = new System.Drawing.Size(365, 309);
			this.groupBoxAddEdit.TabIndex = 10;
			this.groupBoxAddEdit.TabStop = false;
			this.groupBoxAddEdit.Text = "Add/Edit";
			// 
			// listViewInputEffectMap
			// 
			this.listViewInputEffectMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewInputEffectMap.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewInputEffectMap.FullRowSelect = true;
			this.listViewInputEffectMap.HideSelection = false;
			this.listViewInputEffectMap.Location = new System.Drawing.Point(19, 30);
			this.listViewInputEffectMap.Name = "listViewInputEffectMap";
			this.listViewInputEffectMap.Size = new System.Drawing.Size(371, 97);
			this.listViewInputEffectMap.TabIndex = 11;
			this.listViewInputEffectMap.UseCompatibleStateImageBehavior = false;
			this.listViewInputEffectMap.View = System.Windows.Forms.View.Details;
			this.listViewInputEffectMap.SelectedIndexChanged += new System.EventHandler(this.listViewInputEffectMap_SelectedIndexChanged);
			// 
			// buttonRemoveInputEffectMap
			// 
			this.buttonRemoveInputEffectMap.Enabled = false;
			this.buttonRemoveInputEffectMap.Location = new System.Drawing.Point(19, 133);
			this.buttonRemoveInputEffectMap.Name = "buttonRemoveInputEffectMap";
			this.buttonRemoveInputEffectMap.Size = new System.Drawing.Size(75, 23);
			this.buttonRemoveInputEffectMap.TabIndex = 12;
			this.buttonRemoveInputEffectMap.Text = "Remove";
			this.buttonRemoveInputEffectMap.UseVisualStyleBackColor = true;
			this.buttonRemoveInputEffectMap.Click += new System.EventHandler(this.buttonRemoveInputEffectMap_Click);
			// 
			// buttonAddInputEffect
			// 
			this.buttonAddInputEffect.Location = new System.Drawing.Point(24, 280);
			this.buttonAddInputEffect.Name = "buttonAddInputEffect";
			this.buttonAddInputEffect.Size = new System.Drawing.Size(75, 23);
			this.buttonAddInputEffect.TabIndex = 10;
			this.buttonAddInputEffect.Text = "Add";
			this.buttonAddInputEffect.UseVisualStyleBackColor = true;
			this.buttonAddInputEffect.Click += new System.EventHandler(this.buttonAddInputEffect_Click);
			// 
			// buttonUpdateInputEffect
			// 
			this.buttonUpdateInputEffect.Location = new System.Drawing.Point(105, 280);
			this.buttonUpdateInputEffect.Name = "buttonUpdateInputEffect";
			this.buttonUpdateInputEffect.Size = new System.Drawing.Size(75, 23);
			this.buttonUpdateInputEffect.TabIndex = 11;
			this.buttonUpdateInputEffect.Text = "Update";
			this.buttonUpdateInputEffect.UseVisualStyleBackColor = true;
			this.buttonUpdateInputEffect.Click += new System.EventHandler(this.buttonUpdateInputEffect_Click);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Effect";
			this.columnHeader1.Width = 176;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Input Affects";
			this.columnHeader2.Width = 168;
			// 
			// SetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonOK;
			this.ClientSize = new System.Drawing.Size(715, 572);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Input Effect Router";
			this.Load += new System.EventHandler(this.SetupForm_Load);
			this.groupBoxInputDetail.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.groupBoxAddEdit.ResumeLayout(false);
			this.groupBoxAddEdit.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxInputDetail;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TreeView treeViewInputs;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button buttonRemoveInputModule;
		private System.Windows.Forms.Button buttonAddInputModule;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripInputModules;
		private System.Windows.Forms.Button buttonSetup;
		private System.Windows.Forms.Button buttonSetupEffect;
		private System.Windows.Forms.ComboBox comboBoxEffect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panelEffectParameters;
		private System.Windows.Forms.CheckedListBox checkedListBoxNodes;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonRemoveInputEffectMap;
		private System.Windows.Forms.ListView listViewInputEffectMap;
		private System.Windows.Forms.GroupBox groupBoxAddEdit;
		private System.Windows.Forms.Button buttonUpdateInputEffect;
		private System.Windows.Forms.Button buttonAddInputEffect;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
	}
}