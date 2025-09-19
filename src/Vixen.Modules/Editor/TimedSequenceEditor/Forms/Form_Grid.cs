using Common.Broadcast;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Grid : DockContent
	{
		public Form_Grid(Guid id)
		{
			InstanceId = id;
			InitializeComponent();
		}

		private void Form_Grid_DockStateChanged(object sender, EventArgs e)
		{
			// If the Timeline window is NOT docked, then turn on KeyPreview so this form intercepts keystrokes, which is then
			// broadcast to the TimedSequenceEditor keystroke handler (parent window when docked).
			KeyPreview = this.DockState == DockState.Float;
		}

		protected Guid InstanceId { get; init; }

		public Common.Controls.Timeline.TimelineControl TimelineControl 
		{ 
			get 
			{
				return timelineControl;
			}
		}

		private void Form_Grid_KeyDown(object sender, KeyEventArgs e)
		{
			Broadcast.Publish<KeyEventArgs>("KeydownSWF", e);
		}
	}
}
