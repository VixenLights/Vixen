using System.Collections.Generic;

namespace Vixen.Export
{
	public abstract class ExportWriterBase:IExportWriter
	{
		/// <inheritdoc />
		public int SeqPeriodTime { get; set; }

		/// <inheritdoc />
		public abstract void OpenSession(SequenceSessionData sessionData);

		/// <inheritdoc />
		public abstract void WriteNextPeriodData(List<byte> periodData);

		/// <inheritdoc />
		public abstract void CloseSession();

		/// <inheritdoc />
		public string FileType { get; protected set; }

		/// <inheritdoc />
		public string FileTypeDescr { get; protected set; }

		/// <inheritdoc />
		public bool CanCompress { get; protected set; } 

		/// <inheritdoc />
		public bool EnableCompression { get; set; }

		/// <inheritdoc />
		public bool IsFalconFormat { get; protected set; }

		/// <inheritdoc />
		public string Version { get; protected set; } = "1.0";
	}
}