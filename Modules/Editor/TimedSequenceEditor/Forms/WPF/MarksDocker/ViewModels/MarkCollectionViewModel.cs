using System.Windows.Forms;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkCollectionViewModel: ViewModelBase
	{
		public MarkCollectionViewModel(MarkCollection markCollection)
		{
			MarkCollection = markCollection;
		}

		#region MarkCollection model property

		/// <summary>
		/// Gets or sets the MarkCollection value.
		/// </summary>
		[Model]
		public MarkCollection MarkCollection
		{
			get { return GetValue<MarkCollection>(MarkCollectionProperty); }
			private set { SetValue(MarkCollectionProperty, value); }
		}

		/// <summary>
		/// MarkCollection property data.
		/// </summary>
		public static readonly PropertyData MarkCollectionProperty = RegisterProperty("MarkCollection", typeof(MarkCollection));

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

		#region IsEnabled property

		/// <summary>
		/// Gets or sets the IsEnabled value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool IsEnabled
		{
			get { return GetValue<bool>(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		/// <summary>
		/// IsEnabled property data.
		/// </summary>
		public static readonly PropertyData IsEnabledProperty = RegisterProperty("IsEnabled", typeof(bool), null);

		#endregion

		#region IsDefault property

		/// <summary>
		/// Gets or sets the IsDefault value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool IsDefault
		{
			get { return GetValue<bool>(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}

		/// <summary>
		/// IsDefault property data.
		/// </summary>
		public static readonly PropertyData IsDefaultProperty = RegisterProperty("IsDefault", typeof(bool), null);

		#endregion

		#region Decorator property

		/// <summary>
		/// Gets or sets the Decorator value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public MarkDecorator Decorator
		{
			get { return GetValue<MarkDecorator>(DecoratorProperty); }
			set { SetValue(DecoratorProperty, value); }
		}

		/// <summary>
		/// Decorator property data.
		/// </summary>
		public static readonly PropertyData DecoratorProperty = RegisterProperty("Decorator", typeof(MarkDecorator), null);

		#endregion

		#region PickColor command

		private Command _pickColorCommand;

		/// <summary>
		/// Gets the PickColor command.
		/// </summary>
		public Command PickColorCommand
		{
			get { return _pickColorCommand ?? (_pickColorCommand = new Command(PickColor)); }
		}

		/// <summary>
		/// Method to invoke when the PickColor command is executed.
		/// </summary>
		private void PickColor()
		{
			var picker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();
			var result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				Decorator.Color = picker.Color.ToRGB().ToArgb();
			}
		}

		#endregion

	}
}
