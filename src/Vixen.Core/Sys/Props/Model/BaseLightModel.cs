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
		protected override void PropertyModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.			
			Nodes.Clear();
			Nodes.AddRange(Get3DNodePoints());
		}

		#endregion

		#region Abstract Methods
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

		private int _lightSize = 2;
		public int LightSize
		{
			get => _lightSize;
			set => SetProperty(ref _lightSize, value);
		}

		#endregion

		#region Public Methods

		public override void UpdatePropNodes()
		{
			Nodes.Clear();
			Nodes.AddRange(Get3DNodePoints());
		}

		#endregion
	}
}
