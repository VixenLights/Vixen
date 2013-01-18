using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Sequence.Vixen2x
{
	[DataContract]
	public class MarkCollection
	{
		public MarkCollection()
		{
			Marks = new List<TimeSpan>();
			Id = Guid.NewGuid();
			MarkColor = Color.Black;
			Level = 1;
			Enabled = true;
		}

		public MarkCollection(MarkCollection original) {
			Marks = new List<TimeSpan>(original.Marks);
			Id = Guid.NewGuid();
			MarkColor = original.MarkColor;
			Level = original.Level;
			Enabled = original.Enabled;
			Name = original.Name;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public List<TimeSpan> Marks { get; set; }

		[DataMember]
		public Color MarkColor { get; set; }

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public Guid Id { get; internal set; }

		public int MarkCount
		{
			get { return Marks.Count; }
		}
	}
}
