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
			byte[] bSamples = m_audioModule.GetSamples(0, (int)m_audioModule.NumberSamples);

			int dataStep = m_audioModule.BytesPerSample;
			float[] fSamplesAll = new float[m_audioModule.NumberSamples];

			for (j = 0; j < bSamples.Length; j += dataStep)
			{
				fSamplesAll[j / dataStep] =
					dataStep == 2 ? BitConverter.ToInt16(bSamples, j) : BitConverter.ToInt32(bSamples, j);
			}

			double progressVal = 0;
			float[] fSamples = new float[m_plugin.GetPreferredBlockSize()];
			for (j = 0;
				((fSamplesAll.Length - j) >= m_plugin.GetPreferredBlockSize());
				j += stepSize)
			{
				progressVal = ((double)j / (double) fSamplesAll.Length) * 100.0;
				progressDlg.UpdateProgress((int) progressVal);

				Array.Copy(fSamplesAll, j, fSamples, 0, fSamples.Length);
				m_plugin.Process(fSamples,
						ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));
			}

			Array.Clear(fSamples, 0, fSamples.Length);
			Array.Copy(fSamplesAll, j, fSamples, 0, fSamplesAll.Length - j);
			m_plugin.Process(fSamples,
					ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));

			progressDlg.Close();

			return m_plugin.GetRemainingFeatures();
			
		}

		private MarkCollection ExtractMarksFromFeatureset(int output, BeatBarSettings settings)
		{
			MarkCollection mc = new MarkCollection();
			mc.Enabled = true;
			mc.Name = settings.CollectionName;
			mc.MarkColor = settings.Color;

			double lastFeatureMS = -1;
			double featureMS = -1;

			foreach (ManagedFeature feature in m_featureSet[output])
			{
				if (feature.hasTimestamp)
				{
					featureMS = feature.timestamp.totalMilliseconds();
					if ((lastFeatureMS != -1) && (settings.Divisions > 1))
					{
						double interval = (featureMS - lastFeatureMS) /settings.Divisions;
						
						for (double j = lastFeatureMS; j <= featureMS; j += interval)
						{
							mc.Marks.Add(TimeSpan.FromMilliseconds(j));
						}
					}
					else
					{
						mc.Marks.Add(TimeSpan.FromMilliseconds(featureMS));
					}
				}
				lastFeatureMS = featureMS;
			}
			return mc;
		}

		public List<MarkCollection> GenerateMarksFromFeatures(List<MarkCollection> markCollection)
		{
			List<MarkCollection> retVal = markCollection;
			m_plugin = new QMBarBeatTrack(m_audioModule.Frequency);

			m_audioModule.LoadMedia(TimeSpan.Zero);

			ICollection<ManagedParameterDescriptor> parameterDescriptors =
				m_plugin.GetParameterDescriptors();

			ICollection<ManagedOutputDescriptor> outputDescriptors =
				m_plugin.GetOutputDescriptors();

			BeatsAndBarsDialog bbSettings = new BeatsAndBarsDialog();
			bbSettings.Parameters(parameterDescriptors);
			bbSettings.MarkCollectionList = markCollection;

			DialogResult result = bbSettings.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_plugin.SetParameter("bpb",bbSettings.BeatsPerBar());

				m_plugin.Initialise(1,
					(uint)m_plugin.GetPreferredStepSize(),
					(uint)m_plugin.GetPreferredBlockSize());

				m_featureSet = GenerateFeatures();

				if (bbSettings.BeatSettings.Enabled)
				{
					markCollection.RemoveAll(x => x.Name.Equals(bbSettings.BeatSettings.CollectionName));
					retVal.Add(ExtractMarksFromFeatureset(0, bbSettings.BeatSettings));
				}

				if (bbSettings.BarSettings.Enabled)
				{
					markCollection.RemoveAll(x => x.Name.Equals(bbSettings.BarSettings.CollectionName));
					retVal.Add(ExtractMarksFromFeatureset(1, bbSettings.BarSettings));
				}
			}

			return retVal;
		}
	}
}
