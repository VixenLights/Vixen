using System.Runtime.Serialization;
using Vixen.Services.EffectDefaults;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using Xunit;

namespace Vixen.Tests.EffectDefaults;

public sealed class EffectDefaultScrubberTests
{
	[DataContract]
	private sealed class LeafHolder
	{
		[DataMember]
		[ExcludeFromEffectDefault]
		public Guid MarkCollectionId { get; set; }

		[DataMember]
		public string? Name { get; set; }
	}

	[DataContract]
	private sealed class ListHolder
	{
		[DataMember]
		public List<LeafHolder> Items { get; set; } = new();
	}

	[DataContract]
	private sealed class CyclicNode
	{
		[DataMember]
		public string? Label { get; set; }

		[DataMember]
		public CyclicNode? Next { get; set; }
	}

	[Fact]
	public void Scrub_ResetsExcludedMemberAtTopLevel()
	{
		var holder = new LeafHolder { MarkCollectionId = Guid.NewGuid(), Name = "keep" };

		EffectDefaultScrubber.Scrub(holder);

		Assert.Equal(Guid.Empty, holder.MarkCollectionId);
		Assert.Equal("keep", holder.Name);
	}

	[Fact]
	public void Scrub_ResetsExcludedMemberNestedInsideList()
	{
		var list = new ListHolder();
		list.Items.Add(new LeafHolder { MarkCollectionId = Guid.NewGuid(), Name = "a" });
		list.Items.Add(new LeafHolder { MarkCollectionId = Guid.NewGuid(), Name = "b" });

		EffectDefaultScrubber.Scrub(list);

		Assert.All(list.Items, item => Assert.Equal(Guid.Empty, item.MarkCollectionId));
		Assert.Equal(["a", "b"], list.Items.Select(item => item.Name));
	}

	[Fact]
	public void Scrub_LeavesLibraryReferencedCurveUntouched()
	{
		// Curve.Points materializes library data as a side effect of reading it (see Curve.cs), but
		// Curve.UpdateLibraryReference() is null-safe when no CurveLibrary module is loaded (as in this
		// unit test host), so it is safe to read Points here without a running Vixen application.
		var curve = new Curve(CurveType.Flat100) { LibraryReferenceName = "Some Shared Curve" };
		var originalPoints = curve.Points.ToArray();

		EffectDefaultScrubber.Scrub(curve);

		Assert.Equal("Some Shared Curve", curve.LibraryReferenceName);
		Assert.Equal(originalPoints, curve.Points.ToArray());
	}

	[Fact]
	public void Scrub_TerminatesOnReferenceCycle()
	{
		var a = new CyclicNode { Label = "a" };
		var b = new CyclicNode { Label = "b" };
		a.Next = b;
		b.Next = a;

		Exception? ex = Record.Exception(() => EffectDefaultScrubber.Scrub(a));

		Assert.Null(ex);
		Assert.Equal("a", a.Label);
		Assert.Equal("b", b.Label);
	}
}
