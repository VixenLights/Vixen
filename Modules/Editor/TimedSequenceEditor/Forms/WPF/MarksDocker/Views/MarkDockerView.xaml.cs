using System.Collections.Generic;
using VixenModules.App.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Views
{
	public partial class MarkDockerView
	{
		public MarkDockerView()
		{
			InitializeComponent();
		}

		public List<MarkCollection> MarkCollection
		{
			set
			{
				DataContext = new MarkDockerViewModel(value);
			}
		}
	}
}
