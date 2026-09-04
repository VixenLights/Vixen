using Vixen.Module.Effect;
using Vixen.Sys;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class EffectModuleInstanceBasePreRenderTests
{
	private const int ConcurrentCallerCount = 16;
	private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

	[Fact]
	public async Task PreRender_ConcurrentDirtyRequests_ExecutesOnceAndForwardsCancellationToken()
	{
		// Arrange
		using var context = new PreRenderTestContext(TestContext.Current.CancellationToken);
		var calls = Enumerable.Repeat(context, ConcurrentCallerCount)
			.Select(static context => Task.Factory.StartNew(
				static state => ((PreRenderTestContext)state!).PreRender(),
				context,
				context.TestCancellationToken,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default))
			.ToArray();

		try
		{
			Assert.True(context.CallersReady.Wait(TestTimeout, context.TestCancellationToken), "Concurrent callers did not become ready in time.");
			context.StartRendering.Set();
			Assert.True(context.Effect.RenderEntered.Wait(TestTimeout, context.TestCancellationToken), "Pre-render did not start in time.");

			// Act
			context.Effect.ReleaseRender();
			var results = await Task.WhenAll(calls);

			// Assert
			Assert.All(results, Assert.True);
			Assert.Equal(1, context.Effect.PreRenderCount);
			Assert.Equal(1, context.Effect.MaximumConcurrentPreRenders);
			Assert.Same(context.CancellationToken, context.Effect.ReceivedCancellationToken);
			Assert.False(context.Effect.IsDirty);
		}
		finally
		{
			context.Effect.ReleaseRender();
			await Task.WhenAll(calls).WaitAsync(TestTimeout, context.TestCancellationToken);
		}
	}

	private sealed class PreRenderTestContext : IDisposable
	{
		public PreRenderTestContext(CancellationToken testCancellationToken)
		{
			TestCancellationToken = testCancellationToken;
		}

		public BlockingEffect Effect { get; } = new();
		public CancellationTokenSource CancellationToken { get; } = new();
		public CountdownEvent CallersReady { get; } = new(ConcurrentCallerCount);
		public ManualResetEventSlim StartRendering { get; } = new(false);
		public CancellationToken TestCancellationToken { get; }

		public bool PreRender()
		{
			CallersReady.Signal();
			if (!StartRendering.Wait(TestTimeout, TestCancellationToken))
			{
				throw new TimeoutException("Concurrent callers were not released to pre-render the effect.");
			}

			return Effect.PreRender(CancellationToken);
		}

		public void Dispose()
		{
			Effect.DisposeTestSynchronizationPrimitives();
			StartRendering.Dispose();
			CallersReady.Dispose();
			CancellationToken.Dispose();
		}
	}

	private sealed class BlockingEffect : EffectModuleInstanceBase
	{
		private readonly ManualResetEventSlim _releaseRender = new(false);
		private int _activePreRenders;
		private int _maximumConcurrentPreRenders;
		private int _preRenderCount;

		public ManualResetEventSlim RenderEntered { get; } = new(false);
		public int MaximumConcurrentPreRenders => Volatile.Read(ref _maximumConcurrentPreRenders);
		public int PreRenderCount => Volatile.Read(ref _preRenderCount);
		public CancellationTokenSource? ReceivedCancellationToken { get; private set; }

		public void ReleaseRender()
		{
			_releaseRender.Set();
		}

		protected override void TargetNodesChanged()
		{
		}

		protected override void _PreRender(CancellationTokenSource? cancellationToken = null)
		{
			ReceivedCancellationToken = cancellationToken;
			Interlocked.Increment(ref _preRenderCount);
			var activePreRenders = Interlocked.Increment(ref _activePreRenders);
			UpdateMaximumConcurrentPreRenders(activePreRenders);
			RenderEntered.Set();

			try
			{
				if (!_releaseRender.Wait(TestTimeout))
				{
					throw new TimeoutException("The test did not release the pre-render operation in time.");
				}
			}
			finally
			{
				Interlocked.Decrement(ref _activePreRenders);
			}
		}

		protected override EffectIntents _Render()
		{
			return new EffectIntents();
		}

		public void DisposeTestSynchronizationPrimitives()
		{
			RenderEntered.Dispose();
			_releaseRender.Dispose();
		}

		private void UpdateMaximumConcurrentPreRenders(int activePreRenders)
		{
			var currentMaximum = Volatile.Read(ref _maximumConcurrentPreRenders);
			while (activePreRenders > currentMaximum)
			{
				var observedMaximum = Interlocked.CompareExchange(
					ref _maximumConcurrentPreRenders,
					activePreRenders,
					currentMaximum);
				if (observedMaximum == currentMaximum)
				{
					return;
				}

				currentMaximum = observedMaximum;
			}
		}
	}
}
