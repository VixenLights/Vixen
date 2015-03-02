using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Analysis;
using VixenModules.Media.Audio;
using VixenModules.Sequence.Timed;
using QMLibrary;

namespace VixenModules.Analysis.BeatsAndBars
{
	public class BeatsAndBars : AnalysisModuleInstanceBase
	{
		private ManagedPlugin m_plugin = null;
		private IDictionary<int, ICollection<ManagedFeature>> m_featureSet;
		private Audio m_audioModule = null;

		private byte[] m_bSamples;
		private float[] m_fSamplesAll;

		public BeatsAndBars(Audio module)
		{
			m_audioModule = module;
			m_featureSet = null;
		}

		public override void Loading()
		{

		}

		public override void Unloading()
		{

		}

		private IDictionary<int, ICollection<ManagedFeature>> GenerateFeatures()
		{
			int i = 0;
			int j = 0;

			BeatsAndBarsProgress progressDlg = new BeatsAndBarsProgress();
			progressDlg.Show();

			int stepSize = m_plugin.GetPreferredStepSize();

			double progressVal = 0;
			float[] fSamples = new float[m_plugin.GetPreferredBlockSize()];
			for (j = 0;
				((m_fSamplesAll.Length - j) >= m_plugin.GetPreferredBlockSize());
				j += stepSize)
			{
				progressVal = ((double)j / (double) m_fSamplesAll.Length) * 100.0;
				progressDlg.UpdateProgress((int) progressVal);

				Array.Copy(m_fSamplesAll, j, fSamples, 0, fSamples.Length);
				m_plugin.Process(fSamples,
						ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));
			}

			Array.Clear(fSamples, 0, fSamples.Length);
			Array.Copy(m_fSamplesAll, j, fSamples, 0, m_fSamplesAll.Length - j);
			m_plugin.Process(fSamples,
					ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));

			progressDlg.Close();

