using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract, Serializable]
	public class Element
	{
		private string _text;
		public Element()
		{
			Id = Guid.NewGuid().ToString();
			Children = new List<Element>();
			PixelSize = 5;

		}

		public Element(string name)
			: this()
		{

			Name = name;

		}
		
		[DataMember]
		public string Name
		{
			get { return _text; }
			set
			{
				_text = value;
			}
		}

		[Browsable(false)]
		[DataMember]
		public int PixelSize { get; set; }

		[DataMember]
		[Browsable(false)]
		public string Id
		{
			get;
			set;
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
		public List<Pixel> GetNestedPixels()
		{
			List<Pixel> retVal = new List<Pixel>();
			retVal.AddRange(Pixels);
			retVal.AddRange(Children.SelectMany(s => s.GetNestedPixels()));
			return retVal.Distinct().ToList();
		}


		[Browsable(false)]
		[DataMember]
		public bool IsPixel { get; set; }

		[Browsable(false)]
		[DataMember]
		public ElementNode Node { get; set; }

		[Browsable(false)]
		[DataMember]
		public List<Element> Children { get; set; }

		 
	}
}
