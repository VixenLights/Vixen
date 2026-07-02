using System.Xml.Linq;
using Vixen.IO.Xml.Serializer;
using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.IO.Xml.Serializer;

public class XmlElementTagDefinitionSerializerTests
{
	[Fact]
	public void WriteObject_ThenReadObject_RoundTripsScalarFields()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var tag = new ElementTagDefinition(Guid.NewGuid(), "deprecated", "Deprecated", isBuiltIn: true) { SortOrder = 3 };

		// Act
		XElement element = serializer.WriteObject(tag);
		ElementTagDefinition result = serializer.ReadObject(element);

		// Assert
		Assert.Equal(tag.Id, result.Id);
		Assert.Equal(tag.Key, result.Key);
		Assert.Equal(tag.Name, result.Name);
		Assert.Equal(tag.IsBuiltIn, result.IsBuiltIn);
		Assert.Equal(tag.SortOrder, result.SortOrder);
	}

	[Fact]
	public void WriteObject_DescriptionAndDisplayColorUnset_OmitsAttributes()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var tag = new ElementTagDefinition(Guid.NewGuid(), "custom", "Custom", isBuiltIn: false);

		// Act
		XElement element = serializer.WriteObject(tag);

		// Assert
		Assert.Null(element.Attribute("description"));
	}

	[Fact]
	public void WriteObject_ThenReadObject_RoundTripsDescriptionAndDisplayColor()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var tag = new ElementTagDefinition(Guid.NewGuid(), "custom", "Custom", isBuiltIn: false)
		{
			Description = "A custom tag",
			DisplayColor = "#FFA500"
		};

		// Act
		XElement element = serializer.WriteObject(tag);
		ElementTagDefinition result = serializer.ReadObject(element);

		// Assert
		Assert.Equal("A custom tag", result.Description);
		Assert.Equal("#FFA500", result.DisplayColor);
	}

	[Fact]
	public void ReadObject_MissingKeyAttribute_DefaultsToEmptyString()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var element = new XElement("Tag",
			new XAttribute("id", Guid.NewGuid()),
			new XAttribute("name", "Custom"),
			new XAttribute("isBuiltIn", false));

		// Act
		ElementTagDefinition result = serializer.ReadObject(element);

		// Assert
		Assert.Equal(string.Empty, result.Key);
	}

	[Fact]
	public void ReadObject_MissingSortOrderAttribute_DefaultsToZero()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var element = new XElement("Tag",
			new XAttribute("id", Guid.NewGuid()),
			new XAttribute("key", "custom"),
			new XAttribute("name", "Custom"),
			new XAttribute("isBuiltIn", false));

		// Act
		ElementTagDefinition result = serializer.ReadObject(element);

		// Assert
		Assert.Equal(0, result.SortOrder);
	}

	[Fact]
	public void ReadObject_MissingIdAttribute_ReturnsNull()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionSerializer();
		var element = new XElement("Tag", new XAttribute("name", "Custom"));

		// Act
		ElementTagDefinition result = serializer.ReadObject(element);

		// Assert
		Assert.Null(result);
	}
}
