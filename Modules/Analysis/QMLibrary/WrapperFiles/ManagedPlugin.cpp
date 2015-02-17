
#include <msclr\marshal_cppstd.h>
#include <cliext\map>
#include <ManagedPlugin.h>

using namespace System;
using namespace msclr::interop;


ManagedPlugin::ManagedPlugin(IntPtr plugin)
	{
		m_plugin = (Vamp::Plugin*) plugin.ToPointer();

	}

bool ManagedPlugin::Initialise(size_t channels, size_t stepSize, size_t blockSize)
	{
		return m_plugin->initialise(channels, stepSize, blockSize);
	}

void ManagedPlugin::Reset()
	{
		m_plugin->reset();
	}

String^ ManagedPlugin::GetIdentifier()
	{
		std::string str = m_plugin->getIdentifier();
		return gcnew String(str.c_str());
	}

String^ ManagedPlugin::GetName()
	{
		std::string str = m_plugin->getName();
		return gcnew String(str.c_str());
	}

String^ ManagedPlugin::GetDescription()
	{
		std::string str = m_plugin->getDescription();
		return gcnew String(str.c_str());
	}

String^ ManagedPlugin::GetMaker()
	{
		std::string str = m_plugin->getMaker();
		return gcnew String(str.c_str());
	}

int ManagedPlugin::GetPluginVersion()
	{
		return m_plugin->getPluginVersion();
	}

String^ ManagedPlugin::GetCopyright()
	{
		std::string str = m_plugin->getCopyright();
		return gcnew String(str.c_str());
	}

float ManagedPlugin::GetParameter(String^ paramStr)
	{
		return m_plugin->getParameter(marshal_as<std::string>(paramStr));
	}

void ManagedPlugin::SetParameter(String^paramStr, float value)
	{
		m_plugin->setParameter(marshal_as<std::string>(paramStr), value);
}

int ManagedPlugin::GetMinChannelCount()
{
	return m_plugin->getMinChannelCount();
}

int ManagedPlugin::GetMaxChannelCount()
{
	return m_plugin->getMaxChannelCount();
}

int ManagedPlugin::GetPreferredStepSize()
	{
		return m_plugin->getPreferredStepSize();
	}

int ManagedPlugin::GetPreferredBlockSize()
	{
		return m_plugin->getPreferredBlockSize();
	}

ManagedParameterList^ ManagedPlugin::GetParameterDescriptors()
	{
		ManagedParameterDescriptor^ paramDescr = nullptr;
		ManagedParameterListPriv^ retVal = nullptr;
		Vamp::Plugin::ParameterList paramList = m_plugin->getParameterDescriptors();
		if (!paramList.empty())
		{
			retVal = gcnew ManagedParameterListPriv(paramList.size());
			for (int j = 0; j < paramList.size(); j++)
			{
				paramDescr = gcnew ManagedParameterDescriptor();
				paramDescr->defaultValue = paramList[j].defaultValue;
				paramDescr->description = gcnew String(paramList[j].description.c_str());
				paramDescr->identifier = gcnew String(paramList[j].identifier.c_str());
				paramDescr->isQuantized = paramList[j].isQuantized;
				paramDescr->maxValue = paramList[j].maxValue;
				paramDescr->minValue = paramList[j].minValue;
				paramDescr->name = gcnew String(paramList[j].name.c_str());
				paramDescr->quantizeStep = paramList[j].quantizeStep;
				paramDescr->unit = gcnew String(paramList[j].unit.c_str());

				int valNameSize = paramList[j].valueNames.size();
				cliext::vector<String^>^ valueNames = gcnew cliext::vector<String^>(valNameSize);
				for (int k = 0; k < valNameSize; k++)
				{
					valueNames[k] = gcnew String(paramList[j].valueNames[k].c_str());
				}
				paramDescr->valueNames = valueNames;
				retVal[j] = paramDescr;
			}

		}

		return retVal;
	}

