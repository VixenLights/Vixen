using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkCollection
	{
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
