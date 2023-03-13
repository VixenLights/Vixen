namespace VixenApplication.Setup.ElementTemplates
{
	public class CandyCane : NumberedGroup
	{
		public CandyCane() : base(@"CandyCane", @"CandyCane Px", 25)
		{

		}

		#region Overrides of NumberedGroup

		/// <inheritdoc />
		public override string TemplateName => "Candy Cane";

		#endregion
	}
}
