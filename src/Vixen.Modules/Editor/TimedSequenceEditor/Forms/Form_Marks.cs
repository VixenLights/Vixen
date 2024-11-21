using System.Windows.Forms.Integration;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels;
using WeifenLuo.WinFormsUI.Docking;
using VixenModules.Sequence.Timed;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Views;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Marks : DockContent
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ElementHost host;
		private readonly MarkDockerView _markDockerView;
		private MarkDockerViewModel _mdvm;
		private readonly TimedSequenceEditorForm _sequenceEditorForm;

		public Form_Marks(TimedSequenceEditorForm sequenceEditorForm, TimedSequence sequence)
		{
			InitializeComponent();

			_sequenceEditorForm = sequenceEditorForm;
			host = new ElementHost { Dock = DockStyle.Fill };

			Catel.Windows.Controls.UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;

			_mdvm = new MarkDockerViewModel(sequence.LabeledMarkCollections);
			_markDockerView = new MarkDockerView(_mdvm);
			host.Child = _markDockerView;

			Controls.Add(host);

			// Establish automation to intercept quick keys meant for the Timeline window
			host.Child.KeyDown += Form_MarksKeyDown;
			host.Enter += Form_MarksEnter;
		}

		/// <summary>
		/// Intercept KeyDown event
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">Contains the event data</param>
		private void Form_MarksKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			_sequenceEditorForm.HandleQuickKey(e);
		}

		/// <summary>
		/// Intercept when the control is activated
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">Contains the event data</param>
		private void Form_MarksEnter(object sender, EventArgs e)
		{
			// Momentarily activate a special target in order to set the keyboard focus,
			// because if there's no Marks, then there's nothing to good to aim for.
			_markDockerView.target_button.Visibility = System.Windows.Visibility.Visible;
			SendKeys.Send("+{TAB}");
			_markDockerView.target_button.Visibility = System.Windows.Visibility.Hidden;
		}
	}
}
