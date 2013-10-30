using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VixenModules.Property.Color;

namespace VixenModules.App.WebServer.Model
{
	public class Element
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public List<string> Colors { get; set; }

		public List<Element> Children { get; set; }
	}
}
