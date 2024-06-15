using Catel.Data;
using Catel.MVVM;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class CheckBoxStateBase : ViewModelBase
	{
		#region Text property
		/// <summary>
		/// Gets or sets the Text value.
		/// </summary>
		public string Text
		{
			get { return GetValue<string>(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		/// <summary>
		/// Text property data.
		/// </summary>
		public static readonly IPropertyData TextProperty = RegisterProperty<string>(nameof(Text));


		#endregion

		#region Value property
		
		/// <summary>
		/// Gets or sets the Value value.
		/// </summary>
		public bool Value
		{
			get { return GetValue<bool>(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Value property data.
		/// </summary>
		public static readonly IPropertyData ValueProperty = RegisterProperty<bool>(nameof(Value));
		
		#endregion
	}
}