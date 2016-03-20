using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class RowHeightSetting
	{
		public RowHeightSetting()
		{
			RowHeight = 32;
			RowName = "";
		}

		[DataMember]
		public int RowHeight { get; set; }

		[DataMember]
		public string RowName { get; set; }

	}
}