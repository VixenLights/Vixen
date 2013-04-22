using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    public partial class PreviewMegaTreeSetupControl : DisplayItemBaseControl
    {
        private PreviewMegaTree _tree;

        public PreviewMegaTreeSetupControl(DisplayItem displayItem): base(displayItem)
        {
            InitializeComponent();
            _displayItem = displayItem;
            _tree = _displayItem.Shape as PreviewMegaTree;
            Setup();

            displayItem.Shape.OnPropertiesChanged += OnPropertiesChanged;
        }

        ~PreviewMegaTreeSetupControl()
        {
            _displayItem.Shape.OnPropertiesChanged -= OnPropertiesChanged;
        }

        private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
        {
        }

        private void Setup()
        {
            trackBarPixelSize.Value = _tree.PixelSize;
            numericUpDownPixelSize.Value = _tree.PixelSize;
            trackBarBaseHeight.Value = _tree.BaseHeight;
            numericUpDownBaseHeight.Value = _tree.BaseHeight;
            trackBarTopHeight.Value = _tree.TopHeight;
            numericUpDownTopHeight.Value = _tree.TopHeight;
            trackBarDegrees.Value = _tree.Degrees;
            numericUpDownDegrees.Value = _tree.Degrees;
            trackBarTopWidth.Value = _tree.TopWidth;
            numericUpDownTopWidth.Value = _tree.TopWidth;
            trackBarLightsPerString.Value = _tree.LightsPerString;
            numericUpDownLightsPerString.Value = _tree.LightsPerString;
            trackBarStringCount.Value = _tree.StringCount;
            numericUpDownStringCount.Value = _tree.StringCount;

            numericUpDownPixelSize.Minimum = trackBarPixelSize.Minimum;
            numericUpDownPixelSize.Maximum = trackBarPixelSize.Maximum;
            numericUpDownBaseHeight.Minimum = trackBarBaseHeight.Minimum;
            numericUpDownBaseHeight.Maximum = trackBarBaseHeight.Maximum;
            numericUpDownTopHeight.Minimum = trackBarTopHeight.Minimum;
            numericUpDownTopHeight.Maximum = trackBarTopHeight.Maximum;
            numericUpDownDegrees.Minimum = trackBarDegrees.Minimum;
            numericUpDownDegrees.Maximum = trackBarDegrees.Maximum;
            numericUpDownTopWidth.Minimum = trackBarTopWidth.Minimum;
            numericUpDownTopWidth.Maximum = trackBarTopWidth.Maximum;
            numericUpDownLightsPerString.Minimum = trackBarLightsPerString.Minimum;
            numericUpDownLightsPerString.Maximum = trackBarLightsPerString.Maximum;
            numericUpDownStringCount.Minimum = trackBarStringCount.Minimum;
            numericUpDownStringCount.Maximum = trackBarStringCount.Maximum;

            PreviewTools.ComboBoxSetSelectedText(comboBoxStringType, _tree.StringType.ToString());
        }

        private void buttonLinkElements_Click(object sender, EventArgs e)
        {
            List<PreviewBaseShape> shapes = _displayItem.Shape.Strings;
            PreviewSetElements elementsDialog = new PreviewSetElements(shapes);
            elementsDialog.ShowDialog();
        }

        // Pixel Size
        private void SetPixelSize(int i)
        {
            _tree.PixelSize = i;
            numericUpDownPixelSize.Value = i;
            trackBarPixelSize.Value = i;
            trackBarPixelSize.Invalidate();
        }

        private void numericUpDownPixelSize_ValueChanged(object sender, EventArgs e)
        {
            SetPixelSize((int)numericUpDownPixelSize.Value);
        }

        private void trackBarPixelSize_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetPixelSize(trackBarPixelSize.Value);
        }

        // String Count
        private void SetStringCount(int i)
        {
            _tree.StringCount = i;
            trackBarStringCount.Value = i;
            numericUpDownStringCount.Value = i;
            trackBarStringCount.Invalidate();
        }
        
        private void numericUpDownStringCount_ValueChanged(object sender, EventArgs e)
        {
            SetStringCount((int)numericUpDownStringCount.Value);
        }

        private void trackBarStringCount_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetStringCount(trackBarStringCount.Value);
        }

        // Top Height
        private void SetTopHeight(int i)
        {
            _tree.TopHeight = i;
            numericUpDownTopHeight.Value = i;
            trackBarTopHeight.Value = i;
            trackBarTopHeight.Invalidate();
        }

        private void numericUpDownTopHeight_ValueChanged(object sender, EventArgs e)
        {
            SetTopHeight((int)numericUpDownTopHeight.Value);
        }

        private void trackBarTopHeight_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetTopHeight(trackBarTopHeight.Value);
        }

        // Top Width
        private void SetTopWidth(int i)
        {
            _tree.TopWidth = i;
            numericUpDownTopWidth.Value = i;
            trackBarTopWidth.Value = i;
            trackBarTopWidth.Invalidate();
        }

        private void numericUpDownTopWidth_ValueChanged(object sender, EventArgs e)
        {
            SetTopWidth((int)numericUpDownTopWidth.Value);
        }

        private void trackBarTopWidth_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetTopWidth(trackBarTopWidth.Value);
        }

        // Base Height
        private void SetBaseHeight(int i)
        {
            _tree.BaseHeight = i;
            numericUpDownBaseHeight.Value = i;
            trackBarBaseHeight.Value = i;
            trackBarBaseHeight.Invalidate();
        }

        private void numericUpDownBaseHeight_ValueChanged(object sender, EventArgs e)
        {
            SetBaseHeight((int)numericUpDownBaseHeight.Value);
        }

        private void trackBarBaseHeight_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetBaseHeight(trackBarBaseHeight.Value);
        }

        // Degres
        private void SetDegrees(int i)
        {
            _tree.Degrees = i;
            numericUpDownDegrees.Value = i;
            trackBarDegrees.Value = i;
            trackBarDegrees.Invalidate();
        }

        private void numericUpDownDegrees_ValueChanged(object sender, EventArgs e)
        {
            SetDegrees((int)numericUpDownDegrees.Value);
        }

        private void trackBarDegrees_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetDegrees(trackBarDegrees.Value);
        }

        // Lights Per Sring
        private void SetLightsPerString(int i)
        {
            _tree.LightsPerString = i;
            numericUpDownLightsPerString.Value = i;
            trackBarLightsPerString.Value = i;
            trackBarLightsPerString.Invalidate();
        }

        private void numericUpDownLightsPerString_ValueChanged(object sender, EventArgs e)
        {
            SetLightsPerString((int)numericUpDownLightsPerString.Value);
        }

        private void trackBarLightsPerString_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            SetLightsPerString(trackBarLightsPerString.Value);
        }

        private void comboBoxStringType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStringType.SelectedIndex >= 0)
            {
                string text = comboBoxStringType.Items[comboBoxStringType.SelectedIndex].ToString();
                Console.WriteLine("Selected Text: " + text);
                _tree.StringType = PreviewTools.ParseEnum<PreviewBaseShape.StringTypes>(text);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Shapes.PreviewTools.ShowHelp(Properties.Settings.Default.Help_MegaTree);
        }

    }
}
