#ifndef MANAGEDFEATURE_H
#define MANAGEDFEATURE_H

#include <ManagedRealtime.h>

using namespace System::Collections::Generic;

public ref class ManagedFeature
{
public:
	bool hasTimestamp;
	ManagedRealtime^ timestamp;

	bool hasDuration;
	ManagedRealtime^ duration;
	ICollection<float>^ values;

	System::String^ label;

	ManagedFeature() : // defaults for mandatory non-class-type members
		hasTimestamp(false), hasDuration(false) { }
};

typedef ICollection<ManagedFeature^> ManagedFeatureList;

typedef IDictionary<int, ManagedFeatureList^> ManagedFeatureSet; // key is output no

#endif //MANAGEDFEATURE_H