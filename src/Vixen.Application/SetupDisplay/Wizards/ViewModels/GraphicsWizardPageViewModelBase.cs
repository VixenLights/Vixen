using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.OpenGL;
using VixenApplication.SetupDisplay.Wizards.Factory;
using VixenApplication.SetupDisplay.Wizards.HelperTools;
using VixenApplication.SetupDisplay.Wizards.PropFactories;
using VixenModules.App.Props.Models;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Base class for a wizard page that also displays 3-D prop graphics.
	/// </summary>
	/// <typeparam name="TWizardPage">Type of wizard page</typeparam>
	/// <typeparam name="TPropModel">Type of prop model</typeparam>
	public class GraphicsWizardPageViewModelBase<TWizardPage, TPropModel> : WizardPageViewModelBase<TWizardPage>
		where TWizardPage : class, IWizardPage
		where TPropModel : class, ILightPropModel, new()	
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wizardPage">Wizard page model</param>
		protected GraphicsWizardPageViewModelBase(TWizardPage wizardPage) : base(wizardPage)
		{
			LightPropModel = new TPropModel();
			List<IPropModel> propModels = new List<IPropModel>();
			propModels.Add(LightPropModel);

			// Create the prop drawing engine
			DrawingEngine = new OpenGLPropDrawingEngine(propModels);

			foreach (var rotation in Rotations)
			{
				rotation.RotationChanged += OnRotationChanged;
			}
		}

		#endregion

		#region Protected Properties

		public TPropModel LightPropModel { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of rotations to support rotating the props around the x,y, and z axis.
		/// </summary>
		[ViewModelToModel]
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get
			{
				return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
			}
			set
			{
				SetValue(RotationsProperty, value);
			}
		}

		public static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

		/// <summary>
		/// OpenGL prop drawing engine.
		/// </summary>
		public OpenGLPropDrawingEngine DrawingEngine { get; set; } = new OpenGLPropDrawingEngine(new List<IPropModel>());

		#endregion

		#region Private Methods
		/// <summary>
		/// Event handler for when a prop rotation changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OnRotationChanged(object sender, EventArgs e)
		{
			// Get the changed axis and the axis it now duplicates, if any
			var newRotation = sender as AxisRotationViewModel;
			if (newRotation == null)
			{
				return;
			}

			var duplicateRotation = Rotations.FirstOrDefault(x => x != newRotation && x.Axis == newRotation.Axis);

			// If there is an axis duplication (i.e. two axis have the same plane), then...
			if (duplicateRotation != null)
			{
				// Find the Axis that is no longer specified
				var otherRotation = Rotations.FirstOrDefault(x => x != newRotation && x != duplicateRotation);
				if (otherRotation == null)
				{
					return;
				}
				var missingAxis = newRotation.Axes.FirstOrDefault(x => x != duplicateRotation.Axis && x != otherRotation.Axis);

				// Then assign the missing axis to the duplicated plane and swap the rotations between the new and duplicated axes
				duplicateRotation.Axis = missingAxis;
				(duplicateRotation.RotationAngle, newRotation.RotationAngle) = (newRotation.RotationAngle, duplicateRotation.RotationAngle);
			}

			LightPropModel.PropParameters.Update("Rotations", AxisRotationViewModel.ConvertToModel(Rotations));

			// Update the prop nodes
			LightPropModel.UpdatePropNodes();
		}
		#endregion
	}
}
