using System;
using System.Threading.Tasks;
using System.Windows;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class LightViewModel : ViewModelBase, ISelectable, IDisposable
	{
		public LightViewModel(Light ln, ViewModelBase parent)
		{
			Light = ln;
			((IRelationalViewModel)this).SetParentViewModel(parent);
		}

		public LightViewModel(Light ln)
		{
			Light = ln;

		}

		public override string Title => "Light";

		#region LightNode model property

		/// <summary>
		/// Gets or sets the LightNode value.
		/// </summary>
		[Model]
		public Light Light
		{
			get { return GetValue<Light>(LightProperty); }
			private set { SetValue(LightProperty, value); }
		}

		/// <summary>
		/// LightNode property data.
		/// </summary>
		public static readonly PropertyData LightProperty = RegisterProperty("Light", typeof(Light));

		#endregion

		#region Id property

		/// <summary>
		/// Gets or sets the Id value.
		/// </summary>
		[ViewModelToModel("Light")]
		public Guid Id
		{
			get { return GetValue<Guid>(IdProperty); }
			set { SetValue(IdProperty, value); }
		}

		/// <summary>
		/// Id property data.
		/// </summary>
		public static readonly PropertyData IdProperty = RegisterProperty("Id", typeof(Guid), null);

		#endregion

		#region X property

		/// <summary>
		/// Gets or sets the X value.
		/// </summary>
		[ViewModelToModel("Light")]
		public double X
		{
			get { return GetValue<double>(XProperty); }
			set { SetValue(XProperty, value); }
		}

		/// <summary>
		/// X property data.
		/// </summary>
		public static readonly PropertyData XProperty = RegisterProperty("X", typeof(double), null);

		#endregion

		#region Y property

		/// <summary>
		/// Gets or sets the Y value.
		/// </summary>
		[ViewModelToModel("Light")]
		public double Y
		{
			get { return GetValue<double>(YProperty); }
			set { SetValue(YProperty, value); }
		}

		/// <summary>
		/// Y property data.
		/// </summary>
		public static readonly PropertyData YProperty = RegisterProperty("Y", typeof(double), null);

		#endregion

		#region Size property

		/// <summary>
		/// Gets or sets the Size value.
		/// </summary>
		[ViewModelToModel("Light")]
		public double Size
		{
			get { return GetValue<double>(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}

		/// <summary>
		/// Size property data.
		/// </summary>
		public static readonly PropertyData SizeProperty = RegisterProperty("Size", typeof(double), null);

		#endregion

		#region Center property

		/// <summary>
		/// Gets or sets the Center value.
		/// </summary>
		[ViewModelToModel("Light", "X", AdditionalPropertiesToWatch = new[] { "Y" }, ConverterType = typeof(PointMappingConverter))]
		public Point Center
		{
			get { return GetValue<Point>(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		/// <summary>
		/// Center property data.
		/// </summary>
		public static readonly PropertyData CenterProperty = RegisterProperty("Center", typeof(Point), null);

		#endregion

		#region IsSelected property

		/// <summary>
		/// Gets or sets the IsSelected value.
		/// </summary>
		public bool IsSelected
		{
			get { return GetValue<bool>(IsSelectedProperty); }
			set
			{
				bool tempDirty = IsDirty;
				SetValue(IsSelectedProperty, value);
				IsDirty = tempDirty;
			}
		}

		/// <summary>
		/// IsSelected property data.
		/// </summary>
		public static readonly PropertyData IsSelectedProperty = RegisterProperty("IsSelected", typeof(bool));

		#endregion




		// TODO: Register properties using 'vmprop'
		// TODO: Register properties that represent models using 'vmpropmodel'
		// TODO: Register properties that map to models using 'vmpropviewmodeltomodel'
		// TODO: Register commands using 'vmcommand', 'vmcommandwithcanexecute', 'vmtaskcommand' or 'vmtaskcommandwithcanexecute'

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();
			// TODO: Add initialization logic like subscribing to events
		}

		protected override async Task CloseAsync()
		{
			// TODO: Add uninitialization logic like unsubscribing from events

			await base.CloseAsync();
		}

		public void Dispose()
		{
			((IRelationalViewModel)this).SetParentViewModel(null);
		}
	}


}
