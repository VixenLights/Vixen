using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview.CustomProp
{
	public class PropChannel
	{
		public PropChannel() { }
		public PropChannel(string m, int maxChannelID)
		{
			ItemColor = Color.Black;
			ID = maxChannelID + 1;
			Text = m;
		}
		public Color ItemColor { get; set; }
		public string Text { get; set; }

		public int ID { get; set; }

		public string ID_Text
		{
			get
			{
				return string.Format("{0} -> {1}", ID.ToString().PadRight(3), Text);
			}
		}
	}
}
