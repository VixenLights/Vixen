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

		/// <summary>
		/// Retrieves the 2-D node points that make up the prop.
		/// </summary>
		/// <returns>2-D note points that make up the prop</returns>
		protected abstract IEnumerable<NodePoint> Get2DNodePoints();

		#endregion

		#region Public Properties

		private ObservableCollection<NodePoint> _nodes = new();

		public ObservableCollection<NodePoint> Nodes
		{
			get => _nodes;
			set => SetProperty(ref _nodes, value);
		}

		private ObservableCollection<NodePoint> _threeDNodes = new();

		public ObservableCollection<NodePoint> ThreeDNodes
		{
			get => _threeDNodes;
			set => SetProperty(ref _threeDNodes, value);
		}

		public ObservableCollection<AxisRotationModel> Rotations { get; set; }
		#endregion

		#region Public Methods

		public void UpdatePropNodes()
		{
			ThreeDNodes.Clear();
			ThreeDNodes.AddRange(Get3DNodePoints());
		}

		#endregion
	}
}
