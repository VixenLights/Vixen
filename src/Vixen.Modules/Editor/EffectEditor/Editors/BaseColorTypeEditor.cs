using System.Drawing;
using VixenModules.Editor.EffectEditor.Internal;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public abstract class BaseColorTypeEditor : TypeEditor
	{
		protected BaseColorTypeEditor(Type editedType, object inlineTemplate)
			: base(editedType, inlineTemplate, null)
		{
		}

		/// <summary>
		/// Gets the discrete colors that should constrain color editing for the specified component.
		/// </summary>
		/// <param name="component">The component that owns the edited color property.</param>
		/// <returns>The valid discrete colors, or an empty set when full color editing is allowed.</returns>
		protected HashSet<Color> GetDiscreteColors(Object component)
		{
			return Util.GetDiscreteColors(component);
		}
	}
}
