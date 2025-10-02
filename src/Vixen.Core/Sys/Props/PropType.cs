namespace Vixen.Sys.Props
{
	// *** IMPORTANT ***  Keep these two lists in sync.
	public enum PropType
	{
		Arch,
		Tree,
		IntelligentFixture,
		//Custom,
		//Single,
		//Line,
		//PolyLine,
		//Circle,
		//CandyCane,
		//Star,
		//Grid
	}

	public static class PropTypeNames
	{
		// Note: The order listed here will be the order the menu items appear in the UI for adding new props
		private static readonly string[] Names =
		{
			"Arch",
			"Tree",
			"IntelligentFixture"
			//"Custom",
			//"Single",
			//"Line",
			//"PolyLine",
			//"Circle",
			//"CandyCane",
			//"Star",
			//"Grid",
		};

		public static string GetName(PropType type)
		{
			return Names[(int)type];
		}
	}
}