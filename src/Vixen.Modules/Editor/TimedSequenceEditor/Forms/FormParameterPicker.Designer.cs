using System.Windows.Forms;

namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class FormParameterPicker
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
			tableLayoutPanel = new TableLayoutPanel();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			previousButton = new Button();
			nextButton = new Button();
			title = new Label();
			SuspendLayout();

			///
			/// Previous Button
			/// 
			previousButton.Text = "\u00AB";
			previousButton.Font = new Font("Arial", 20);
			previousButton.AutoSize = true;
			previousButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			previousButton.Dock = DockStyle.None;
			previousButton.Click += PreviousButton_Click;
			previousButton.TabIndex = 1;
			previousButton.Anchor = AnchorStyles.None;

			///
			/// Next Button
			/// 
			nextButton.Text = "\u00BB";
			nextButton.Font = new Font("Arial", 20);
			nextButton.AutoSize = true;
			nextButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			nextButton.Dock = DockStyle.None;
			nextButton.Click += NextButton_Click;
			nextButton.Anchor = AnchorStyles.None;

			///
			/// Title
			/// 
			title.AutoSize = true;
			title.Dock = DockStyle.Fill;
			title.Anchor = AnchorStyles.Top;

			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(1, 1);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(914, 1067);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(828, 165);
			this.flowLayoutPanel1.TabIndex = 2;
			this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);

			tableLayoutPanel.AutoSize = true;
			tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			tableLayoutPanel.Location = new System.Drawing.Point(1, 1);
			tableLayoutPanel.ColumnCount = 3;
			tableLayoutPanel.RowCount = 2;
			tableLayoutPanel.Controls.Add(previousButton, 0, 1);
			tableLayoutPanel.Controls.Add(flowLayoutPanel1, 1, 1);
			tableLayoutPanel.Controls.Add(title, 1, 0);
			tableLayoutPanel.Controls.Add(nextButton, 2, 1);

			// 
			// FormParameterPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.ClientSize = new System.Drawing.Size(830, 167);
			this.Controls.Add(this.tableLayoutPanel);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "FormParameterPicker";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Text = "FormParameterPicker";
			this.Activated += FormParameterPicker_Activated;
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private Button previousButton;
		private Button nextButton;
		private Label title;
	}
}