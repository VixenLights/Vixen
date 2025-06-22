using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropComponentTreeViewModel : ViewModelBase
	{
		public PropComponentTreeViewModel(IProp prop)
		{
			ArgumentNullException.ThrowIfNull(prop, nameof(prop));
			
			Prop = prop;
			PropComponents = new FastObservableCollection<PropComponentViewModel>(prop.PropComponents.Select(x => new PropComponentViewModel(x)));
			UserComponents = new FastObservableCollection<PropComponentViewModel>(prop.UserDefinedPropComponents.Select(x => new PropComponentViewModel(x)));
			
			prop.PropComponents.CollectionChanged += PropComponents_CollectionChanged;
			prop.UserDefinedPropComponents.CollectionChanged += UserDefinedPropComponents_CollectionChanged;
		}

		#region Prop model property

		/// <summary>
		/// Gets or sets the Prop value.
		/// </summary>
		[Model]
		public IProp Prop
		{
			get { return GetValue<IProp>(PropProperty); }
			set { SetValue(PropProperty, value); }
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly IPropertyData PropProperty = RegisterProperty<IProp>(nameof(Prop));

		#endregion

		private void UserDefinedPropComponents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UserComponents.Clear();
			UserComponents.AddItems(Prop.UserDefinedPropComponents.Select(x => new PropComponentViewModel(x)));
		}

		private void PropComponents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			using (PropComponents.SuspendChangeNotifications(SuspensionMode.None))
			{
				PropComponents.Clear();
				PropComponents.AddItems(Prop.PropComponents.Select(x => new PropComponentViewModel(x)));
			}
		}

		#region PropComponents property

		/// <summary>
		/// Gets or sets the PropComponents value.
		/// </summary>
		public FastObservableCollection<PropComponentViewModel> PropComponents
		{
			get { return GetValue<FastObservableCollection<PropComponentViewModel>>(PropComponentsProperty); }
			set { SetValue(PropComponentsProperty, value); }
		}

		/// <summary>
		/// PropComponents property data.
		/// </summary>
		public static readonly IPropertyData PropComponentsProperty = RegisterProperty<FastObservableCollection<PropComponentViewModel>>(nameof(PropComponents));

		#endregion
		#region UserComponents property

		/// <summary>
		/// Gets or sets the UserComponents value.
		/// </summary>
		public FastObservableCollection<PropComponentViewModel> UserComponents
		{
			get { return GetValue<FastObservableCollection<PropComponentViewModel>>(UserComponentsProperty); }
			set { SetValue(UserComponentsProperty, value); }
		}

		/// <summary>
		/// UserComponents property data.
		/// </summary>
		public static readonly IPropertyData UserComponentsProperty = RegisterProperty<FastObservableCollection<PropComponentViewModel>>(nameof(UserComponents));

		#endregion
	}
}