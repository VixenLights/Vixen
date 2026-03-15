using System.Windows.Markup;

namespace Common.WPFCommon.MarkupExtensions
{
	public class EnumBindingSourceExtension : MarkupExtension
	{
		public Type EnumType { get; set; }
		public EnumBindingSourceExtension() { }

		public EnumBindingSourceExtension(Type enumType)
		{
			this.EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (EnumType == null) throw new InvalidOperationException("EnumType must be specified.");

			// Get actual enum values
			return Enum.GetValues(EnumType);
		}
	}
}