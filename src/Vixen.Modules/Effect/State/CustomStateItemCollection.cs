using System.Collections.Specialized;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Maintains the custom State item rows for the State effect.
	/// </summary>
	public sealed class CustomStateItemCollection: ExpandoObjectObservableCollection<CustomStateItem, CustomStateItem>
	{
		private State? _parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomStateItemCollection" /> class.
		/// </summary>
		public CustomStateItemCollection() :
			base("CustomStateItems")
		{
		}

		/// <summary>
		/// Gets or sets the parent State effect that provides State item options.
		/// </summary>
		/// <value>The parent State effect.</value>
		public State? Parent
		{
			get => _parent;
			set
			{
				_parent = value;
				foreach (var item in this)
				{
					item.Parent = _parent;
				}
			}
		}

		/// <inheritdoc />
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			foreach (var item in this)
			{
				item.Parent = Parent;
			}
		}

		/// <inheritdoc />
		public override int GetMinimumItemCount()
		{
			return Parent?.RenderSource == StateRenderSource.Custom ? 1 : 0;
		}
	}
}
