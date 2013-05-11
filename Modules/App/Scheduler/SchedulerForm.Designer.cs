namespace VixenModules.App.Scheduler {
	partial class SchedulerForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchedulerForm));
			this.toolStripView = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonToday = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonDayView = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonWeekView = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAgendaView = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkBoxEnableSchedule = new System.Windows.Forms.CheckBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemAddEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.scheduleDayView = new VixenModules.App.Scheduler.ScheduleDayView();
			this.toolStripView.SuspendLayout();
			this.panel1.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripView
			// 
			this.toolStripView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonToday,
            this.toolStripSeparator1,
            this.toolStripButtonDayView,
            this.toolStripButtonWeekView,
            this.toolStripButtonAgendaView,
            this.toolStripSeparator2});
			this.toolStripView.Location = new System.Drawing.Point(0, 0);
			this.toolStripView.Name = "toolStripView";
			this.toolStripView.Size = new System.Drawing.Size(530, 25);
			this.toolStripView.TabIndex = 3;
			this.toolStripView.Text = "View";
			this.toolStripView.Visible = false;
			// 
			// toolStripButtonToday
			// 
			this.toolStripButtonToday.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonToday.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonToday.Image")));
			this.toolStripButtonToday.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonToday.Name = "toolStripButtonToday";
			this.toolStripButtonToday.Size = new System.Drawing.Size(44, 22);
			this.toolStripButtonToday.Text = "Today";
			this.toolStripButtonToday.ToolTipText = "Go to today";
			this.toolStripButtonToday.Click += new System.EventHandler(this.toolStripButtonToday_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonDayView
			// 
			this.toolStripButtonDayView.Checked = true;
			this.toolStripButtonDayView.CheckOnClick = true;
			this.toolStripButtonDayView.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonDayView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDayView.Image")));
			this.toolStripButtonDayView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonDayView.Name = "toolStripButtonDayView";
			this.toolStripButtonDayView.Size = new System.Drawing.Size(74, 22);
			this.toolStripButtonDayView.Text = "Day view";
			this.toolStripButtonDayView.Click += new System.EventHandler(this.toolStripButtonDayView_Click);
			// 
			// toolStripButtonWeekView
			// 
			this.toolStripButtonWeekView.CheckOnClick = true;
			this.toolStripButtonWeekView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonWeekView.Image")));
			this.toolStripButtonWeekView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonWeekView.Name = "toolStripButtonWeekView";
			this.toolStripButtonWeekView.Size = new System.Drawing.Size(83, 22);
			this.toolStripButtonWeekView.Text = "Week view";
			this.toolStripButtonWeekView.Click += new System.EventHandler(this.toolStripButtonWeekView_Click);
			// 
			// toolStripButtonAgendaView
			// 
			this.toolStripButtonAgendaView.CheckOnClick = true;
			this.toolStripButtonAgendaView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAgendaView.Image")));
			this.toolStripButtonAgendaView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonAgendaView.Name = "toolStripButtonAgendaView";
			this.toolStripButtonAgendaView.Size = new System.Drawing.Size(95, 22);
			this.toolStripButtonAgendaView.Text = "Agenda view";
			this.toolStripButtonAgendaView.Click += new System.EventHandler(this.toolStripButtonAgendaView_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.checkBoxEnableSchedule);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 405);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(530, 45);
			this.panel1.TabIndex = 10;
			// 
			// checkBoxEnableSchedule
			// 
			this.checkBoxEnableSchedule.AutoSize = true;
			this.checkBoxEnableSchedule.Location = new System.Drawing.Point(12, 14);
			this.checkBoxEnableSchedule.Name = "checkBoxEnableSchedule";
			this.checkBoxEnableSchedule.Size = new System.Drawing.Size(123, 17);
			this.checkBoxEnableSchedule.TabIndex = 2;
			this.checkBoxEnableSchedule.Text = "Enable the schedule";
			this.checkBoxEnableSchedule.UseVisualStyleBackColor = true;
			this.checkBoxEnableSchedule.CheckedChanged += new System.EventHandler(this.checkBoxEnableSchedule_CheckedChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(443, 10);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(362, 10);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddEdit,
            this.toolStripMenuItemRemove});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(118, 48);
			// 
			// toolStripMenuItemAddEdit
			// 
			this.toolStripMenuItemAddEdit.Name = "toolStripMenuItemAddEdit";
			this.toolStripMenuItemAddEdit.Size = new System.Drawing.Size(117, 22);
			this.toolStripMenuItemAddEdit.Text = "Edit";
			// 
			// toolStripMenuItemRemove
			// 
			this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
			this.toolStripMenuItemRemove.Size = new System.Drawing.Size(117, 22);
			this.toolStripMenuItemRemove.Text = "Remove";
			// 
			// scheduleDayView
			// 
			this.scheduleDayView.AutoScroll = true;
			this.scheduleDayView.CurrentDate = new System.DateTime(2013, 1, 31, 0, 0, 0, 0);
			this.scheduleDayView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scheduleDayView.Location = new System.Drawing.Point(0, 0);
			this.scheduleDayView.Name = "scheduleDayView";
			this.scheduleDayView.Size = new System.Drawing.Size(530, 405);
			this.scheduleDayView.TabIndex = 11;
			this.scheduleDayView.TimeDoubleClick += new System.EventHandler<VixenModules.App.Scheduler.ScheduleEventArgs>(this.scheduleDayView_TimeDoubleClick);
			this.scheduleDayView.ItemDoubleClick += new System.EventHandler<VixenModules.App.Scheduler.ScheduleItemArgs>(this.scheduleDayView_ItemDoubleClick);
			this.scheduleDayView.LeftButtonClick += new System.EventHandler(this.scheduleDayView_LeftButtonClick);
			this.scheduleDayView.RightButtonClick += new System.EventHandler(this.scheduleDayView_RightButtonClick);
			// 
			// SchedulerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(530, 450);
			this.Controls.Add(this.scheduleDayView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStripView);
			this.KeyPreview = true;
			this.Name = "SchedulerForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Show Scheduler";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SchedulerForm_FormClosing);
			this.Load += new System.EventHandler(this.SchedulerForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SchedulerForm_KeyDown);
			this.toolStripView.ResumeLayout(false);
			this.toolStripView.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStripView;
		private System.Windows.Forms.ToolStripButton toolStripButtonToday;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButtonDayView;
		private System.Windows.Forms.ToolStripButton toolStripButtonWeekView;
		private System.Windows.Forms.ToolStripButton toolStripButtonAgendaView;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBoxEnableSchedule;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddEdit;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemove;
		private System.Windows.Forms.ToolTip toolTip;
		private ScheduleDayView scheduleDayView;
	}
}