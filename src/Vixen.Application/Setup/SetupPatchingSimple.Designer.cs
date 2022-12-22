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
			if (disposing && (components != null)) {
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
			this.components = new System.ComponentModel.Container();
			this.groupBoxElements = new System.Windows.Forms.GroupBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.labelFilterCount = new System.Windows.Forms.Label();
			this.labelElementCount = new System.Windows.Forms.Label();
			this.labelGroupCount = new System.Windows.Forms.Label();
			this.labelItemCount = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.labelUnconnectedPatchPointCount = new System.Windows.Forms.Label();
			this.labelConnectedPatchPointCount = new System.Windows.Forms.Label();
			this.labelPatchPointCount = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonUnpatchElements = new System.Windows.Forms.Button();
			this.checkBoxReverseElementOrder = new System.Windows.Forms.CheckBox();
			this.groupBoxControllers = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelLastOutput = new System.Windows.Forms.Label();
			this.labelFirstOutput = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.labelUnpatchedOutputCount = new System.Windows.Forms.Label();
			this.labelPatchedOutputCount = new System.Windows.Forms.Label();
			this.labelOutputCount = new System.Windows.Forms.Label();
			this.labelControllerCount = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.buttonUnpatchControllers = new System.Windows.Forms.Button();
			this.checkBoxReverseOutputOrder = new System.Windows.Forms.CheckBox();
			this.groupBoxPatching = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBoxElementOptions = new System.Windows.Forms.GroupBox();
			this.radioButtonAllAvailablePatchPoints = new System.Windows.Forms.RadioButton();
			this.radioButtonUnconnectedPatchPointsOnly = new System.Windows.Forms.RadioButton();
			this.groupBoxOutputOptions = new System.Windows.Forms.GroupBox();
			this.radioButtonAllOutputs = new System.Windows.Forms.RadioButton();
			this.radioButtonUnpatchedOutputsOnly = new System.Windows.Forms.RadioButton();
			this.labelPatchWarning = new System.Windows.Forms.Label();
			this.labelPatchSummary = new System.Windows.Forms.Label();
			this.buttonDoPatching = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBoxElements.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBoxControllers.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBoxPatching.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.groupBoxElementOptions.SuspendLayout();
			this.groupBoxOutputOptions.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxElements
			// 
			this.groupBoxElements.AutoSize = true;
			this.groupBoxElements.Controls.Add(this.panel2);
			this.groupBoxElements.Controls.Add(this.buttonUnpatchElements);
			this.groupBoxElements.Location = new System.Drawing.Point(3, 3);
			this.groupBoxElements.Name = "groupBoxElements";
			this.groupBoxElements.Size = new System.Drawing.Size(220, 257);
			this.groupBoxElements.TabIndex = 0;
			this.groupBoxElements.TabStop = false;
			this.groupBoxElements.Text = "Selected Elements";
			this.groupBoxElements.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.Controls.Add(this.labelFilterCount);
			this.panel2.Controls.Add(this.labelElementCount);
			this.panel2.Controls.Add(this.labelGroupCount);
			this.panel2.Controls.Add(this.labelItemCount);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.labelUnconnectedPatchPointCount);
			this.panel2.Controls.Add(this.labelConnectedPatchPointCount);
			this.panel2.Controls.Add(this.labelPatchPointCount);
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.label6);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Location = new System.Drawing.Point(6, 18);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(208, 189);
			this.panel2.TabIndex = 23;
			// 
			// labelFilterCount
			// 
			this.labelFilterCount.AutoSize = true;
			this.labelFilterCount.Location = new System.Drawing.Point(87, 161);
			this.labelFilterCount.Name = "labelFilterCount";
			this.labelFilterCount.Size = new System.Drawing.Size(13, 13);
			this.labelFilterCount.TabIndex = 21;
			this.labelFilterCount.Text = "0";
			// 
			// labelElementCount
			// 
			this.labelElementCount.AutoSize = true;
			this.labelElementCount.Location = new System.Drawing.Point(87, 139);
			this.labelElementCount.Name = "labelElementCount";
			this.labelElementCount.Size = new System.Drawing.Size(13, 13);
			this.labelElementCount.TabIndex = 20;
			this.labelElementCount.Text = "0";
			// 
			// labelGroupCount
			// 
			this.labelGroupCount.AutoSize = true;
			this.labelGroupCount.Location = new System.Drawing.Point(87, 117);
			this.labelGroupCount.Name = "labelGroupCount";
			this.labelGroupCount.Size = new System.Drawing.Size(13, 13);
			this.labelGroupCount.TabIndex = 19;
			this.labelGroupCount.Text = "0";
			// 
			// labelItemCount
			// 
			this.labelItemCount.AutoSize = true;
			this.labelItemCount.Location = new System.Drawing.Point(87, 95);
			this.labelItemCount.Name = "labelItemCount";
			this.labelItemCount.Size = new System.Drawing.Size(13, 13);
			this.labelItemCount.TabIndex = 18;
			this.labelItemCount.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 161);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Filters:";
			this.toolTip1.SetToolTip(this.label4, "The number of filters found in the patching connections from the selected element" +
        "s.");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 139);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Elements:";
			this.toolTip1.SetToolTip(this.label3, "The number of elements found in (or descending from) the selected elements.");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 117);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "Groups:";
			this.toolTip1.SetToolTip(this.label2, "The number of groups found from the selected elements.");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 14;
			this.label1.Text = "Items:";
			this.toolTip1.SetToolTip(this.label1, "The number of items selected in the element view.");
			// 
			// labelUnconnectedPatchPointCount
			// 
			this.labelUnconnectedPatchPointCount.AutoSize = true;
			this.labelUnconnectedPatchPointCount.Location = new System.Drawing.Point(131, 57);
			this.labelUnconnectedPatchPointCount.Name = "labelUnconnectedPatchPointCount";
			this.labelUnconnectedPatchPointCount.Size = new System.Drawing.Size(13, 13);
			this.labelUnconnectedPatchPointCount.TabIndex = 13;
			this.labelUnconnectedPatchPointCount.Text = "0";
			// 
			// labelConnectedPatchPointCount
			// 
			this.labelConnectedPatchPointCount.AutoSize = true;
			this.labelConnectedPatchPointCount.Location = new System.Drawing.Point(131, 37);
			this.labelConnectedPatchPointCount.Name = "labelConnectedPatchPointCount";
			this.labelConnectedPatchPointCount.Size = new System.Drawing.Size(13, 13);
			this.labelConnectedPatchPointCount.TabIndex = 12;
			this.labelConnectedPatchPointCount.Text = "0";
			// 
			// labelPatchPointCount
			// 
			this.labelPatchPointCount.AutoSize = true;
			this.labelPatchPointCount.Location = new System.Drawing.Point(131, 17);
			this.labelPatchPointCount.Name = "labelPatchPointCount";
			this.labelPatchPointCount.Size = new System.Drawing.Size(13, 13);
			this.labelPatchPointCount.TabIndex = 11;
			this.labelPatchPointCount.Text = "0";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 57);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(75, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Unconnected:";
			this.toolTip1.SetToolTip(this.label7, "The number of Patch Points connected to the selected elements that have nothing e" +
        "lse connected.");
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(62, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Connected:";
			this.toolTip1.SetToolTip(this.label6, "The number of Patch Points connected to the selected elements that are being patc" +
        "hed to controller outputs.");
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Total Patch Points:";
			this.toolTip1.SetToolTip(this.label5, "The total number of Patch Points connected to the selected elements.  Patch Point" +
        "s are the outputs from any element or filter, but before it gets to the controll" +
        "ers.");
			// 
			// buttonUnpatchElements
			// 
			this.buttonUnpatchElements.AutoSize = true;
			this.buttonUnpatchElements.Location = new System.Drawing.Point(49, 213);
			this.buttonUnpatchElements.Name = "buttonUnpatchElements";
			this.buttonUnpatchElements.Size = new System.Drawing.Size(120, 25);
			this.buttonUnpatchElements.TabIndex = 22;
			this.buttonUnpatchElements.Text = "Unpatch Elements";
			this.buttonUnpatchElements.UseVisualStyleBackColor = true;
			this.buttonUnpatchElements.Click += new System.EventHandler(this.buttonUnpatchElements_Click);
			this.buttonUnpatchElements.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonUnpatchElements.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// checkBoxReverseElementOrder
			// 
			this.checkBoxReverseElementOrder.AutoSize = true;
			this.checkBoxReverseElementOrder.Location = new System.Drawing.Point(9, 65);
			this.checkBoxReverseElementOrder.Name = "checkBoxReverseElementOrder";
			this.checkBoxReverseElementOrder.Size = new System.Drawing.Size(136, 17);
			this.checkBoxReverseElementOrder.TabIndex = 23;
			this.checkBoxReverseElementOrder.Text = "Reverse Element Order";
			this.toolTip1.SetToolTip(this.checkBoxReverseElementOrder, "The order in which Elements will be patched to controller outputs. This does not " +
        "effect the order in which color channels are patched to controller outputs.");
			this.checkBoxReverseElementOrder.UseVisualStyleBackColor = true;
			this.checkBoxReverseElementOrder.CheckedChanged += new System.EventHandler(this.checkBoxReverseElementOrder_CheckedChanged);
			// 
			// groupBoxControllers
			// 
			this.groupBoxControllers.AutoSize = true;
			this.groupBoxControllers.Controls.Add(this.panel1);
			this.groupBoxControllers.Controls.Add(this.buttonUnpatchControllers);
			this.groupBoxControllers.Location = new System.Drawing.Point(229, 3);
			this.groupBoxControllers.Name = "groupBoxControllers";
			this.groupBoxControllers.Size = new System.Drawing.Size(214, 257);
			this.groupBoxControllers.TabIndex = 1;
			this.groupBoxControllers.TabStop = false;
			this.groupBoxControllers.Text = "Selected Controllers";
			this.groupBoxControllers.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.labelLastOutput);
			this.panel1.Controls.Add(this.labelFirstOutput);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.labelUnpatchedOutputCount);
			this.panel1.Controls.Add(this.labelPatchedOutputCount);
			this.panel1.Controls.Add(this.labelOutputCount);
			this.panel1.Controls.Add(this.labelControllerCount);
			this.panel1.Controls.Add(this.label15);
			this.panel1.Controls.Add(this.label16);
			this.panel1.Controls.Add(this.label20);
			this.panel1.Controls.Add(this.label21);
			this.panel1.Location = new System.Drawing.Point(7, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(201, 189);
			this.panel1.TabIndex = 34;
			// 
			// labelLastOutput
			// 
			this.labelLastOutput.AutoSize = true;
			this.labelLastOutput.Location = new System.Drawing.Point(73, 161);
			this.labelLastOutput.Name = "labelLastOutput";
			this.labelLastOutput.Size = new System.Drawing.Size(67, 13);
			this.labelLastOutput.TabIndex = 33;
			this.labelLastOutput.Text = "Controller #0";
			// 
			// labelFirstOutput
			// 
			this.labelFirstOutput.AutoSize = true;
			this.labelFirstOutput.Location = new System.Drawing.Point(73, 139);
			this.labelFirstOutput.Name = "labelFirstOutput";
			this.labelFirstOutput.Size = new System.Drawing.Size(67, 13);
			this.labelFirstOutput.TabIndex = 32;
			this.labelFirstOutput.Text = "Controller #0";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 161);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(63, 13);
			this.label9.TabIndex = 31;
			this.label9.Text = "Last output:";
			this.toolTip1.SetToolTip(this.label9, "The last output in the list of selected outputs (as will be used for patching).");
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 139);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(62, 13);
			this.label8.TabIndex = 30;
			this.label8.Text = "First output:";
			this.toolTip1.SetToolTip(this.label8, "The first output in the list of selected outputs (as will be used for patching).");
			// 
			// labelUnpatchedOutputCount
			// 
			this.labelUnpatchedOutputCount.AutoSize = true;
			this.labelUnpatchedOutputCount.Location = new System.Drawing.Point(87, 57);
			this.labelUnpatchedOutputCount.Name = "labelUnpatchedOutputCount";
			this.labelUnpatchedOutputCount.Size = new System.Drawing.Size(13, 13);
			this.labelUnpatchedOutputCount.TabIndex = 27;
			this.labelUnpatchedOutputCount.Text = "0";
			// 
			// labelPatchedOutputCount
			// 
			this.labelPatchedOutputCount.AutoSize = true;
			this.labelPatchedOutputCount.Location = new System.Drawing.Point(87, 37);
			this.labelPatchedOutputCount.Name = "labelPatchedOutputCount";
			this.labelPatchedOutputCount.Size = new System.Drawing.Size(13, 13);
			this.labelPatchedOutputCount.TabIndex = 26;
			this.labelPatchedOutputCount.Text = "0";
			// 
			// labelOutputCount
			// 
			this.labelOutputCount.AutoSize = true;
			this.labelOutputCount.Location = new System.Drawing.Point(87, 17);
			this.labelOutputCount.Name = "labelOutputCount";
			this.labelOutputCount.Size = new System.Drawing.Size(13, 13);
			this.labelOutputCount.TabIndex = 22;
			this.labelOutputCount.Text = "0";
			// 
			// labelControllerCount
			// 
			this.labelControllerCount.AutoSize = true;
			this.labelControllerCount.Location = new System.Drawing.Point(87, 95);
			this.labelControllerCount.Name = "labelControllerCount";
			this.labelControllerCount.Size = new System.Drawing.Size(13, 13);
			this.labelControllerCount.TabIndex = 21;
			this.labelControllerCount.Text = "0";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(8, 57);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(63, 13);
			this.label15.TabIndex = 20;
			this.label15.Text = "Unpatched:";
			this.toolTip1.SetToolTip(this.label15, "The number of controller outputs selected that are not connected to anything.");
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(8, 37);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(50, 13);
			this.label16.TabIndex = 19;
			this.label16.Text = "Patched:";
			this.toolTip1.SetToolTip(this.label16, "The number of controller outputs selected that are already connected to something" +
        ".");
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(8, 17);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(47, 13);
			this.label20.TabIndex = 15;
			this.label20.Text = "Outputs:";
			this.toolTip1.SetToolTip(this.label20, "The total number of controller outputs selected.");
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(8, 95);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(59, 13);
			this.label21.TabIndex = 14;
			this.label21.Text = "Controllers:";
			this.toolTip1.SetToolTip(this.label21, "The number of controllers (or part thereof) selected.");
			// 
			// buttonUnpatchControllers
			// 
			this.buttonUnpatchControllers.AutoSize = true;
			this.buttonUnpatchControllers.Location = new System.Drawing.Point(54, 213);
			this.buttonUnpatchControllers.Name = "buttonUnpatchControllers";
			this.buttonUnpatchControllers.Size = new System.Drawing.Size(120, 25);
			this.buttonUnpatchControllers.TabIndex = 28;
			this.buttonUnpatchControllers.Text = "Unpatch Controllers";
			this.buttonUnpatchControllers.UseVisualStyleBackColor = true;
			this.buttonUnpatchControllers.Click += new System.EventHandler(this.buttonUnpatchControllers_Click);
			this.buttonUnpatchControllers.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonUnpatchControllers.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// checkBoxReverseOutputOrder
			// 
			this.checkBoxReverseOutputOrder.AutoSize = true;
			this.checkBoxReverseOutputOrder.Location = new System.Drawing.Point(12, 65);
			this.checkBoxReverseOutputOrder.Name = "checkBoxReverseOutputOrder";
			this.checkBoxReverseOutputOrder.Size = new System.Drawing.Size(186, 17);
			this.checkBoxReverseOutputOrder.TabIndex = 29;
			this.checkBoxReverseOutputOrder.Text = "Reverse order of selected outputs";
			this.checkBoxReverseOutputOrder.UseVisualStyleBackColor = true;
			this.checkBoxReverseOutputOrder.CheckedChanged += new System.EventHandler(this.checkBoxReverseOutputOrder_CheckedChanged);
			// 
			// groupBoxPatching
			// 
			this.groupBoxPatching.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxPatching.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.groupBoxPatching, 2);
			this.groupBoxPatching.Controls.Add(this.tableLayoutPanel2);
			this.groupBoxPatching.Controls.Add(this.labelPatchWarning);
			this.groupBoxPatching.Controls.Add(this.labelPatchSummary);
			this.groupBoxPatching.Controls.Add(this.buttonDoPatching);
			this.groupBoxPatching.Location = new System.Drawing.Point(3, 266);
			this.groupBoxPatching.Name = "groupBoxPatching";
			this.groupBoxPatching.Size = new System.Drawing.Size(445, 231);
			this.groupBoxPatching.TabIndex = 2;
			this.groupBoxPatching.TabStop = false;
			this.groupBoxPatching.Text = "Patching Options";
			this.groupBoxPatching.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.groupBoxElementOptions, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.groupBoxOutputOptions, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(439, 107);
			this.tableLayoutPanel2.TabIndex = 5;
			// 
			// groupBoxElementOptions
			// 
			this.groupBoxElementOptions.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.groupBoxElementOptions.AutoSize = true;
			this.groupBoxElementOptions.Controls.Add(this.checkBoxReverseElementOrder);
			this.groupBoxElementOptions.Controls.Add(this.radioButtonAllAvailablePatchPoints);
			this.groupBoxElementOptions.Controls.Add(this.radioButtonUnconnectedPatchPointsOnly);
			this.groupBoxElementOptions.Location = new System.Drawing.Point(6, 3);
			this.groupBoxElementOptions.Name = "groupBoxElementOptions";
			this.groupBoxElementOptions.Size = new System.Drawing.Size(207, 101);
			this.groupBoxElementOptions.TabIndex = 1;
			this.groupBoxElementOptions.TabStop = false;
			this.groupBoxElementOptions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// radioButtonAllAvailablePatchPoints
			// 
			this.radioButtonAllAvailablePatchPoints.AutoSize = true;
			this.radioButtonAllAvailablePatchPoints.Location = new System.Drawing.Point(8, 42);
			this.radioButtonAllAvailablePatchPoints.Name = "radioButtonAllAvailablePatchPoints";
			this.radioButtonAllAvailablePatchPoints.Size = new System.Drawing.Size(163, 17);
			this.radioButtonAllAvailablePatchPoints.TabIndex = 1;
			this.radioButtonAllAvailablePatchPoints.Text = "Use all available patch points";
			this.radioButtonAllAvailablePatchPoints.UseVisualStyleBackColor = true;
			this.radioButtonAllAvailablePatchPoints.CheckedChanged += new System.EventHandler(this.radioButtonPatching_CheckedChanged);
			// 
			// radioButtonUnconnectedPatchPointsOnly
			// 
			this.radioButtonUnconnectedPatchPointsOnly.AutoSize = true;
			this.radioButtonUnconnectedPatchPointsOnly.Checked = true;
			this.radioButtonUnconnectedPatchPointsOnly.Location = new System.Drawing.Point(8, 19);
			this.radioButtonUnconnectedPatchPointsOnly.Name = "radioButtonUnconnectedPatchPointsOnly";
			this.radioButtonUnconnectedPatchPointsOnly.Size = new System.Drawing.Size(193, 17);
			this.radioButtonUnconnectedPatchPointsOnly.TabIndex = 0;
			this.radioButtonUnconnectedPatchPointsOnly.TabStop = true;
			this.radioButtonUnconnectedPatchPointsOnly.Text = "Use unconnected patch points only";
			this.radioButtonUnconnectedPatchPointsOnly.UseVisualStyleBackColor = true;
			this.radioButtonUnconnectedPatchPointsOnly.CheckedChanged += new System.EventHandler(this.radioButtonPatching_CheckedChanged);
			// 
			// groupBoxOutputOptions
			// 
			this.groupBoxOutputOptions.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.groupBoxOutputOptions.AutoSize = true;
			this.groupBoxOutputOptions.Controls.Add(this.radioButtonAllOutputs);
			this.groupBoxOutputOptions.Controls.Add(this.radioButtonUnpatchedOutputsOnly);
			this.groupBoxOutputOptions.Controls.Add(this.checkBoxReverseOutputOrder);
			this.groupBoxOutputOptions.Location = new System.Drawing.Point(227, 3);
			this.groupBoxOutputOptions.Name = "groupBoxOutputOptions";
			this.groupBoxOutputOptions.Size = new System.Drawing.Size(204, 101);
			this.groupBoxOutputOptions.TabIndex = 2;
			this.groupBoxOutputOptions.TabStop = false;
			this.groupBoxOutputOptions.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// radioButtonAllOutputs
			// 
			this.radioButtonAllOutputs.AutoSize = true;
			this.radioButtonAllOutputs.Location = new System.Drawing.Point(12, 42);
			this.radioButtonAllOutputs.Name = "radioButtonAllOutputs";
			this.radioButtonAllOutputs.Size = new System.Drawing.Size(184, 17);
			this.radioButtonAllOutputs.TabIndex = 3;
			this.radioButtonAllOutputs.Text = "Use all selected controller outputs";
			this.radioButtonAllOutputs.UseVisualStyleBackColor = true;
			this.radioButtonAllOutputs.CheckedChanged += new System.EventHandler(this.radioButtonPatching_CheckedChanged);
			// 
			// radioButtonUnpatchedOutputsOnly
			// 
			this.radioButtonUnpatchedOutputsOnly.AutoSize = true;
			this.radioButtonUnpatchedOutputsOnly.Checked = true;
			this.radioButtonUnpatchedOutputsOnly.Location = new System.Drawing.Point(12, 19);
			this.radioButtonUnpatchedOutputsOnly.Name = "radioButtonUnpatchedOutputsOnly";
			this.radioButtonUnpatchedOutputsOnly.Size = new System.Drawing.Size(158, 17);
			this.radioButtonUnpatchedOutputsOnly.TabIndex = 2;
			this.radioButtonUnpatchedOutputsOnly.TabStop = true;
			this.radioButtonUnpatchedOutputsOnly.Text = "Only use unpatched outputs";
			this.radioButtonUnpatchedOutputsOnly.UseVisualStyleBackColor = true;
			this.radioButtonUnpatchedOutputsOnly.CheckedChanged += new System.EventHandler(this.radioButtonPatching_CheckedChanged);
			// 
			// labelPatchWarning
			// 
			this.labelPatchWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.labelPatchWarning.AutoSize = true;
			this.labelPatchWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
			this.labelPatchWarning.Location = new System.Drawing.Point(86, 149);
			this.labelPatchWarning.Name = "labelPatchWarning";
			this.labelPatchWarning.Size = new System.Drawing.Size(277, 13);
			this.labelPatchWarning.TabIndex = 4;
			this.labelPatchWarning.Text = "WARNING: too many elements, some will not be patched";
			// 
			// labelPatchSummary
			// 
			this.labelPatchSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.labelPatchSummary.AutoSize = true;
			this.labelPatchSummary.Location = new System.Drawing.Point(76, 129);
			this.labelPatchSummary.Name = "labelPatchSummary";
			this.labelPatchSummary.Size = new System.Drawing.Size(298, 13);
			this.labelPatchSummary.TabIndex = 3;
			this.labelPatchSummary.Text = "This will patch 9999 element points to 9999 controller outputs.";
			// 
			// buttonDoPatching
			// 
			this.buttonDoPatching.AutoSize = true;
			this.buttonDoPatching.Location = new System.Drawing.Point(150, 180);
			this.buttonDoPatching.Name = "buttonDoPatching";
			this.buttonDoPatching.Size = new System.Drawing.Size(143, 32);
			this.buttonDoPatching.TabIndex = 0;
			this.buttonDoPatching.Text = "Patch Elements";
			this.buttonDoPatching.UseVisualStyleBackColor = true;
			this.buttonDoPatching.Click += new System.EventHandler(this.buttonDoPatching_Click);
			this.buttonDoPatching.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonDoPatching.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 200;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 200;
			this.toolTip1.ReshowDelay = 40;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.groupBoxElements, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBoxControllers, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBoxPatching, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(451, 500);
			this.tableLayoutPanel1.TabIndex = 34;
			// 
			// SetupPatchingSimple
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.DoubleBuffered = true;
			this.Name = "SetupPatchingSimple";
			this.Size = new System.Drawing.Size(451, 507);
			this.Load += new System.EventHandler(this.SetupPatchingSimple_Load);
			this.groupBoxElements.ResumeLayout(false);
			this.groupBoxElements.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupBoxControllers.ResumeLayout(false);
			this.groupBoxControllers.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBoxPatching.ResumeLayout(false);
			this.groupBoxPatching.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.groupBoxElementOptions.ResumeLayout(false);
			this.groupBoxElementOptions.PerformLayout();
			this.groupBoxOutputOptions.ResumeLayout(false);
			this.groupBoxOutputOptions.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private TableLayoutPanel tableLayoutPanel1;
		private TableLayoutPanel tableLayoutPanel2;
		private CheckBox checkBoxReverseElementOrder;
		private Panel panel1;
		private Panel panel2;
	}
}
