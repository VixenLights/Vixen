using Common.Controls.ColorManagement.ColorModels;
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
			ItemColor = XYZ.FromRGB(new RGB(Color.Black));
			ID = maxChannelID + 1;
			Text = m;
		}

		protected virtual void OnChange(EventArgs e)
		{
			if (changed)
				if (Changed != null)
				{
					Changed(this, e);
					changed = false;
				}
		}

		public event EventHandler Changed;
		bool changed = false;
		private string _text;
		private int _id;
		XYZ _itemColor;
		public XYZ ItemColor
		{
			get { return _itemColor; }
			set
			{
				changed = _itemColor != value;
				_itemColor;
				OnChange(new EventArgs());
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				changed = _text != value;
				_text = value;
				OnChange(new EventArgs());
			}
		}

		public int ID
		{
			get { return _id; }
			set
			{
				changed = _id != value;
				_id = value;
				OnChange(new EventArgs());
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
