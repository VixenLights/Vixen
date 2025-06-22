using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys.Props.Components;

namespace VixenApplication.SetupDisplay.ViewModels
{

	/// <summary>
	/// Represents the view model for a prop component in the setup display.
	/// </summary>
	/// <remarks>
	/// This class provides functionality for managing and interacting with a prop component, 
	/// including its properties, target nodes, and selection/expansion states. It implements 
	/// <see cref="ISelectableExpander"/> and <see cref="IEquatable{T}"/> to support selection, 
	/// expansion, and equality comparison. It also utilizes delayed fulfillment of the wrapper models for child nodes
	/// and only creates them when the node is expanded.
	/// </remarks>
	public class PropComponentViewModel : ViewModelBase, ISelectableExpander, IEquatable<PropComponentViewModel>
	{
		public PropComponentViewModel(IPropComponent propComponent)
		{
			ArgumentNullException.ThrowIfNull(propComponent, nameof(propComponent));
			PropComponent = propComponent;
			TargetNodes = new FastObservableCollection<ElementNodeViewModel?>([null]);
			
			propComponent.PropertyChanged += PropComponent_PropertyChanged;
		}

		private void PropComponent_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (nameof(TargetNodes).Equals(e.PropertyName))
			{
				if (!IsExpanded && _childrenPopulated)
				{
					_childrenPopulated = false;
					return;
				}
				
				if (IsExpanded && _childrenPopulated)
				{
					TargetNodes.Clear();
					TargetNodes.AddItems(PropComponent.TargetNodes.Select(x => new ElementNodeViewModel(x)));
				}
			}
		}

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel]
		public String Name
		{
			get { return GetValue<String>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<String>(nameof(Name));

		#endregion

		#region ComponentType property

		/// <summary>
		/// Gets or sets the ComponentType value.
		/// </summary>
		[ViewModelToModel]
		public PropComponentType ComponentType
		{
			get { return GetValue<PropComponentType>(PropComponentTypeProperty); }
			set { SetValue(PropComponentTypeProperty, value); }
		}

		/// <summary>
		/// ComponentType property data.
		/// </summary>
		public static readonly IPropertyData PropComponentTypeProperty = RegisterProperty<PropComponentType>(nameof(ComponentType));

		#endregion

		#region PropComponent model property

		/// <summary>
		/// Gets or sets the PropComponent value.
		/// </summary>
		[Model]
		public IPropComponent PropComponent
		{
			get { return GetValue<IPropComponent>(PropComponentProperty); }
			set { SetValue(PropComponentProperty, value); }
		}

		/// <summary>
		/// PropComponent property data.
		/// </summary>
		public static readonly IPropertyData PropComponentProperty = RegisterProperty<IPropComponent>(nameof(PropComponent));

		#endregion

		#region TargetNodes property

		/// <summary>
		/// Gets or sets the TargetNodes value.
		/// </summary>
		public FastObservableCollection<ElementNodeViewModel?> TargetNodes
		{
			get { return GetValue<FastObservableCollection<ElementNodeViewModel?>>(TargetNodesProperty); }
			set { SetValue(TargetNodesProperty, value); }
		}

		/// <summary>
		/// TargetNodes property data.
		/// </summary>
		public static readonly IPropertyData TargetNodesProperty = RegisterProperty<FastObservableCollection<ElementNodeViewModel?>>(nameof(TargetNodes));

		#endregion

		public bool IsSelected { get; set; }
		
		#region IsExpanded property

		private bool _childrenPopulated = false;
		/// <summary>
		/// Gets or sets the IsExpanded value.
		/// </summary>
		public bool IsExpanded
		{
			get { return GetValue<bool>(IsExpandedProperty); }
			set
			{
				if (value && !_childrenPopulated)
				{
					TargetNodes.Clear();
					//Populate Child VM
					TargetNodes.AddItems(PropComponent.TargetNodes.Select(x => new ElementNodeViewModel(x)));
					_childrenPopulated = true;
				}
				SetValue(IsExpandedProperty, value);
			}
		}

		/// <summary>
		/// IsExpanded property data.
		/// </summary>
		public static readonly IPropertyData IsExpandedProperty = RegisterProperty<bool>(nameof(IsExpanded));

		#endregion

		public bool IsGroup => PropComponent.TargetNodes.Any();

		public bool Equals(PropComponentViewModel? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return PropComponent.Id == other.PropComponent.Id;
		}
	}
}