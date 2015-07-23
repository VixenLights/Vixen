using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using VixenModules.Media.Audio;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class AutomaticMusicDetection : Form
	{
		public AutomaticMusicDetection(Audio audio)
		{
			InitializeComponent();
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			btnOk.BackgroundImage = Resources.HeadingBackgroundImage;
			btnPreviewAudio.BackgroundImage = Resources.HeadingBackgroundImage;
			_audio = audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;
			freqTimer = new Timer();
			freqTimer.Interval = 10;
			freqTimer.Tick += freqTimer_Tick;
			freqTimer.Enabled = true;

			this.numericUpDown1.Value = Accuracy;
		}

		private Properties.Settings settings = new Properties.Settings();


		private List<int> _indexes;
		private Audio _audio;

		public List<int> Indexes
		{
			get { return _indexes; }
			set
			{
				_indexes = value;
				for (int i = 0; i < 120; i++) {
					var box = FindCheckBox(i.ToString());
					box.Checked = _indexes.Contains(i);
				}
			}
		}

		public int Accuracy
		{
			get { return settings.MusicAccuracy; }
			set { settings.MusicAccuracy = value; }
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			var Tag = int.Parse(((CheckBox) sender).Tag as string);

			if (((CheckBox) sender).Checked && !_indexes.Contains(Tag))
				_indexes.Add(Tag);
			else if (!((CheckBox) sender).Checked && _indexes.Contains(Tag))
				_indexes.Remove(Tag);
		}

		private void _audio_FrequencyDetected(object sender, FrequencyEventArgs e)
		{
			if (e.Frequency > 0) {
				var box = FindCheckBox(e.Index.ToString());
				TimeSpan t = TimeSpan.MinValue;
				freqs.TryRemove(e.Index, out t);
				freqs.TryAdd(e.Index, e.Time);
			}
		}

		private Timer freqTimer;

		private void freqTimer_Tick(object sender, EventArgs e)
		{
		    if (_audio != null && _audio.DetectionNotes != null)
		    {
		        for (int item = 0; item < _audio.DetectionNotes.Count(); item++)
		        {
		            TimeSpan value = TimeSpan.MinValue;

		            if (freqs.TryGetValue(item, out value))
		            {
		                var highlight = (_audio.Position - value) <= TimeSpan.FromMilliseconds(Accuracy);
		                if (_audio.Position == TimeSpan.MinValue)
		                    highlight = false;
		                highlightCheckbox(item, highlight);
		            }
		            else
		                highlightCheckbox(item, false);
		        }
		    }
		    else
		    {
		        this.freqTimer.Enabled = false;
		    }
		}

		private ConcurrentDictionary<int, TimeSpan> freqs = new ConcurrentDictionary<int, TimeSpan>();

		private CheckBox FindCheckBox(string tag)
		{
			List<Control> elements = new List<Control>();

			int count = this.Controls.Count;
			if (count > 0) {
				for (int j = 0; j < count; j++) {
					var child = this.Controls[j];
					if (child.GetType() == typeof (GroupBox)) {
						for (int k = 0; k < child.Controls.Count; k++) {
							var box = child.Controls[k];
							if (box.GetType() == typeof (CheckBox)) {
								if (((CheckBox) box).Tag as string == tag)
									return (CheckBox) box;
							}
						}
					}
				}
			}
			return null;
		}

		private delegate void highlightCheckboxDelegate(int id, bool Highlight);

		private void highlightCheckbox(int id, bool Highlight)
		{
			if (this.InvokeRequired)
				this.Invoke(new highlightCheckboxDelegate(highlightCheckbox), id, Highlight);
			else {
				var box = FindCheckBox(id.ToString());
				if (box != null)
					box.BackColor = Highlight ? Color.Blue : this.BackColor;
			}
		}

		private void btnPreviewAudio_Click(object sender, EventArgs e)
		{
			switch (btnPreviewAudio.Text) {
				case "Stop":
					_audio.Stop();
					btnPreviewAudio.Text = "Preview Audio";
					this.freqs.Clear();
					break;
				default:
					this.freqs.Clear();
					_audio.Start();
					btnPreviewAudio.Text = "Stop";

					break;
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			this.Accuracy = (int) numericUpDown1.Value;
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		#region Draw lines and GroupBox borders
		//set color for box borders.
		private Color _borderColor = Color.FromArgb(136, 136, 136);

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			//used to draw the boards and text for the groupboxes to change the default box color.
			//get the text size in groupbox
			Size tSize = TextRenderer.MeasureText((sender as GroupBox).Text, Font);

			e.Graphics.Clear(BackColor);
			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, _borderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 10;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
			e.Graphics.DrawString((sender as GroupBox).Text, Font, new SolidBrush(Color.FromArgb(221, 221, 221)), textRect);
		}
		#endregion
	}
}