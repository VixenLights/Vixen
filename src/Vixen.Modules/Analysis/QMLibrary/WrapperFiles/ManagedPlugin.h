#ifndef MANAGEDPLUGIN_H
#define MANAGEDPLUGIN_H

#include <vamp-sdk\Plugin.h>
#include <ManagedParameter.h>
#include <ManagedOutput.h>
#include <ManagedFeature.h>

namespace QMLibrary
{
	public ref class ManagedPlugin
	{


	protected:
		ManagedPlugin();

		Vamp::Plugin *m_plugin = nullptr;

	public:

		enum class InputDomain
		{
			TimeDomain,
			FrequencyDomain,
			Unknown
		};


		virtual bool Initialise(size_t channels, size_t stepSize, size_t blockSize);
		virtual void Reset();
		virtual System::String^ GetIdentifier();
		virtual System::String^ GetName();
		virtual System::String^ GetDescription();
		virtual System::String^ GetMaker();
		virtual int GetPluginVersion();
		virtual System::String^ GetCopyright();
		virtual float GetParameter(System::String^ paramStr);
		virtual void SetParameter(System::String^paramStr, float value);
		virtual int GetMinChannelCount();
		virtual int GetMaxChannelCount();
		virtual int GetPreferredStepSize();
		virtual int GetPreferredBlockSize();
		virtual ManagedParameterList^ GetParameterDescriptors();
		virtual ManagedOutputList^ GetOutputDescriptors();
		virtual InputDomain GetInputDomain();
		virtual ManagedFeatureSet^ convertToManagedFeatureSet(Vamp::Plugin::FeatureSet unManagedFS);
		virtual ManagedFeatureSet^ Process(array<float>^ inputBuffer, ManagedRealtime^ timestamp);
		virtual ManagedFeatureSet^ GetRemainingFeatures();
	};


}
#endif  //MANAGEDPLUGIN_H