using Common.Messages.LivePreview;
using Moq;
using VixenModules.App.LivePreview;
using Xunit;

namespace Vixen.Tests.LivePreview
{
	public class LivePreviewServiceTests
	{
		private readonly Mock<IContextFactory> _factory;
		private readonly Mock<ILiveContext> _defaultContext;
		private readonly LivePreviewService _service;

		public LivePreviewServiceTests()
		{
			_factory = new Mock<IContextFactory>();
			_defaultContext = new Mock<ILiveContext>();
			_factory.Setup(f => f.GetOrCreate("LivePreview")).Returns(_defaultContext.Object);
			_service = new LivePreviewService(_factory.Object);
		}

		[Fact]
		public void GetOrCreateContext_ReturnsDefaultContext_WhenContextNameIsNull()
		{
			var first  = _service.GetOrCreateContext(null);
			var second = _service.GetOrCreateContext(null);

			_factory.Verify(f => f.GetOrCreate("LivePreview"), Times.Once());
			Assert.Same(first, second);
		}

		[Fact]
		public void GetOrCreateContext_ReturnsNamedContext_WhenContextNameProvided()
		{
			var namedContext = new Mock<ILiveContext>();
			_factory.Setup(f => f.GetOrCreate("Web Server")).Returns(namedContext.Object);

			var result = _service.GetOrCreateContext("Web Server");

			_factory.Verify(f => f.GetOrCreate("Web Server"), Times.Once());
			Assert.Same(namedContext.Object, result);
		}

		[Fact]
		public void GetOrCreateContext_ReturnsDifferentContexts_ForDifferentNames()
		{
			var contextA = new Mock<ILiveContext>();
			var contextB = new Mock<ILiveContext>();
			_factory.Setup(f => f.GetOrCreate("A")).Returns(contextA.Object);
			_factory.Setup(f => f.GetOrCreate("B")).Returns(contextB.Object);

			var a = _service.GetOrCreateContext("A");
			var b = _service.GetOrCreateContext("B");

			Assert.NotSame(a, b);
			_factory.Verify(f => f.GetOrCreate("A"), Times.Once());
			_factory.Verify(f => f.GetOrCreate("B"), Times.Once());
		}

		[Fact]
		public void ClearActiveEffects_ClearsOnlyTargetContext_WhenContextNameProvided()
		{
			var contextA = new Mock<ILiveContext>();
			var contextB = new Mock<ILiveContext>();
			_factory.Setup(f => f.GetOrCreate("A")).Returns(contextA.Object);
			_factory.Setup(f => f.GetOrCreate("B")).Returns(contextB.Object);
			_service.GetOrCreateContext("A");
			_service.GetOrCreateContext("B");

			_service.ClearActiveEffects("A");

			contextA.Verify(c => c.Clear(It.IsAny<bool>()), Times.Once());
			contextB.Verify(c => c.Clear(It.IsAny<bool>()), Times.Never());
		}

		[Fact]
		public void ClearActiveEffects_ClearsDefaultContext_WhenContextNameIsNull()
		{
			_service.GetOrCreateContext(null);

			_service.ClearActiveEffects(null);

			_defaultContext.Verify(c => c.Clear(It.IsAny<bool>()), Times.Once());
		}

		[Fact]
		public void TurnOffElement_CallsTerminateNode_WithCorrectId()
		{
			var id = Guid.NewGuid();
			_service.GetOrCreateContext(null);

			_service.TurnOffElement(id, null);

			_defaultContext.Verify(c => c.TerminateNode(id), Times.Once());
		}

		[Fact]
		public void TurnOffElements_CallsTerminateNodes_WithCorrectIds()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			_service.GetOrCreateContext(null);

			_service.TurnOffElements([id1, id2], null);

			_defaultContext.Verify(c => c.TerminateNodes(
				It.Is<IEnumerable<Guid>>(ids => ids.Contains(id1) && ids.Contains(id2))),
				Times.Once());
		}

		[Fact]
		public void ReleaseContext_ReleasesContext_WhenContextExists()
		{
			var namedContext = new Mock<ILiveContext>();
			_factory.Setup(f => f.GetOrCreate("Web Server")).Returns(namedContext.Object);
			_service.GetOrCreateContext("Web Server");

			_service.ReleaseContext("Web Server");

			_factory.Verify(f => f.Release(namedContext.Object), Times.Once());
			// Second call should not find the context (already removed)
			_service.ReleaseContext("Web Server");
			_factory.Verify(f => f.Release(namedContext.Object), Times.Once());
		}

		[Fact]
		public void ReleaseContext_IsNoOp_WhenContextNotFound()
		{
			_service.ReleaseContext("NonExistent");

			_factory.Verify(f => f.Release(It.IsAny<ILiveContext>()), Times.Never());
		}
	}
}
