#ifndef MANAGEDPLUGIN_H
#define MANAGEDPLUGIN_H

#include <vamp-sdk\Plugin.h>
#include <ManagedParameter.h>
#include <ManagedOutput.h>
#include <ManagedFeature.h>


public ref class ManagedPlugin 
{

protected:
	Vamp::Plugin *m_plugin = nullptr;

public:

	enum class InputDomain
	{
		TimeDomain,
		FrequencyDomain,
		Unknown
	};

	ManagedPlugin(System::IntPtr plugin);
	bool Initialise(size_t channels, size_t stepSize, size_t blockSize);
	void Reset();
	System::String^ GetIdentifier();
	System::String^ GetName();
	System::String^ GetDescription();
	System::String^ GetMaker();
	int GetPluginVersion();
	System::String^ GetCopyright();
	float GetParameter(System::String^ paramStr);
	void SetParameter(System::String^paramStr, float value);
	int GetMinChannelCount();
	int GetMaxChannelCount();
	int GetPreferredStepSize();
	int GetPreferredBlockSize();
	ManagedParameterList^ GetParameterDescriptors();
	ManagedOutputList^ GetOutputDescriptors();
	InputDomain GetInputDomain();
	ManagedFeatureSet^ convertToManagedFeatureSet(Vamp::Plugin::FeatureSet unManagedFS);
	ManagedFeatureSet^ Process(array<float>^ inputBuffer, ManagedRealtime^ timestamp);
	ManagedFeatureSet^ GetRemainingFeatures();
};

#endif  //MANAGEDPLUGIN_H