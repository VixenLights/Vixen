using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Export
{
    public interface IExportWriter
    {
        int SeqPeriodTime { get; set; }
        void OpenSession(SequenceSessionData sessionData);
        void WriteNextPeriodData(List<Byte> periodData);
        void CloseSession();
        string FileType { get; }
        string FileTypeDescr { get; }
		bool CanCompress { get; }
		bool EnableCompression { get; set; }
		bool IsFalconFormat { get; }
		string Version { get; }
    }

}
