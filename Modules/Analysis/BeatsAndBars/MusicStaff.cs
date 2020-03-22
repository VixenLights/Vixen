using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class MusicStaff : UserControl
	{
		private const int TSLABEL_XOFFSET = 80;
		private static int _firstBarOffset;
		private const int DIV_Y_OFFSET = 20;
		private const int VAL_LABEL_OFFSET = 25;
		private const int LABEL_VAL_OFFSET = 5;
		private double m_beatPeriod;
		private double _scale = 1;

		public MusicStaff()
		{
			InitializeComponent();
			_scale = ScalingTools.GetScaleFactor();
			_firstBarOffset = ScaleValue(TSLABEL_XOFFSET) + ScaleValue(45);

			BeatsPerBar = 4;
			//NoteSize = 4;

			BPMLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			BarPeriodLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			DivTimeLabelVal.TextAlign = ContentAlignment.MiddleLeft;
			BeatsPerBarLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F, FontStyle.Bold);
			NoteSizeLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F, FontStyle.Bold);
			BPMLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size, FontStyle.Bold);
			BarPeriodLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size, FontStyle.Bold);
			DivTimeLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size, FontStyle.Bold);
			staffPictureBox.Width = ClientSize.Width - 5;
		}

		private int ScaleValue(int x)
		{
			return (int)(x * _scale);
		}

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
				int actualNoteSize = NoteSize * (int)((splitBeatsCB.Checked) ? 2 : 1);

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
				new Point(ScaleValue(TSLABEL_XOFFSET), staffPictureBox.Location.Y + staffPictureBox.Height/2-BeatsPerBarLabel.Height);

			NoteSizeLabel.Location =
				new Point(ScaleValue(TSLABEL_XOFFSET), BeatsPerBarLabel.Location.Y + BeatsPerBarLabel.Height + 1);

			BPMLabelVal.Text = (60000/m_beatPeriod).ToString("F1");
			BPMLabelVal.Location = 
				new Point(BPMLabel.Location.X + BPMLabel.PreferredWidth + ScaleValue(LABEL_VAL_OFFSET), BPMLabel.Location.Y);

			BarPeriodLabel.Location =
				new Point(BPMLabelVal.Location.X + BPMLabelVal.PreferredWidth + ScaleValue(VAL_LABEL_OFFSET), BPMLabelVal.Location.Y);
			BarPeriodLabelVal.Location =
				new Point(BarPeriodLabel.Location.X + BarPeriodLabel.PreferredWidth + ScaleValue(LABEL_VAL_OFFSET), BarPeriodLabel.Location.Y);
			BarPeriodLabelVal.Text = (BarPeriod / 1000).ToString("F2");

			double divTime = splitBeatsCB.Checked ? m_beatPeriod/2 : m_beatPeriod;
			DivTimeLabel.Location =
				new Point(BarPeriodLabelVal.Location.X + BarPeriodLabelVal.PreferredWidth + ScaleValue(VAL_LABEL_OFFSET), BarPeriodLabelVal.Location.Y);
			DivTimeLabelVal.Location =
				new Point(DivTimeLabel.Location.X + DivTimeLabel.PreferredWidth + ScaleValue(LABEL_VAL_OFFSET), DivTimeLabel.Location.Y);
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
					staffPictureBox.Location.Y - ScaleValue(DIV_Y_OFFSET));

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

		private void tsRightButton_Click(object sender, EventArgs e)
		{
			if (BeatsPerBar < 16)
			{
				BeatsPerBar++;
				if (SettingChanged != null)
				{
					SettingChanged(sender, e);
				}
				Invalidate();
			}
		}

		private void tsLeftButton_Click(object sender, EventArgs e)
		{
			if (BeatsPerBar > 2)
			{
				BeatsPerBar--;
				if (SettingChanged != null)
				{
					SettingChanged(sender, e);
				}
					
				Invalidate();
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

		public double BeatPeriod
		{
			get { return m_beatPeriod; }

			set
			{
				m_beatPeriod = value;
				Invalidate();
			}
		}

		public double BarPeriod
		{
			get { return m_beatPeriod*BeatsPerBar;  }
		}

		private void ContextTSChanged(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			BeatsPerBar = Convert.ToInt32(mi.Tag);

			if (SettingChanged != null)
			{
				SettingChanged(sender, e);
			}

			Invalidate();
			
		}

		private void ShowContextmenu(Control parentControl, Point displayPoint)
		{
			ContextMenu mnuContextMenu = new ContextMenu();
			for (int j = 2; j <= 16; j++)
			{
				MenuItem mi = new MenuItem(j.ToString() + "/" + CalcNoteSize(j).ToString(), new EventHandler(ContextTSChanged));
				if (j == BeatsPerBar)
				{
					mi.Checked = true;
				}
				mi.Tag = j;
				mnuContextMenu.MenuItems.Add(mi);
			}
			this.ContextMenu = mnuContextMenu;
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
