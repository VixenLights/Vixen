using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;
using VixenModules.App.CustomPropEditor.Model.InternalVendorInventory;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class VendorInventoryWindowViewModel : ViewModelBase
	{
		private readonly IProcessService _processService;
		public VendorInventoryWindowViewModel(List<ModelInventory> vendorInventories, IProcessService processService)
		{
			_processService = processService;
			VendorInventories = vendorInventories;
			SelectedInventory = VendorInventories.FirstOrDefault();
		}

		#region Overrides of ViewModelBase

		/// <inheritdoc />
		public override string Title => "Vendor Inventory";

		#endregion

		#region SelectedInventory property

		/// <summary>
		/// Gets or sets the SelectedInventory value.
		/// </summary>
		public ModelInventory SelectedInventory
		{
			get { return GetValue<ModelInventory>(SelectedInventoryProperty); }
			set { SetValue(SelectedInventoryProperty, value); }
		}

		/// <summary>
		/// SelectedInventory property data.
		/// </summary>
		public static readonly PropertyData SelectedInventoryProperty = RegisterProperty("SelectedInventory", typeof(ModelInventory));

		#endregion

		#region Inventory model property

		/// <summary>
		/// Gets or sets the Inventory value.
		/// </summary>
		[Model]
		public List<ModelInventory> VendorInventories
		{
			get { return GetValue<List<ModelInventory>>(InventoryProperty); }
			private set { SetValue(InventoryProperty, value); }
		}

		/// <summary>
		/// Inventory property data.
		/// </summary>
		public static readonly PropertyData InventoryProperty = RegisterProperty("VendorInventories", typeof(List<ModelInventory>));

		#endregion

		#region SelectedProduct property

		/// <summary>
		/// Gets or sets the SelectedProduct value.
		/// </summary>
		public Product SelectedProduct
		{
			get { return GetValue<Product>(SelectedProductProperty); }
			set { SetValue(SelectedProductProperty, value); }
		}

		/// <summary>
		/// SelectedProduct property data.
		/// </summary>
		public static readonly PropertyData SelectedProductProperty = RegisterProperty("SelectedProduct", typeof(Product));

		#endregion

		#region SelectedModelLink property

		/// <summary>
		/// Gets or sets the SelectedModelLink value.
		/// </summary>
		public ModelLink SelectedModelLink
		{
			get { return GetValue<ModelLink>(SelectedModelLinkProperty); }
			set { SetValue(SelectedModelLinkProperty, value); }
		}

		/// <summary>
		/// SelectedModelLink property data.
		/// </summary>
		public static readonly PropertyData SelectedModelLinkProperty = RegisterProperty("SelectedModelLink", typeof(ModelLink));

		#endregion

		#region IsModelValid property

		/// <summary>
		/// Gets or sets the ShowModelTab value.
		/// </summary>
		public bool IsModelValid
		{
			get { return GetValue<bool>(IsModelValidProperty); }
			set { SetValue(IsModelValidProperty, value); }
		}

		/// <summary>
		/// ShowModelTab property data.
		/// </summary>
		public static readonly PropertyData IsModelValidProperty = RegisterProperty("IsModelValid", typeof(bool));

		#endregion

		#region IsProductVisible property

		#region IsProductVisible property

		/// <summary>
		/// Gets or sets the IsProductVisible value.
		/// </summary>
		public bool IsProductVisible
		{
			get { return GetValue<bool>(IsProductVisibleProperty); }
			set { SetValue(IsProductVisibleProperty, value); }
		}

		/// <summary>
		/// IsProductVisible property data.
		/// </summary>
		public static readonly PropertyData IsProductVisibleProperty = RegisterProperty("IsProductVisible", typeof(bool));

		#endregion

		#endregion

		#region IsProductViewSelected property

		/// <summary>
		/// Gets or sets the ProductTabSelected value.
		/// </summary>
		public bool IsProductViewSelected
		{
			get { return GetValue<bool>(IsProductViewSelectedProperty); }
			set { SetValue(IsProductViewSelectedProperty, value); }
		}

		/// <summary>
		/// ProductTabSelected property data.
		/// </summary>
		public static readonly PropertyData IsProductViewSelectedProperty = RegisterProperty("IsProductViewSelected", typeof(bool));

		#endregion

		#region SelectProduct command

		private Command<Product> _selectProductCommand;

		/// <summary>
		/// Gets the SelectProduct command.
		/// </summary>
		public Command<Product> SelectProductCommand
		{
			get { return _selectProductCommand ?? (_selectProductCommand = new Command<Product>(SelectProduct)); }
		}

		/// <summary>
		/// Method to invoke when the SelectProduct command is executed.
		/// </summary>
		private void SelectProduct(Product p)
		{
			SelectedModelLink = null;
			SelectedProduct = p;
			IsProductVisible = p != null;
			IsModelValid = p.ModelLinks != null && p.ModelLinks.Any(x => x.Link != null);
			IsProductViewSelected = true;

		}

		#endregion

		#region DialogResult property

		/// <summary>
		/// Gets or sets the DialogResult value.
		/// </summary>
		public bool DialogResult
		{
			get { return GetValue<bool>(DialogResultProperty); }
			set { SetValue(DialogResultProperty, value); }
		}

		/// <summary>
		/// DialogResult property data.
		/// </summary>
		public static readonly PropertyData DialogResultProperty = RegisterProperty("DialogResult", typeof(bool));

		#endregion

		#region ShowProductPage command

		private Command<Uri> _navigateToUrlCommand;

		/// <summary>
		/// Gets the ShowProductPage command.
		/// </summary>
		public Command<Uri> NavigateToUrlCommand
		{
			get { return _navigateToUrlCommand ?? (_navigateToUrlCommand = new Command<Uri>(NavigateToUrl)); }
		}

		/// <summary>
		/// Method to invoke when the ShowProductPage command is executed.
		/// </summary>
		private void NavigateToUrl(Uri url)
		{
			_processService.StartProcess(url.AbsoluteUri);
		}

		#endregion

		#region SendEmail command

		private Command<string> _sendEmailCommand;

		/// <summary>
		/// Gets the SendEmail command.
		/// </summary>
		public Command<string> SendEmailCommand
		{
			get { return _sendEmailCommand ?? (_sendEmailCommand = new Command<string>(SendEmail)); }
		}

		/// <summary>
		/// Method to invoke when the SendEmail command is executed.
		/// </summary>
		private void SendEmail(string address)
		{
			if (!address.StartsWith("mailto:"))
			{
				address = $"mailto:{address}";
			}
			_processService.StartProcess(address);
		}

		#endregion

		#region ImportModel command

		private Command<ModelLink> _importModelCommand;

		/// <summary>
		/// Gets the ImportModel command.
		/// </summary>
		public Command<ModelLink> ImportModelCommand
		{
			get { return _importModelCommand ?? (_importModelCommand = new Command<ModelLink>(ImportModel)); }
		}

		/// <summary>
		/// Method to invoke when the ImportModel command is executed.
		/// </summary>
		private void ImportModel(ModelLink ml)
		{
			if (SelectedProduct != null && ml != null && ml.Link != null)
			{
				SelectedModelLink = ml;
				DialogResult = true;
				CloseViewModelAsync(true);
			}
		}

		#endregion


	}
}