			return m_plugin.GetRemainingFeatures();
			
		}

		private MarkCollection ExtractAllMarksFromFeatureSet(BeatBarSettingsData settings)
		{
			MarkCollection mc = new MarkCollection();
			mc.Enabled = true;
			mc.Name = settings.AllCollectionName;

			double lastFeatureMS = -1;
			double featureMS = -1;

			foreach (ManagedFeature feature in m_featureSet[0])
			{
				if (feature.hasTimestamp)
				{
					featureMS = feature.timestamp.totalMilliseconds();
					if (lastFeatureMS != -1)
					{
						double interval = (featureMS - lastFeatureMS) / settings.Divisions;
						for (int j = 0; j < settings.Divisions; j++)
						{
							mc.Marks.Add(TimeSpan.FromMilliseconds(lastFeatureMS + (interval * j)));
						}
					}
					else
					{
						mc.Marks.Add(TimeSpan.FromMilliseconds(featureMS));
					}
					lastFeatureMS = featureMS;
				}
			}
			return mc;
		}

		private MarkCollection ExtractBarMarksFromFeatureSet(BeatBarSettingsData settings)
		{
			MarkCollection mc = new MarkCollection();
			mc.Enabled = true;
			mc.Name = settings.BarsCollectionName;

			double featureMS = -1;

			foreach (ManagedFeature feature in m_featureSet[1])
			{
				if (feature.hasTimestamp)
				{
					featureMS = feature.timestamp.totalMilliseconds();
					mc.Marks.Add(TimeSpan.FromMilliseconds(featureMS));
				}
			}
			return mc;
		}

		private List<MarkCollection> ExtractBeatCollectionsFromFeatureSet(BeatBarSettingsData settings)
		{
			List<MarkCollection> retVal = new List<MarkCollection>();
			string[] collectionNames = settings.BeatCollectionNames(false);

			for (int j = 1; j <= collectionNames.Length; j++)
			{
				MarkCollection mc = new MarkCollection();
				mc.Enabled = true;
				mc.Name = collectionNames[j-1];

				double featureMS = -1;

				foreach (ManagedFeature feature in m_featureSet[2])
				{
					if ((feature.hasTimestamp) && (feature.label == j.ToString()))
					{
						featureMS = feature.timestamp.totalMilliseconds();
						mc.Marks.Add(TimeSpan.FromMilliseconds(featureMS));
					}
				}
				retVal.Add(mc);
			}
			return retVal;
		}

		private List<MarkCollection> ExtractSplitCollectionsFromFeatureSet(BeatBarSettingsData settings)
		{		
			List<MarkCollection> retVal = new List<MarkCollection>();
			string[] collectionNames = settings.BeatCollectionNames(true);
			KeyValuePair<int,double>[] tsValuePairs = new KeyValuePair<int, double>[(m_featureSet[2].Count * 2)];

			int count = 0;

			double featureMS = -1;
			double lastFeatureMS = -1;
			int labelVal = 0;

			ManagedFeature lastFeature = null;

			foreach (ManagedFeature feature in m_featureSet[2])
			{
				if (lastFeature == null)
				{
					lastFeature = feature;
					continue;
				}

				labelVal = (Convert.ToInt32(lastFeature.label) * 2) - 1;
				lastFeatureMS = lastFeature.timestamp.totalMilliseconds();

				tsValuePairs[count++] =
					new KeyValuePair<int, double>(labelVal, lastFeatureMS);

				featureMS = feature.timestamp.totalMilliseconds();
				tsValuePairs[count] =
					new KeyValuePair<int, double>(labelVal + 1,
						lastFeatureMS + ((featureMS - lastFeatureMS) / settings.Divisions));

				count++;
				lastFeature = feature;
			}

			for (int j = 1; j <= collectionNames.Length; j++)
			{
				MarkCollection mc = new MarkCollection();
				mc.Enabled = true;
				mc.Name = collectionNames[j - 1];

				foreach (KeyValuePair<int,double> tsValue in tsValuePairs)
				{
					if (tsValue.Key == j)
					{
						mc.Marks.Add(TimeSpan.FromMilliseconds(tsValue.Value));
					}
				}

				retVal.Add(mc);
			}
			return retVal;
		}


		public List<MarkCollection> GenerateMarksFromFeatures(List<MarkCollection> markCollection)
		{
			List<MarkCollection> retVal = markCollection;
			m_plugin = new QMBarBeatTrack(m_audioModule.Frequency);

			m_audioModule.LoadMedia(TimeSpan.Zero);
			m_bSamples = m_audioModule.GetSamples(0, (int)m_audioModule.NumberSamples);
			m_fSamplesAll = new float[m_audioModule.NumberSamples];

			int dataStep = m_audioModule.BytesPerSample;

			for (int j = 0; j < m_bSamples.Length; j += dataStep)
			{
				m_fSamplesAll[j / dataStep] = dataStep == 2 ? 
					BitConverter.ToInt16(m_bSamples, j) : BitConverter.ToInt32(m_bSamples, j);
			}

			ICollection<ManagedParameterDescriptor> parameterDescriptors =
				m_plugin.GetParameterDescriptors();

			ICollection<ManagedOutputDescriptor> outputDescriptors =
				m_plugin.GetOutputDescriptors();

			BeatsAndBarsDialog bbSettings = new BeatsAndBarsDialog(m_plugin);
			bbSettings.Parameters(parameterDescriptors);
			bbSettings.MarkCollectionList = markCollection;

			DialogResult result = bbSettings.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_featureSet = GenerateFeatures();
				String[] beatCollectionNames = bbSettings.Settings.BeatCollectionNames(false);
				String[] splitCollectionNames = bbSettings.Settings.BeatCollectionNames(true);

				if (bbSettings.Settings.AllFeaturesEnabled)
				{
					markCollection.RemoveAll(x => x.Name.Equals(bbSettings.Settings.AllCollectionName));
					MarkCollection mc = ExtractAllMarksFromFeatureSet(bbSettings.Settings);
					mc.MarkColor = bbSettings.Settings.Color;
					retVal.Add(mc);
				}

				if (bbSettings.Settings.BarsEnabled)
				{
					markCollection.RemoveAll(x => x.Name.Equals(bbSettings.Settings.BarsCollectionName));
					MarkCollection mc = ExtractBarMarksFromFeatureSet(bbSettings.Settings);
					mc.MarkColor = bbSettings.Settings.Color;
					retVal.Add(mc);					
				}

				if (bbSettings.Settings.BeatCollectionsEnabled)
				{
					foreach (String name in beatCollectionNames)
					{
						markCollection.RemoveAll(x => x.Name.Equals(name));
					}
					List<MarkCollection> mcl = ExtractBeatCollectionsFromFeatureSet(bbSettings.Settings);
					mcl.ForEach(x => x.MarkColor = bbSettings.Settings.Color);
					retVal.AddRange(mcl);
				}

				if (bbSettings.Settings.BeatSplitsEnabled)
				{
					foreach (String name in splitCollectionNames)
					{
						markCollection.RemoveAll(x => x.Name.Equals(name));
					}
					List<MarkCollection> mcl = ExtractSplitCollectionsFromFeatureSet(bbSettings.Settings);
					mcl.ForEach(x => x.MarkColor = bbSettings.Settings.Color);
					retVal.AddRange(mcl);
				}
			}

			return retVal;
		}
	}
}
