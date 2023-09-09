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

		protected Guid InstanceId { get; init; }

		public Common.Controls.Timeline.TimelineControl TimelineControl 
		{ 
			get 
			{
				return timelineControl;
			}
		}
	}
}
