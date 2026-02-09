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

		/// <summary>
		/// Shows are parameter picker window.
		/// </summary>
		/// <param name="controls">The controls to render the items in the picker.</param>
		/// <param name="closeInterval">The auto cancel interval.</param>
		public FormParameterPicker(IEnumerable<EffectParameterPickerControl> controls, double closeInterval=8000)
		{
			InitializeComponent();

			var groupedControls = controls.GroupBy(x => x.PropertyInfo.Owner).ToList();

			if (groupedControls.Count > 1)
			{
				int index = 1;
				flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
				foreach (IGrouping<object, EffectParameterPickerControl> effectParameterPickerControls in groupedControls)
				{
					var grpBox = new GroupBox()
					{
						Text = $@"{effectParameterPickerControls.First().PropertyInfo.OwnerDisplayName} {index}",
						AutoSize = true,
						AutoSizeMode = AutoSizeMode.GrowAndShrink
					};
					ThemeUpdateControls.UpdateControls(grpBox);
					flowLayoutPanel1.Controls.Add(grpBox);
					var flowRow = new FlowLayoutPanel
					{
						FlowDirection = FlowDirection.RightToLeft,
						Margin = new Padding(5, 5, 5, 5),
						Dock = DockStyle.Fill,
						AutoSize = true,
						AutoSizeMode = AutoSizeMode.GrowOnly
					};
					grpBox.Controls.Add(flowRow);
					foreach (EffectParameterPickerControl control in effectParameterPickerControls.Reverse())
					{
						control.Margin = new Padding(0);
						control.Click += ParameterControl_Clicked;
						flowRow.Controls.Add(control);
					}

					index++;
				}
			}

			else
			{
				flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
				foreach (EffectParameterPickerControl control in controls.Reverse())
				{
					control.Click += ParameterControl_Clicked;
					flowLayoutPanel1.Controls.Add(control);
				}
			}

				
			_timer.Interval = closeInterval;
			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(() => CloseForm(DialogResult.Cancel));
				return;
			}
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

		public PropertyMetaData PropertyInfo { get; private set; }

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
