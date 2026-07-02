using System.Xml.Linq;
using Vixen.IO.Xml.Serializer;
using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.IO.Xml.Serializer;

public class XmlElementTagDefinitionCollectionSerializerTests
{
	[Fact]
	public void WriteObject_ThenReadObject_RoundTripsAllItems()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionCollectionSerializer();
		var tags = new List<ElementTagDefinition>
		{
			new ElementTagDefinition(Guid.NewGuid(), "deprecated", "Deprecated", isBuiltIn: true),
			new ElementTagDefinition(Guid.NewGuid(), string.Empty, "Outdoor", isBuiltIn: false)
		};

		// Act
		XElement wrapper = new XElement("Root", serializer.WriteObject(tags));
		IEnumerable<ElementTagDefinition> result = serializer.ReadObject(wrapper);

		// Assert
		Assert.Equal(tags.Select(t => t.Id), result.Select(t => t.Id));
	}

	[Fact]
	public void ReadObject_NoTagsElementPresent_ReturnsEmptyCollection()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionCollectionSerializer();
		var wrapper = new XElement("Root");

		// Act
		IEnumerable<ElementTagDefinition> result = serializer.ReadObject(wrapper);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void WriteObject_EmptyCollection_ProducesEmptyTagsElement()
	{
		// Arrange
		var serializer = new XmlElementTagDefinitionCollectionSerializer();

		// Act
		XElement element = serializer.WriteObject(Enumerable.Empty<ElementTagDefinition>());

		// Assert
		Assert.False(element.HasElements);
	}
}
