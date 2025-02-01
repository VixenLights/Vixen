using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview
{
	public partial class ResizePreviewForm : BaseForm
	{
		private int _origWidth, _origHeight;
		private int _newWidth, _newHeight;
        private bool _lockAspect = true;

		public ResizePreviewForm(int width, int height, bool scaleShapes)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			_origWidth = width;
			_origHeight = height;
			_scaleShapes.Checked = scaleShapes; 
		}

		public new int Width
		{
			get { return _newWidth; }
		}

		public new int Height
		{
			get { return _newHeight; }
		}

		public bool ScaleShapes
		{
			get
			{
				return _scaleShapes.Checked;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_newWidth = (int) numericWidth.Value;
			_newHeight = (int) numericHeight.Value;
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ResizePreviewForm_Load(object sender, EventArgs e)
		{
			numericWidth.Value = _origWidth;
			numericHeight.Value = _origHeight;
			labelWidth.Text = _origWidth.ToString();
			labelHeight.Text = _origHeight.ToString();
            SetupAspect();
        }

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Background);
		}

        private void pictureBoxLock_Click(object sender, EventArgs e)
        {
            _lockAspect = !_lockAspect;
            SetupAspect();
        }

        private void SetupAspect()
        {
            if (_lockAspect)
            {
                pictureBoxLock.Image = imageListLocks.Images["link"];
                numericHeight.Enabled = false;
            }
            else
            {
                pictureBoxLock.Image = imageListLocks.Images["unlink"];
                numericHeight.Enabled = true;
            }
        }

        private void numericWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numericHeight.Value < 10) numericHeight.Value = 10;
            if (_lockAspect) { 
                double aspect = (double)numericWidth.Value / (double)_origWidth;
                numericHeight.Value = (int)((double)_origHeight * aspect);
            }
        }
	}
}