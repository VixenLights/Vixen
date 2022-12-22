namespace Common.WPFCommon.Converters
{
	public class NullToBooleanConverter:NullConverter<bool>
	{
		public NullToBooleanConverter():base(true, false)
		{
			
		}
	}
}
