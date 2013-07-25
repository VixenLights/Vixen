using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Preview.VixenPreview
{
	internal class TemplateComboBoxItem
	{
		private string _caption = string.Empty;
		private string _fileName = string.Empty;

		public TemplateComboBoxItem(string caption, string fileName)
		{
			Caption = caption;
			FileName = fileName;
		}

		public override string ToString()
		{
			return Caption;
		}

		public string Caption
		{
			get { return _caption; }
			set { _caption = value; }
		}

		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}
	}
}