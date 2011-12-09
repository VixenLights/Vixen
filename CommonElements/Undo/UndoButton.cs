using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace CommonElements
{
    public class UndoButton : ToolStripSplitButton
    {
        private ToolStripDropDown m_toolDrop = null;
        private ToolStripControlHost m_toolHost = null;
        private Control m_dropControl = null;

        public UndoButton()
        {
            this.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.Image = UndoButtonTest.Properties.Resources.Edit_UndoHS;
            this.Image = Resources.Edit_UndoHS;

            // There is no OnDropDownOpening to override, so I guess we have to do it this way.
            this.DropDownOpening += UndoButton_DropDownOpening;

            //m_dropControl = InitListBox();
            m_dropControl = InitCustomControl();

            m_toolHost = new ToolStripControlHost(m_dropControl);
            m_toolHost.Size = new Size(120, 120);
            m_toolHost.Margin = new Padding(0);

            m_toolDrop = new ToolStripDropDown();
            m_toolDrop.Padding = new Padding(0);
            m_toolDrop.Items.Add(m_toolHost);
            m_toolDrop.MouseWheel += m_toolDrop_MouseWheel;
        }


        void m_toolDrop_MouseWheel(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Wheel");
        }

        private Control InitCustomControl()
        {
            UndoDropDownControl c = new UndoDropDownControl();
            c.MinimumSize = new Size(200, 120);

            for (int i = 0; i < 30; i++)
                c.Items.Add("Item " + i.ToString());
            c.Caption = "Undo X items";

            return c;
        }

        private Control InitListBox()
        {
            ListBox lb = new ListBox();
            lb.IntegralHeight = false;
            lb.MinimumSize = new Size(120, 120);  // <- important
            for (int i = 0; i < 10; i++)
                lb.Items.Add("Item " + i.ToString());
            return lb;
        }
        
        private void UndoButton_DropDownOpening(object sender, EventArgs e)
        {
            m_toolDrop.Show(this.Parent, new Point(this.Bounds.Left, this.Bounds.Bottom));
            m_dropControl.Focus();
        }


    }
}
