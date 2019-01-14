namespace Vixen.Export
{
	public class FSEQCompressedWriter:FSEQWriter
	{
		public FSEQCompressedWriter()
		{
			Compress = true;
		}

		public override string FileTypeDescr => "Falcon Player Sequence Compressed";
	}
}
