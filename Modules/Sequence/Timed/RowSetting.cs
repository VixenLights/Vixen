using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class RowSetting
	{
		public RowSetting()
		{
			RowHeight = 32;
			RowIndex = "0";
		}

		[DataMember]
		public int RowHeight { get; set; }

		[DataMember]
		public string RowIndex { get; set; }

	}
}