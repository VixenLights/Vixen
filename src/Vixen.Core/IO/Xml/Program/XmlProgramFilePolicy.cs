using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;
using Vixen.Sys;

namespace Vixen.IO.Xml.Program
{
	internal class XmlProgramFilePolicy : ProgramFilePolicy
	{
		private Sys.Program _program;
		private XElement _content;

		public XmlProgramFilePolicy(Sys.Program program, XElement content)
		{
			_program = program;
			_content = content;
		}

		protected override void WriteSequences()
		{
			XmlSequenceListSerializer sequenceListSerializer = new XmlSequenceListSerializer();
			_content.Add(sequenceListSerializer.WriteObject(_program.Sequences));
		}

		protected override void ReadSequences()
		{
			XmlSequenceListSerializer sequenceListSerializer = new XmlSequenceListSerializer();
			IEnumerable<ISequence> sequences = sequenceListSerializer.ReadObject(_content);

			_program.Sequences.Clear();
			_program.Sequences.AddRange(sequences);
		}
	}
}