using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewShapeBaseSetupControl : DisplayItemBaseControl
	{
		public PreviewShapeBaseSetupControl(PreviewBaseShape shape) : base(shape)
		{
			InitializeComponent();
			Title = $"{shape.TypeName} Properties";
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonHelp.Image = Tools.GetIcon(Resources.help, iconSize);
			ThemeUpdateControls.UpdateControls(this);
			ThemePropertyGridRenderer.PropertyGridRender(propertyGrid);
			propertyGrid.SelectedObject = Shape;
			propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
			Shape.OnPropertiesChanged += OnPropertiesChanged;
			if (ScalingTools.GetScaleFactor() >= 2)
			{
				propertyGrid.LargeButtons = true;
			}
		}

		private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			// Have the shape validate the edit
			Shape.Validate(e.ChangedItem.Label);

			OnPropertyEdited();
		}

		~PreviewShapeBaseSetupControl()
		{
			Shape.OnPropertiesChanged -= OnPropertiesChanged;
		}

		private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
		{
            if (InvokeRequired)
            {
                BeginInvoke(new Action(propertyGrid.Refresh));
			}
            else
            {
                propertyGrid.Refresh();
			}
        }

		protected virtual void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_BasicShapes);
		}
	}

}