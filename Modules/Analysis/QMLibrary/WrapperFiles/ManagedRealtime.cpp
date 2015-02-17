
#include <ManagedRealtime.h>

using namespace System;

ManagedRealtime::ManagedRealtime()
	{
		m_realTime = new Vamp::RealTime();
	}

ManagedRealtime::ManagedRealtime(Vamp::RealTime vampManagedRealtime)
	{
		m_realTime = new Vamp::RealTime(vampManagedRealtime);
		m_debugStr = gcnew String(m_realTime->toText().c_str());
	}

ManagedRealtime::ManagedRealtime(int s, int n)
	{
		m_realTime = new Vamp::RealTime(s, n);
	}

ManagedRealtime::ManagedRealtime(const ManagedRealtime %rhs)
	{
		m_realTime = new Vamp::RealTime(*rhs.m_realTime);
	}

	int ManagedRealtime::usec()
	{ 
		return m_realTime->usec();
	}

	int ManagedRealtime::msec() 
	{ 
		return m_realTime->msec();
	}

	double ManagedRealtime::totalMilliseconds()
	{
		return (m_realTime->sec * 1000) + (m_realTime->nsec / 1000000);
	}

	ManagedRealtime^ ManagedRealtime::fromSeconds(double sec)
	{
		return gcnew ManagedRealtime(Vamp::RealTime::fromSeconds(sec));
	}

	ManagedRealtime^ ManagedRealtime::fromMilliseconds(int msec)
	{
		return gcnew ManagedRealtime(Vamp::RealTime::fromMilliseconds(msec));
	}

	ManagedRealtime^ ManagedRealtime::operator=(ManagedRealtime ^r) {
		return gcnew ManagedRealtime(*m_realTime = *(r->m_realTime));
			}

	ManagedRealtime^ ManagedRealtime::operator+(ManagedRealtime ^r)  {
				return gcnew ManagedRealtime(*m_realTime + *(r->m_realTime));
			}
	ManagedRealtime^ ManagedRealtime::operator-(ManagedRealtime ^r)  {
				return gcnew ManagedRealtime(*m_realTime - *(r->m_realTime));
			}
	ManagedRealtime^ ManagedRealtime::operator-()  {
				return gcnew ManagedRealtime(m_realTime->operator-());
			}

	bool ManagedRealtime::operator <(ManagedRealtime ^r)  {
		return *m_realTime < *(r->m_realTime);
	}

	bool ManagedRealtime::operator >(ManagedRealtime ^r)  {
		return *m_realTime > *(r->m_realTime);
	}

	bool ManagedRealtime::operator==(ManagedRealtime ^r)  {
		return *m_realTime == *(r->m_realTime);
	}

	bool ManagedRealtime::operator!=(ManagedRealtime ^r)  {
		return *m_realTime != *(r->m_realTime);
	}

	bool ManagedRealtime::operator>=(ManagedRealtime ^r) {
		return *m_realTime >= *(r->m_realTime);
	}

	bool ManagedRealtime::operator<=(ManagedRealtime ^r)  {
		return *m_realTime <= *(r->m_realTime);
	}

	ManagedRealtime^ ManagedRealtime::operator/(int d) 
	{
		return gcnew ManagedRealtime(*m_realTime / d);
	};

	double ManagedRealtime::operator/(ManagedRealtime ^r) {
		return *m_realTime / *(r->m_realTime);
	}

	String^ ManagedRealtime::toString() {
		return gcnew String(m_realTime->toString().c_str());
	}

	ManagedRealtime::operator Vamp::RealTime() {
		return *m_realTime;
	}

	String^ ManagedRealtime::toText()
			{
				return this->toText(false);
			}

	String^ ManagedRealtime::toText(bool fixedDp) {
		return gcnew String(m_realTime->toText(fixedDp).c_str());
	}

	long ManagedRealtime::realTime2Frame(ManagedRealtime ^r, unsigned int sampleRate)
	{
		return Vamp::RealTime::realTime2Frame(*(r->m_realTime), sampleRate);
	}

	ManagedRealtime^ ManagedRealtime::frame2RealTime(long frame, unsigned int sampleRate)
	{
		return gcnew ManagedRealtime(Vamp::RealTime::frame2RealTime(frame, sampleRate));
	}

