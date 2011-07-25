using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
    public partial class Form1 : Form
    {
        TimelineControl tc;


        public Form1()
        {
            InitializeComponent();

            // Timeline
            tc = new TimelineControl();
            tc.Location = new Point(50, 50);
            tc.Size = new Size(800, 400);
            tc.BorderStyle = BorderStyle.Fixed3D;
            tc.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            this.Controls.Add(tc);

            // Rows
            tc.Rows.Add(new TimelineRow() { Height=50, Tag="Row 0" });
            tc.Rows.Add(new TimelineRow() { Height=100, Tag="Row 1" });

            // Elements
            tc.Rows[0].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Red,
                    Offset = TimeSpan.FromSeconds(1.5),
                    Duration = TimeSpan.FromSeconds(3.5),
                    Tag = "Red"
                }
            );

            tc.Rows[1].Elements.Add(new TimelineElement()
                {
                    BackColor = Color.Green,
                    Offset = TimeSpan.FromSeconds(0),
                    Duration = TimeSpan.FromSeconds(2),
                    Tag = "Green"
                }
            );
            


            tc.ElementsMoved += new EventHandler<ElementMovedEventArgs>(tc_ElementsMoved);
        }

        void tc_ElementsMoved(object sender, ElementMovedEventArgs e)
        {
            StringBuilder s = new StringBuilder("Elements moved:\n");
            foreach (TimelineElement elem in e.MovedElements)
                s.AppendLine(elem.Tag.ToString());
            MessageBox.Show(s.ToString());
        }

        private void buttonElemAt_Click(object sender, EventArgs e)
        {
            StringBuilder s = new StringBuilder("Elements at 2 sec:\n");
            foreach (TimelineElement elem in tc.ElementsAtTime(TimeSpan.FromSeconds(2.0)))
                s.AppendLine(elem.Tag.ToString());
            MessageBox.Show(s.ToString());
        }



        private void buttonReset_Click(object sender, EventArgs e)
        {
            tc.Rows[0].Elements[0].Offset = TimeSpan.FromSeconds(1.5);
            tc.Rows[0].Elements[0].Duration = TimeSpan.FromSeconds(3.5);

            tc.Rows[1].Elements[0].Offset = TimeSpan.FromSeconds(0);
            tc.Rows[1].Elements[0].Duration = TimeSpan.FromSeconds(2);
        }
    }


}
