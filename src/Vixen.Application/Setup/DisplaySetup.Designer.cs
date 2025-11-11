using Common.Controls.Scaling;
using Common.Resources.Properties;

namespace VixenApplication.Setup
{
	partial class DisplaySetup
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
			tableLayoutPanelElementSetup = new TableLayoutPanel();
			elementHeaderLineDivider = new Label();
			elementLabel = new Label();
			patchingPaneTableLayoutPanel = new TableLayoutPanel();
			patchingHeaderLabel = new Label();
			patchingPanelLineSeperator = new Label();
			radioButtonPatchingGraphical = new RadioButton();
			radioButtonPatchingSimple = new RadioButton();
			tableLayoutPanelControllerSetup = new TableLayoutPanel();
			controllerHeaderLineDivider = new Label();
			controllersHeaderLabel = new Label();
			flowLayoutOkCancelButtons = new FlowLayoutPanel();
			buttonOk = new Button();
			buttonCancel = new Button();
			buttonHelp = new Button();
			tableLayoutPanelContainer = new TableLayoutPanel();
			tableLayoutPanelElementSetup.SuspendLayout();
			patchingPaneTableLayoutPanel.SuspendLayout();
			tableLayoutPanelControllerSetup.SuspendLayout();
			flowLayoutOkCancelButtons.SuspendLayout();
			tableLayoutPanelContainer.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanelElementSetup
			// 
			tableLayoutPanelElementSetup.ColumnCount = 1;
			tableLayoutPanelElementSetup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanelElementSetup.Controls.Add(elementHeaderLineDivider, 0, 1);
			tableLayoutPanelElementSetup.Controls.Add(elementLabel, 0, 0);
			tableLayoutPanelElementSetup.Dock = DockStyle.Fill;
			tableLayoutPanelElementSetup.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
			tableLayoutPanelElementSetup.Location = new Point(6, 6);
			tableLayoutPanelElementSetup.Name = "tableLayoutPanelElementSetup";
			tableLayoutPanelElementSetup.RowCount = 3;
			tableLayoutPanelElementSetup.RowStyles.Add(new RowStyle());
			tableLayoutPanelElementSetup.RowStyles.Add(new RowStyle());
			tableLayoutPanelElementSetup.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanelElementSetup.Size = new Size(325, 725);
			tableLayoutPanelElementSetup.TabIndex = 4;
			// 
			// elementHeaderLineDivider
			// 
			elementHeaderLineDivider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			elementHeaderLineDivider.BorderStyle = BorderStyle.Fixed3D;
			elementHeaderLineDivider.Location = new Point(3, 25);
			elementHeaderLineDivider.Name = "elementHeaderLineDivider";
			elementHeaderLineDivider.Size = new Size(319, 2);
			elementHeaderLineDivider.TabIndex = 3;
			// 
			// elementLabel
			// 
			elementLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			elementLabel.AutoSize = true;
			elementLabel.Location = new Point(5, 5);
			elementLabel.Margin = new Padding(5);
			elementLabel.Name = "elementLabel";
			elementLabel.Size = new Size(315, 15);
			elementLabel.TabIndex = 1;
			elementLabel.Text = "Elements";
			elementLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// patchingPaneTableLayoutPanel
			// 
			patchingPaneTableLayoutPanel.ColumnCount = 2;
			patchingPaneTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			patchingPaneTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			patchingPaneTableLayoutPanel.Controls.Add(patchingHeaderLabel, 0, 0);
			patchingPaneTableLayoutPanel.Controls.Add(patchingPanelLineSeperator, 0, 1);
			patchingPaneTableLayoutPanel.Controls.Add(radioButtonPatchingGraphical, 1, 2);
			patchingPaneTableLayoutPanel.Controls.Add(radioButtonPatchingSimple, 0, 2);
			patchingPaneTableLayoutPanel.Dock = DockStyle.Fill;
			patchingPaneTableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
			patchingPaneTableLayoutPanel.Location = new Point(340, 6);
			patchingPaneTableLayoutPanel.Name = "patchingPaneTableLayoutPanel";
			patchingPaneTableLayoutPanel.RowCount = 4;
			patchingPaneTableLayoutPanel.RowStyles.Add(new RowStyle());
			patchingPaneTableLayoutPanel.RowStyles.Add(new RowStyle());
			patchingPaneTableLayoutPanel.RowStyles.Add(new RowStyle());
			patchingPaneTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			patchingPaneTableLayoutPanel.Size = new Size(601, 725);
			patchingPaneTableLayoutPanel.TabIndex = 10;
			// 
			// patchingHeaderLabel
			// 
			patchingHeaderLabel.Anchor = AnchorStyles.None;
			patchingHeaderLabel.AutoSize = true;
			patchingPaneTableLayoutPanel.SetColumnSpan(patchingHeaderLabel, 2);
			patchingHeaderLabel.Location = new Point(273, 5);
			patchingHeaderLabel.Margin = new Padding(5);
			patchingHeaderLabel.Name = "patchingHeaderLabel";
			patchingHeaderLabel.Size = new Size(54, 15);
			patchingHeaderLabel.TabIndex = 4;
			patchingHeaderLabel.Text = "Patching";
			patchingHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// patchingPanelLineSeperator
			// 
			patchingPanelLineSeperator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			patchingPanelLineSeperator.BorderStyle = BorderStyle.Fixed3D;
			patchingPaneTableLayoutPanel.SetColumnSpan(patchingPanelLineSeperator, 2);
			patchingPanelLineSeperator.Location = new Point(3, 25);
			patchingPanelLineSeperator.Name = "patchingPanelLineSeperator";
			patchingPanelLineSeperator.Size = new Size(595, 2);
			patchingPanelLineSeperator.TabIndex = 69;
			// 
			// radioButtonPatchingGraphical
			// 
			radioButtonPatchingGraphical.Anchor = AnchorStyles.None;
			radioButtonPatchingGraphical.AutoSize = true;
			radioButtonPatchingGraphical.Location = new Point(399, 30);
			radioButtonPatchingGraphical.Name = "radioButtonPatchingGraphical";
			radioButtonPatchingGraphical.Size = new Size(103, 19);
			radioButtonPatchingGraphical.TabIndex = 5;
			radioButtonPatchingGraphical.TabStop = true;
			radioButtonPatchingGraphical.Text = "Graphical View";
			radioButtonPatchingGraphical.UseVisualStyleBackColor = true;
			radioButtonPatchingGraphical.CheckedChanged += radioButtonPatchingGraphical_CheckedChanged;
			// 
			// radioButtonPatchingSimple
			// 
			radioButtonPatchingSimple.Anchor = AnchorStyles.None;
			radioButtonPatchingSimple.AutoSize = true;
			radioButtonPatchingSimple.Location = new Point(94, 30);
			radioButtonPatchingSimple.Name = "radioButtonPatchingSimple";
			radioButtonPatchingSimple.Size = new Size(111, 19);
			radioButtonPatchingSimple.TabIndex = 3;
			radioButtonPatchingSimple.TabStop = true;
			radioButtonPatchingSimple.Text = "Simple Patching";
			radioButtonPatchingSimple.UseVisualStyleBackColor = true;
			radioButtonPatchingSimple.CheckedChanged += radioButtonPatchingSimple_CheckedChanged;
			// 
			// tableLayoutPanelControllerSetup
			// 
			tableLayoutPanelControllerSetup.ColumnCount = 1;
			tableLayoutPanelControllerSetup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanelControllerSetup.Controls.Add(controllerHeaderLineDivider, 0, 1);
			tableLayoutPanelControllerSetup.Controls.Add(controllersHeaderLabel, 0, 0);
			tableLayoutPanelControllerSetup.Controls.Add(flowLayoutOkCancelButtons, 0, 3);
			tableLayoutPanelControllerSetup.Dock = DockStyle.Fill;
			tableLayoutPanelControllerSetup.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
			tableLayoutPanelControllerSetup.Location = new Point(950, 6);
			tableLayoutPanelControllerSetup.Name = "tableLayoutPanelControllerSetup";
			tableLayoutPanelControllerSetup.RowCount = 4;
			tableLayoutPanelControllerSetup.RowStyles.Add(new RowStyle());
			tableLayoutPanelControllerSetup.RowStyles.Add(new RowStyle());
			tableLayoutPanelControllerSetup.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanelControllerSetup.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
			tableLayoutPanelControllerSetup.Size = new Size(325, 725);
			tableLayoutPanelControllerSetup.TabIndex = 65;
			// 
			// controllerHeaderLineDivider
			// 
			controllerHeaderLineDivider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			controllerHeaderLineDivider.BorderStyle = BorderStyle.Fixed3D;
			controllerHeaderLineDivider.Location = new Point(3, 25);
			controllerHeaderLineDivider.Name = "controllerHeaderLineDivider";
			controllerHeaderLineDivider.Size = new Size(319, 2);
			controllerHeaderLineDivider.TabIndex = 68;
			// 
			// controllersHeaderLabel
			// 
			controllersHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			controllersHeaderLabel.AutoSize = true;
			controllersHeaderLabel.Location = new Point(5, 5);
			controllersHeaderLabel.Margin = new Padding(5);
			controllersHeaderLabel.Name = "controllersHeaderLabel";
			controllersHeaderLabel.Size = new Size(315, 15);
			controllersHeaderLabel.TabIndex = 5;
			controllersHeaderLabel.Text = "Controllers";
			controllersHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// flowLayoutOkCancelButtons
			// 
			flowLayoutOkCancelButtons.Controls.Add(buttonOk);
			flowLayoutOkCancelButtons.Controls.Add(buttonCancel);
			flowLayoutOkCancelButtons.Controls.Add(buttonHelp);
			flowLayoutOkCancelButtons.Dock = DockStyle.Fill;
			flowLayoutOkCancelButtons.Location = new Point(3, 688);
			flowLayoutOkCancelButtons.Name = "flowLayoutOkCancelButtons";
			flowLayoutOkCancelButtons.Size = new Size(319, 34);
			flowLayoutOkCancelButtons.TabIndex = 69;
			// 
			// buttonOk
			// 
			buttonOk.Anchor = AnchorStyles.None;
			buttonOk.AutoSize = true;
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.Location = new Point(3, 3);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(89, 29);
			buttonOk.TabIndex = 11;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			buttonOk.Click += buttonOk_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.None;
			buttonCancel.AutoSize = true;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(98, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(89, 29);
			buttonCancel.TabIndex = 12;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonHelp
			// 
			buttonHelp.Anchor = AnchorStyles.None;
			buttonHelp.Image = Common.Resources.Tools.GetIcon(Resources.help, (int)(16 * ScalingTools.GetScaleFactor()));
			buttonHelp.AutoSize = true;
			buttonHelp.ImageAlign = ContentAlignment.MiddleLeft;
			buttonHelp.Location = new Point(193, 3);
			buttonHelp.Name = "buttonHelp";
			buttonHelp.Size = new Size(89, 29);
			buttonHelp.TabIndex = 60;
			buttonHelp.Text = "Help";
			buttonHelp.UseVisualStyleBackColor = true;
			buttonHelp.Click += buttonHelp_Click;
			// 
			// tableLayoutPanelContainer
			// 
			tableLayoutPanelContainer.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
			tableLayoutPanelContainer.ColumnCount = 3;
			tableLayoutPanelContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelContainer.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanelContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelContainer.Controls.Add(tableLayoutPanelControllerSetup, 2, 0);
			tableLayoutPanelContainer.Controls.Add(tableLayoutPanelElementSetup, 0, 0);
			tableLayoutPanelContainer.Controls.Add(patchingPaneTableLayoutPanel, 1, 0);
			tableLayoutPanelContainer.Dock = DockStyle.Fill;
			tableLayoutPanelContainer.Location = new Point(0, 0);
			tableLayoutPanelContainer.Name = "tableLayoutPanelContainer";
			tableLayoutPanelContainer.RowCount = 1;
			tableLayoutPanelContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanelContainer.Size = new Size(1281, 737);
			tableLayoutPanelContainer.TabIndex = 11;
			// 
			// DisplaySetup
			// 
			AcceptButton = buttonOk;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = buttonCancel;
			ClientSize = new Size(1281, 737);
			Controls.Add(tableLayoutPanelContainer);
			DoubleBuffered = true;
			Name = "DisplaySetup";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Display Setup";
			Load += DisplaySetup_Load;
			tableLayoutPanelElementSetup.ResumeLayout(false);
			tableLayoutPanelElementSetup.PerformLayout();
			patchingPaneTableLayoutPanel.ResumeLayout(false);
			patchingPaneTableLayoutPanel.PerformLayout();
			tableLayoutPanelControllerSetup.ResumeLayout(false);
			tableLayoutPanelControllerSetup.PerformLayout();
			flowLayoutOkCancelButtons.ResumeLayout(false);
			flowLayoutOkCancelButtons.PerformLayout();
			tableLayoutPanelContainer.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Label elementHeaderLineDivider;
		private Label elementLabel;
		private TableLayoutPanel tableLayoutPanelElementSetup;
		private RadioButton radioButtonPatchingGraphical;
		private Label patchingHeaderLabel;
		private RadioButton radioButtonPatchingSimple;
		private Button buttonCancel;
		private Button buttonOk;
		private Button buttonHelp;
		private TableLayoutPanel tableLayoutPanelControllerSetup;
		private Label controllersHeaderLabel;
		private Label controllerHeaderLineDivider;
		private TableLayoutPanel patchingPaneTableLayoutPanel;
		private Label patchingPanelLineSeperator;
		private TableLayoutPanel tableLayoutPanelContainer;
		private FlowLayoutPanel flowLayoutOkCancelButtons;
	}
}