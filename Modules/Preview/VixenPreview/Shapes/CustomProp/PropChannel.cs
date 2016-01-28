using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{

	public class PropChannel
	{
		public enum StringTypes
		{
			Standard,
			Pixel,
			//            Flood
		}

		public PropChannel()
		{
			Id = Guid.NewGuid().ToString();
			Children = new List<PropChannel>();
			PixelSize = 5;
		}

		public PropChannel(string m)
			: this()
		{

			Name = m;

		}


		private string _text;


		public StringTypes StringType { get; set; }
		public string Name
		{
			get { return _text; }
			set
			{
				_text = value;
			}
		}

		public int PixelSize { get; set; }

		[Browsable(false)]
		public string Id
		{
			get;
			set;
		}

		[Browsable(false)]
		public string ID_Text
		{
			get
			{
				return string.Format("{0} -> {1}", Id.ToString().PadRight(3), Name);
			}
		}
		public List<PreviewPixel> Points = new List<PreviewPixel>();

		[Browsable(false)]
		public ElementNode Node { get; set; }
		[Browsable(false)]
		public List<PropChannel> Children { get; set; }
	}
}
