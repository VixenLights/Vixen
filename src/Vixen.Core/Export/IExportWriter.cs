using Vixen.Module.Controller;

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
        
        IList<Guid> ControllerIDs { get; } 
        IList<int>  ControllerChannels { get; }
        DateTime SequenceTimeStamp { get; set; }
    }

}
