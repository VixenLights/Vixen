using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Sys.Marks;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class TimedSequenceData : BaseSequence.SequenceData
	{
		[DataMember(EmitDefaultValue = false)]
		public List<MarkCollection> MarkCollections { get; set; }

		[DataMember]
		public List<Vixen.Sys.Marks.MarkCollection> LabeledMarkCollections { get; set; }

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
			MarkCollections = new List<MarkCollection>();
			LabeledMarkCollections = new List<Vixen.Sys.Marks.MarkCollection>();
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
			LabeledMarkCollections = new List<Vixen.Sys.Marks.MarkCollection>();
			foreach (var markCollection in MarkCollections)
			{
				var lmc = new Vixen.Sys.Marks.MarkCollection();
				lmc.Name = markCollection.Name;
				lmc.Level = markCollection.Level;
				lmc.IsEnabled = markCollection.Enabled;
				lmc.Decorator = new MarkDecorator
				{
					Color = markCollection.MarkColor,
					IsBold = markCollection.Bold,
					IsSolidLine = markCollection.SolidLine
				};
				markCollection.Marks.ForEach(x => lmc.AddMark(new Mark(x) { Text = @"Mark" }));  
				LabeledMarkCollections.Add(lmc);
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
			result.LabeledMarkCollections = LabeledMarkCollections.Select(x => (Vixen.Sys.Marks.MarkCollection)x.Clone()).ToList();
			return result;
		}
	}
}