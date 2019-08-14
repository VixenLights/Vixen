namespace VixenApplication.Setup.ElementTemplates
{
	public class Star : NumberedGroup
	{
		public Star() : base(@"Star", @"Star Px", 40)
		{

		}

		#region Overrides of NumberedGroup

		/// <inheritdoc />
		public override string TemplateName => "Star";

		#endregion
	}
}
