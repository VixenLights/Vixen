using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models
{
	/// <summary>
	/// Maintains a rotation around a coordinate axis.
	/// </summary>
	public class AxisRotationViewModel : ViewModelBase
	{
		#region Public Properties
		
		/// <summary>
		/// Collection of available axis.
		/// </summary>
		public List<string> Axes { get; } = new List<string> { "X", "Y", "Z" };

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Axis to rotate around.
		/// </summary>
		public string Axis
		{
			get { return GetValue<string>(AxisProperty); }
			set
			{
				SetValue(AxisProperty, value);

				RotationChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		private static readonly IPropertyData AxisProperty = RegisterProperty<string>(nameof(Axis));

		/// <summary>
		/// Amount of rotation in degrees.
		/// </summary>
		public int RotationAngle
		{
			get { return GetValue<int>(RotationAngleProperty); }
			set
			{
				SetValue(RotationAngleProperty, value);
				RotationChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		private static readonly IPropertyData RotationAngleProperty = RegisterProperty<int>(nameof(RotationAngle));

		public int RotationAngleDefault
		{
			get { return GetValue<int>(RotationAngleDefaultProperty); }
			set { SetValue(RotationAngleDefaultProperty, value); }
		}
		private static readonly IPropertyData RotationAngleDefaultProperty = RegisterProperty<int>(nameof(RotationAngleDefault));
		#endregion

		#region Events

		/// <summary>
		/// Event when one of the rotation properties have changed.
		/// </summary>
		public event EventHandler? RotationChanged;

		#endregion

		/// <summary>
		/// Convert rotation object from ViewModel to Model
		/// </summary>
		/// <param name="rotations">Specifies the Rotation object in ViewModel format</param>
		/// <returns>Returns an <see cref="System.Collections.ObjectModel.ObservableCollection{AxisRotationModel}"/>
		/// that specifies the Model version of the rotations.</returns>
		public static ObservableCollection<AxisRotationModel> ConvertToModel(ObservableCollection<AxisRotationViewModel> rotations)
		{
			// Transfer the rotations from the view model to the model
			var models = new ObservableCollection<AxisRotationModel>();
			if (rotations != null)
			{
				foreach (AxisRotationViewModel rotationViewModel in rotations)
				{
					AxisRotationModel rotationMdl = new();
					rotationMdl.ConvertAxis(rotationViewModel.Axis);
					rotationMdl.RotationAngle = rotationViewModel.RotationAngle;
					models.Add(rotationMdl);
				}
			}
			return models;
		}

		/// <summary>
		/// Convert rotation object from Model to ViewModel
		/// </summary>
		/// <param name="rotations">Specifies the Rotation object in Model format</param>
		/// <returns>Returns an <see cref="System.Collections.ObjectModel.ObservableCollection{AxisRotationViewModel}"/>
		/// that specifies the ViewModel version of the rotations.</returns>
		public static ObservableCollection<AxisRotationViewModel> ConvertToViewModel(ObservableCollection<AxisRotationModel> rotations)
		{
			// Transfer the rotations from the model to the view model
			var models = new ObservableCollection<AxisRotationViewModel>();
			if (rotations != null)
			{
				foreach (AxisRotationModel rotationModel in rotations)
				{
					models.Add(new AxisRotationViewModel() { Axis = AxisRotationModel.ConvertAxis(rotationModel.Axis), RotationAngle = rotationModel.RotationAngle });
				}
			}
			return models;
		}

	}
}
