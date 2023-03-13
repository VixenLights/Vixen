#ifndef MANAGEDPARAMETERLIST_H
#define MANAGEDPARAMETERLIST_H

using namespace System::Collections::Generic;
namespace QMLibrary
{
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

	typedef ICollection<ManagedParameterDescriptor^> ManagedParameterList;

}

#endif //MANAGEDPARAMETERLIST_H