#ifndef MANAGEDPARAMETERLIST_H
#define MANAGEDPARAMETERLIST_H

#include <cliext\vector>
using namespace System::Collections::Generic;

public ref class ManagedParameterDescriptor
{
public:
	System::String^ identifier;
	System::String^ name;
	System::String^ description;
	System::String^ unit;
	float minValue;
	float maxValue;
	float defaultValue;
	bool isQuantized;
	float quantizeStep;
	ICollection<System::String^>^ valueNames;

	ManagedParameterDescriptor() : // the defaults are invalid: you must set them
		minValue(0), maxValue(0), defaultValue(0), isQuantized(false) { }
};

typedef cliext::vector<ManagedParameterDescriptor^> ManagedParameterListPriv;
typedef ICollection<ManagedParameterDescriptor^> ManagedParameterList;

#endif //MANAGEDPARAMETERLIST_H