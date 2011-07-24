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
            tc.AutoScroll = true;

            // Row
            var tr1 = new TimelineRow();
            tr1.Height = 50;
            tr1.Tag = "Row 1";
            tc.Rows.Add(tr1);

            var tr2 = new TimelineRow();
            tr2.Height = 75;
            tr2.Tag = "Row 2";
            tc.Rows.Add(tr2);

            // Element
            var te = new TimelineElement();
            te.BackColor = Color.Red;
            te.Duration = TimeSpan.FromSeconds(3.5);
            te.Offset = TimeSpan.FromSeconds(1.5);
            te.Tag = "Red te";
            tr2.Elements.Add(te);

            var te2 = new TimelineElement();
            te2.BackColor = Color.LightGreen;
            te2.Duration = TimeSpan.FromSeconds(1.5);
            te2.Offset = TimeSpan.FromSeconds(1.5);
            te2.Tag = "Green te2";
            tr1.Elements.Add(te2);


            this.Controls.Add(tc);


            tc.ElementsMoved += new EventHandler<ElementMovedEventArgs>(tc_ElementsMoved);
        }

        void tc_ElementsMoved(object sender, ElementMovedEventArgs e)
        {
            StringBuilder s = new StringBuilder("Elements moved:\n");
            foreach (TimelineElement elem in e.MovedElements)
                s.AppendLine(elem.Tag.ToString());
            MessageBox.Show(s.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder s = new StringBuilder("Elements at 2 sec:\n");
            foreach (TimelineElement elem in tc.ElementsAtTime(TimeSpan.FromSeconds(2.0)))
                s.AppendLine(elem.Tag.ToString());
            MessageBox.Show(s.ToString());
        }
    }


}
