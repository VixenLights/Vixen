using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.OpenGL;
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
			// Create a temporary prop model
			LightPropModel = new TPropModel();
			List<IPropModel> propModels = new List<IPropModel>();
			propModels.Add(LightPropModel);
			
			// Create the prop drawing engine
			DrawingEngine = new OpenGLPropDrawingEngine(propModels);

			// Create the collection of view model rotations
			Rotations = new();

			// Create the X Axis rotation view model
			AxisRotationViewModel xRotation = new AxisRotationViewModel();
			xRotation.Axis = "X";
			xRotation.RotationChanged += OnRotationChanged;
			Rotations.Add(xRotation);

			// Create the Y Axis rotation view model
			AxisRotationViewModel yRotation = new AxisRotationViewModel();
			yRotation.Axis = "Y";
			yRotation.RotationChanged += OnRotationChanged;
			Rotations.Add(yRotation);

			// Create the Z Axis rotation view model
			AxisRotationViewModel zRotation = new AxisRotationViewModel();
			zRotation.Axis = "Z";
			zRotation.RotationChanged += OnRotationChanged;
			Rotations.Add(zRotation);
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Light prop model used to generate the graphics.
		/// </summary>
		protected TPropModel LightPropModel { get; set; }

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
		/// Converts from axis string to enumeration.
		/// </summary>
		/// <param name="axis">String to convert</param>
		/// <returns>Equivalent enumeration of the string</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Axis GetAxis(string axis)
		{
			return axis switch
			{
				"X" => Axis.XAxis,
				"Y" => Axis.YAxis,
				"Z" => Axis.ZAxis,
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		/// <summary>
		/// Event handler for when a prop rotation changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OnRotationChanged(object sender, EventArgs e)
		{
			// Update the prop nodes
			LightPropModel.UpdatePropNodes();
		}

		#endregion
	}
}
