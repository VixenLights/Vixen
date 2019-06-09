using System.Collections.Generic;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class Category : BindableBase
	{
		private uint _id;
		private string _name;
		private List<Category> _categories;
		private List<Product> _products;

		public Category()
		{
			Products = new List<Product>();
		}

		/// <summary>
		/// Category id that is unique to each vendor
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
		/// Category name
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
		/// Sub categories
		/// </summary>
		public List<Category> Categories
		{
			get => _categories;
			set
			{
				if (Equals(value, _categories)) return;
				_categories = value;
				OnPropertyChanged(nameof(Categories));
			}
		}

		/// <summary>
		/// Products belonging to this category
		/// </summary>
		public List<Product> Products
		{
			get => _products;
			set
			{
				if (Equals(value, _products)) return;
				_products = value;
				OnPropertyChanged(nameof(Products));
			}
		}
	}
}
