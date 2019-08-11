using System;
using NUnit.Framework;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.Import.Tests
{
	[TestFixture()]
	public class XLightsInventoryTests
	{
		[Test()]
		public void ImportTest()
		{
			XModelInventoryImporter inventoryImporter = new XModelInventoryImporter();
			var ds = new DownloadService();
			var xml = ds.GetFileAsStringAsync(new Uri("http://hohenseefamily.com/xlights/boscoyo.xml"));
			var result = inventoryImporter.Import(xml.Result);
			result.Wait();
			Assert.IsNotNull(result.Result.Vendor);
			Assert.IsNotNull(result.Result.Vendor.Name);
			Assert.IsNotNull(result.Result.Vendor.Contact);
			Assert.IsNotNull(result.Result.Vendor.Email);
		}
	}
}