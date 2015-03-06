using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.App.WebServer.Model
{
	public class ElementState
	{
		public Guid Id { get; set; }
		public int Duration { get; set; }
		public String Color { get; set; }
		public double Intensity { get; set; }
	}
}
