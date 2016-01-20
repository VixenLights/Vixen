using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class PropChannel
	{
		public PropChannel() { }
		public PropChannel(string m, int maxChannelID)
		{
			ItemColor = XYZ.FromRGB(new RGB(Color.Black));
			ID = maxChannelID + 1;
			Text = m;
		}


		private string _text;
		private int _id;
		XYZ _itemColor;
		
		
		public XYZ ItemColor
		{
			get { return _itemColor; }
			set
			{
				_itemColor = value;

			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
			}
		}

		public int ID
		{
			get { return _id; }
			set
			{
				_id = value;
			}
		}


		public string ID_Text
		{
			get
			{
				return string.Format("{0} -> {1}", ID.ToString().PadRight(3), Text);
			}
		}
	}
}
