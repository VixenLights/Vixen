using System.Collections.ObjectModel;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Marks;
using VixenModules.App.Marks;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class GridMarkSnapPointTests
{
	[Fact]
	public void MarkCollections_WhenConfigured_RegistersEachVisibleMarkBoundary()
	{
		MarkCollection firstCollection = CreateCollection(showTailGridLines: true);
		Mark firstMark = AddMark(firstCollection, 1, 2);
		MarkCollection secondCollection = CreateCollection();
		Mark secondMark = AddMark(secondCollection, 5, 1);

		using Grid grid = CreateGrid(firstCollection, secondCollection);

		Assert.Equal(2, grid.MarkSnapPointRegistrations.Count);
		MarkSnapPointRegistration firstRegistration = grid.MarkSnapPointRegistrations[firstMark];
		Assert.Equal(firstMark.StartTime, firstRegistration.StartSnapPoint.SnapTime);
		Assert.NotNull(firstRegistration.EndSnapPoint);
		Assert.Equal(firstMark.EndTime, firstRegistration.EndSnapPoint!.SnapTime);
		MarkSnapPointRegistration secondRegistration = grid.MarkSnapPointRegistrations[secondMark];
		Assert.Equal(secondMark.StartTime, secondRegistration.StartSnapPoint.SnapTime);
		Assert.Null(secondRegistration.EndSnapPoint);
	}

	[Fact]
	public void MarkCollections_WhenMarksShareATime_RegistersSeparateSnapDetails()
	{
		MarkCollection collection = CreateCollection();
		Mark firstMark = AddMark(collection, 1, 1);
		Mark secondMark = AddMark(collection, 1, 2);

		using Grid grid = CreateGrid(collection);

		Assert.NotSame(
			grid.MarkSnapPointRegistrations[firstMark].StartSnapPoint,
			grid.MarkSnapPointRegistrations[secondMark].StartSnapPoint);
		Assert.Equal(
			grid.MarkSnapPointRegistrations[firstMark].StartSnapPoint.SnapTime,
			grid.MarkSnapPointRegistrations[secondMark].StartSnapPoint.SnapTime);
	}

	[Fact]
	public void MarkCollections_WhenGridOrTailLinesAreDisabled_RegistersOnlyEligibleDetails()
	{
		MarkCollection hiddenCollection = CreateCollection(showGridLines: false);
		Mark hiddenMark = AddMark(hiddenCollection, 1, 1);
		MarkCollection startOnlyCollection = CreateCollection();
		Mark startOnlyMark = AddMark(startOnlyCollection, 3, 2);

		using Grid grid = CreateGrid(hiddenCollection, startOnlyCollection);

		Assert.DoesNotContain(hiddenMark, grid.MarkSnapPointRegistrations.Keys);
		MarkSnapPointRegistration registration = grid.MarkSnapPointRegistrations[startOnlyMark];
		Assert.Equal(startOnlyMark.StartTime, registration.StartSnapPoint.SnapTime);
		Assert.Null(registration.EndSnapPoint);
	}

	[Fact]
	public void TimePerPixelChange_WhenSnapPointsAreRegistered_PreservesRegistrationDetailIdentity()
	{
		MarkCollection collection = CreateCollection(showTailGridLines: true);
		Mark mark = AddMark(collection, 2, 1);

		using Grid grid = CreateGrid(collection);
		MarkSnapPointRegistration registration = grid.MarkSnapPointRegistrations[mark];
		SnapDetails startSnapPoint = registration.StartSnapPoint;
		SnapDetails endSnapPoint = registration.EndSnapPoint!;
		TimeSpan originalStartWindow = startSnapPoint.SnapStart;

		grid.TimePerPixel = TimeSpan.FromMilliseconds(50);

		Assert.Same(startSnapPoint, grid.MarkSnapPointRegistrations[mark].StartSnapPoint);
		Assert.Same(endSnapPoint, grid.MarkSnapPointRegistrations[mark].EndSnapPoint);
		Assert.NotEqual(originalStartWindow, startSnapPoint.SnapStart);
		Assert.Equal(mark.StartTime, startSnapPoint.SnapTime);
		Assert.Equal(mark.EndTime, endSnapPoint.SnapTime);
	}

	[Fact]
	public void MarksMoving_WhenOneMarkMoves_ReplacesOnlyItsRegisteredDetails()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection collection = CreateCollection(showTailGridLines: true);
		Mark movingMark = AddMark(collection, 1, 1);
		Mark stationaryMark = AddMark(collection, 4, 1);

		using Grid grid = CreateGrid(instanceId, collection);
		MarkSnapPointRegistration previousRegistration = grid.MarkSnapPointRegistrations[movingMark];
		MarkSnapPointRegistration stationaryRegistration = grid.MarkSnapPointRegistrations[stationaryMark];
		movingMark.StartTime = TimeSpan.FromSeconds(2);

		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs(new List<IMark> { movingMark }));

		MarkSnapPointRegistration currentRegistration = grid.MarkSnapPointRegistrations[movingMark];
		Assert.False(grid.ContainsStaticSnapPoint(previousRegistration.StartSnapPoint));
		Assert.False(grid.ContainsStaticSnapPoint(previousRegistration.EndSnapPoint!));
		Assert.True(grid.ContainsStaticSnapPoint(currentRegistration.StartSnapPoint));
		Assert.True(grid.ContainsStaticSnapPoint(currentRegistration.EndSnapPoint!));
		Assert.Equal(movingMark.StartTime, currentRegistration.StartSnapPoint.SnapTime);
		Assert.Equal(movingMark.EndTime, currentRegistration.EndSnapPoint!.SnapTime);
		Assert.Same(stationaryRegistration, grid.MarkSnapPointRegistrations[stationaryMark]);
		Assert.True(grid.ContainsStaticSnapPoint(stationaryRegistration.StartSnapPoint));
		Assert.True(grid.ContainsStaticSnapPoint(stationaryRegistration.EndSnapPoint!));
	}

	[Fact]
	public void MarksMoving_WhenDuplicateTimeMarkMoves_RetainsTheOtherMarkDetail()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection collection = CreateCollection();
		Mark movingMark = AddMark(collection, 1, 1);
		Mark stationaryMark = AddMark(collection, 1, 2);

		using Grid grid = CreateGrid(instanceId, collection);
		MarkSnapPointRegistration previousRegistration = grid.MarkSnapPointRegistrations[movingMark];
		MarkSnapPointRegistration stationaryRegistration = grid.MarkSnapPointRegistrations[stationaryMark];
		movingMark.StartTime = TimeSpan.FromSeconds(3);

		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs(new List<IMark> { movingMark, movingMark }));

		Assert.False(grid.ContainsStaticSnapPoint(previousRegistration.StartSnapPoint));
		Assert.True(grid.ContainsStaticSnapPoint(stationaryRegistration.StartSnapPoint));
		Assert.Same(stationaryRegistration, grid.MarkSnapPointRegistrations[stationaryMark]);
		Assert.Equal(movingMark.StartTime, grid.MarkSnapPointRegistrations[movingMark].StartSnapPoint.SnapTime);
	}

	[Fact]
	public void MarksMoving_WhenMarksFromMultipleCollectionsMove_UpdatesEachRegistration()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection firstCollection = CreateCollection(showTailGridLines: true);
		Mark firstMark = AddMark(firstCollection, 1, 1);
		MarkCollection secondCollection = CreateCollection();
		Mark secondMark = AddMark(secondCollection, 4, 1);

		using Grid grid = CreateGrid(instanceId, firstCollection, secondCollection);
		firstMark.StartTime = TimeSpan.FromSeconds(2);
		secondMark.StartTime = TimeSpan.FromSeconds(5);

		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs(new List<IMark> { firstMark, secondMark }));

		Assert.Equal(firstMark.StartTime, grid.MarkSnapPointRegistrations[firstMark].StartSnapPoint.SnapTime);
		Assert.Equal(firstMark.EndTime, grid.MarkSnapPointRegistrations[firstMark].EndSnapPoint!.SnapTime);
		Assert.Equal(secondMark.StartTime, grid.MarkSnapPointRegistrations[secondMark].StartSnapPoint.SnapTime);
		Assert.Null(grid.MarkSnapPointRegistrations[secondMark].EndSnapPoint);
	}

	[Fact]
	public void MarksMoving_WhenSeveralUpdatesArriveBeforePaint_UpdatesSnapPointsWithoutImmediateGridInvalidation()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection collection = CreateCollection(showTailGridLines: true);
		Mark movingMark = AddMark(collection, 1, 1);

		using Grid grid = CreateGrid(instanceId, collection);
		grid.CreateControl();
		int invalidationCount = 0;
		grid.Invalidated += (_, _) => invalidationCount++;

		movingMark.StartTime = TimeSpan.FromSeconds(2);
		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs([movingMark]));
		movingMark.StartTime = TimeSpan.FromSeconds(3);
		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs([movingMark]));
		movingMark.StartTime = TimeSpan.FromSeconds(4);
		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs([movingMark]));

		Assert.Equal(movingMark.StartTime, grid.MarkSnapPointRegistrations[movingMark].StartSnapPoint.SnapTime);
		Assert.Equal(movingMark.EndTime, grid.MarkSnapPointRegistrations[movingMark].EndSnapPoint!.SnapTime);
		Assert.Equal(0, invalidationCount);
	}

	[Fact]
	public void MarksMoving_WhenInvalidationIsSuppressed_UpdatesSnapPointsWithoutRequestingGridInvalidation()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection collection = CreateCollection(showTailGridLines: true);
		Mark movingMark = AddMark(collection, 1, 1);

		using Grid grid = CreateGrid(instanceId, collection);
		grid.CreateControl();
		int invalidationCount = 0;
		grid.Invalidated += (_, _) => invalidationCount++;
		grid.SuppressInvalidate = true;
		movingMark.StartTime = TimeSpan.FromSeconds(2);

		TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs([movingMark]));

		Assert.Equal(movingMark.StartTime, grid.MarkSnapPointRegistrations[movingMark].StartSnapPoint.SnapTime);
		Assert.Equal(movingMark.EndTime, grid.MarkSnapPointRegistrations[movingMark].EndSnapPoint!.SnapTime);
		Assert.Equal(0, invalidationCount);
	}

	[Fact]
	public void MarksMoving_WhenGridIsDisposed_DoesNotInvokeTheDisposedControl()
	{
		Guid instanceId = Guid.NewGuid();
		MarkCollection collection = CreateCollection();
		Mark mark = AddMark(collection, 1, 1);
		Grid grid = CreateGrid(instanceId, collection);

		grid.Dispose();
		mark.StartTime = TimeSpan.FromSeconds(2);
		Exception? exception = Record.Exception(() =>
			TimeLineGlobalEventManager.Manager(instanceId).OnMarksMoving(new MarksMovingEventArgs(new List<IMark> { mark })));

		Assert.Null(exception);
		Assert.Empty(grid.MarkSnapPointRegistrations);
	}

	private static Grid CreateGrid(params IMarkCollection[] markCollections)
	{
		return CreateGrid(Guid.NewGuid(), markCollections);
	}

	private static Grid CreateGrid(Guid instanceId, params IMarkCollection[] markCollections)
	{
		TimeInfo timeInfo = new()
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		Grid grid = new(timeInfo, instanceId);
		grid.MarkCollections = new ObservableCollection<IMarkCollection>(markCollections);
		return grid;
	}

	private static MarkCollection CreateCollection(bool showGridLines = true, bool showTailGridLines = false)
	{
		return new MarkCollection
		{
			ShowGridLines = showGridLines,
			ShowTailGridLines = showTailGridLines
		};
	}

	private static Mark AddMark(MarkCollection collection, int startSeconds, int durationSeconds)
	{
		Mark mark = new(TimeSpan.FromSeconds(startSeconds))
		{
			Duration = TimeSpan.FromSeconds(durationSeconds)
		};
		collection.AddMark(mark);
		return mark;
	}
}
