using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class MusicStaff : UserControl
	{
		private const int TSLABEL_XOFFSET = 85;
		private const int FIRST_BAR_OFFSET = TSLABEL_XOFFSET + 35;
		private const int DIV_Y_OFFSET = 10;

		public MusicStaff()
		{
			InitializeComponent();
			BeatsPerBar = 4;
			//NoteSize = 4;

			BeatsPerBarLabel.BackColor = Color.Transparent;
			NoteSizeLabel.BackColor = Color.Transparent;

		}

		public int BeatsPerBar { get; set; }

		public bool SplitBeats
		{
			get { return splitBeatsCB.Checked; }
		}

		public int NoteSize
		{
			get
			{
				int retVal = 0;
				if (BeatsPerBar == 2)
				{
					retVal = 2;
				}
				else if (BeatsPerBar <= 4)
				{
					retVal = 4;
				}
				else if (BeatsPerBar <= 8)
				{
					retVal = 8;
				}
				else if (BeatsPerBar <= 16)
				{
					retVal = 16;
				}

				return retVal;

			}
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
			pictureBox1.Invalidate();
			BeatsPerBarLabel.Text = BeatsPerBar.ToString();
			NoteSizeLabel.Text = NoteSize.ToString();

			BeatsPerBarLabel.Location =
				new Point(TSLABEL_XOFFSET, pictureBox1.Location.Y + 17);

			NoteSizeLabel.Location =
				new Point(TSLABEL_XOFFSET, BeatsPerBarLabel.Location.Y + BeatsPerBarLabel.Height + 1);

		}

		private void staffBox1_Paint(object sender, PaintEventArgs e)
		{

			int marksInBar = BeatsPerBar * ((splitBeatsCB.Checked) ? 2 : 1);
			for (int j = 0; j < marksInBar; j++)
			{
				decimal interval = (decimal)(pictureBox1.Width - 16 - FIRST_BAR_OFFSET) / marksInBar;

				Bitmap noteBitmap = NotesizeBitmap;

				// Create points that define line.
				Point point1 = new Point(FIRST_BAR_OFFSET + (int)(interval * j), 
					BeatsPerBarLabel.Location.Y - DIV_Y_OFFSET);

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
				Invalidate();
			}
		}

		private void tsLeftButton_Click(object sender, EventArgs e)
		{
			if (BeatsPerBar > 2)
			{
				BeatsPerBar--;
				Invalidate();
			}
		}

		private void splitBeatsCB_CheckedChanged(object sender, EventArgs e)
		{
			Invalidate();
		}
	}
}
