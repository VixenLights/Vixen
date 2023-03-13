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
