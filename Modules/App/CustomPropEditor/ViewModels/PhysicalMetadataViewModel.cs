using System;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;
using PropertyData = Catel.Data.PropertyData;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
    public class PhysicalMetadataViewModel : ViewModelBase
	{
		public PhysicalMetadataViewModel(PhysicalMetadata physicalMetadata)
		{
			PhysicalMetadata = physicalMetadata;
		}

		#region PropertyName model property

		/// <summary>
		/// Gets or sets the PropertyName value.
		/// </summary>
		[Browsable(false)]
		[Model]
		public PhysicalMetadata PhysicalMetadata
		{
			get { return GetValue<PhysicalMetadata>(PhysicalMetadataProperty); }
			private set { SetValue(PhysicalMetadataProperty, value); }
		}

		/// <summary>
		/// PropertyName property data.
		/// </summary>
		public static readonly PropertyData PhysicalMetadataProperty = RegisterProperty("PhysicalMetadata", typeof(PhysicalMetadata));

		#endregion

		#region Height property

		/// <summary>
		/// Gets or sets the Height value.
		/// </summary>
		[PropertyOrder(1)]
		[ViewModelToModel("PhysicalMetadata")]
		public string Height
		{
			get { return GetValue<string>(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		/// Height property data.
		/// </summary>
		public static readonly PropertyData HeightProperty = RegisterProperty("Height", typeof(string), null);

		#endregion

		#region Width property

		/// <summary>
		/// Gets or sets the Width value.
		/// </summary>
		[PropertyOrder(1)]
		[ViewModelToModel("PhysicalMetadata")]
		public string Width
		{
			get { return GetValue<string>(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		/// Width property data.
		/// </summary>
		public static readonly PropertyData WidthProperty = RegisterProperty("Width", typeof(string), null);

		#endregion

		#region Material property

		/// <summary>
		/// Gets or sets the Material value.
		/// </summary>
		[PropertyOrder(0)]
		[ViewModelToModel("PhysicalMetadata")]
		public string Material
		{
			get { return GetValue<string>(MaterialProperty); }
			set { SetValue(MaterialProperty, value); }
		}

		/// <summary>
		/// Material property data.
		/// </summary>
		public static readonly PropertyData MaterialProperty = RegisterProperty("Material", typeof(string), null);

		#endregion

		#region Depth property

		/// <summary>
		/// Gets or sets the Depth value.
		/// </summary>
		[PropertyOrder(3)]
		[ViewModelToModel("PhysicalMetadata")]
		public string Depth
		{
			get { return GetValue<string>(DepthProperty); }
			set { SetValue(DepthProperty, value); }
		}

		/// <summary>
		/// Depth property data.
		/// </summary>
		public static readonly PropertyData DepthProperty = RegisterProperty("Depth", typeof(string), null);

		#endregion

		#region NodeCount property

		/// <summary>
		/// Gets or sets the NodeCount value.
		/// </summary>
		[PropertyOrder(4)]
		[DisplayName("Node Count")]
		[ViewModelToModel("PhysicalMetadata")]
		public string NodeCount
		{
			get { return GetValue<string>(NodeCountProperty); }
			set { SetValue(NodeCountProperty, value); }
		}

		/// <summary>
		/// NodeCount property data.
		/// </summary>
		public static readonly PropertyData NodeCountProperty = RegisterProperty("NodeCount", typeof(string));

		#endregion

		#region BulbType property

		/// <summary>
		/// Gets or sets the BulbType value.
		/// </summary>
		[PropertyOrder(5)]
		[DisplayName("Bulb Type")]
		[ViewModelToModel("PhysicalMetadata")]
		[TypeConverter(typeof(BulbTypeConverter))]
		public string BulbType
		{
			get { return GetValue<string>(BulbTypeProperty); }
			set { SetValue(BulbTypeProperty, value); }
		}

		/// <summary>
		/// BulbType property data.
		/// </summary>
		public static readonly PropertyData BulbTypeProperty = RegisterProperty("BulbType", typeof(string), null);

		#endregion

		#region ColorMode property

		/// <summary>
		/// Gets or sets the ColorMode value.
		/// </summary>
		[PropertyOrder(6)]
		[DisplayName("Color Mode")]
		[Description("Declares color handling for the entire prop. \nIf the prop contains multiple color modes, choose other.")]
		[ViewModelToModel("PhysicalMetadata")]
		public ColorMode ColorMode
		{
			get { return GetValue<ColorMode>(ColorModeProperty); }
			set { SetValue(ColorModeProperty, value); }
		}

		/// <summary>
		/// ColorMode property data.
		/// </summary>
		public static readonly PropertyData ColorModeProperty = RegisterProperty("ColorMode", typeof(ColorMode), null);

		#endregion

		#region Overrides

		//We are not using these properties in the view so hiding them so the property giris does not expose them.

		[Browsable(false)]
		public new DateTime ViewModelConstructionTime => base.ViewModelConstructionTime;

		[Browsable(false)]
		public new int UniqueIdentifier => base.UniqueIdentifier;

		[Browsable(false)]
		public new string Title => base.Title;

		[Browsable(false)]
		public new bool IsClosed => base.IsClosed;

		[Browsable(false)]
		public new IViewModel ParentViewModel => base.ParentViewModel;

		#endregion
	}
}
