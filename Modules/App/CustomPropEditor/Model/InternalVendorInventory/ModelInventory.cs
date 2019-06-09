using System;
using System.Collections.Generic;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class ModelInventory : BindableBase
	{
		private Vendor _vendor;
		private DateTime _dateTime;
		private List<Category> _inventory;
		private Guid _id;

		public ModelInventory()
		{
			Id= Guid.NewGuid();
		}

		public Guid Id
		{
			get => _id;
			set
			{
				if (value.Equals(_id)) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		/// <summary>
		/// Vendor profile
		/// </summary>
		public Vendor Vendor
		{
			get => _vendor;
			set
			{
				if (Equals(value, _vendor)) return;
				_vendor = value;
				OnPropertyChanged(nameof(Vendor));
			}
		}

		/// <summary>
		/// Date time of the inventory capture
		/// </summary>
		public DateTime DateTime
		{
			get => _dateTime;
			set
			{
				if (value.Equals(_dateTime)) return;
				_dateTime = value;
				OnPropertyChanged(nameof(DateTime));
			}
		}

		/// <summary>
		/// Inventory of the categories and products.
		/// </summary>
		public List<Category> Inventory
		{
			get => _inventory;
			set
			{
				if (Equals(value, _inventory)) return;
				_inventory = value;
				OnPropertyChanged(nameof(Inventory));
			}
		}
	}
}
