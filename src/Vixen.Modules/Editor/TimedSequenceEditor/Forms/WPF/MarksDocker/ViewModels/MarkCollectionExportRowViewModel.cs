using Catel.Data;
using Catel.MVVM;
using Vixen.Marks;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkCollectionExportRowViewModel: ViewModelBase
	{
		public MarkCollectionExportRowViewModel(IMarkCollection markCollection)
		{
			MarkCollection = markCollection;
			IsIncluded = true;
			//Default to including text on the types that have text
			IsTextIncluded = markCollection.CollectionType is MarkCollectionType.Phrase or MarkCollectionType.Word or MarkCollectionType.Phoneme;
		}

		#region MarkCollection property

		/// <summary>
		/// Gets or sets the MarkCollection value.
		/// </summary>
		public IMarkCollection MarkCollection
		{
			get { return GetValue<IMarkCollection>(MarkCollectionProperty); }
			set { SetValue(MarkCollectionProperty, value); }
		}

		/// <summary>
		/// MarkCollection property data.
		/// </summary>
		public static readonly IPropertyData MarkCollectionProperty = RegisterProperty<IMarkCollection>(nameof(MarkCollection));

		#endregion

		#region IsIncluded property

		/// <summary>
		/// Gets or sets the IsIncluded value.
		/// </summary>
		public bool IsIncluded
		{
			get { return GetValue<bool>(IsIncludedProperty); }
			set { SetValue(IsIncludedProperty, value); }
		}

		/// <summary>
		/// IsIncluded property data.
		/// </summary>
		public static readonly IPropertyData IsIncludedProperty = RegisterProperty<bool>(nameof(IsIncluded));

		#endregion

		#region IsTextIncluded property

		/// <summary>
		/// Gets or sets the IsTextIncluded value.
		/// </summary>
		public bool IsTextIncluded
		{
			get { return GetValue<bool>(IsTextIncludedProperty); }
			set { SetValue(IsTextIncludedProperty, value); }
		}

		/// <summary>
		/// IsTextIncluded property data.
		/// </summary>
		public static readonly IPropertyData IsTextIncludedProperty = RegisterProperty<bool>(nameof(IsTextIncluded));

		#endregion

	}
}