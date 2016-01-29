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
	
		public PropChannel()
		{
			Id = Guid.NewGuid().ToString();
			Children = new List<PropChannel>();
			PixelSize = 5;
		}

		public PropChannel(string name)
			: this()
		{

			Name = name;

		}


		private string _text;


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
		public List<PreviewPixel> Pixels = new List<PreviewPixel>();

		[Browsable(false)]
		public ElementNode Node { get; set; }
		[Browsable(false)]
		public List<PropChannel> Children { get; set; }
	}
}
