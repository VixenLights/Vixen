using Common.Controls;
using Common.Controls.Theme;
using Newtonsoft.Json.Linq;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class BulkEffectMoveForm : BaseForm
	{
		public BulkEffectMoveForm():this(TimeSpan.Zero, TimeSpan.MaxValue)
		{
			
		}

		public BulkEffectMoveForm(TimeSpan startTime, TimeSpan sequencelength)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Start = startTime;
			End = startTime;
			txtStartTime.Maximum = sequencelength;
			txtEndTime.Maximum = sequencelength;
			txtOffset.Maximum = sequencelength;
			Offset = TimeSpan.Zero;
		}

		public TimeSpan End
		{
			get
			{
				return txtEndTime.TimeSpan;
			}
			set
			{
				txtEndTime.TimeSpan = value;
			}
		}
		public TimeSpan Start
		{
			set
			{
				txtStartTime.TimeSpan = value;
			}
			get
			{
				return txtStartTime.TimeSpan;
			}
		}

		public TimeSpan Offset
		{
			get
			{
				return txtOffset.TimeSpan;
			}
			set
			{
				txtOffset.TimeSpan = value;
			}
		}

		public bool IsForward
		{
			get { return radioButtonForward.Checked; }
		}


		public bool ProcessVisibleRows
		{
			get { return checkBoxVisibleRows.Checked; }
		}

		public bool ClipEffects
		{
			get { return checkBoxClipEffects.Checked; }
		}

		public bool ProcessMarks
		{
			get { return checkBoxMoveMarks.Checked; }
		}


		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
