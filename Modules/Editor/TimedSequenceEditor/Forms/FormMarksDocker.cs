using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Views;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using MarkCollection = VixenModules.App.Marks.MarkCollection;

namespace VixenModules.Editor.TimedSequenceEditor.Forms
{
	public partial class FormMarksDocker : DockContent
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ElementHost host;
		private readonly MarkDockerView _markDockerView;
		private TimedSequence _sequence;

		public FormMarksDocker(TimedSequence sequence)
		{
			InitializeComponent();
			host = new ElementHost { Dock = DockStyle.Fill };

			_markDockerView = new MarkDockerView();

			host.Child = _markDockerView;

			Controls.Add(host);
			_sequence = sequence;
			PopulateMarkCollectionsList(_sequence.LabeledMarkCollections);
		}

		public void PopulateMarkCollectionsList(List<MarkCollection> markCollections)
		{
			_markDockerView.MarkCollection = markCollections;
		}
	}
}
