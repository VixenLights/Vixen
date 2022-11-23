using System.Windows.Controls;
using VixenModules.Editor.FixtureGraphics.WPF;

namespace VixenModules.Editor.FixtureWizard.Wizard.Views
{
	/// <summary>
	/// This class is responsible for drawing two moving heads on the control.
    /// These moving heads are strictly to add visual interest to the user control.
	/// </summary>
	public class WizardUserControlBase : Catel.Windows.Controls.UserControl
    {
		#region Fields

        /// <summary>
        /// Left moving head.
        /// </summary>
		private MovingHeadWPF _leftMovingHead = new MovingHeadWPF();
        
        /// <summary>
        /// Right moving head.
        /// </summary>
        private MovingHeadWPF _rightMovingHead = new MovingHeadWPF();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Displays two moving head fixtures on the user control using the specified viewports.
        /// </summary>
        /// <param name="size">Width and height of the moving head</param>
        /// <param name="color">Color of the light beam</param>
        /// <param name="leftViewport">Left viewport to render in</param>
        /// <param name="rightViewport">Right viewport to render in</param>
        protected void DrawMovingHeads(
            int size,             
            System.Drawing.Color color,
            Viewport3D leftViewport,
            Viewport3D rightViewport)
        {
            // Configure the left moving head
            _leftMovingHead.MovingHead.IncludeLegend = false;
            _leftMovingHead.MovingHead.TiltAngle = 45.0;
            _leftMovingHead.MovingHead.PanAngle = 35.0;
            _leftMovingHead.MovingHead.BeamColor = color;
            _leftMovingHead.MovingHead.BeamLength = 20;
            _leftMovingHead.MovingHead.Focus = 20;
            
            // Draw the left moving head
            _leftMovingHead.DrawFixtureNoBitmap(size, size, 1.0, 0, 0, leftViewport);            

            // Configure the right moving head
            _rightMovingHead.MovingHead.IncludeLegend = false;
            _rightMovingHead.MovingHead.TiltAngle = 45.0;
            _rightMovingHead.MovingHead.PanAngle = 315.0;
            _rightMovingHead.MovingHead.BeamColor = color;
            _rightMovingHead.MovingHead.BeamLength = 20;
            _rightMovingHead.MovingHead.Focus = 20;

            // Draw the right moving head
            _rightMovingHead.DrawFixtureNoBitmap(size, size, 1.0, 0, 0, rightViewport);
        }

        #endregion
    }
}
