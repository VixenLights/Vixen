using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

		private Audio m_audioModule = null;

		public BeatsAndBars(Audio module)
		{
			m_audioModule = module;

		}

		public override void Loading()
		{

		}

		public override void Unloading()
		{

		}

		public List<MarkCollection> GenerateMarksFromFeatures()
		{
			List<MarkCollection> retVal = new List<MarkCollection>();
			if (InitPlugin())
			{ 
				int stepSize = m_plugin.GetPreferredStepSize();
				m_audioModule.LoadMedia(TimeSpan.Zero);

				ICollection<ManagedParameterDescriptor> parameterDescriptors =
					m_plugin.GetParameterDescriptors();

				ICollection<ManagedOutputDescriptor> outputDescriptors =
					m_plugin.GetOutputDescriptors();

				//Rename description for Beat Locations
				outputDescriptors.First().description = "Beat Locations";
				//Remove last two outputs as we have no interest in them here
				outputDescriptors.Remove(outputDescriptors.Last());
				outputDescriptors.Remove(outputDescriptors.Last());

				BeatsAndBarsSettings bbSettings = new BeatsAndBarsSettings();
				//bbSettings.Parameters(parameterDescriptors, outputDescriptors);
				bbSettings.Parameters(parameterDescriptors);
				bbSettings.Outputs(outputDescriptors);
				bbSettings.ShowDialog();

				IDictionary<int, ICollection<ManagedFeature>> featureSet;

				int i = 0;
				int j = 0;

				byte[] bSamples = m_audioModule.GetSamples(0,(int)m_audioModule.NumberSamples);

				int dataStep = m_audioModule.BytesPerSample;
				float[] fSamplesAll = new float[m_audioModule.NumberSamples];
				for (j = 0; j < bSamples.Length; j += dataStep)
				{
					fSamplesAll[j / dataStep] =
						dataStep == 2 ? BitConverter.ToInt16(bSamples, j) : BitConverter.ToInt32(bSamples, j);
				}

				float [] fSamples = new float[m_plugin.GetPreferredBlockSize()];
				for (j = 0;
					((fSamplesAll.Length - j) >= m_plugin.GetPreferredBlockSize());
					j += stepSize)
				{
					Array.Copy(fSamplesAll,j,fSamples,0,fSamples.Length);
					m_plugin.Process(fSamples,
							ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));
				}

				Array.Clear(fSamples, 0, fSamples.Length);
				Array.Copy(fSamplesAll, j, fSamples, 0, fSamplesAll.Length - j);
				m_plugin.Process(fSamples,
						ManagedRealtime.frame2RealTime(j, (uint)m_audioModule.Frequency));

				featureSet = m_plugin.GetRemainingFeatures();


				int numOutputs = m_plugin.GetOutputDescriptors().Count();
				for (int pluginOutput = 0; pluginOutput < numOutputs; pluginOutput++)
				{
					MarkCollection mc = new MarkCollection();
					mc.Enabled = true;
					mc.Name = "Output " + pluginOutput;

					foreach (ManagedFeature feature in featureSet[pluginOutput])
					//Hardcode this for the moment featureSet[1] is Bars This may ned a discovery function
					{
						if (feature.hasTimestamp)
						{
							mc.Marks.Add(TimeSpan.FromMilliseconds(feature.timestamp.totalMilliseconds()));
						}
					}
					retVal.Add(mc);
				}
			}
			return retVal;
		}

		private bool InitPlugin()
		{
			bool didInit = true;
			//if (m_plugin == null)
			//{
				m_plugin = new QMBarBeatTrack(m_audioModule.Frequency);
				didInit = m_plugin.Initialise(1, 
					(uint)m_plugin.GetPreferredStepSize(),
					(uint)m_plugin.GetPreferredBlockSize());
			//}
			//if (didInit == false)
			//{
			//	m_plugin = null;
			//}

			return didInit;
		}

	}
}
