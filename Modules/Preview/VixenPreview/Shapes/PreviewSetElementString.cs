using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class PreviewSetElementString
	{
		private string _stringName = "";
		private List<PreviewPixel> _pixels = new List<PreviewPixel>();

		public string StringName
		{
			get { return _stringName; }
			set { _stringName = value; }
		}

		public List<PreviewPixel> Pixels
		{
			get { return _pixels; }
		}
	}
}