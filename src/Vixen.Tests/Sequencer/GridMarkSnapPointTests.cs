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

	private static Grid CreateGrid(params IMarkCollection[] markCollections)
	{
		TimeInfo timeInfo = new()
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		Grid grid = new(timeInfo, Guid.NewGuid());
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
