using System.ComponentModel;
using Common.Controls.Scaling;
using Common.Controls.Theme;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class MusicStaff : UserControl
	{
		private const int TsLabelXOffset = 80;
		private static int _firstBarOffset;
		private const int DivYOffset = 20;
		private const int ValLabelOffset = 25;
		private const int LabelValOffset = 5;
		private double _beatPeriod;
		private readonly double _scale;

		public MusicStaff()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			_scale = ScalingTools.GetScaleFactor();
			_firstBarOffset = ScaleValue(TsLabelXOffset) + ScaleValue(45);

			BeatsPerBar = 4;
			//NoteSize = 4;

			BPMLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			BarPeriodLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			DivTimeLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			if (SystemFonts.MessageBoxFont != null)
			{
				BeatsPerBarLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F, FontStyle.Bold);
				NoteSizeLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F, FontStyle.Bold);
				BPMLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size,
					FontStyle.Bold);
				BarPeriodLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size,
					FontStyle.Bold);
				DivTimeLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size,
					FontStyle.Bold);
			}

			staffPictureBox.Width = ClientSize.Width - 5;
		}

		private int ScaleValue(int x)
		{
			return (int)(x * _scale);
		}

		[Bindable(true), Category("Display"), DefaultValue(4),
		 Description("Set the value for the number of beats per bar")]
		public int BeatsPerBar { get; set; }

		public bool SplitBeats
		{
			get { return splitBeatsCB.Checked; }
		}

		private int CalcNoteSize(int bpb)
		{
			int retVal = 0;
			if (bpb == 2)
			{
				retVal = 2;
			}
			else if (bpb <= 4)
			{
				retVal = 4;
			}
			else if (bpb <= 8)
			{
				retVal = 8;
			}
			else if (bpb <= 16)
			{
				retVal = 16;
			}

			return retVal;
			
		}

		public int NoteSize
		{
			get { return CalcNoteSize(BeatsPerBar); }
		}

		private Bitmap NotesizeBitmap
		{
			get
			{
				Bitmap retVal = null;
				int actualNoteSize = NoteSize * (splitBeatsCB.Checked ? 2 : 1);

				if (actualNoteSize == 2)
				{
					retVal = Properties.Resources.halfnote;
				}
				else if (actualNoteSize == 4)
				{
					retVal = Properties.Resources.quarternote;
				}
				else if (actualNoteSize == 8)
				{
					retVal = Properties.Resources.eigthnote;
				}
				else if (actualNoteSize == 16)
				{
					retVal = Properties.Resources.sixteenthnote;
				}
				else if (actualNoteSize == 32)
				{
					retVal = Properties.Resources._32ndNote;
				}

				return retVal;

			}
		}
		
		private void MusicStaff_Paint(object sender, PaintEventArgs e)
		{
			staffPictureBox.Width = ClientSize.Width - 17;
			staffPictureBox.Invalidate();
			BeatsPerBarLabel.Text = BeatsPerBar.ToString();
			NoteSizeLabel.Text = NoteSize.ToString();

			BeatsPerBarLabel.Location =
				new Point(ScaleValue(TsLabelXOffset), staffPictureBox.Location.Y + staffPictureBox.Height/2-BeatsPerBarLabel.Height);

			NoteSizeLabel.Location =
				new Point(ScaleValue(TsLabelXOffset), BeatsPerBarLabel.Location.Y + BeatsPerBarLabel.Height + 1);

			BPMLabelVal.Text = (60000/_beatPeriod).ToString("F1");
			BPMLabelVal.Location = 
				new Point(BPMLabel.Location.X + BPMLabel.PreferredWidth + ScaleValue(LabelValOffset), BPMLabel.Location.Y);

			BarPeriodLabel.Location =
				new Point(BPMLabelVal.Location.X + BPMLabelVal.PreferredWidth + ScaleValue(ValLabelOffset), BPMLabelVal.Location.Y);
			BarPeriodLabelVal.Location =
				new Point(BarPeriodLabel.Location.X + BarPeriodLabel.PreferredWidth + ScaleValue(LabelValOffset), BarPeriodLabel.Location.Y);
			BarPeriodLabelVal.Text = (BarPeriod / 1000).ToString("F2");

			double divTime = splitBeatsCB.Checked ? _beatPeriod/2 : _beatPeriod;
			DivTimeLabel.Location =
				new Point(BarPeriodLabelVal.Location.X + BarPeriodLabelVal.PreferredWidth + ScaleValue(ValLabelOffset), BarPeriodLabelVal.Location.Y);
			DivTimeLabelVal.Location =
				new Point(DivTimeLabel.Location.X + DivTimeLabel.PreferredWidth + ScaleValue(LabelValOffset), DivTimeLabel.Location.Y);
			DivTimeLabelVal.Text = divTime.ToString("F0");

		}

		public delegate void SettingChangedEventHandler(object sender, EventArgs e);
		public event SettingChangedEventHandler SettingChanged;

		private void staffBox1_Paint(object sender, PaintEventArgs e)
		{
			int marksInBar = BeatsPerBar * ((splitBeatsCB.Checked) ? 2 : 1);
			for (int j = 0; j < marksInBar; j++)
			{
				decimal interval = (decimal)(staffPictureBox.Width - ScaleValue(16) - _firstBarOffset) / marksInBar;

				Bitmap noteBitmap = NotesizeBitmap;

				Point point1 = new Point(_firstBarOffset + (int)(interval * j), 
					staffPictureBox.Location.Y - ScaleValue(DivYOffset));

				if ((splitBeatsCB.Checked) && ((j %2) == 1))
				{
					e.Graphics.DrawImage(noteBitmap, point1);	
				}
				else
				{
					e.Graphics.DrawImage(noteBitmap, point1);
				}
			}
		}

		private void splitBeatsCB_CheckedChanged(object sender, EventArgs e)
		{
			if (SettingChanged != null)
			{
				SettingChanged(sender, e);
			}
			Invalidate();
		}

		[Bindable(true), Category("Display"), DefaultValue(0),
		 Description("Set the value for the number of beats per period")]
		public double BeatPeriod
		{
			get { return _beatPeriod; }

			set
			{
				_beatPeriod = value;
				Invalidate();
			}
		}

		public double BarPeriod
		{
			get { return _beatPeriod*BeatsPerBar;  }
		}

		private void ContextTsChanged(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem mi)
			{
				BeatsPerBar = Convert.ToInt32(mi.Tag);
				if (SettingChanged != null)
				{
					SettingChanged(sender, e);
				}

				Invalidate();
			}
		}

		private void ShowContextmenu(Control parentControl, Point displayPoint)
		{
			ContextMenuStrip mnuContextMenu = new ContextMenuStrip();
			for (int j = 2; j <= 16; j++)
			{
				ToolStripMenuItem mi = new ToolStripMenuItem(j + "/" + CalcNoteSize(j), null, ContextTsChanged);
				if (j == BeatsPerBar)
				{
					mi.Checked = true;
				}
				mi.Tag = j;
				mnuContextMenu.Items.Add(mi);
			}
			this.ContextMenuStrip = mnuContextMenu;
			mnuContextMenu.Show(parentControl, displayPoint);
		}

		private void BeatsPerBarLabel_Click(object sender, EventArgs e)
		{
			ShowContextmenu(BeatsPerBarLabel, new Point(0,0));
		}

		private void NoteSizeLabel_Click(object sender, EventArgs e)
		{
			ShowContextmenu(NoteSizeLabel, new Point(0,0));
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			ShowContextmenu(staffPictureBox, staffPictureBox.PointToClient(Cursor.Position));
		}
	}
}
