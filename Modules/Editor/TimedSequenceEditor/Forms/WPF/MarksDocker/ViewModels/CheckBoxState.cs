using Catel.Data;
using Catel.MVVM;
using Vixen.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class CheckBoxState:ViewModelBase
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
		public static readonly PropertyData TextProperty = RegisterProperty("Text", typeof(string));

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
		public static readonly PropertyData ValueProperty = RegisterProperty("Value", typeof(bool));

		#endregion

		public MarkCollectionType Type { get; set; }
	}
}
