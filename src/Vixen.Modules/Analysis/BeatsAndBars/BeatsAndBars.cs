using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using QMLibrary;
using Vixen.Extensions;
using Vixen.Marks;
using Vixen.Module.Analysis;
using VixenModules.App.Marks;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	public class BeatsAndBars : AnalysisModuleInstanceBase
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private QMBarBeatTrack _plugin;
		private IDictionary<int, ICollection<ManagedFeature>> _featureSet;
		private readonly Audio _audioModule;

		private byte[] _bSamples;
		private float[] _fSamplesAll;
		private float[] _fSamplesPreview;

		private const int PreviewTime = 10;

		public BeatsAndBars(Audio module)
		{
			_audioModule = module;
			_featureSet = null;
		}

		public override void Loading() { }
		public override void Unloading() { }

		private IDictionary<int, ICollection<ManagedFeature>> GenerateFeatures(ManagedPlugin plugin, float[] fSampleData, bool showProgress = true)
		{
			if (!showProgress)
			{
				return GenerateFeatures(plugin, fSampleData, null);
			}

			IDictionary<int, ICollection<ManagedFeature>> result = null;
			Exception exception = null;

			using (var progressDialog = new BeatsAndBarsProgress())
			{
				progressDialog.Shown += async (sender, _) =>
				{
					var dialog = sender as BeatsAndBarsProgress;

					try
					{
						ArgumentNullException.ThrowIfNull(dialog);
						IProgress<(int Value, bool IsFinalizing)> progress =
							new Progress<(int Value, bool IsFinalizing)>(update =>
							{
								if (update.IsFinalizing)
								{
									dialog.SetFinalizing();
								}
								else
								{
									dialog.UpdateProgress(update.Value);
								}
							});

						result = await Task.Run(() => GenerateFeatures(plugin, fSampleData, progress));
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						dialog?.Close();
					}
				};

				progressDialog.ShowDialog();
			}

			if (exception != null)
			{
				ExceptionDispatchInfo.Capture(exception).Throw();
			}

			return result;
		}

		private IDictionary<int, ICollection<ManagedFeature>> GenerateFeatures(
			ManagedPlugin plugin,
			float[] fSampleData,
			IProgress<(int Value, bool IsFinalizing)> progress)
		{
			IDictionary<int, ICollection<ManagedFeature>> retVal =
				new ConcurrentDictionary<int, ICollection<ManagedFeature>>();

			int stepSize = plugin.GetPreferredStepSize();
			int processedBlockCount = 0;

			uint frequency = (uint)_audioModule.Frequency;
			if (frequency != 0)
			{
				float[] fSamples = new float[plugin.GetPreferredBlockSize()];
				int j;
				for (j = 0;
				     ((fSampleData.Length - j) >= plugin.GetPreferredBlockSize());
				     j += stepSize)
				{
					var progressVal = (j / (double)fSampleData.Length) * 100.0;
					progress?.Report(((int)progressVal, false));

					Array.Copy(fSampleData, j, fSamples, 0, fSamples.Length);
					plugin.Process(fSamples,
							ManagedRealtime.frame2RealTime(j, (uint)_audioModule.Frequency));
					processedBlockCount++;
				}

				Array.Clear(fSamples, 0, fSamples.Length);
				Array.Copy(fSampleData, j, fSamples, 0, fSampleData.Length - j);
				plugin.Process(fSamples,
						ManagedRealtime.frame2RealTime(j, (uint)_audioModule.Frequency));
				processedBlockCount++;

				progress?.Report((100, true));
				retVal = plugin.GetRemainingFeatures();
				LogFeatureSet(processedBlockCount, retVal);
			}

			return retVal;
		}

		private static void LogFeatureSet(int processedBlockCount, IDictionary<int, ICollection<ManagedFeature>> featureSet)
		{
			var missingOutputs = Enumerable.Range(0, 4)
				.Where(outputIndex => !featureSet.ContainsKey(outputIndex))
				.ToArray();
			var featureCounts = string.Join(", ", featureSet
				.OrderBy(output => output.Key)
				.Select(output => $"{output.Key}={output.Value.Count}"));

			Logging.Info(
				"Beat analysis processed {ProcessedBlockCount} audio blocks and returned feature counts: {FeatureCounts}.",
				processedBlockCount,
				featureCounts);

			if (missingOutputs.Length > 0)
			{
				Logging.Warn(
					"Beat analysis did not return the expected output indexes: {MissingOutputIndexes}. Returned feature counts: {FeatureCounts}.",
					string.Join(", ", missingOutputs),
					featureCounts);
			}
		}

		private void RemoveDuplicateMarks(ref MarkCollection mcOrig, List<MarkCollection> otherCollections)
		{
			if (otherCollections != null)
			{
				foreach (var otherCollection in otherCollections)
				{
					mcOrig.RemoveAll(x => otherCollection.Marks.Any(m => m.StartTime == x.StartTime && m.Duration == x.Duration));
				}
			}
		}

		private MarkCollection 
			ExtractAllMarksFromFeatureSet(ICollection<ManagedFeature> featureSet, 
											BeatBarSettingsData settings)
		{
			MarkCollection mc = new MarkCollection();
			mc.Name = settings.AllCollectionName;

			double lastFeatureMs = -1;

			foreach (ManagedFeature feature in featureSet)
			{
				if (feature.hasTimestamp)
				{
					var featureMs = feature.timestamp.totalMilliseconds();
					if (lastFeatureMs != -1)
					{
						double interval = (featureMs - lastFeatureMs) / settings.Divisions;
						for (int j = 0; j < settings.Divisions; j++)
						{
							mc.AddMark(new Mark(TimeSpan.FromMilliseconds(lastFeatureMs + (interval * j))));
						}
					}
					else
					{
						mc.AddMark(new Mark(TimeSpan.FromMilliseconds(featureMs)));
					}
					lastFeatureMs = featureMs;
				}
			}
			return mc;
		}

		private MarkCollection 
			ExtractBarMarksFromFeatureSet(ICollection<ManagedFeature> featureSet, 
											BeatBarSettingsData settings)
		{
			MarkCollection mc = new MarkCollection();
			mc.Name = settings.BarsCollectionName;

			foreach (ManagedFeature feature in featureSet)
			{
				if (feature.hasTimestamp)
				{
					var featureMs = feature.timestamp.totalMilliseconds();
					mc.AddMark(new Mark(TimeSpan.FromMilliseconds(featureMs)));
				}
			}
			return mc;
		}

		private List<MarkCollection> 
			ExtractBeatCollectionsFromFeatureSet(ICollection<ManagedFeature> featureSet, 
													BeatBarSettingsData settings,
													List<MarkCollection> otherMarks = null)
		{
			List<MarkCollection> retVal = new List<MarkCollection>();
			string[] collectionNames = settings.BeatCollectionNames(false);

			for (int j = 1; j <= collectionNames.Length; j++)
			{
				MarkCollection mc = new MarkCollection();
				mc.Name = collectionNames[j-1];

				foreach (ManagedFeature feature in featureSet)
				{
					if (feature.hasTimestamp && feature.label == j.ToString())
					{
						var featureMs = feature.timestamp.totalMilliseconds();
						mc.AddMark(new Mark(TimeSpan.FromMilliseconds(featureMs)));
					}
				}
				RemoveDuplicateMarks(ref mc, otherMarks);
				retVal.Add(mc);
			}
			return retVal;
		}

		private List<MarkCollection> 
			ExtractSplitCollectionsFromFeatureSet(ICollection<ManagedFeature> featureSet, 
													BeatBarSettingsData settings,
													List<MarkCollection> otherMarks = null )
		{		
			List<MarkCollection> retVal = new List<MarkCollection>();
			string[] collectionNames = settings.BeatCollectionNames(true);
			KeyValuePair<int,double>[] tsValuePairs = new KeyValuePair<int, double>[(featureSet.Count * 2)];

			int count = 0;

			ManagedFeature lastFeature = null;

			foreach (ManagedFeature feature in featureSet)
			{
				if (lastFeature == null)
				{
					lastFeature = feature;
					continue;
				}

				var labelVal = (Convert.ToInt32(lastFeature.label) * 2) - 1;
				var lastFeatureMs = lastFeature.timestamp.totalMilliseconds();

				tsValuePairs[count++] =
					new KeyValuePair<int, double>(labelVal, lastFeatureMs);

				var featureMs = feature.timestamp.totalMilliseconds();
				tsValuePairs[count] =
					new KeyValuePair<int, double>(labelVal + 1,
						lastFeatureMs + ((featureMs - lastFeatureMs) / settings.Divisions));

				count++;
				lastFeature = feature;
			}

			for (int j = 1; j <= collectionNames.Length; j++)
			{
				MarkCollection mc = new MarkCollection();
				mc.Name = collectionNames[j - 1];
				foreach (KeyValuePair<int,double> tsValue in tsValuePairs)
				{
					if (tsValue.Key == j)
					{
						mc.AddMark(new Mark(TimeSpan.FromMilliseconds(tsValue.Value)));
					}
				}
				RemoveDuplicateMarks(ref mc, otherMarks);
				retVal.Add(mc);
			}
			return retVal;
		}

		private double EstimateBeatPeriod(ICollection<ManagedFeature> features)
		{
			double retVal = 0;
			double lastFeatureMS = -1;
			bool startCalcs = false;

			foreach (ManagedFeature feature in features)
			{
				startCalcs = (feature.label.Equals("1") ? true : startCalcs);

				if ((feature.hasTimestamp) && (startCalcs))
				{
					var featureMs = feature.timestamp.totalMilliseconds();

					if ((lastFeatureMS != -1) && (retVal == 0))
					{
						retVal = featureMs - lastFeatureMS;
					}

					if (lastFeatureMS > 0)
					{
						retVal = (retVal + (featureMs - lastFeatureMS)) / 2;
					}

					lastFeatureMS = featureMs;
				}
			}
			return retVal;
		}

		private BeatBarPreviewData GeneratePreviewData()
		{
			BeatBarPreviewData previewData = new BeatBarPreviewData(1);
			using QMBarBeatTrack plugin = new QMBarBeatTrack(_audioModule.Frequency);
			plugin.SetParameter("bpb", 4);

			plugin.Initialise(1,
				(uint)plugin.GetPreferredStepSize(),
				(uint)plugin.GetPreferredBlockSize());


			IDictionary<int, ICollection<ManagedFeature>> featureSet = GenerateFeatures(plugin, _fSamplesPreview, false);
			previewData.BeatPeriod = EstimateBeatPeriod(featureSet[2]);

			BeatBarSettingsData settings = new BeatBarSettingsData("Preview");
			settings.Divisions = 1;
			settings.BeatSplitsEnabled = false;
			settings.NoteSize = 4;
			settings.BeatsPerBar = 4;

			List<MarkCollection> collections = ExtractBeatCollectionsFromFeatureSet(featureSet[2], settings);
			MarkCollection allCollection = new MarkCollection();
			allCollection.Name = "Beat Marks";
			collections.ForEach(x => allCollection.AddMarks(x.Marks));
			allCollection.EnsureOrder();
			previewData.PreviewCollection = allCollection;

			settings.BeatSplitsEnabled = true;
			settings.Divisions = 2;
			collections = ExtractSplitCollectionsFromFeatureSet(featureSet[2], settings);
			allCollection = new MarkCollection();
			allCollection.Name = "Beat Split Marks";
			collections.ForEach(x => allCollection.AddMarks(x.Marks));
			allCollection.EnsureOrder();
			previewData.PreviewSplitCollection = allCollection;

			return previewData;
		}

		private void BuildMarkCollections(ICollection<IMarkCollection> markCollection, 
															BeatBarSettingsData settings)
		{
			List<MarkCollection> retVal = new List<MarkCollection>();

			_featureSet = GenerateFeatures(_plugin, _fSamplesAll);
			String[] beatCollectionNames = settings.BeatCollectionNames(false);
			String[] splitCollectionNames = settings.BeatCollectionNames(true);

			if (settings.BarsEnabled)
			{
				markCollection.RemoveAll(x => x.Name.Equals(settings.BarsCollectionName));
				var mc = ExtractBarMarksFromFeatureSet(_featureSet[1], settings);
				mc.Decorator.Color = settings.BarsColor;
				retVal.Add(mc);
			}

			if (settings.BeatCollectionsEnabled)
			{
				foreach (String name in beatCollectionNames)
				{
					markCollection.RemoveAll(x => x.Name.Equals(name));
				}
				List<MarkCollection> mcl = ExtractBeatCollectionsFromFeatureSet(_featureSet[2], settings, retVal);
				mcl.ForEach(x => x.Decorator.Color = settings.BeatCountsColor);
				retVal.AddRange(mcl);
			}

			if (settings.BeatSplitsEnabled)
			{
				foreach (String name in splitCollectionNames)
				{
					markCollection.RemoveAll(x => x.Name.Equals(name));
				}
				List<MarkCollection> mcl = ExtractSplitCollectionsFromFeatureSet(_featureSet[2], settings, retVal);
				mcl.ForEach(x => x.Decorator.Color = settings.BeatSplitsColor);
				retVal.AddRange(mcl);
			}

			if (settings.AllFeaturesEnabled)
			{
				markCollection.RemoveAll(x => x.Name.Equals(settings.AllCollectionName));
				MarkCollection mc = ExtractAllMarksFromFeatureSet(_featureSet[0], settings);
				mc.Decorator.Color = settings.AllFeaturesColor;
				retVal.Add(mc);
			}

			retVal.RemoveAll(x => x.Marks.Count == 0);
			
			markCollection.AddRange(retVal.OrderBy(x => x.Name));
		}

		public void DoBeatBarDetection(ICollection<IMarkCollection> markCollection)
		{
			if (_audioModule.Channels != 0)
			{
				_plugin = new QMBarBeatTrack(_audioModule.Frequency);
				try
				{
					_bSamples = _audioModule.GetRawAudioSamples();
					_fSamplesAll = new float[_bSamples.Length / _audioModule.BytesPerSample];
					_fSamplesPreview = new float[(int)(_audioModule.Frequency * PreviewTime)];

					int dataStep = _audioModule.BytesPerSample;

					for (int j = 0, sampleNum = 0; j < _bSamples.Length; j += dataStep, sampleNum++)
					{
						_fSamplesAll[sampleNum] = dataStep == 2 ?
							BitConverter.ToInt16(_bSamples, j) : BitConverter.ToInt32(_bSamples, j);
					}

					Array.Copy(_fSamplesAll,
								_fSamplesPreview,
								(int)Math.Min((_audioModule.Frequency * PreviewTime), _fSamplesAll.Length));

					BeatsAndBarsDialog bbSettings = new BeatsAndBarsDialog(_audioModule);
					bbSettings.PreviewData = GeneratePreviewData();
					
					DialogResult result = bbSettings.ShowDialog();
					if (result == DialogResult.OK)
					{
						_plugin.SetParameter("bpb", bbSettings.Settings.BeatsPerBar);

						_plugin.Initialise(1,
							(uint)_plugin.GetPreferredStepSize(),
							(uint)_plugin.GetPreferredBlockSize());

						BuildMarkCollections(markCollection, bbSettings.Settings);
					}
				}
				finally
				{
					_plugin.Dispose();
					_plugin = null;
				}
			}

			if (markCollection.Any() && !markCollection.Any(x => x.IsDefault))
			{
				markCollection.First().IsDefault = true;
			}
		}

	}

	public class BeatBarPreviewData
	{
		public BeatBarPreviewData(double period)
		{
			BeatPeriod = period;
		}

		public double BeatPeriod { get; set; }
		public MarkCollection PreviewCollection { get; set; }
		public MarkCollection PreviewSplitCollection { get; set; }
	}

	public class BeatBarSettingsData
	{
		public bool BarsEnabled { get; set; }
		public bool BeatCollectionsEnabled { get; set; }
		public bool BeatSplitsEnabled { get; set; }
		public bool AllFeaturesEnabled { get; set; }
		public string CollectionBaseName { get; set; }

		public int Divisions { get; set; }

		public Color AllFeaturesColor { get; set; }
		public Color BarsColor { get; set; }
		public Color BeatCountsColor { get; set; }
		public Color BeatSplitsColor { get; set; }

		public int BeatsPerBar { get; set; }
		public int NoteSize { get; set; }

		public BeatBarPreviewData PreviewData { get; set; }

		public String AllCollectionName
		{
			get { return CollectionBaseName + " - All"; }
		}

		public String BarsCollectionName
		{
			get { return CollectionBaseName + " 1/" + NoteSize + " Beat #1 (Bar)"; }
		}

		/// <summary>
		/// Gets the names of the beat mark collections for the configured meter and subdivision.
		/// </summary>
		/// <param name="addDivisions"><see langword="true"/> to return names for each beat subdivision; otherwise, <see langword="false"/>.</param>
		/// <returns>A collection of beat mark collection names.</returns>
		public String[] BeatCollectionNames(bool addDivisions)
		{
			int collections = BeatsPerBar * (addDivisions ? Divisions : 1);
			
			String[] retVal = new string[collections];

			for (int j = 0; j < collections; j++)
			{
				// ReSharper disable once PossibleLossOfFraction
				decimal colNum = (addDivisions ? j/2 : j) + 1;
				retVal[j] = CollectionBaseName + " Beat #" + colNum +
				            (addDivisions ? (j%2 == 0 ? "a" : "b") : "");
			}

			return retVal;
		}

		public BeatBarSettingsData(string collectionBaseName)
		{
			BarsEnabled = false;
			BeatCollectionsEnabled = false;
			BeatSplitsEnabled = false;
			AllFeaturesEnabled = false;

			CollectionBaseName = collectionBaseName;
			AllFeaturesColor = Color.White;
			BarsColor = Color.White;
			BeatCountsColor = Color.White;
			BeatSplitsColor = Color.White;
			Divisions = 1;
		}

	}
}