ManagedOutputList^ ManagedPlugin::GetOutputDescriptors()
	{
		ManagedOutputDescriptor^ outDescr = nullptr;
		ManagedOutputListPriv^ retVal = nullptr;
		Vamp::Plugin::OutputList outList = m_plugin->getOutputDescriptors();
		if (!outList.empty())
		{
			retVal = gcnew ManagedOutputListPriv(outList.size());
			for (int j = 0; j < outList.size(); j++)
			{
				outDescr = gcnew ManagedOutputDescriptor();
				outDescr->binCount = outList[j].binCount;

				int binNameSize = outList[j].binNames.size();
				cliext::vector<String^>^ binNames = gcnew cliext::vector<String^>(binNameSize);
				for (int k = 0; k < binNameSize; k++)
				{
					binNames[k] = gcnew String(outList[j].binNames[k].c_str());
				}
				outDescr->binNames = binNames;
				outDescr->description = gcnew String(outList[j].description.c_str());
				outDescr->hasDuration = outList[j].hasDuration;
				outDescr->hasFixedBinCount = outList[j].hasFixedBinCount;
				outDescr->hasKnownExtents = outList[j].hasKnownExtents;
				outDescr->identifier = gcnew String(outList[j].identifier.c_str());
				outDescr->isQuantized = outList[j].isQuantized;
				outDescr->maxValue = outList[j].maxValue;
				outDescr->minValue = outList[j].minValue;
				outDescr->name = gcnew String(outList[j].name.c_str());
				outDescr->quantizeStep = outList[j].quantizeStep;
				outDescr->sampleRate = outList[j].sampleRate;

				switch (outList[j].sampleType)
				{
				case (Vamp::Plugin::OutputDescriptor::SampleType::FixedSampleRate) :
				{
					outDescr->sampleType = ManagedOutputDescriptor::SampleType::FixedSampleRate;
					break;
				}

				case (Vamp::Plugin::OutputDescriptor::SampleType::OneSamplePerStep) :
				{
					outDescr->sampleType = ManagedOutputDescriptor::SampleType::OneSamplePerStep;
					break;
				}

				case (Vamp::Plugin::OutputDescriptor::SampleType::VariableSampleRate) :
				{
					outDescr->sampleType = ManagedOutputDescriptor::SampleType::VariableSampleRate;
					break;
				}

				default:
				{
					outDescr->sampleType = ManagedOutputDescriptor::SampleType::Uknown;
					break;
				}
				}

				outDescr->unit = gcnew String(outList[j].unit.c_str());


				retVal[j] = outDescr;
			}

		}

		return retVal;
	}

ManagedPlugin::InputDomain ManagedPlugin::GetInputDomain()
	{
		return InputDomain::TimeDomain;
	}


ManagedFeatureSet^ ManagedPlugin::convertToManagedFeatureSet(Vamp::Plugin::FeatureSet unManagedFS)
	{
		ManagedFeatureSetPriv^ retVal = gcnew ManagedFeatureSetPriv();

		Vamp::Plugin::FeatureSet::iterator fsIterator;
		Vamp::Plugin::FeatureList::iterator flIterator;
		for (fsIterator = unManagedFS.begin(); fsIterator != unManagedFS.end(); ++fsIterator)
		{
			ManagedFeatureListPriv^ featureList = gcnew ManagedFeatureListPriv();
			for (flIterator = fsIterator->second.begin(); flIterator != fsIterator->second.end(); ++flIterator)
			{
				ManagedFeature^ feature = gcnew ManagedFeature();

				if (flIterator->hasDuration)
				{
					feature->hasDuration = true;
					feature->duration = gcnew ManagedRealtime(flIterator->duration);
				}
				
				if (flIterator->hasTimestamp)
				{
					feature->hasTimestamp = true;
					feature->timestamp = gcnew ManagedRealtime(flIterator->timestamp);
				}
				
				feature->label = gcnew String(flIterator->label.c_str());
				cliext::vector<float>^ newVals = gcnew cliext::vector<float>();
				std::vector<float>::iterator valueIter;
				for (valueIter = flIterator->values.begin(); valueIter != flIterator->values.end(); ++valueIter)
				{
					newVals->push_back(*valueIter);
				}
				feature->values = newVals;
				featureList->push_back(feature);
			}
			retVal[fsIterator->first] = featureList;
		}

		return retVal;

	}

	ManagedFeatureSet^ ManagedPlugin::Process(array<float>^ inputBuffer, ManagedRealtime^ timestamp)
	{
		pin_ptr<float> pinnedData = &inputBuffer[0];
		float* bufData = pinnedData;

		Vamp::Plugin::FeatureSet features = m_plugin->process(&bufData, timestamp);
		return convertToManagedFeatureSet(features);

	}

	ManagedFeatureSet^ ManagedPlugin::GetRemainingFeatures()
	{
		Vamp::Plugin::FeatureSet features = m_plugin->getRemainingFeatures();
		return convertToManagedFeatureSet(features);

	}
