using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using CommonElements.Timeline;

namespace Timeline
{
    public partial class Form1 : Form
	{
        public Form1()
        {
            InitializeComponent();

            // Rows
			TimelineRow row1 = timelineControl1.AddRow("Row 1");
			TimelineRow row2 = timelineControl1.AddRow("Row 2", row1);
			TimelineRow row3 = timelineControl1.AddRow("Row 3", row1);
			TimelineRow row4 = timelineControl1.AddRow("Row 4", row1);
			TimelineRow row5 = timelineControl1.AddRow("Row 5", row4);
			TimelineRow row6 = timelineControl1.AddRow("Row 6", row4);
			TimelineRow row7 = timelineControl1.AddRow("Row 7", row4);
			TimelineRow row8 = timelineControl1.AddRow("Row 8");
			TimelineRow row9 = timelineControl1.AddRow("Row 9");
			TimelineRow row10 = timelineControl1.AddRow("Row 10");
			TimelineRow row11 = timelineControl1.AddRow("Row 11");
			TimelineRow row12 = timelineControl1.AddRow("Row 12", row11);
			TimelineRow row13 = timelineControl1.AddRow("Row 13", row12);
			TimelineRow row14 = timelineControl1.AddRow("Row 14", row13);

			row1.TreeOpen = true;

			row1.AddElement(new TimelineElement() {
				BackColor = Color.Red,
				StartTime = TimeSpan.FromSeconds(1.2),
				Duration = TimeSpan.FromSeconds(3),
				Tag = "Red"
			}
			);

			row2.AddElement(new TimelineElement() {
				BackColor = Color.Green,
				StartTime = TimeSpan.FromSeconds(0),
				Duration = TimeSpan.FromSeconds(2),
				Tag = "Green"
			}
			);




			row2.AddElement(new TimelineElement() {
				BackColor = Color.Blue,
				StartTime = TimeSpan.FromSeconds(4),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Blue"
			}
			);

			row3.AddElement(new TimelineElement() {
				BackColor = Color.Orange,
				StartTime = TimeSpan.FromSeconds(4),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Orange"
			}
			);

			row4.AddElement(new TimelineElement() {
				BackColor = Color.Purple,
				StartTime = TimeSpan.FromSeconds(4),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Purple"
			}
			);




			row2.AddElement(new TimelineElement() {
				BackColor = Color.Blue,
				StartTime = TimeSpan.FromSeconds(2),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Blue"
			}
			);

			row3.AddElement(new TimelineElement() {
				BackColor = Color.Orange,
				StartTime = TimeSpan.FromSeconds(2),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Orange"
			}
			);

			row4.AddElement(new TimelineElement() {
				BackColor = Color.Purple,
				StartTime = TimeSpan.FromSeconds(2),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Purple"
			}
			);




			row5.AddElement(new TimelineElement() {
				BackColor = Color.FromArgb(255, 255, 0, 0),
				StartTime = TimeSpan.FromSeconds(0),
				Duration = TimeSpan.FromSeconds(2),
				Tag = "Red"
			}
			);

			row5.AddElement(new TimelineElement() {
				BackColor = Color.FromArgb(255, 0, 255, 0),
				StartTime = TimeSpan.FromSeconds(1),
				Duration = TimeSpan.FromSeconds(2),
				Tag = "Green"
			}
			);

			row5.AddElement(new TimelineElement() {
				BackColor = Color.FromArgb(255, 0, 0, 255),
				StartTime = TimeSpan.FromSeconds(2),
				Duration = TimeSpan.FromSeconds(2),
				Tag = "Blue"
			}
			);




			row5.AddElement(new TimelineElement() {
				BackColor = Color.Black,
				StartTime = TimeSpan.FromSeconds(4),
				Duration = TimeSpan.FromSeconds(1),
				Tag = "Black"
			}
			);

			row13.AddElement(new TimelineElement() {
				BackColor = Color.CornflowerBlue,
				StartTime = TimeSpan.FromSeconds(6),
				Duration = TimeSpan.FromSeconds(3),
				Tag = "CornflowerBlue"
			}
			);

			row14.AddElement(new TimelineElement() {
				BackColor = Color.Firebrick,
				StartTime = TimeSpan.FromSeconds(2),
				Duration = TimeSpan.FromSeconds(7),
				Tag = "Firebrick"
			}
			);
            

			timelineControl1.AddSnapTime(TimeSpan.FromSeconds(3.3), 5);
			timelineControl1.AddSnapTime(TimeSpan.FromSeconds(4.6), 10);
			timelineControl1.AddSnapTime(TimeSpan.FromSeconds(20), 40);


			timelineControl1.ElementsMoved += new EventHandler<MultiElementEventArgs>(tc_ElementsMoved);
			timelineControl1.ElementDoubleClicked += new EventHandler<ElementEventArgs>(tc_ElementDoubleClicked);
            //timelineControl1.SelectedElements.CollectionChanged += new EventHandler(SelectedElements_CollectionChanged);

			//tg.MouseClick += new MouseEventHandler(tc_MouseClick);
			//tg.MouseDown += new MouseEventHandler(tc_MouseDown);
			//tg.MouseDoubleClick += new MouseEventHandler(tc_MouseDoubleClick);
			//tg.MouseUp += new MouseEventHandler(tc_MouseUp);
        }


