using System.ComponentModel;
using Common.Controls;
using Common.Controls.Theme;
using Vixen.Module.Effect;
using Timer = System.Timers.Timer;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormParameterPicker : BaseForm
	{
		private readonly Timer _timer = new Timer();
		List<List<EffectParameterPickerControl>> _controlsList = new List<List<EffectParameterPickerControl>>();
		int _currentIndex = 0;
		double _closeInterval = 8000;

		/// <summary>
		/// Shows the parameter picker window.
		/// </summary>
		/// <param name="closeInterval">The auto cancel interval.</param>
		public FormParameterPicker(double closeInterval = 8000)
		{
			InitializeComponent();
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			ThemeUpdateControls.UpdateControls(this);

			nextButton.Font = new Font("Arial", 20);
			previousButton.Font = new Font("Arial", 20);
			tableLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

			StartPosition = FormStartPosition.Manual;
			Top = MousePosition.Y;
			Left = ((MousePosition.X + Width) < Screen.FromControl(this).Bounds.Width)
				? MousePosition.X
				: MousePosition.X - Width;

			_closeInterval = closeInterval;
			_timer.Elapsed += _timer_Elapsed;
		}

		/// <summary>
		/// Shows the parameter picker window.
		/// </summary>
		/// <param name="controls">The controls to render the items in the picker.</param>
		/// <param name="closeInterval">The auto cancel interval.</param>
		public FormParameterPicker(IEnumerable<EffectParameterPickerControl> controls, double closeInterval=8000) : this(closeInterval)
		{
			LoadParameterPicker(controls.ToList());
		}

		private void FormParameterPicker_Activated(object sender, EventArgs e)
		{
			PopulatePickerGUI(_currentIndex);
			_timer.Start();
		}

		public void LoadParameterPicker(List<EffectParameterPickerControl> controls)
		{
			_controlsList.Add(controls);

			// For every page of controls, increase the close interval
			_timer.Interval = _closeInterval * _controlsList.Count();
		}

		private void PopulatePickerGUI(int index)
		{
			flowLayoutPanel1.Controls.Clear();

			// If this is the first page, then don't show the Previous button
			if (index == 0)
			{
				tableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
				tableLayoutPanel.ColumnStyles[0].Width = 0;
				previousButton.Visible = false;
			}
			// Else show the Previous button
			else
			{
				tableLayoutPanel.ColumnStyles[0].SizeType = SizeType.AutoSize;
				previousButton.Visible = true;
			}

			// Display all the controls for the current index
			foreach (EffectParameterPickerControl control in _controlsList[index])
				{
					control.Click += ParameterControl_Clicked;
					flowLayoutPanel1.Controls.Add(control);
				}

			// If this is the last page, then don't show the Next button
			if (index == _controlsList.Count() - 1)
			{
				tableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
				tableLayoutPanel.ColumnStyles[2].Width = 0;
				nextButton.Visible = false;
			}
			// Else show the Next button
			else
			{
				tableLayoutPanel.ColumnStyles[2].SizeType = SizeType.AutoSize;
				nextButton.Visible = true;
			}
		}

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			CloseForm(DialogResult.Cancel);
		}

		private void CloseForm(DialogResult result)
		{
			DialogResult = result;
			Close();
		}

		private void ParameterControl_Clicked(object sender, EventArgs e)
		{
			_timer.Stop();
			EffectParameterPickerControl control = (EffectParameterPickerControl)sender;
			PropertyInfo = control.PropertyInfo;
			EffectPropertyInfo = control.EffectPropertyInfo;
			SelectedControl = control;
			CloseForm(DialogResult.OK);
		}

		private void PreviousButton_Click(object sender, EventArgs e)
		{
			_currentIndex = Math.Max(0, _currentIndex - 1);
			PopulatePickerGUI(_currentIndex);
		}

		private void NextButton_Click(object sender, EventArgs e)
		{
			_currentIndex = Math.Min(_controlsList.Count() - 1, _currentIndex + 1);
			PopulatePickerGUI(_currentIndex);
		}

		public PropertyDescriptor PropertyInfo { get; private set; }

		public IEffectModuleDescriptor EffectPropertyInfo { get; set; }

		public int ParameterIndex { get; set; }

		public EffectParameterPickerControl SelectedControl { get; private set; }

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{
			if (flowLayoutPanel1.BorderStyle != BorderStyle.FixedSingle) return;
			const int thickness = 3;
			const int halfThickness = thickness/2;
			using (Pen p = new Pen(Color.Black, thickness))
			{
				e.Graphics.DrawRectangle(p, new Rectangle(halfThickness,
					halfThickness,
					flowLayoutPanel1.ClientSize.Width - thickness,
					flowLayoutPanel1.ClientSize.Height - thickness));
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				CloseForm(DialogResult.Cancel);
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
