using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract, Serializable]
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

		[DataMember]
		public string Name
		{
			get { return _text; }
			set
			{
				_text = value;
			}
		}

		[DataMember]
		public int PixelSize { get; set; }

		[DataMember]
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

		List<Pixel> _pixels;
		[DataMember]
		[Browsable(false)]
		public List<Pixel> Pixels
		{
			get
			{
				if (_pixels == null) _pixels = new List<Pixel>();
				return _pixels;
			}
			set
			{
				_pixels = value;
			}
		}

		[DataMember]
		public bool IsPixel { get; set; } 

		[Browsable(false)]
		[DataMember]
		public ElementNode Node { get; set; }

		[Browsable(false)]
		[DataMember]
		public List<PropChannel> Children { get; set; }
	}
}
