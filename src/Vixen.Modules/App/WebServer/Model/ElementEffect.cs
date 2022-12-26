namespace VixenModules.App.WebServer.Model
{
	public class ElementEffect
	{
		public Guid Id { get; set; }
		public int Duration { get; set; }
		public string EffectName { get; set; }
		public int Intensity { get; set; }
		public Dictionary<string, double> Color { get; set; }
		public Dictionary<double, double> LevelCurve { get; set; }
		public Dictionary<string, string> Options { get; set; }
		
	}
}
