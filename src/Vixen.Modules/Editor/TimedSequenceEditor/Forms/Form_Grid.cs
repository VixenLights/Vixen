using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Grid : DockContent
	{
		public Form_Grid()
		{
			InitializeComponent();
		}

		public Common.Controls.Timeline.TimelineControl TimelineControl 
		{ 
			get 
			{
				return timelineControl;
			}
		}
	}
}
