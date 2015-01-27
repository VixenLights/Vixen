using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.App.WebServer.Model
{
	public class ElementEffect
	{
		public Guid Id { get; set; }
		public int Milliseconds { get; set; }
		public string EffectName { get; set; }
		public Dictionary<string, double> Color { get; set; }
		public Dictionary<double, double> LevelCurve { get; set; }
		public Dictionary<string, string> Options { get; set; }
		
	}
}
