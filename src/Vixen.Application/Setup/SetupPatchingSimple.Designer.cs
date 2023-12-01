using System.Windows.Forms;

namespace VixenApplication.Setup
{
	partial class SetupPatchingSimple
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
			components = new System.ComponentModel.Container();
			groupBoxElements = new GroupBox();
			tableLayoutPanel4 = new TableLayoutPanel();
			labelFilterCount = new Label();
			label5 = new Label();
			buttonUnpatchElements = new Button();
			labelElementCount = new Label();
			labelPatchPointCount = new Label();
			labelGroupCount = new Label();
			label3 = new Label();
			label6 = new Label();
			labelItemCount = new Label();
			labelConnectedPatchPointCount = new Label();
			label2 = new Label();
			label7 = new Label();
			labelUnconnectedPatchPointCount = new Label();
			label1 = new Label();
			label4 = new Label();
			checkBoxReverseElementOrder = new CheckBox();
			groupBoxControllers = new GroupBox();
			tableLayoutPanel3 = new TableLayoutPanel();
			labelLastOutput = new Label();
			buttonUnpatchControllers = new Button();
			label20 = new Label();
			label9 = new Label();
			labelFirstOutput = new Label();
			labelOutputCount = new Label();
			label16 = new Label();
			label8 = new Label();
			labelPatchedOutputCount = new Label();
			labelControllerCount = new Label();
			labelUnpatchedOutputCount = new Label();
			label21 = new Label();
			label15 = new Label();
			checkBoxReverseOutputOrder = new CheckBox();
			groupBoxPatching = new GroupBox();
			tableLayoutPanelPatchingOptions = new TableLayoutPanel();
			groupBoxElementOptions = new GroupBox();
			radioButtonAllAvailablePatchPoints = new RadioButton();
			radioButtonUnconnectedPatchPointsOnly = new RadioButton();
			buttonDoPatching = new Button();
			labelPatchWarning = new Label();
			groupBoxOutputOptions = new GroupBox();
			radioButtonAllOutputs = new RadioButton();
			radioButtonUnpatchedOutputsOnly = new RadioButton();
			labelPatchSummary = new Label();
			toolTip1 = new ToolTip(components);
			tableLayoutPanelContainer = new TableLayoutPanel();
			groupBoxElements.SuspendLayout();
			tableLayoutPanel4.SuspendLayout();
			groupBoxControllers.SuspendLayout();
			tableLayoutPanel3.SuspendLayout();
			groupBoxPatching.SuspendLayout();
			tableLayoutPanelPatchingOptions.SuspendLayout();
			groupBoxElementOptions.SuspendLayout();
			groupBoxOutputOptions.SuspendLayout();
			tableLayoutPanelContainer.SuspendLayout();
			SuspendLayout();
			// 
			// groupBoxElements
			// 
			groupBoxElements.AutoSize = true;
			groupBoxElements.Controls.Add(tableLayoutPanel4);
			groupBoxElements.Dock = DockStyle.Fill;
			groupBoxElements.Location = new Point(4, 3);
			groupBoxElements.Margin = new Padding(4, 3, 4, 3);
			groupBoxElements.Name = "groupBoxElements";
			groupBoxElements.Padding = new Padding(4, 3, 4, 3);
			groupBoxElements.Size = new Size(258, 234);
			groupBoxElements.TabIndex = 0;
			groupBoxElements.TabStop = false;
			groupBoxElements.Text = "Selected Elements";
			groupBoxElements.Paint += groupBoxes_Paint;
			// 
			// tableLayoutPanel4
			// 
			tableLayoutPanel4.AutoSize = true;
			tableLayoutPanel4.ColumnCount = 2;
			tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel4.Controls.Add(labelFilterCount, 1, 7);
			tableLayoutPanel4.Controls.Add(label5, 0, 0);
			tableLayoutPanel4.Controls.Add(buttonUnpatchElements, 0, 8);
			tableLayoutPanel4.Controls.Add(labelElementCount, 1, 6);
			tableLayoutPanel4.Controls.Add(labelPatchPointCount, 1, 0);
			tableLayoutPanel4.Controls.Add(labelGroupCount, 1, 5);
			tableLayoutPanel4.Controls.Add(label3, 0, 6);
			tableLayoutPanel4.Controls.Add(label6, 0, 1);
			tableLayoutPanel4.Controls.Add(labelItemCount, 1, 4);
			tableLayoutPanel4.Controls.Add(labelConnectedPatchPointCount, 1, 1);
			tableLayoutPanel4.Controls.Add(label2, 0, 5);
			tableLayoutPanel4.Controls.Add(label7, 0, 2);
			tableLayoutPanel4.Controls.Add(labelUnconnectedPatchPointCount, 1, 2);
			tableLayoutPanel4.Controls.Add(label1, 0, 4);
			tableLayoutPanel4.Controls.Add(label4, 0, 7);
			tableLayoutPanel4.Dock = DockStyle.Fill;
			tableLayoutPanel4.Location = new Point(4, 19);
			tableLayoutPanel4.Name = "tableLayoutPanel4";
			tableLayoutPanel4.RowCount = 9;
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle());
			tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel4.Size = new Size(250, 212);
			tableLayoutPanel4.TabIndex = 24;
			// 
			// labelFilterCount
			// 
			labelFilterCount.AutoSize = true;
			labelFilterCount.Location = new Point(116, 162);
			labelFilterCount.Margin = new Padding(4);
			labelFilterCount.Name = "labelFilterCount";
			labelFilterCount.Size = new Size(13, 15);
			labelFilterCount.TabIndex = 21;
			labelFilterCount.Text = "0";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(4, 4);
			label5.Margin = new Padding(4);
			label5.Name = "label5";
			label5.Size = new Size(104, 15);
			label5.TabIndex = 4;
			label5.Text = "Total Patch Points:";
			toolTip1.SetToolTip(label5, "The total number of Patch Points connected to the selected elements.  Patch Points are the outputs from any element or filter, but before it gets to the controllers.");
			// 
			// buttonUnpatchElements
			// 
			buttonUnpatchElements.Anchor = AnchorStyles.None;
			buttonUnpatchElements.AutoSize = true;
			tableLayoutPanel4.SetColumnSpan(buttonUnpatchElements, 2);
			buttonUnpatchElements.Location = new Point(68, 184);
			buttonUnpatchElements.Margin = new Padding(4, 3, 4, 3);
			buttonUnpatchElements.Name = "buttonUnpatchElements";
			buttonUnpatchElements.Size = new Size(113, 25);
			buttonUnpatchElements.TabIndex = 22;
			buttonUnpatchElements.Text = "Unpatch Elements";
			buttonUnpatchElements.UseVisualStyleBackColor = true;
			buttonUnpatchElements.Click += buttonUnpatchElements_Click;
			buttonUnpatchElements.MouseLeave += buttonBackground_MouseLeave;
			buttonUnpatchElements.MouseHover += buttonBackground_MouseHover;
			// 
			// labelElementCount
			// 
			labelElementCount.AutoSize = true;
			labelElementCount.Location = new Point(116, 139);
			labelElementCount.Margin = new Padding(4);
			labelElementCount.Name = "labelElementCount";
			labelElementCount.Size = new Size(13, 15);
			labelElementCount.TabIndex = 20;
			labelElementCount.Text = "0";
			// 
			// labelPatchPointCount
			// 
			labelPatchPointCount.AutoSize = true;
			labelPatchPointCount.Location = new Point(116, 4);
			labelPatchPointCount.Margin = new Padding(4);
			labelPatchPointCount.Name = "labelPatchPointCount";
			labelPatchPointCount.Size = new Size(13, 15);
			labelPatchPointCount.TabIndex = 11;
			labelPatchPointCount.Text = "0";
			// 
			// labelGroupCount
			// 
			labelGroupCount.AutoSize = true;
			labelGroupCount.Location = new Point(116, 116);
			labelGroupCount.Margin = new Padding(4);
			labelGroupCount.Name = "labelGroupCount";
			labelGroupCount.Size = new Size(13, 15);
			labelGroupCount.TabIndex = 19;
			labelGroupCount.Text = "0";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(4, 139);
			label3.Margin = new Padding(4);
			label3.Name = "label3";
			label3.Size = new Size(58, 15);
			label3.TabIndex = 16;
			label3.Text = "Elements:";
			toolTip1.SetToolTip(label3, "The number of elements found in (or descending from) the selected elements.");
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(4, 27);
			label6.Margin = new Padding(4);
			label6.Name = "label6";
			label6.Size = new Size(68, 15);
			label6.TabIndex = 5;
			label6.Text = "Connected:";
			toolTip1.SetToolTip(label6, "The number of Patch Points connected to the selected elements that are being patched to controller outputs.");
			// 
			// labelItemCount
			// 
			labelItemCount.AutoSize = true;
			labelItemCount.Location = new Point(116, 93);
			labelItemCount.Margin = new Padding(4);
			labelItemCount.Name = "labelItemCount";
			labelItemCount.Size = new Size(13, 15);
			labelItemCount.TabIndex = 18;
			labelItemCount.Text = "0";
			// 
			// labelConnectedPatchPointCount
			// 
			labelConnectedPatchPointCount.AutoSize = true;
			labelConnectedPatchPointCount.Location = new Point(116, 27);
			labelConnectedPatchPointCount.Margin = new Padding(4);
			labelConnectedPatchPointCount.Name = "labelConnectedPatchPointCount";
			labelConnectedPatchPointCount.Size = new Size(13, 15);
			labelConnectedPatchPointCount.TabIndex = 12;
			labelConnectedPatchPointCount.Text = "0";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(4, 116);
			label2.Margin = new Padding(4);
			label2.Name = "label2";
			label2.Size = new Size(48, 15);
			label2.TabIndex = 15;
			label2.Text = "Groups:";
			toolTip1.SetToolTip(label2, "The number of groups found from the selected elements.");
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new Point(4, 50);
			label7.Margin = new Padding(4);
			label7.Name = "label7";
			label7.Size = new Size(81, 15);
			label7.TabIndex = 6;
			label7.Text = "Unconnected:";
			toolTip1.SetToolTip(label7, "The number of Patch Points connected to the selected elements that have nothing else connected.");
			// 
			// labelUnconnectedPatchPointCount
			// 
			labelUnconnectedPatchPointCount.AutoSize = true;
			labelUnconnectedPatchPointCount.Location = new Point(116, 50);
			labelUnconnectedPatchPointCount.Margin = new Padding(4);
			labelUnconnectedPatchPointCount.Name = "labelUnconnectedPatchPointCount";
			labelUnconnectedPatchPointCount.Size = new Size(13, 15);
			labelUnconnectedPatchPointCount.TabIndex = 13;
			labelUnconnectedPatchPointCount.Text = "0";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(4, 93);
			label1.Margin = new Padding(4);
			label1.Name = "label1";
			label1.Size = new Size(39, 15);
			label1.TabIndex = 14;
			label1.Text = "Items:";
			toolTip1.SetToolTip(label1, "The number of items selected in the element view.");
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(4, 162);
			label4.Margin = new Padding(4);
			label4.Name = "label4";
			label4.Size = new Size(41, 15);
			label4.TabIndex = 17;
			label4.Text = "Filters:";
			toolTip1.SetToolTip(label4, "The number of filters found in the patching connections from the selected elements.");
			// 
			// checkBoxReverseElementOrder
			// 
			checkBoxReverseElementOrder.AutoSize = true;
			checkBoxReverseElementOrder.Location = new Point(10, 75);
			checkBoxReverseElementOrder.Margin = new Padding(4, 3, 4, 3);
			checkBoxReverseElementOrder.Name = "checkBoxReverseElementOrder";
			checkBoxReverseElementOrder.Size = new Size(145, 19);
			checkBoxReverseElementOrder.TabIndex = 23;
			checkBoxReverseElementOrder.Text = "Reverse Element Order";
			toolTip1.SetToolTip(checkBoxReverseElementOrder, "The order in which Elements will be patched to controller outputs. This does not effect the order in which color channels are patched to controller outputs.");
			checkBoxReverseElementOrder.UseVisualStyleBackColor = true;
			checkBoxReverseElementOrder.CheckedChanged += checkBoxReverseElementOrder_CheckedChanged;
			// 
			// groupBoxControllers
			// 
			groupBoxControllers.AutoSize = true;
			groupBoxControllers.Controls.Add(tableLayoutPanel3);
			groupBoxControllers.Dock = DockStyle.Fill;
			groupBoxControllers.Location = new Point(270, 3);
			groupBoxControllers.Margin = new Padding(4, 3, 4, 3);
			groupBoxControllers.Name = "groupBoxControllers";
			groupBoxControllers.Padding = new Padding(4, 3, 4, 3);
			groupBoxControllers.Size = new Size(259, 234);
			groupBoxControllers.TabIndex = 1;
			groupBoxControllers.TabStop = false;
			groupBoxControllers.Text = "Selected Controllers";
			groupBoxControllers.Paint += groupBoxes_Paint;
			// 
			// tableLayoutPanel3
			// 
			tableLayoutPanel3.AutoSize = true;
			tableLayoutPanel3.ColumnCount = 2;
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel3.Controls.Add(labelLastOutput, 1, 7);
			tableLayoutPanel3.Controls.Add(buttonUnpatchControllers, 0, 8);
			tableLayoutPanel3.Controls.Add(label20, 0, 0);
			tableLayoutPanel3.Controls.Add(label9, 0, 7);
			tableLayoutPanel3.Controls.Add(labelFirstOutput, 1, 6);
			tableLayoutPanel3.Controls.Add(labelOutputCount, 1, 0);
			tableLayoutPanel3.Controls.Add(label16, 0, 1);
			tableLayoutPanel3.Controls.Add(label8, 0, 6);
			tableLayoutPanel3.Controls.Add(labelPatchedOutputCount, 1, 1);
			tableLayoutPanel3.Controls.Add(labelControllerCount, 1, 4);
			tableLayoutPanel3.Controls.Add(labelUnpatchedOutputCount, 1, 2);
			tableLayoutPanel3.Controls.Add(label21, 0, 4);
			tableLayoutPanel3.Controls.Add(label15, 0, 2);
			tableLayoutPanel3.Dock = DockStyle.Fill;
			tableLayoutPanel3.Location = new Point(4, 19);
			tableLayoutPanel3.Name = "tableLayoutPanel3";
			tableLayoutPanel3.RowCount = 9;
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel3.Size = new Size(251, 212);
			tableLayoutPanel3.TabIndex = 35;
			// 
			// labelLastOutput
			// 
			labelLastOutput.AutoSize = true;
			labelLastOutput.Location = new Point(83, 159);
			labelLastOutput.Margin = new Padding(4);
			labelLastOutput.Name = "labelLastOutput";
			labelLastOutput.Size = new Size(76, 15);
			labelLastOutput.TabIndex = 33;
			labelLastOutput.Text = "Controller #0";
			// 
			// buttonUnpatchControllers
			// 
			buttonUnpatchControllers.Anchor = AnchorStyles.None;
			buttonUnpatchControllers.AutoSize = true;
			tableLayoutPanel3.SetColumnSpan(buttonUnpatchControllers, 2);
			buttonUnpatchControllers.Location = new Point(60, 182);
			buttonUnpatchControllers.Margin = new Padding(4, 3, 4, 3);
			buttonUnpatchControllers.Name = "buttonUnpatchControllers";
			buttonUnpatchControllers.Size = new Size(130, 25);
			buttonUnpatchControllers.TabIndex = 28;
			buttonUnpatchControllers.Text = "Unpatch Controllers";
			buttonUnpatchControllers.UseVisualStyleBackColor = true;
			buttonUnpatchControllers.Click += buttonUnpatchControllers_Click;
			buttonUnpatchControllers.MouseLeave += buttonBackground_MouseLeave;
			buttonUnpatchControllers.MouseHover += buttonBackground_MouseHover;
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new Point(4, 4);
			label20.Margin = new Padding(4);
			label20.Name = "label20";
			label20.Size = new Size(53, 15);
			label20.TabIndex = 15;
			label20.Text = "Outputs:";
			toolTip1.SetToolTip(label20, "The total number of controller outputs selected.");
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new Point(4, 159);
			label9.Margin = new Padding(4);
			label9.Name = "label9";
			label9.Size = new Size(70, 15);
			label9.TabIndex = 31;
			label9.Text = "Last output:";
			toolTip1.SetToolTip(label9, "The last output in the list of selected outputs (as will be used for patching).");
			// 
			// labelFirstOutput
			// 
			labelFirstOutput.AutoSize = true;
			labelFirstOutput.Location = new Point(83, 136);
			labelFirstOutput.Margin = new Padding(4);
			labelFirstOutput.Name = "labelFirstOutput";
			labelFirstOutput.Size = new Size(76, 15);
			labelFirstOutput.TabIndex = 32;
			labelFirstOutput.Text = "Controller #0";
			// 
			// labelOutputCount
			// 
			labelOutputCount.AutoSize = true;
			labelOutputCount.Location = new Point(83, 4);
			labelOutputCount.Margin = new Padding(4);
			labelOutputCount.Name = "labelOutputCount";
			labelOutputCount.Size = new Size(13, 15);
			labelOutputCount.TabIndex = 22;
			labelOutputCount.Text = "0";
			// 
			// label16
			// 
			label16.AutoSize = true;
			label16.Location = new Point(4, 27);
			label16.Margin = new Padding(4);
			label16.Name = "label16";
			label16.Size = new Size(53, 15);
			label16.TabIndex = 19;
			label16.Text = "Patched:";
			toolTip1.SetToolTip(label16, "The number of controller outputs selected that are already connected to something.");
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point(4, 136);
			label8.Margin = new Padding(4);
			label8.Name = "label8";
			label8.Size = new Size(71, 15);
			label8.TabIndex = 30;
			label8.Text = "First output:";
			toolTip1.SetToolTip(label8, "The first output in the list of selected outputs (as will be used for patching).");
			// 
			// labelPatchedOutputCount
			// 
			labelPatchedOutputCount.AutoSize = true;
			labelPatchedOutputCount.Location = new Point(83, 27);
			labelPatchedOutputCount.Margin = new Padding(4);
			labelPatchedOutputCount.Name = "labelPatchedOutputCount";
			labelPatchedOutputCount.Size = new Size(13, 15);
			labelPatchedOutputCount.TabIndex = 26;
			labelPatchedOutputCount.Text = "0";
			// 
			// labelControllerCount
			// 
			labelControllerCount.AutoSize = true;
			labelControllerCount.Location = new Point(83, 93);
			labelControllerCount.Margin = new Padding(4);
			labelControllerCount.Name = "labelControllerCount";
			labelControllerCount.Size = new Size(13, 15);
			labelControllerCount.TabIndex = 21;
			labelControllerCount.Text = "0";
			// 
			// labelUnpatchedOutputCount
			// 
			labelUnpatchedOutputCount.AutoSize = true;
			labelUnpatchedOutputCount.Location = new Point(83, 50);
			labelUnpatchedOutputCount.Margin = new Padding(4);
			labelUnpatchedOutputCount.Name = "labelUnpatchedOutputCount";
			labelUnpatchedOutputCount.Size = new Size(13, 15);
			labelUnpatchedOutputCount.TabIndex = 27;
			labelUnpatchedOutputCount.Text = "0";
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new Point(4, 93);
			label21.Margin = new Padding(4);
			label21.Name = "label21";
			label21.Size = new Size(68, 15);
			label21.TabIndex = 14;
			label21.Text = "Controllers:";
			toolTip1.SetToolTip(label21, "The number of controllers (or part thereof) selected.");
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Location = new Point(4, 50);
			label15.Margin = new Padding(4);
			label15.Name = "label15";
			label15.Size = new Size(68, 15);
			label15.TabIndex = 20;
			label15.Text = "Unpatched:";
			toolTip1.SetToolTip(label15, "The number of controller outputs selected that are not connected to anything.");
			// 
			// checkBoxReverseOutputOrder
			// 
			checkBoxReverseOutputOrder.AutoSize = true;
			checkBoxReverseOutputOrder.Location = new Point(14, 75);
			checkBoxReverseOutputOrder.Margin = new Padding(4, 3, 4, 3);
			checkBoxReverseOutputOrder.Name = "checkBoxReverseOutputOrder";
			checkBoxReverseOutputOrder.Size = new Size(201, 19);
			checkBoxReverseOutputOrder.TabIndex = 29;
			checkBoxReverseOutputOrder.Text = "Reverse order of selected outputs";
			checkBoxReverseOutputOrder.UseVisualStyleBackColor = true;
			checkBoxReverseOutputOrder.CheckedChanged += checkBoxReverseOutputOrder_CheckedChanged;
			// 
			// groupBoxPatching
			// 
			groupBoxPatching.AutoSize = true;
			tableLayoutPanelContainer.SetColumnSpan(groupBoxPatching, 2);
			groupBoxPatching.Controls.Add(tableLayoutPanelPatchingOptions);
			groupBoxPatching.Dock = DockStyle.Fill;
			groupBoxPatching.Location = new Point(4, 243);
			groupBoxPatching.Margin = new Padding(4, 3, 4, 3);
			groupBoxPatching.Name = "groupBoxPatching";
			groupBoxPatching.Padding = new Padding(4, 3, 4, 3);
			groupBoxPatching.Size = new Size(525, 223);
			groupBoxPatching.TabIndex = 2;
			groupBoxPatching.TabStop = false;
			groupBoxPatching.Text = "Patching Options";
			groupBoxPatching.Paint += groupBoxes_Paint;
			// 
			// tableLayoutPanelPatchingOptions
			// 
			tableLayoutPanelPatchingOptions.AutoSize = true;
			tableLayoutPanelPatchingOptions.ColumnCount = 2;
			tableLayoutPanelPatchingOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelPatchingOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelPatchingOptions.Controls.Add(groupBoxElementOptions, 0, 0);
			tableLayoutPanelPatchingOptions.Controls.Add(buttonDoPatching, 0, 3);
			tableLayoutPanelPatchingOptions.Controls.Add(labelPatchWarning, 0, 2);
			tableLayoutPanelPatchingOptions.Controls.Add(groupBoxOutputOptions, 1, 0);
			tableLayoutPanelPatchingOptions.Controls.Add(labelPatchSummary, 0, 1);
			tableLayoutPanelPatchingOptions.Dock = DockStyle.Top;
			tableLayoutPanelPatchingOptions.Location = new Point(4, 19);
			tableLayoutPanelPatchingOptions.Margin = new Padding(0);
			tableLayoutPanelPatchingOptions.Name = "tableLayoutPanelPatchingOptions";
			tableLayoutPanelPatchingOptions.RowCount = 5;
			tableLayoutPanelPatchingOptions.RowStyles.Add(new RowStyle());
			tableLayoutPanelPatchingOptions.RowStyles.Add(new RowStyle());
			tableLayoutPanelPatchingOptions.RowStyles.Add(new RowStyle());
			tableLayoutPanelPatchingOptions.RowStyles.Add(new RowStyle());
			tableLayoutPanelPatchingOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanelPatchingOptions.Size = new Size(517, 201);
			tableLayoutPanelPatchingOptions.TabIndex = 5;
			// 
			// groupBoxElementOptions
			// 
			groupBoxElementOptions.AutoSize = true;
			groupBoxElementOptions.Controls.Add(checkBoxReverseElementOrder);
			groupBoxElementOptions.Controls.Add(radioButtonAllAvailablePatchPoints);
			groupBoxElementOptions.Controls.Add(radioButtonUnconnectedPatchPointsOnly);
			groupBoxElementOptions.Dock = DockStyle.Fill;
			groupBoxElementOptions.Location = new Point(4, 3);
			groupBoxElementOptions.Margin = new Padding(4, 3, 4, 3);
			groupBoxElementOptions.Name = "groupBoxElementOptions";
			groupBoxElementOptions.Padding = new Padding(4, 3, 4, 3);
			groupBoxElementOptions.Size = new Size(250, 116);
			groupBoxElementOptions.TabIndex = 1;
			groupBoxElementOptions.TabStop = false;
			groupBoxElementOptions.Paint += groupBoxes_Paint;
			// 
			// radioButtonAllAvailablePatchPoints
			// 
			radioButtonAllAvailablePatchPoints.AutoSize = true;
			radioButtonAllAvailablePatchPoints.Location = new Point(9, 48);
			radioButtonAllAvailablePatchPoints.Margin = new Padding(4, 3, 4, 3);
			radioButtonAllAvailablePatchPoints.Name = "radioButtonAllAvailablePatchPoints";
			radioButtonAllAvailablePatchPoints.Size = new Size(177, 19);
			radioButtonAllAvailablePatchPoints.TabIndex = 1;
			radioButtonAllAvailablePatchPoints.Text = "Use all available patch points";
			radioButtonAllAvailablePatchPoints.UseVisualStyleBackColor = true;
			radioButtonAllAvailablePatchPoints.CheckedChanged += radioButtonPatching_CheckedChanged;
			// 
			// radioButtonUnconnectedPatchPointsOnly
			// 
			radioButtonUnconnectedPatchPointsOnly.AutoSize = true;
			radioButtonUnconnectedPatchPointsOnly.Checked = true;
			radioButtonUnconnectedPatchPointsOnly.Location = new Point(9, 22);
			radioButtonUnconnectedPatchPointsOnly.Margin = new Padding(4, 3, 4, 3);
			radioButtonUnconnectedPatchPointsOnly.Name = "radioButtonUnconnectedPatchPointsOnly";
			radioButtonUnconnectedPatchPointsOnly.Size = new Size(212, 19);
			radioButtonUnconnectedPatchPointsOnly.TabIndex = 0;
			radioButtonUnconnectedPatchPointsOnly.TabStop = true;
			radioButtonUnconnectedPatchPointsOnly.Text = "Use unconnected patch points only";
			radioButtonUnconnectedPatchPointsOnly.UseVisualStyleBackColor = true;
			radioButtonUnconnectedPatchPointsOnly.CheckedChanged += radioButtonPatching_CheckedChanged;
			// 
			// buttonDoPatching
			// 
			buttonDoPatching.Anchor = AnchorStyles.None;
			buttonDoPatching.AutoSize = true;
			tableLayoutPanelPatchingOptions.SetColumnSpan(buttonDoPatching, 2);
			buttonDoPatching.Location = new Point(175, 172);
			buttonDoPatching.Margin = new Padding(4);
			buttonDoPatching.Name = "buttonDoPatching";
			buttonDoPatching.Size = new Size(167, 25);
			buttonDoPatching.TabIndex = 0;
			buttonDoPatching.Text = "Patch Elements";
			buttonDoPatching.UseVisualStyleBackColor = true;
			buttonDoPatching.Click += buttonDoPatching_Click;
			buttonDoPatching.MouseLeave += buttonBackground_MouseLeave;
			buttonDoPatching.MouseHover += buttonBackground_MouseHover;
			// 
			// labelPatchWarning
			// 
			labelPatchWarning.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			labelPatchWarning.AutoSize = true;
			tableLayoutPanelPatchingOptions.SetColumnSpan(labelPatchWarning, 2);
			labelPatchWarning.ForeColor = Color.FromArgb(80, 80, 255);
			labelPatchWarning.Location = new Point(104, 149);
			labelPatchWarning.Margin = new Padding(4);
			labelPatchWarning.Name = "labelPatchWarning";
			labelPatchWarning.Size = new Size(309, 15);
			labelPatchWarning.TabIndex = 4;
			labelPatchWarning.Text = "WARNING: too many elements, some will not be patched";
			// 
			// groupBoxOutputOptions
			// 
			groupBoxOutputOptions.AutoSize = true;
			groupBoxOutputOptions.Controls.Add(radioButtonAllOutputs);
			groupBoxOutputOptions.Controls.Add(radioButtonUnpatchedOutputsOnly);
			groupBoxOutputOptions.Controls.Add(checkBoxReverseOutputOrder);
			groupBoxOutputOptions.Dock = DockStyle.Fill;
			groupBoxOutputOptions.Location = new Point(262, 3);
			groupBoxOutputOptions.Margin = new Padding(4, 3, 4, 3);
			groupBoxOutputOptions.Name = "groupBoxOutputOptions";
			groupBoxOutputOptions.Padding = new Padding(4, 3, 4, 3);
			groupBoxOutputOptions.Size = new Size(251, 116);
			groupBoxOutputOptions.TabIndex = 2;
			groupBoxOutputOptions.TabStop = false;
			groupBoxOutputOptions.Paint += groupBoxes_Paint;
			// 
			// radioButtonAllOutputs
			// 
			radioButtonAllOutputs.AutoSize = true;
			radioButtonAllOutputs.Location = new Point(14, 48);
			radioButtonAllOutputs.Margin = new Padding(4, 3, 4, 3);
			radioButtonAllOutputs.Name = "radioButtonAllOutputs";
			radioButtonAllOutputs.Size = new Size(203, 19);
			radioButtonAllOutputs.TabIndex = 3;
			radioButtonAllOutputs.Text = "Use all selected controller outputs";
			radioButtonAllOutputs.UseVisualStyleBackColor = true;
			radioButtonAllOutputs.CheckedChanged += radioButtonPatching_CheckedChanged;
			// 
			// radioButtonUnpatchedOutputsOnly
			// 
			radioButtonUnpatchedOutputsOnly.AutoSize = true;
			radioButtonUnpatchedOutputsOnly.Checked = true;
			radioButtonUnpatchedOutputsOnly.Location = new Point(14, 22);
			radioButtonUnpatchedOutputsOnly.Margin = new Padding(4, 3, 4, 3);
			radioButtonUnpatchedOutputsOnly.Name = "radioButtonUnpatchedOutputsOnly";
			radioButtonUnpatchedOutputsOnly.Size = new Size(175, 19);
			radioButtonUnpatchedOutputsOnly.TabIndex = 2;
			radioButtonUnpatchedOutputsOnly.TabStop = true;
			radioButtonUnpatchedOutputsOnly.Text = "Only use unpatched outputs";
			radioButtonUnpatchedOutputsOnly.UseVisualStyleBackColor = true;
			radioButtonUnpatchedOutputsOnly.CheckedChanged += radioButtonPatching_CheckedChanged;
			// 
			// labelPatchSummary
			// 
			labelPatchSummary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			labelPatchSummary.AutoSize = true;
			tableLayoutPanelPatchingOptions.SetColumnSpan(labelPatchSummary, 2);
			labelPatchSummary.Location = new Point(92, 126);
			labelPatchSummary.Margin = new Padding(4);
			labelPatchSummary.Name = "labelPatchSummary";
			labelPatchSummary.Size = new Size(333, 15);
			labelPatchSummary.TabIndex = 3;
			labelPatchSummary.Text = "This will patch 9999 element points to 9999 controller outputs.";
			// 
			// toolTip1
			// 
			toolTip1.AutomaticDelay = 200;
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 200;
			toolTip1.ReshowDelay = 40;
			// 
			// tableLayoutPanelContainer
			// 
			tableLayoutPanelContainer.AutoSize = true;
			tableLayoutPanelContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanelContainer.ColumnCount = 2;
			tableLayoutPanelContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanelContainer.Controls.Add(groupBoxElements, 0, 0);
			tableLayoutPanelContainer.Controls.Add(groupBoxControllers, 1, 0);
			tableLayoutPanelContainer.Controls.Add(groupBoxPatching, 0, 1);
			tableLayoutPanelContainer.Dock = DockStyle.Top;
			tableLayoutPanelContainer.Location = new Point(0, 0);
			tableLayoutPanelContainer.Margin = new Padding(4, 3, 4, 3);
			tableLayoutPanelContainer.Name = "tableLayoutPanelContainer";
			tableLayoutPanelContainer.RowCount = 2;
			tableLayoutPanelContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanelContainer.RowStyles.Add(new RowStyle());
			tableLayoutPanelContainer.Size = new Size(533, 469);
			tableLayoutPanelContainer.TabIndex = 34;
			// 
			// SetupPatchingSimple
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoScroll = true;
			AutoSize = true;
			Controls.Add(tableLayoutPanelContainer);
			DoubleBuffered = true;
			Margin = new Padding(4, 3, 4, 3);
			Name = "SetupPatchingSimple";
			Size = new Size(533, 493);
			Load += SetupPatchingSimple_Load;
			groupBoxElements.ResumeLayout(false);
			groupBoxElements.PerformLayout();
			tableLayoutPanel4.ResumeLayout(false);
			tableLayoutPanel4.PerformLayout();
			groupBoxControllers.ResumeLayout(false);
			groupBoxControllers.PerformLayout();
			tableLayoutPanel3.ResumeLayout(false);
			tableLayoutPanel3.PerformLayout();
			groupBoxPatching.ResumeLayout(false);
			groupBoxPatching.PerformLayout();
			tableLayoutPanelPatchingOptions.ResumeLayout(false);
			tableLayoutPanelPatchingOptions.PerformLayout();
			groupBoxElementOptions.ResumeLayout(false);
			groupBoxElementOptions.PerformLayout();
			groupBoxOutputOptions.ResumeLayout(false);
			groupBoxOutputOptions.PerformLayout();
			tableLayoutPanelContainer.ResumeLayout(false);
			tableLayoutPanelContainer.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private GroupBox groupBoxElements;
		private Label labelUnconnectedPatchPointCount;
		private Label labelConnectedPatchPointCount;
		private Label labelPatchPointCount;
		private Label label7;
		private Label label6;
		private Label label5;
		private GroupBox groupBoxControllers;
		private Label labelUnpatchedOutputCount;
		private Label labelPatchedOutputCount;
		private Label labelOutputCount;
		private Label labelControllerCount;
		private Label label15;
		private Label label16;
		private Label label20;
		private Label label21;
		private GroupBox groupBoxPatching;
		private Label labelFilterCount;
		private Label labelElementCount;
		private Label labelGroupCount;
		private Label labelItemCount;
		private Label label4;
		private Label label3;
		private Label label2;
		private Label label1;
		private Button buttonDoPatching;
		private GroupBox groupBoxOutputOptions;
		private RadioButton radioButtonAllOutputs;
		private RadioButton radioButtonUnpatchedOutputsOnly;
		private GroupBox groupBoxElementOptions;
		private RadioButton radioButtonAllAvailablePatchPoints;
		private RadioButton radioButtonUnconnectedPatchPointsOnly;
		private Label labelPatchWarning;
		private Label labelPatchSummary;
		private ToolTip toolTip1;
		private Button buttonUnpatchElements;
		private Button buttonUnpatchControllers;
		private CheckBox checkBoxReverseOutputOrder;
		private Label labelLastOutput;
		private Label labelFirstOutput;
		private Label label9;
		private Label label8;
		private TableLayoutPanel tableLayoutPanelContainer;
		private TableLayoutPanel tableLayoutPanelPatchingOptions;
		private CheckBox checkBoxReverseElementOrder;
		private TableLayoutPanel tableLayoutPanel4;
		private TableLayoutPanel tableLayoutPanel3;
	}
}
