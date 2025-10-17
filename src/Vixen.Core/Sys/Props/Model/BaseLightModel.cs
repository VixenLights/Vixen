#nullable enable
using System.Collections.ObjectModel;
using Vixen.Extensions;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Maintains a base light model.
	/// </summary>
	public abstract class BaseLightModel : BasePropModel, ILightPropModel
	{
		#region Protected Methods
		/// <summary>
		/// Updates the nodes when a model property changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void PropertyModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.			
			Nodes.Clear();
			Nodes.AddRange(Get3DNodePoints());
		}

		/// <summary>
		/// Rotates the NodePoints around the center of a 0,1 matrix.
		/// </summary>
		/// <param name="nodePoints"></param>
		/// <param name="angleInDegrees"></param>
		protected static void RotateNodePoints(List<NodePoint> nodePoints, int angleInDegrees)
		{
			double centerX = .5;
			double centerY = .5;
			double angleInRadians = angleInDegrees * (Math.PI / 180);
			double cosTheta = Math.Cos(angleInRadians);
			double sinTheta = Math.Sin(angleInRadians);
			foreach (var nodePoint in nodePoints)
			{
				double x =
					cosTheta * (nodePoint.X - centerX) -
						sinTheta * (nodePoint.Y - centerY);
				 double y =
					sinTheta * (nodePoint.X - centerX) +
					 cosTheta * (nodePoint.Y - centerY);

				nodePoint.X = x + centerX;
				nodePoint.Y = y + centerY;
			}
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Sets the context data for the prop.
		/// </summary>
		/// <param name="data">The context of the parent class.</param>
		public abstract void SetContext(object data);

		/// <summary>
		/// Retrieves the 3-D node points that make up the prop.
		/// </summary>
		/// <returns>3-D note points that make up the prop</returns>
		protected abstract IEnumerable<NodePoint> Get3DNodePoints();
		
		#endregion

		#region Public Properties
				
		private ObservableCollection<NodePoint> _nodes = new();

		public ObservableCollection<NodePoint> Nodes
		{
			get => _nodes;
			set => SetProperty(ref _nodes, value);
		}

		private int _rotationAngle;

		/// <summary>
		/// The angle at which the core Prop is rotated.
		/// </summary>
		public int RotationAngle
		{
			get => _rotationAngle;
			set => SetProperty(ref _rotationAngle, value);
		}

		public ObservableCollection<AxisRotationModel> Rotations { get; set; }
		#endregion

		#region Public Methods

		public void UpdatePropNodes()
		{
			Nodes.Clear();
			Nodes.AddRange(Get3DNodePoints());
		}

		#endregion
	}
}
