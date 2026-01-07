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

		/// <summary>
		/// Converts the enumeration into a display string.
		/// </summary>
		/// <param name="axis">Enumeration to convert</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ConvertAxis(Vixen.Sys.Props.Model.Axis axis)
		{
			Axis = axis switch
			{
				Vixen.Sys.Props.Model.Axis.XAxis => "X",
				Vixen.Sys.Props.Model.Axis.YAxis => "Y",
				Vixen.Sys.Props.Model.Axis.ZAxis => "Z",
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}


		public static readonly IPropertyData AxisProperty = RegisterProperty<string>(nameof(Axis));

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

		public static readonly IPropertyData RotationAngleProperty = RegisterProperty<int>(nameof(RotationAngle));

		#endregion

		#region Events

		/// <summary>
		/// Event when one of the rotation properties have changed.
		/// </summary>
		public event EventHandler RotationChanged;

		#endregion

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

	}
}
