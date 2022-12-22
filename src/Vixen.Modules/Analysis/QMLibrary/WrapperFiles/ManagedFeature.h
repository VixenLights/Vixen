#ifndef MANAGEDFEATURE_H
#define MANAGEDFEATURE_H

#include <ManagedRealtime.h>
#include <cliext\vector>
#include <cliext\map>

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

typedef cliext::vector<ManagedFeature^> ManagedFeatureListPriv;
typedef ICollection<ManagedFeature^> ManagedFeatureList;

typedef cliext::map<int, ManagedFeatureList^> ManagedFeatureSetPriv; // key is output no
typedef IDictionary<int, ManagedFeatureList^> ManagedFeatureSet; // key is output no

#endif //MANAGEDFEATURE_H