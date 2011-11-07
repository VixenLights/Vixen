namespace Scheduler {
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
			this.toolStripButtonMonthView = new System.Windows.Forms.ToolStripButton();
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
			this.scheduleDay1 = new Scheduler.ScheduleDay();
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
            this.toolStripButtonMonthView,
            this.toolStripButtonAgendaView,
            this.toolStripSeparator2});
			this.toolStripView.Location = new System.Drawing.Point(0, 0);
			this.toolStripView.Name = "toolStripView";
			this.toolStripView.Size = new System.Drawing.Size(530, 25);
			this.toolStripView.TabIndex = 3;
			this.toolStripView.Text = "View";
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
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonDayView
			// 
			this.toolStripButtonDayView.CheckOnClick = true;
			this.toolStripButtonDayView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDayView.Image")));
			this.toolStripButtonDayView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonDayView.Name = "toolStripButtonDayView";
			this.toolStripButtonDayView.Size = new System.Drawing.Size(74, 22);
			this.toolStripButtonDayView.Text = "Day view";
			// 
			// toolStripButtonWeekView
			// 
			this.toolStripButtonWeekView.CheckOnClick = true;
			this.toolStripButtonWeekView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonWeekView.Image")));
			this.toolStripButtonWeekView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonWeekView.Name = "toolStripButtonWeekView";
			this.toolStripButtonWeekView.Size = new System.Drawing.Size(83, 22);
			this.toolStripButtonWeekView.Text = "Week view";
			// 
			// toolStripButtonMonthView
			// 
			this.toolStripButtonMonthView.CheckOnClick = true;
			this.toolStripButtonMonthView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMonthView.Image")));
			this.toolStripButtonMonthView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonMonthView.Name = "toolStripButtonMonthView";
			this.toolStripButtonMonthView.Size = new System.Drawing.Size(90, 22);
			this.toolStripButtonMonthView.Text = "Month view";
			// 
			// toolStripButtonAgendaView
			// 
			this.toolStripButtonAgendaView.CheckOnClick = true;
			this.toolStripButtonAgendaView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAgendaView.Image")));
			this.toolStripButtonAgendaView.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonAgendaView.Name = "toolStripButtonAgendaView";
			this.toolStripButtonAgendaView.Size = new System.Drawing.Size(95, 22);
			this.toolStripButtonAgendaView.Text = "Agenda view";
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
			this.panel1.Location = new System.Drawing.Point(0, 306);
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
			// scheduleDay1
			// 
			this.scheduleDay1.AutoScroll = true;
			this.scheduleDay1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scheduleDay1.Location = new System.Drawing.Point(0, 25);
			this.scheduleDay1.Name = "scheduleDay1";
			this.scheduleDay1.Size = new System.Drawing.Size(530, 281);
			this.scheduleDay1.TabIndex = 16;
			// 
			// SchedulerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(530, 351);
			this.Controls.Add(this.scheduleDay1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStripView);
			this.Name = "SchedulerForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Show Scheduler";
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
		private System.Windows.Forms.ToolStripButton toolStripButtonMonthView;
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
		private ScheduleDay scheduleDay1;
	}
}