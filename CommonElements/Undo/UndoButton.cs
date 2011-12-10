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
        private UndoDropDownControl m_dropControl = null;

        private const int SetWidth = 140;
        private const int SetHeight = 200;

        public UndoButton()
        {
            // Initialize the custom control
            m_dropControl = new UndoDropDownControl() {
                MinimumSize = new Size(SetWidth, SetHeight)  // <- important
            };

            // ...hosted by a ToolStripControlHost
            m_toolHost = new ToolStripControlHost(m_dropControl) {
                Size = new Size(SetWidth, SetHeight),
                Margin = new Padding(0)
            };

            // ... and shown in a ToolStripDropDown.
            m_toolDrop = new ToolStripDropDown() {
                Padding = new Padding(0)
            };
            m_toolDrop.Items.Add(m_toolHost);

            this.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ButtonType = UndoButtonType.UndoButton;    // Default.

            // There is no OnDropDownOpening to override, so I guess we have to do it this way.
            this.DropDownOpening += UndoButton_DropDownOpening;
        }

        public UndoButtonType ButtonType
        {
            get { return m_dropControl.ButtonType; }
            set
            {
                m_dropControl.ButtonType = value;
                switch (value) {
                    case UndoButtonType.UndoButton:
                        this.Image = Resources.Edit_UndoHS;
                        break;

                    case UndoButtonType.RedoButton:
                        this.Image = Resources.Edit_RedoHS;
                        break;
                }
            }
        }

        public ListBox.ObjectCollection UndoItems
        {
            get { return m_dropControl.Items; }
        }

        
        private void UndoButton_DropDownOpening(object sender, EventArgs e)
        {
            m_toolDrop.Show(this.Parent, new Point(this.Bounds.Left, this.Bounds.Bottom));
            m_dropControl.Focus();
        }

    }

    public enum UndoButtonType
    {
        UndoButton,
        RedoButton,
    }
}
