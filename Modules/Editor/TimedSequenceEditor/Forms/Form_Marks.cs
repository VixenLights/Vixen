using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Vixen.Marks;
using WeifenLuo.WinFormsUI.Docking;
using VixenModules.Sequence.Timed;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Views;
using MarkCollection = VixenModules.App.Marks.MarkCollection;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Marks : DockContent
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ElementHost host;
		private readonly MarkDockerView _markDockerView;
		
		public Form_Marks(TimedSequence sequence)
		{
			InitializeComponent();
			host = new ElementHost { Dock = DockStyle.Fill };

			_markDockerView = new MarkDockerView();

			host.Child = _markDockerView;

			Controls.Add(host);
			PopulateMarkCollectionsList(sequence.LabeledMarkCollections);
		}

		public void PopulateMarkCollectionsList(ObservableCollection<IMarkCollection> markCollections)
		{
			_markDockerView.MarkCollection = markCollections;
		}
	}

}
