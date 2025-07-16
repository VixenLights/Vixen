#nullable enable
using System.Collections.ObjectModel;
using Vixen.Model;

namespace Vixen.Sys.Props.Model
{
	public abstract class BaseLightModel : BindableBase, ILightPropModel
	{
		private int _rotationAngle;
		private ObservableCollection<NodePoint> _nodes = new();

		public Guid Id { get; init; } = Guid.NewGuid();

		public ObservableCollection<NodePoint> Nodes
		{
			get => _nodes;
			set => SetProperty(ref _nodes, value);
		}

		/// <summary>
		/// The angle at which the core Prop is rotated.
		/// </summary>
		public int RotationAngle
		{
			get => _rotationAngle;
			set => SetProperty(ref _rotationAngle, value);
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
				nodePoint.X =
					cosTheta * (nodePoint.X - centerX) -
						sinTheta * (nodePoint.Y - centerY) + centerX;
				nodePoint.Y =
					sinTheta * (nodePoint.X - centerX) +
					 cosTheta * (nodePoint.Y - centerY) + centerY;

			}
		}


	}
}
