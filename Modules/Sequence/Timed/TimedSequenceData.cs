using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Marks;
using Vixen.Module;
using VixenModules.App.Marks;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	[KnownType(typeof(App.Marks.MarkCollection))]
	public class TimedSequenceData : BaseSequence.SequenceData
	{
		[DataMember(EmitDefaultValue = false)]
		public List<MarkCollection> MarkCollections { get; set; }

		[DataMember]
		public ObservableCollection<IMarkCollection> LabeledMarkCollections { get; set; }

		[DataMember]
		public TimeSpan TimePerPixel { get; set; }

		[DataMember]
		public TimeSpan VisibleTimeStart { get; set; }

		[DataMember]
		public int DefaultRowHeight { get; set; }

		[DataMember]
		public int DefaultSplitterDistance { get; set; }

		[DataMember]
		public RowSettings RowSettings { get; set; }
		

		[DataMember]
		public TimeSpan? DefaultPlaybackEndTime { get; set; }

		[DataMember]
		public TimeSpan? DefaultPlaybackStartTime { get; set; }

		public TimedSequenceData()
		{
			MarkCollections = default(List<MarkCollection>);
			LabeledMarkCollections = new ObservableCollection<IMarkCollection>();
			TimePerPixel = TimeSpan.MinValue;
			DefaultRowHeight = 0;
			RowSettings = new RowSettings();
			VisibleTimeStart = TimeSpan.MinValue;
			DefaultSplitterDistance = 0;
			DefaultPlaybackStartTime = TimeSpan.Zero;
			DefaultPlaybackEndTime = TimeSpan.Zero;
		}

		public void ConvertMarksToLabeledMarks()
		{
			//Temp method to convert until existing code is refactored and migration can occur
			LabeledMarkCollections = new ObservableCollection<IMarkCollection>();
			foreach (var markCollection in MarkCollections)
			{
				var lmc = new App.Marks.MarkCollection();
				lmc.Name = markCollection.Name;
				lmc.Level = markCollection.Level;
				lmc.ShowGridLines = markCollection.Enabled;
				lmc.Decorator = new MarkDecorator
				{
					Color = markCollection.MarkColor,
					IsBold = markCollection.Bold,
					IsSolidLine = markCollection.SolidLine
				};
				markCollection.Marks.ForEach(x => lmc.AddMark(new Mark(x)));  
				LabeledMarkCollections.Add(lmc);
			}

			if (LabeledMarkCollections.Any())
			{
				//Set one of them active
				var mc = LabeledMarkCollections.FirstOrDefault(x => x.IsVisible);
				if (mc != null)
				{
					mc.IsDefault = true;
				}
				else
				{
					LabeledMarkCollections.First().IsDefault = true;
				}
			}
			
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			if (RowSettings == null)
			{
				RowSettings = new RowSettings();
			}

			if (LabeledMarkCollections == null)
			{
				ConvertMarksToLabeledMarks();
				MarkCollections =  default(List<MarkCollection>);
			}
		}

		public override IModuleDataModel Clone()
		{
			TimedSequenceData result = new TimedSequenceData();
			// Cloning each MarkCollection so that the cloned data objects don't share references
			// and step on each other.
			result.LabeledMarkCollections = new ObservableCollection<IMarkCollection>(LabeledMarkCollections.Select(x => (IMarkCollection)x.Clone()));
			return result;
		}
	}
}