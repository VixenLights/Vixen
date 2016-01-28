using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{

	public class PropChannel
	{
		public PropChannel()
		{
			Id = Guid.NewGuid().ToString();
			Children = new List<PropChannel>();
		}

		public PropChannel(string m)
			: this()
		{
			ItemColor = XYZ.FromRGB(new RGB(Color.Black));

			Name = m;
	 
		}


		private string _text;

		XYZ _itemColor;


		public XYZ ItemColor
		{
			get { return _itemColor; }
			set
			{
				_itemColor = value;

			}
		}

		public string Name
		{
			get { return _text; }
			set
			{
				_text = value;
			}
		}

		public string Id
		{
			get;
			set;
		}


		public string ID_Text
		{
			get
			{
				return string.Format("{0} -> {1}", Id.ToString().PadRight(3), Name);
			}
		}
		public List<Point> Points = new List<Point>();

		public ElementNode Node { get; set; }
		public List<PropChannel> Children { get; set; }
	}
}
