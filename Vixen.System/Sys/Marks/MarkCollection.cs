using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkCollection: ICloneable
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

		[DataMember]
		public MarkDecorator Decorator { get; set; }

		public void EnsureOrder()
		{
			Marks.Sort();
		}

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return new MarkCollection()
			{
				IsEnabled = IsEnabled,
				Level = Level,
				Name = Name,
				Marks = Marks.Select(x => (Mark)x.Clone()).ToList(),
				Decorator = (MarkDecorator)Decorator.Clone()
			};
		}

		#endregion
	}
}
