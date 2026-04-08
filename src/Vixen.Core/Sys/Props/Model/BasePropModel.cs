using OpenTK.Mathematics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Vixen.Model;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Abstract base class for prop model.
	/// </summary>
	public abstract class BasePropModel : BindableBase, IPropModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		protected BasePropModel()
		{
			// Register for model property changes
			PropertyChanged += PropertyModelChanged;

			// Initialize the Axis rotations
			AxisRotations = new ObservableCollection<AxisRotationModel>();
			AxisRotations.Add(new AxisRotationModel() { Axis = Axis.XAxis, RotationAngle = 0 });
			AxisRotations.Add(new AxisRotationModel() { Axis = Axis.YAxis, RotationAngle = 0 });
			AxisRotations.Add(new AxisRotationModel() { Axis = Axis.ZAxis, RotationAngle = 0 });
		}

		#endregion

		#region IPropModel

		/// <inheritdoc/>		
		public Guid Id { get; init; } = Guid.NewGuid();


		private ObservableCollection<AxisRotationModel> _rotations;

		/// <summary>
		/// Collection of axis rotations.
		/// </summary>
		public ObservableCollection<AxisRotationModel> AxisRotations				
		{
			get => _rotations;
			set
			{
				// Handle changes to both the Rotation collection and individual elements within the Rotation collection
				if (ReferenceEquals(_rotations, value))
				{
					return;
				}

				if (_rotations != null)
				{
					_rotations.CollectionChanged -= Rotations_CollectionChanged;
					UnsubscribeFromItems(_rotations);
				}

				_rotations = value;

				if (_rotations != null)
				{
					_rotations.CollectionChanged += Rotations_CollectionChanged;
					SubscribeToItems(_rotations);
				}

				OnPropertyChanged(nameof(AxisRotations));
			}
        }

		#endregion

		#region Public Methods

		/// <summary>
		/// Updates prop nodes for Axis rotation changes.
		/// </summary>
		public virtual void UpdatePropNodes()
		{
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Allows derived models to update calculated state when a model property changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected abstract void PropertyModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e);

		/// <summary>
		/// Rotates the specified vertices by up to three axis rotations.
		/// </summary>
		/// <param name="vertices">Vertices to rotate</param>
		protected void RotatePoints(List<NodePoint> vertices, ObservableCollection<AxisRotationModel> rotations)
		{
			if (rotations == null)
			{
				return;
			}

			// Loop over the rotations because order matters
			foreach (AxisRotationModel rm in rotations)
			{
				// If there is a rotation angle then...
				if (rm.RotationAngle != 0.0)
				{
					// Create the applicable rotation matrix
					Matrix4 rotation = GetRotationMatrix(rm.Axis, rm.RotationAngle);

					// Loop over the vertices
					foreach (NodePoint pt in vertices)
					{
						// Convert the vertice to an OpenTK Vector
						Vector4 point4 = new Vector4((float)pt.X, (float)pt.Y, (float)pt.Z, 1.0f); // w = 1 for position

						// Rotate the vertex
						Vector4 rotatedPoint4 = rotation * point4;

						// Push the data back into the NodePoint
						pt.X = rotatedPoint4.X;
						pt.Y = rotatedPoint4.Y;
						pt.Z = rotatedPoint4.Z;
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets a rotation matrix for the specified axis and degrees of rotation.
		/// </summary>
		/// <param name="axis">Axis to rotate around</param>
		/// <param name="degrees">Amount to rotate in degrees</param>
		/// <returns>Rotation matrix for the specified axis</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Matrix4 GetRotationMatrix(Axis axis, float degrees)
		{
			float radians = MathHelper.DegreesToRadians(degrees);
			return axis switch
			{
				Axis.XAxis => Matrix4.CreateRotationX(radians),
				Axis.YAxis => Matrix4.CreateRotationY(radians),
				Axis.ZAxis => Matrix4.CreateRotationZ(radians),
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		/// <summary>
		/// Event handler for when the AxisRotations collection changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Rotations_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			// If items have been removed then...
			if (e.OldItems != null)
			{
				// Unsubscribe from events from the removed items
				UnsubscribeFromItems(e.OldItems.Cast<AxisRotationModel>());
			}

			// If new items have been added then...
			if (e.NewItems != null)
			{ 
				// Subscribe to changed events from the added items
				SubscribeToItems(e.NewItems.Cast<AxisRotationModel>());
			}

			// Update the prop nodes based on the rotation
			UpdatePropNodes();
		}

		/// <summary>
		/// Subscribes to PropertyChanged events from the items in the specified collection.
		/// </summary>
		/// <param name="items">New items to register to events from</param>
		private void SubscribeToItems(IEnumerable<AxisRotationModel> items)
		{
			// Loop over the items
			foreach (AxisRotationModel item in items)
			{
				// Register for changed events
				item.PropertyChanged += RotationItem_PropertyChanged;
			}
		}

		/// <summary>
		/// Unsubscribes to PropertyChanged events from the items in the specified collection.
		/// </summary>
		/// <param name="items">Removed items to un-register events from</param>
		private void UnsubscribeFromItems(IEnumerable<AxisRotationModel> items)
		{
			// Loop over the items
			foreach (AxisRotationModel item in items)
			{
				// Un-register for change events
				item.PropertyChanged -= RotationItem_PropertyChanged;
			}
		}

		/// <summary>
		/// Property changed event for AxisRotation models.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void RotationItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AxisRotationModel.RotationAngle) ||
				e.PropertyName == nameof(AxisRotationModel.Axis))
			{
				UpdatePropNodes();
			}
		}

		#endregion
	}
}
