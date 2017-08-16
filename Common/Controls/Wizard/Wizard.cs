using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls.Wizard
{
	// a Wizard is a self-contained class which can be set up, called to run, and give a result from
	// the process. It can be treated in a similar style to a dialog or window.
	public abstract class Wizard
	{
		protected Wizard()
		{
			_currentStageIndex = 0;
			WizardForm = null;
			WizardActive = false;
			WizardDialogResult = DialogResult.None;
		}

		protected abstract List<WizardStage> Stages { get; }

		public abstract string WizardTitle { get; }

		private int _currentStageIndex;

		public WizardStage CurrentStage
		{
			get
			{
				if (_currentStageIndex >= Stages.Count)
					return null;
				return Stages[_currentStageIndex];
			}
		}

		public bool IsFinalStage
		{
			get { return (_currentStageIndex >= Stages.Count - 1); }
		}

		public bool IsFirstStage
		{
			get { return (_currentStageIndex <= 0); }
		}

		// if showModal is true, it will show the dialog as modal, and control will return when the
		// wizard has completed. If false, it will display the wizard and return, with the wizard
		// still open/running. The WizardActive property can be used to determine if the wizard is
		// still open, and the WizardForm property can be used to interact with it. Once it has
		// closed, the WizardDialogResult will contain the result from the dialog.
		public void Start(bool showModal)
		{
			_currentStageIndex = 0;

			if (showModal) {
				using (WizardForm = new WizardForm(this)) {
					WizardActive = true;
					WizardDialogResult = WizardForm.ShowDialog();
					WizardActive = false;
					_wizardFinished();
					return;
				}
			}

			WizardActive = true;
			WizardForm = new WizardForm(this);
			WizardForm.WizardFormFinished += WizardForm_WizardFormFinished;
			WizardForm.Show();
		}

		private void WizardForm_WizardFormFinished(object sender, EventArgs e)
		{
			WizardActive = false;
			WizardForm form = sender as WizardForm;
			if (form != null)
			{
				WizardDialogResult = form.DialogResult;
			}
			_wizardFinished();

			if (form!=null && !form.IsDisposed)
			{
				form.Dispose();
			}
		}

		public DialogResult WizardDialogResult { get; private set; }

		public bool WizardActive { get; private set; }

		public WizardForm WizardForm { get; private set; }

		public bool CanMoveNext
		{
			get { return CurrentStage.CanMoveNext; }
		}

		public bool CanMovePrevious
		{
			get
			{
				if (IsFirstStage)
					return false;
				return CurrentStage.CanMovePrevious;
			}
		}

		public bool IsPreviousVisible
		{
			get { return CurrentStage.IsPreviousVisible; }
		}

		public bool IsCancelVisible
		{
			get { return CurrentStage.IsCancelVisible; }
		}

		public void MovePrevious()
		{
			if (_currentStageIndex > 0)
				_currentStageIndex--;
		}

		public void MoveNext()
		{
			if (!IsFinalStage)
				_currentStageIndex++;
		}

		public event EventHandler WizardFinished;

		private void _wizardFinished()
		{
			if (WizardFinished != null)
				WizardFinished(this, EventArgs.Empty);
		}
	}
}