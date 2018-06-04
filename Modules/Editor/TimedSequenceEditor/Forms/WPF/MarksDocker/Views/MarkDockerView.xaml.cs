using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Views
{
	public partial class MarkDockerView
	{
		public MarkDockerView()
		{
			InitializeComponent();
		}

		public ObservableCollection<IMarkCollection> MarkCollection
		{
			set
			{
				DataContext = new MarkDockerViewModel(value);
			}
		}
	}
}
