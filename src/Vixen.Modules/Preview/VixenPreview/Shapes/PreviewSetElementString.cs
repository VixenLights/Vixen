﻿namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class PreviewSetElementString
	{
		private string _stringName = string.Empty;
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