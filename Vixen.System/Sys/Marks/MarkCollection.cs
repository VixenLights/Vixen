using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkCollection
	{
		public MarkCollection()
		{
			Decorator = new MarkDecorator();
			Marks = new List<Mark>();
			Id = Guid.NewGuid();
			Level = 1;
			IsEnabled = true;
		}
		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsEnabled { get; set; }
		
		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public List<Mark> Marks { get; set; }

		public MarkDecorator Decorator { get; set; }

		public void EnsureOrder()
		{
			Marks.Sort();
		}
	}
}
