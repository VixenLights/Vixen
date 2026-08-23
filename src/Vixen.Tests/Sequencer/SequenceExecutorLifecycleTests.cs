using System.ComponentModel;
using System.Reflection;
using BaseSequence;
using Moq;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Utility;
using Xunit;

namespace Vixen.Tests.Sequencer;

/// <summary>
/// Verifies sequence-executor behavior at the boundary between a timer-posted completion callback and lifecycle cleanup.
/// </summary>
[Collection(SequenceExecutorTestCollection.Name)]
public sealed class SequenceExecutorLifecycleTests
{
	private static readonly TimeSpan EndTime = TimeSpan.FromSeconds(1);

	/// <summary>
	/// Verifies that a live looping sequence restarts when its queued natural-end callback is dispatched.
	/// </summary>
	[Fact]
	public void PlayLoop_WhenNaturalEndCallbackIsDispatched_RestartsAndRaisesRestartedEvent()
	{
		WithExecutor((executor, timing, synchronizationContext) =>
		{
			var restartCount = 0;
			executor.SequenceReStarted += (_, _) => restartCount++;

			StartAndQueueNaturalEnd(executor, timing);
			synchronizationContext.DispatchSingle();

			Assert.Equal(2, timing.StartCount);
			Assert.Equal(TimeSpan.FromMilliseconds(1), timing.Position);
			Assert.Equal(1, restartCount);
		});
	}

	/// <summary>
	/// Verifies that stopping playback invalidates a queued loop restart.
	/// </summary>
	[Fact]
	public void PlayLoop_WhenStoppedBeforeNaturalEndCallbackDispatch_DoesNotRestartOrRaiseRestartedEvent()
	{
		WithExecutor((executor, timing, synchronizationContext) =>
		{
			var restartCount = 0;
			executor.SequenceReStarted += (_, _) => restartCount++;

			StartAndQueueNaturalEnd(executor, timing);
			executor.Stop();
			var startCountAfterStop = timing.StartCount;

			var exception = Record.Exception(synchronizationContext.DispatchSingle);

			Assert.Null(exception);
			Assert.Equal(startCountAfterStop, timing.StartCount);
			Assert.Equal(0, restartCount);
			Assert.False(executor.IsRunning);
		});
	}

	/// <summary>
	/// Verifies that disposing playback invalidates a queued loop restart without accessing released state.
	/// </summary>
	[Fact]
	public void PlayLoop_WhenDisposedBeforeNaturalEndCallbackDispatch_DoesNotThrowOrRestart()
	{
		WithExecutor((executor, timing, synchronizationContext) =>
		{
			var restartCount = 0;
			executor.SequenceReStarted += (_, _) => restartCount++;

			StartAndQueueNaturalEnd(executor, timing);
			executor.Dispose();
			var startCountAfterDispose = timing.StartCount;

			var exception = Record.Exception(synchronizationContext.DispatchSingle);

			Assert.Null(exception);
			Assert.Equal(startCountAfterDispose, timing.StartCount);
			Assert.Equal(0, restartCount);
		});
	}

	/// <summary>
	/// Verifies that a queued non-loop natural-end callback stops playback and raises one end event.
	/// </summary>
	[Fact]
	public void Play_WhenNaturalEndCallbackIsDispatched_StopsAndRaisesEndedEvent()
	{
		WithExecutor((executor, timing, synchronizationContext) =>
		{
			var endedCount = 0;
			executor.SequenceEnded += (_, _) => endedCount++;

			executor.Play(TimeSpan.Zero, EndTime);
			StopEndCheckTimer(executor);
			timing.Position = EndTime;
			Assert.True(CheckForNaturalEnd(executor));

			synchronizationContext.DispatchSingle();

			Assert.Equal(1, timing.StopCount);
			Assert.Equal(1, endedCount);
			Assert.False(executor.IsRunning);
		});
	}

	/// <summary>
	/// Verifies that a timer callback arriving after disposal does not access released timer state.
	/// </summary>
	[Fact]
	public void EndCheckTimerElapsed_WhenExecutorIsDisposed_DoesNotThrow()
	{
		WithExecutor((executor, _, _) =>
		{
			executor.PlayLoop(TimeSpan.Zero, EndTime);
			StopEndCheckTimer(executor);
			executor.Dispose();

			var exception = Record.Exception(() => InvokeEndCheckTimerElapsed(executor));

			Assert.Null(exception);
		});
	}

	private static void WithExecutor(Action<SequenceExecutor, TestTiming, QueuedSynchronizationContext> test)
	{
		var originalSynchronizationContext = AsyncOperationManager.SynchronizationContext;
		var synchronizationContext = new QueuedSynchronizationContext();
		AsyncOperationManager.SynchronizationContext = synchronizationContext;

		try
		{
			var timing = new TestTiming();
			var sequence = new Mock<ISequence>();
			sequence.SetupGet(x => x.Length).Returns(EndTime);
			sequence.Setup(x => x.GetTiming()).Returns(timing);
			sequence.Setup(x => x.GetAllMedia()).Returns(Enumerable.Empty<IMediaModuleInstance>());

			using var executor = new SequenceExecutor { Sequence = sequence.Object };
			test(executor, timing, synchronizationContext);
		}
		finally
		{
			AsyncOperationManager.SynchronizationContext = originalSynchronizationContext;
		}
	}

	private static void StartAndQueueNaturalEnd(SequenceExecutor executor, TestTiming timing)
	{
		executor.PlayLoop(TimeSpan.Zero, EndTime);
		StopEndCheckTimer(executor);
		timing.Position = EndTime;
		Assert.True(CheckForNaturalEnd(executor));
	}

	private static bool CheckForNaturalEnd(SequenceExecutor executor)
	{
		var method = typeof(SequenceExecutor).GetMethod("_CheckForNaturalEnd", BindingFlags.Instance | BindingFlags.NonPublic)!;
		return (bool) method.Invoke(executor, null)!;
	}

	private static void StopEndCheckTimer(SequenceExecutor executor)
	{
		var timer = (HighResolutionTimer) GetField(executor, "_endCheckTimer");
		timer.Stop();
	}

	private static void InvokeEndCheckTimerElapsed(SequenceExecutor executor)
	{
		var method = typeof(SequenceExecutor).GetMethod("_EndCheckTimerElapsed", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(executor, [null, null]);
	}

	private static object GetField(object target, string fieldName)
	{
		var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		return field.GetValue(target)!;
	}

	private sealed class QueuedSynchronizationContext : SynchronizationContext
	{
		private readonly Queue<(SendOrPostCallback Callback, object? State)> _callbacks = [];

		public override void Post(SendOrPostCallback callback, object? state)
		{
			_callbacks.Enqueue((callback, state));
		}

		public void DispatchSingle()
		{
			if (!_callbacks.TryDequeue(out var callback))
			{
				throw new InvalidOperationException("No callback was queued for dispatch.");
			}

			callback.Callback(callback.State);
		}
	}

	private sealed class TestTiming : ITiming
	{
		public TimeSpan Position { get; set; }
		public bool SupportsVariableSpeeds => false;
		public float Speed { get; set; }
		public int StartCount { get; private set; }
		public int StopCount { get; private set; }

		public void Start()
		{
			StartCount++;
			if (Position == TimeSpan.Zero)
			{
				Position = TimeSpan.FromMilliseconds(1);
			}
		}

		public void Stop()
		{
			StopCount++;
		}

		public void Pause()
		{
		}

		public void Resume()
		{
		}
	}

}