        private int m_logIndex = 0;
        private void _logMessage(string message)
        {
            textBoxLog.Text = String.Format("{0}) {1}{2}{3}",
                m_logIndex++, message, Environment.NewLine, textBoxLog.Text);
        }

        private string _elementList(IEnumerable<TimelineElement> elements)
        {
            StringBuilder s = new StringBuilder(); 
            foreach (TimelineElement elem in elements)
                s.AppendFormat("{0}, ", elem.Tag.ToString());
            return s.ToString();
        }


        void tc_ElementsMoved(object sender, MultiElementEventArgs e)
        {
            _logMessage("Elements moved: " + _elementList(e.Elements));
        }

        void tc_ElementDoubleClicked(object sender, ElementEventArgs e)
        {
            _logMessage(e.Element.Tag.ToString() + " double-clicked.");
        }

        void buttonElemAt_Click(object sender, EventArgs e)
        {
			//var elems = tg.ElementsAtTime(TimeSpan.FromSeconds(2.0));
			//_logMessage("Elements at 2 sec: " + _elementList(elems));
        }

		//void SelectedElements_CollectionChanged(object sender, EventArgs e)
		//{
		//    _logMessage("Selected elements: " + _elementList((TimelineElementCollection)sender));
		//}


        
        void tc_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MouseClick");
        }
        void tc_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MouseDoubleClick");
        }
        void tc_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MouseDown");
        }
        void tc_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MouseUp");
        }


        private void buttonReset_Click(object sender, EventArgs e)
        {
			//tg.Rows[0].Elements[0].Offset = TimeSpan.FromSeconds(1.5);
			//tg.Rows[0].Elements[0].Duration = TimeSpan.FromSeconds(3.5);

			//tg.Rows[1].Elements[0].Offset = TimeSpan.FromSeconds(0);
			//tg.Rows[1].Elements[0].Duration = TimeSpan.FromSeconds(2);
        }

		private void buttonZoomIn_Click(object sender, EventArgs e)
		{
			timelineControl1.Zoom((2.0 / 3.0));
		}

		private void buttonZoomOut_Click(object sender, EventArgs e)
		{
			timelineControl1.Zoom((3.0 / 2.0));
		}

		private void buttonGenericDebug_Click(object sender, EventArgs e)
		{
			//_logMessage("tc total time: " + tg.TotalTime);
			//_logMessage("tc visible time span: " + tg.VisibleTimeSpan);
			//_logMessage("tc visible time start: " + tg.VisibleTimeStart);
			//_logMessage("tc visible time end: " + tg.VisibleTimeEnd);
		}

		private void buttonGenericDebug2_Click(object sender, EventArgs e)
		{
			timelineControl1.VisibleTimeStart = TimeSpan.FromSeconds(5);
		}

		private void buttonAlignLeft_Click(object sender, EventArgs e)
		{
			timelineControl1.AlignSelectedElementsLeft();
		}
    }


}
