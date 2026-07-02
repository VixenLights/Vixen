using System.Collections.ObjectModel;
using System.Linq;
using Vixen.Services;

namespace Common.ElementTagColorEditor.ViewModels
{
	/// <summary>
	/// Backs <see cref="Views.ElementTagColorEditorWindow"/>, listing the built-in element tags for color editing.
	/// </summary>
	/// <remarks>
	/// Exposes an <see cref="ObservableCollection{T}"/> rather than fixed fields for the three built-in tags so a
	/// future Tag Manager feature can extend this same list to include user-defined tags without changing this
	/// view model's shape.
	/// </remarks>
	public sealed class ElementTagColorEditorViewModel
	{
		public ElementTagColorEditorViewModel()
		{
			Tags = new ObservableCollection<TagColorItem>(
				ElementTagService.Instance.GetAll()
					.Where(tag => tag.IsBuiltIn)
					.OrderBy(tag => tag.SortOrder)
					.Select(tag => new TagColorItem(tag)));
		}

		/// <summary>
		/// Gets the tags being edited.
		/// </summary>
		public ObservableCollection<TagColorItem> Tags { get; }

		/// <summary>
		/// Writes every item's staged color back to its underlying <see cref="Vixen.Sys.ElementTagDefinition"/>.
		/// </summary>
		public void CommitChanges()
		{
			foreach (TagColorItem item in Tags)
			{
				item.CommitColor();
			}
		}
	}
}
