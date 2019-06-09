using NUnit.Framework;
using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.Tests
{
	[TestFixture()]
	public class XLightsInventoryTests
	{
		[Test()]
		public void ImportTest()
		{
			XModelInventoryImporter inventoryImporter = new XModelInventoryImporter();
			var result = inventoryImporter.Import("http://hohenseefamily.com/xlights/boscoyo.xml");
			result.Wait();
			Assert.IsNotNull(result.Result.Vendor);
			Assert.IsNotNull(result.Result.Vendor.Name);
			Assert.IsNotNull(result.Result.Vendor.Contact);
			Assert.IsNotNull(result.Result.Vendor.Email);
		}
	}
}