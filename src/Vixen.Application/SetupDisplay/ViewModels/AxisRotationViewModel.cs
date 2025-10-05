using Catel.Data;
using Catel.MVVM;

namespace VixenApplication.SetupDisplay.ViewModels
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
	}
}
