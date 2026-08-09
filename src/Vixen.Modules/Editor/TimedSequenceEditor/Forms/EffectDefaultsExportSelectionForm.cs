using Common.Controls;
using Common.Controls.Theme;
using Vixen.Services.EffectDefaults;

namespace VixenModules.Editor.TimedSequenceEditor
{
	/// <summary>
	/// Lists the currently saved effect defaults with checkboxes, so a user can choose which ones to export to a
	/// file (see <see cref="EffectDefaultsService.Export"/>).
	/// </summary>
	public partial class EffectDefaultsExportSelectionForm : BaseForm
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EffectDefaultsExportSelectionForm"/> class, pre-checking
		/// every entry in <paramref name="summaries"/>.
		/// </summary>
		/// <param name="summaries">The saved effect defaults to list, typically from
		/// <see cref="EffectDefaultsService.GetSummaries"/>.</param>
		public EffectDefaultsExportSelectionForm(IEnumerable<EffectDefaultSummary> summaries)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			foreach (EffectDefaultSummary summary in summaries.OrderBy(summary => summary.EffectName))
			{
				checkedListBoxEffects.Items.Add(summary, true);
			}
		}

		/// <summary>
		/// Gets the effect type <c>TypeId</c>s the user checked.
		/// </summary>
		public IReadOnlyCollection<Guid> SelectedEffectTypeIds
		{
			get
			{
				return checkedListBoxEffects.CheckedItems
					.Cast<EffectDefaultSummary>()
					.Select(summary => summary.TypeId)
					.ToList();
			}
		}
	}
}
