using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Stores a custom State item row for the State effect.
	/// </summary>
	[DataContract]
	public sealed class CustomStateItemData
	{
		/// <summary>
		/// Gets or sets the stable identifier of the selected State item.
		/// </summary>
		/// <value>The selected State item identifier, or <see cref="Guid.Empty" /> for the Iterate-only <c>&lt;None&gt;</c> row.</value>
		[DataMember]
		public Guid StateItemId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the color override used when rendering this custom State item row.
		/// </summary>
		/// <value>The custom row color.</value>
		[DataMember]
		public Color Color { get; set; } = Color.White;

		/// <summary>
		/// Creates a clone of this custom State item row.
		/// </summary>
		/// <returns>A cloned custom State item row.</returns>
		public CustomStateItemData CreateInstanceForClone()
		{
			return new CustomStateItemData
			{
				StateItemId = StateItemId,
				Color = Color
			};
		}
	}
}
