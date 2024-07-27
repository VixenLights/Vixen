namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Configures preview Moving Head intelligent fixtures.
	/// </summary>
	public class PreviewMovingHeadSetupControl : PreviewShapeBaseSetupControl
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="shape">Shape being configured</param>
		public PreviewMovingHeadSetupControl(PreviewBaseShape shape) : base(shape)
		{
		}

		#endregion

		#region Protected Methods

		/// <inheritdoc/>
		protected override void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			base.PropertyGrid_PropertyValueChanged(s, e);

			// Have the shape validate the edit
			((PreviewMovingHead)Shape).Validate(e.ChangedItem.Label);
		}

		#endregion
	}
}