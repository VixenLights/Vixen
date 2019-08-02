using System;
using System.Collections.Generic;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class Product : BindableBase
	{
		private uint _id;
		private List<uint> _categoryIds;
		private string _name;
		private string _productType;
		private Uri _url;
		private Uri _imageUrl;
		private string _material;
		private string _width;
		private string _height;
		private string _thickness;
		private uint _pixelCount;
		private string _pixelDescription;
		private string _pixelSpacing;
		private string _notes;
		private List<ModelLink> _modelLinks;
		private Uri _vixenPropLink;

		/// <summary>
		/// Unique Id within the Vendor for the product.
		/// </summary>
		public uint Id
		{
			get => _id;
			set
			{
				if (value == _id) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		/// <summary>
		/// Unique Vendor Category Id
		/// </summary>
		public List<uint> CategoryIds
		{
			get => _categoryIds;
			set
			{
				if (value == _categoryIds) return;
				_categoryIds = value;
				OnPropertyChanged(nameof(CategoryIds));
			}
		}

		/// <summary>
		/// Name of the product
		/// </summary>
		public string Name
		{
			get => _name;
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Product Type
		/// </summary>
		public string ProductType
		{
			get => _productType;
			set
			{
				if (value == _productType) return;
				_productType = value;
				OnPropertyChanged(nameof(ProductType));
			}
		}

		/// <summary>
		/// Url to the product page on the vendor website 
		/// </summary>
		public Uri Url
		{
			get => _url;
			set
			{
				if (Equals(value, _url)) return;
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}

		/// <summary>
		/// Url to a representative image of the product on the vendor website
		/// </summary>
		public Uri ImageUrl
		{
			get => _imageUrl;
			set
			{
				if (Equals(value, _imageUrl)) return;
				_imageUrl = value;
				OnPropertyChanged(nameof(ImageUrl));
			}
		}

		/// <summary>
		/// Material the product is primarily made from.
		/// </summary>
		public string Material
		{
			get => _material;
			set
			{
				if (value == _material) return;
				_material = value;
				OnPropertyChanged(nameof(Material));
			}
		}

		/// <summary>
		/// The width of the product
		/// </summary>
		public string Width
		{
			get => _width;
			set
			{
				if (value == _width) return;
				_width = value;
				OnPropertyChanged(nameof(Width));
			}
		}

		/// <summary>
		/// The height of the product
		/// </summary>
		public string Height
		{
			get => _height;
			set
			{
				if (value == _height) return;
				_height = value;
				OnPropertyChanged(nameof(Height));
			}
		}

		/// <summary>
		/// The thickness of the product
		/// </summary>
		public string Thickness
		{
			get => _thickness;
			set
			{
				if (value == _thickness) return;
				_thickness = value;
				OnPropertyChanged(nameof(Thickness));
			}
		}

		/// <summary>
		/// How many pixel lights the product supports
		/// </summary>
		public uint PixelCount
		{
			get => _pixelCount;
			set
			{
				if (value == _pixelCount) return;
				_pixelCount = value;
				OnPropertyChanged(nameof(PixelCount));
			}
		}

		/// <summary>
		/// The description of the pixel types the product supports
		/// </summary>
		public string PixelDescription
		{
			get => _pixelDescription;
			set
			{
				if (value == _pixelDescription) return;
				_pixelDescription = value;
				OnPropertyChanged(nameof(PixelDescription));
			}
		}

		/// <summary>
		/// The pixel spacing the product requires.
		/// </summary>
		public string PixelSpacing
		{
			get => _pixelSpacing;
			set
			{
				if (value == _pixelSpacing) return;
				_pixelSpacing = value;
				OnPropertyChanged(nameof(PixelSpacing));
			}
		}

		/// <summary>
		/// General notes bout the product
		/// </summary>
		public string Notes
		{
			get => _notes;
			set
			{
				if (value == _notes) return;
				_notes = value;
				OnPropertyChanged(nameof(Notes));
			}
		}

		/// <summary>
		/// Link to the software model for the product
		/// </summary>
		public List<ModelLink> ModelLinks
		{
			get => _modelLinks;
			set
			{
				if (Equals(value, _modelLinks)) return;
				_modelLinks = value;
				OnPropertyChanged(nameof(ModelLinks));
			}
		}

		
	}
}
