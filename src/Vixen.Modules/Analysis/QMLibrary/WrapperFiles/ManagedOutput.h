#ifndef MANAGEDOUTPUT_H
#define MANAGEDOUTPUT_H

using namespace System;
using namespace System::Collections::Generic;

public ref class ManagedOutputDescriptor
{
public:
	String^ identifier;
	String^ name;
	String^ description;
	String^ unit;
	bool hasFixedBinCount;
	int binCount;
	ICollection<String^>^ binNames;
	bool hasKnownExtents;
	float minValue;
	float maxValue;
	bool isQuantized;
	float quantizeStep;

	enum class SampleType {
		OneSamplePerStep,
		FixedSampleRate,
		VariableSampleRate,
		Uknown
	};

	SampleType sampleType;
	float sampleRate;
	bool hasDuration;

	ManagedOutputDescriptor() : // defaults for mandatory non-class-type members
		hasFixedBinCount(false), hasKnownExtents(false), isQuantized(false),
		sampleType(SampleType::OneSamplePerStep), sampleRate(0), hasDuration(false) { }
};

typedef IList<ManagedOutputDescriptor^> ManagedOutputList;

#endif //MANAGEDOUTPUT_H