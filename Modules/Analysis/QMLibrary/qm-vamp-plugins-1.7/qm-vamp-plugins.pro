
TEMPLATE = lib
CONFIG += plugin warn_on release
CONFIG -= qt

linux-g++* {
    QMAKE_CXXFLAGS_RELEASE += -DNDEBUG -O3 -fno-exceptions -fPIC -ffast-math -msse -mfpmath=sse -ftree-vectorize -fomit-frame-pointer
    DEFINES += USE_PTHREADS
    INCLUDEPATH += ../vamp-plugin-sdk ../qm-dsp
    LIBPATH += ../vamp-plugin-sdk ../qm-dsp
}

linux-g++ {
    LIBS += -static-libgcc -Wl,-Bstatic -lqm-dsp -lvamp-sdk -L/usr/lib/sse2/atlas -L/usr/lib/atlas/sse -llapack -lblas $$system(g++ -print-file-name=libstdc++.a) -lc -Wl,-Bdynamic -lpthread -Wl,--version-script=vamp-plugin.map
}

linux-g++-64 {
    QMAKE_CXXFLAGS_RELEASE += -msse2
    LIBS += -Lbuild/linux/amd64  -Wl,-Bstatic -lqm-dsp -lvamp-sdk -llapack -lcblas -latlas -lc -Wl,-Bdynamic -lpthread -Wl,--version-script=vamp-plugin.map
}

macx-g++* {
    QMAKE_MAC_SDK=/Developer/SDKs/MacOSX10.4u.sdk
    QMAKE_CXXFLAGS_RELEASE += -mmacosx-version-min=10.4 -O2 -g0
    QMAKE_CFLAGS_RELEASE += -mmacosx-version-min=10.4 
    CONFIG += x86 ppc x86_64
    QMAKE_CXX = g++-4.0
    QMAKE_CC = gcc-4.0
    QMAKE_LINK = g++-4.0
    DEFINES += USE_PTHREADS
    LIBS += -mmacosx-version-min=10.4 -lqm-dsp -L../inst/lib -lvamp-sdk -framework Accelerate -lpthread -exported_symbols_list vamp-plugin.list
    INCLUDEPATH += ../vamp-plugin-sdk ../qm-dsp
    LIBPATH += ../include ../lib ../qm-dsp
}

win32-x-g++ {
    QMAKE_CXXFLAGS_RELEASE += -DNDEBUG -O2 -march=pentium3 -msse
    INCLUDEPATH += ../include ../qm-dsp
    LIBPATH += ./build/mingw32 ../lib ../qm-dsp ../qm-dsp/release 
    LIBS += -shared -Wl,-Bstatic -lqm-dsp -lvamp-sdk -llapack -lcblas -latlas -lf77blas -lg2cstubs -Wl,-Bdynamic -Wl,--version-script=vamp-plugin.map
}

solaris* {
    QMAKE_CXXFLAGS_RELEASE += -DNDEBUG -fast
    INCLUDEPATH += /usr/local/include ../qm-dsp
    INCLUDEPATH += /opt/ATLAS3.9.14/include
    LIBPATH += ../qm-dsp /opt/ATLAS3.9.14/lib
    DEFINES += USE_PTHREADS
    LIBS += -Bstatic -lqm-dsp -lvamp-sdk -llapack -lcblas -latlas -Bdynamic -lpthread -Wl,--version-script=vamp-plugin.map -lCstd -lCrun
}

OBJECTS_DIR = tmp_obj
MOC_DIR = tmp_moc

DEPENDPATH += plugins
INCLUDEPATH += . plugins

# Input
HEADERS += plugins/AdaptiveSpectrogram.h \
           plugins/BarBeatTrack.h \
           plugins/BeatTrack.h \
           plugins/DWT.h \
           plugins/OnsetDetect.h \
           plugins/ChromagramPlugin.h \
           plugins/ConstantQSpectrogram.h \
           plugins/KeyDetect.h \
           plugins/MFCCPlugin.h \
           plugins/SegmenterPlugin.h \
           plugins/SimilarityPlugin.h \
           plugins/TonalChangeDetect.h \
           plugins/Transcription.h
SOURCES += g2cstubs.c \
           plugins/AdaptiveSpectrogram.cpp \
           plugins/BarBeatTrack.cpp \
           plugins/BeatTrack.cpp \
           plugins/DWT.cpp \
           plugins/OnsetDetect.cpp \
           plugins/ChromagramPlugin.cpp \
           plugins/ConstantQSpectrogram.cpp \
           plugins/KeyDetect.cpp \
           plugins/MFCCPlugin.cpp \
           plugins/SegmenterPlugin.cpp \
           plugins/SimilarityPlugin.cpp \
           plugins/TonalChangeDetect.cpp \
           plugins/Transcription.cpp \
           ./libmain.cpp

