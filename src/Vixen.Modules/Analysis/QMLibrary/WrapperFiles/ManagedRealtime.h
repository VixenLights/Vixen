#ifndef MANAGEDREALTIME_H
#define MANAGEDREALTIME_H

#include <vamp-sdk\RealTime.h>

public ref class ManagedRealtime
{
private:
	Vamp::RealTime* m_realTime;
	System::String^ m_debugStr;

public:

	ManagedRealtime();
	ManagedRealtime(int s, int n);
	ManagedRealtime(const ManagedRealtime %rhs);
	ManagedRealtime(Vamp::RealTime vampManagedRealtime);

	int usec();
	int msec();

	double totalMilliseconds();

	static ManagedRealtime^ fromSeconds(double sec);
	static ManagedRealtime^ fromMilliseconds(int msec);
	//static ManagedRealtime^ fromTimeval(const struct timeval &);

	ManagedRealtime^ operator=(ManagedRealtime ^r);

	ManagedRealtime^ operator+(ManagedRealtime ^r);
	ManagedRealtime^ operator-(ManagedRealtime ^r);
	ManagedRealtime^ operator-();

	bool operator <(ManagedRealtime ^r);

	bool operator >(ManagedRealtime ^r);

	bool operator==(ManagedRealtime ^r);

	bool operator!=(ManagedRealtime ^r);

	bool operator>=(ManagedRealtime ^r);

	bool operator<=(ManagedRealtime ^r);

	ManagedRealtime^ operator/(int d);

	double operator/(ManagedRealtime ^r);

	System::String^ toString();

	System::String^ toText();

	System::String^ toText(bool fixedDp);

	static long realTime2Frame(ManagedRealtime ^r, unsigned int sampleRate);

	static ManagedRealtime^ frame2RealTime(long frame, unsigned int sampleRate);

	static const ManagedRealtime zeroTime;

	operator Vamp::RealTime();

	
};

#endif //ManagedRealtime_H