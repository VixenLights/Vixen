namespace VixenApplication.Setup.ElementTemplates
{
	public class Arch : NumberedGroup
	{
		public Arch():base(@"Arch", @"Arch Px", 25)
		{
			
		}

		#region Overrides of NumberedGroup

		/// <inheritdoc />
		public override string TemplateName => "Arch";

		#endregion
	}
}
