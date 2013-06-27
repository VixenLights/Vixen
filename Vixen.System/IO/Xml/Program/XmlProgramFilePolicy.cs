using System.Collections.Generic;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;

namespace Vixen.IO.Xml.Program
{
	using Vixen.Sys;

	internal class XmlProgramFilePolicy : ProgramFilePolicy
	{
		private Program _program;
		private XElement _content;

		public XmlProgramFilePolicy(Program program, XElement content)
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