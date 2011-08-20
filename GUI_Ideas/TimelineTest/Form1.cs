using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Timeline
{
    public partial class Form1 : Form
	{
		TimelineGrid tg;

        public Form1()
        {
            InitializeComponent();

			// temporary hack to get this working; need to implement proper interfaces in the TimelineControl
			tg = timelineControl1.Grid;

            // Rows
			tg.Rows.Add(new TimelineRow() { Height = 50, Name = "Row 0" });
			tg.Rows.Add(new TimelineRow() { Height = 100, Name = "Row 1" });
			tg.Rows.Add(new TimelineRow() { Height = 100, Name = "Row 2" });
			tg.Rows.Add(new TimelineRow() { Height = 100, Name = "Row 3" });
			tg.Rows.Add(new TimelineRow() { Height = 100, Name = "Row 4" });

            // Elements
            tg.Rows[0].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Red,
                    Offset = TimeSpan.FromSeconds(1.5),
                    Duration = TimeSpan.FromSeconds(3.5),
                    Tag = "Red"
                }
            );

            tg.Rows[1].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Green,
                    Offset = TimeSpan.FromSeconds(0),
                    Duration = TimeSpan.FromSeconds(2),
                    Tag = "Green"
                }
            );
            
            tg.Rows[1].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Blue,
                    Offset = TimeSpan.FromSeconds(4),
                    Duration = TimeSpan.FromSeconds(1),
                    Tag = "Blue"
                }
            );

            tg.Rows[2].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Orange,
                    Offset = TimeSpan.FromSeconds(4),
                    Duration = TimeSpan.FromSeconds(1),
                    Tag = "Orange"
                }
            );

            tg.Rows[3].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Purple,
                    Offset = TimeSpan.FromSeconds(4),
                    Duration = TimeSpan.FromSeconds(1),
                    Tag = "Purple"
                }
            );


            tg.Rows[4].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Black,
                    Offset = TimeSpan.FromSeconds(4),
                    Duration = TimeSpan.FromSeconds(1),
                    Tag = "Black"
                }
            );
            

			tg.AddSnapTime(TimeSpan.FromSeconds(3.3), 5);
			tg.AddSnapTime(TimeSpan.FromSeconds(4.6), 10);


            tg.ElementsMoved += new EventHandler<MultiElementEventArgs>(tc_ElementsMoved);
            tg.ElementDoubleClicked += new EventHandler<ElementEventArgs>(tc_ElementDoubleClicked);
            //tg.SelectedElements.CollectionChanged += new EventHandler(SelectedElements_CollectionChanged);

            tg.MouseClick += new MouseEventHandler(tc_MouseClick);
            tg.MouseDown += new MouseEventHandler(tc_MouseDown);
            tg.MouseDoubleClick += new MouseEventHandler(tc_MouseDoubleClick);
            tg.MouseUp += new MouseEventHandler(tc_MouseUp);
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
            var elems = tg.ElementsAtTime(TimeSpan.FromSeconds(2.0));
            _logMessage("Elements at 2 sec: " + _elementList(elems));
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
            tg.Rows[0].Elements[0].Offset = TimeSpan.FromSeconds(1.5);
            tg.Rows[0].Elements[0].Duration = TimeSpan.FromSeconds(3.5);

            tg.Rows[1].Elements[0].Offset = TimeSpan.FromSeconds(0);
            tg.Rows[1].Elements[0].Duration = TimeSpan.FromSeconds(2);
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
			_logMessage("tc total time: " + tg.TotalTime);
			_logMessage("tc visible time span: " + tg.VisibleTimeSpan);
			_logMessage("tc visible time start: " + tg.VisibleTimeStart);
			_logMessage("tc visible time end: " + tg.VisibleTimeEnd);
		}

		private void buttonGenericDebug2_Click(object sender, EventArgs e)
		{
			timelineControl1.VisibleTimeStart = TimeSpan.FromSeconds(5);
		}
    }


}
