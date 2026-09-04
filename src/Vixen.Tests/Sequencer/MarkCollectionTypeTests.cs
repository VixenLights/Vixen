using System.Runtime.Serialization;
using TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels;
using Vixen.Marks;
using VixenModules.App.Marks;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class MarkCollectionTypeTests
{
	[Fact]
	public void Values_PreserveExistingSerializedValuesAndAppendState()
	{
		Assert.Equal(0, (int)MarkCollectionType.Generic);
		Assert.Equal(1, (int)MarkCollectionType.Phrase);
		Assert.Equal(2, (int)MarkCollectionType.Word);
		Assert.Equal(3, (int)MarkCollectionType.Phoneme);
		Assert.Equal(4, (int)MarkCollectionType.State);
	}

	[Fact]
	public void NativeMarkCollectionSerialization_RoundTripsStateType()
	{
		var collection = new MarkCollection
		{
			Name = "State labels",
			CollectionType = MarkCollectionType.State
		};
		var serializer = new DataContractSerializer(typeof(MarkCollection));

		using var stream = new MemoryStream();
		serializer.WriteObject(stream, collection);
		stream.Position = 0;

		var roundTrip = Assert.IsType<MarkCollection>(serializer.ReadObject(stream));

		Assert.Equal(MarkCollectionType.State, roundTrip.CollectionType);
	}

	[Fact]
	public void DockerTypeMenu_ContainsStateAndStateIsNonLinkable()
	{
		var menuTypes = Enum.GetValues<MarkCollectionType>();

		Assert.Contains(MarkCollectionType.State, menuTypes);
		Assert.DoesNotContain(MarkCollectionType.State, new[] { MarkCollectionType.Phoneme, MarkCollectionType.Word });
	}

	[Fact]
	public void ExportRow_StateCollectionIncludesTextByDefault()
	{
		var row = new MarkCollectionExportRowViewModel(new MarkCollection
		{
			CollectionType = MarkCollectionType.State
		});

		Assert.True(row.IsTextIncluded);
	}
}
