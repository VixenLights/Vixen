#include <QMFiles/BarBeatTrack.h>
#include <QMBarBeatTrack.h>

namespace QMLibrary
{
	QMBarBeatTrack::QMBarBeatTrack(float inputSampleRate)
	{
		m_plugin = new BarBeatTracker(inputSampleRate);
	}
}

