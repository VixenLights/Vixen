// QMSandbox2.cpp : Defines the entry point for the console application.
//

#include <fstream>
#include <vamp-sdk/Realtime.h>
#include <BarBeatTrack.h>

using namespace std;

bool runQMTest()
{
	
	int blockSize;
	int stepSize;

	BarBeatTracker* barBeats = new BarBeatTracker(44100);
	std::cout << barBeats->getName() << std::endl;
	std::cout << barBeats->getIdentifier() << std::endl;
	std::cout << barBeats->getDescription() << std::endl;
	std::cout << barBeats->getCopyright() << std::endl;
	std::cout << barBeats->getMaker() << std::endl;
	std::cout << std::endl;
	
	blockSize = barBeats->getPreferredBlockSize();
	stepSize = barBeats->getPreferredStepSize();

	barBeats->initialise(1, stepSize, blockSize);
	
	std::cout << "--------Output Descriptors--------------------" << std::endl;
	Vamp::Plugin::OutputList outList = barBeats->getOutputDescriptors();

	for (std::vector<Vamp::Plugin::OutputDescriptor>::iterator it = outList.begin(); it != outList.end(); ++it)
	{
		std::cout << (*it).description << std::endl;
	}

	Vamp::RealTime timeStamp;
	Vamp::Plugin::FeatureSet featureSet;
	
	streampos size;
	char * memblock = new char [blockSize * 2];
	int channels = barBeats->getMaxChannelCount();

	float *filebuf = new float[blockSize * channels];
	float **plugbuf = new float*[channels];
	for (int c = 0; c < channels; ++c)
	{
		plugbuf[c] = new float[blockSize + 2];
	}

	ifstream file("Jingle-Bells-Techno-edit.pcm", ios::in | ios::binary | ios::ate);
	if (file.is_open())
	{
		size = file.tellg();
		for (int j = 0; j < size; j += blockSize)
		{
			file.seekg(j, ios::beg);
			file.read(memblock, blockSize * 2);
			for (int k = 0; k < blockSize; k++)
			{
				(*plugbuf)[k] = (float)((memblock[(k * 2) + 1] << 8) + (memblock[k * 2])) ;
			}
			
			timeStamp = Vamp::RealTime::frame2RealTime(j, 44100);
			barBeats->process(plugbuf, timeStamp);

		}
	}
	else
	{
		cout << "Unable to open file";
	}
	file.close();
	featureSet = barBeats->getRemainingFeatures();

	std::cout << "--------Beat Marks--------------------" << std::endl;
	for (Vamp::Plugin::FeatureList::iterator fiter = featureSet[1].begin(); fiter != featureSet[1].end(); ++fiter)
	{
		Vamp::Plugin::Feature feature = *fiter;
		if (feature.hasTimestamp)
		{
			cout << feature.timestamp.toText() << " Seconds" << endl;
		}
	}
	
	return 0;
}
