using System.ComponentModel;
using System.Drawing;
using Vixen.Attributes;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Maintains an editable custom State item row for the State effect.
	/// </summary>
	[ExpandableObject]
	public sealed class CustomStateItem: ExpandoObjectBase, IDiscreteColorProvider
	{
		private Color _color = Color.White;
		private Guid _stateItemId = Guid.Empty;

		/// <summary>
		/// Gets or sets the parent State effect that provides State item options.
		/// </summary>
		/// <value>The parent State effect.</value>
		[Browsable(false)]
		public State? Parent { get; set; }

		/// <summary>
		/// Gets or sets the stable identifier of the selected State item.
		/// </summary>
		/// <value>The selected State item identifier, or <see cref="Guid.Empty" /> for the Iterate-only <c>&lt;None&gt;</c> row.</value>
		[Browsable(false)]
		public Guid StateItemId
		{
			get => _stateItemId;
			set
			{
				if (_stateItemId == value)
				{
					return;
				}

				_stateItemId = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StateItem));
			}
		}

		/// <summary>
		/// Gets or sets the selected State item display label.
		/// </summary>
		/// <value>The selected State item display label.</value>
		[ProviderDisplayName(@"StateItem")]
		[ProviderDescription(@"StateItem")]
		[TypeConverter(typeof(CustomStateItemNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(0)]
		public string StateItem
		{
			get => Parent?.GetCustomStateItemLabel(this) ?? State.NoStateItemsLabel;
			set => Parent?.SelectCustomStateItem(this, value);
		}

		/// <summary>
		/// Gets or sets the color override used when rendering this custom State item row.
		/// </summary>
		/// <value>The custom row color.</value>
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public Color Color
		{
			get => _color;
			set
			{
				if (_color == value)
				{
					return;
				}

				_color = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Creates serialized data for this custom State item row.
		/// </summary>
		/// <returns>The serialized custom State item row data.</returns>
		public CustomStateItemData CreateData()
		{
			return new CustomStateItemData
			{
				StateItemId = StateItemId,
				Color = Color
			};
		}

		/// <summary>
		/// Updates this custom State item row from serialized data.
		/// </summary>
		/// <param name="data">The serialized custom State item row data.</param>
		public void UpdateFromData(CustomStateItemData data)
		{
			StateItemId = data.StateItemId;
			Color = data.Color;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return StateItem;
		}

		/// <inheritdoc />
		public HashSet<Color> GetDiscreteColors()
		{
			return Parent?.GetCustomStateItemValidColors(this) ?? [];
		}

		internal void RefreshStateItemOptions()
		{
			OnPropertyChanged(nameof(StateItem));
		}
	}
}
