using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory;
using VixenModules.App.CustomPropEditor.Model.InternalVendorInventory;
using Category = VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory.Category;
using Vendor = VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory.Vendor;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class XModelInventoryMapper
	{
		public ModelInventory Map(XModelInventory xModelInventory)
		{
			ModelInventory mi = new ModelInventory();

			Dictionary<uint, List<Product>> productMap = MapModels(xModelInventory.Models);

			var cat = MapCategories(xModelInventory.Categories, productMap);
			mi.Vendor = MapVendor(xModelInventory.Vendor);
			mi.Inventory = cat;
			mi.DateTime = DateTime.Now;

			return mi;
		}

		private static Model.InternalVendorInventory.Vendor MapVendor(Vendor vendor)
		{
			Model.InternalVendorInventory.Vendor v = new Model.InternalVendorInventory.Vendor();
			v.Name = vendor.Name;
			v.Contact = vendor.Contact;
			v.Description = vendor.Notes;
			v.Email = vendor.Email;
			v.Phone = vendor.Phone;

			v.WebLinks = new List<WebLink>();
			if (!string.IsNullOrEmpty(vendor.Website))
			{
				v.WebLinks.Add(new WebLink(@"Website", new Uri(vendor.Website)));
			}
			if (!string.IsNullOrEmpty(vendor.Facebook))
			{
				v.WebLinks.Add(new WebLink(@"Facebook", new Uri(vendor.Facebook)));
			}

			if (!string.IsNullOrEmpty(vendor.LogoLink))
			{
				v.Logo = new Uri(vendor.LogoLink);
			}

			return v;
		}

		private static Dictionary<uint, List<Product>> MapModels(List<Model.ExternalVendorInventory.Model> models)
		{
			Dictionary<uint, List<Product>> productMap = new Dictionary<uint, List<Product>>();

			foreach (var model in models)
			{
				var p = new Product
				{
					Id = model.Id,
					Name = model.Name,
					CategoryIds = model.CategoryIds,
					Material = model.Material,
					Height = model.Height,
					Width = model.Width,
					Thickness = model.Thickness,
					Notes = model.Notes,
					PixelCount = model.PixelCount,
					PixelDescription = model.PixelDescription,
					PixelSpacing = model.PixelSpacing,
					ProductType = model.Type
			};
				if (model.ImageFile != null && model.ImageFile.Any() && !string.IsNullOrEmpty(model.ImageFile.First()))
				{
					p.ImageUrl = new Uri(model.ImageFile.First());
				}

				if (model.Wiring != null && model.Wiring.Any())
				{
					int index = 1;
					p.ModelLinks = model.Wiring.Where(l=> l.IsValid).Select(x => new ModelLink(ModelType.XModel, string.IsNullOrEmpty(x.Name)?$"Model {index++}":x.Name, x.Description, x.XModelLink != null?new Uri(x.XModelLink):null))
						.ToList();
				}
				else
				{
					p.ModelLinks = new List<ModelLink>();
				}
				
				if (!string.IsNullOrEmpty(model.Weblink))
				{
					p.Url = new Uri(model.Weblink);
				}

				foreach (var categoryId in p.CategoryIds)
				{
					if (productMap.TryGetValue(categoryId, out var catProducts))
					{
						catProducts.Add(p);
					}
					else
					{
						productMap.Add(categoryId, new List<Product>(new[] { p }));
					}
				}
				
			}

			return productMap;
		}

		private static List<Model.InternalVendorInventory.Category> MapCategories(List<Category> categories, Dictionary<uint, List<Product>> productMap)
		{
			List<Model.InternalVendorInventory.Category> cList = new List<Model.InternalVendorInventory.Category>();
			foreach (var category in categories)
			{
				Model.InternalVendorInventory.Category c = new Model.InternalVendorInventory.Category();
				c.Name = category.Name;
				c.Id = category.Id;
				if (productMap.TryGetValue(c.Id, out var products))
				{
					c.Products = products;
				}

				c.Categories = MapCategories(category.Categories, productMap);
				cList.Add(c);
			}

			return cList;
		}
	}
}
