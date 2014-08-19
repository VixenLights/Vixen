using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Export
{
    public interface IExportWriter
    {
        UInt16 SeqPeriodTime { get; set; }
        void WriteFileHeader();
        void WriteFileFooter();
        void OpenSession(string fileName, Int32 numPeriods, Int32 numChannels);
        void WriteNextPeriodData(List<Byte> periodData);
        void CloseSession();
        string FileType { get; }
        string FileTypeDescr { get; }
    }

}
