using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Common.Controls
{
	public sealed class PictureComboBoxItem
	{
		private Image _image = null;
		private int _width = 0;
		private int _height = 0;

		public PictureComboBoxItem(string text, System.IO.FileInfo file, int width, int height)
		{
			Text = text;
			File = file;
			_width = width;
			_height = height;
			IsResource = false;
			if (System.IO.File.Exists(file.FullName)) {
				try {
					Image = Image.FromFile(file.FullName);
				}
				catch {
					// invalid picture!
				}
			}
		}

		public PictureComboBoxItem(string text, string resourceName, int width, int height, Type source)
		{
			Text = text;
			_width = width;
			_height = height;
			ResourceName = resourceName;
			SourceType = source;
			IsResource = true;
			Image = Image.FromStream(source.Assembly.GetManifestResourceStream(resourceName));
		}

		public string ResourceName { get; set; }
		
		public bool IsResource { get; set; }
		
		public Type SourceType { get; set; }

		public Image Image
		{
			get { return _image; }
			set { _image = new Bitmap(value, new Size(_width, _height)); }
		}

		public override string ToString()
		{
			return Text;
		}

		public FileInfo File { get; set; }
		public string Text { get; set; }
	}
}