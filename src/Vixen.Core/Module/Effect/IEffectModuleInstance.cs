using System.ComponentModel;

namespace Vixen.Module.Effect
{
	public interface IEffectModuleInstance : IEffect, IModuleInstance
	{
		 bool ForceGenerateVisualRepresentation { get;   }

		void MarkDirty();
		void Removing();

		/// <summary>
		/// Update a property and notify of content change.
		/// </summary>
		/// <param name="descriptor">Specifies the property's descriptor</param>
		/// <param name="effect">Specifies the effect the property belongs to. If the effect contains sub-effects (i.e. Wave), this is the sub-effect instance.</param>
		/// <param name="newProperty">Specifies the new property value to set</param>
		virtual void UpdateProperty(PropertyDescriptor descriptor, object effect, Object newProperty)
		{
		}

		/// <summary>
		/// Refresh the sub-effect's MVVM bindings.
		/// </summary>
		virtual void UpdateNotifyContentChanged()
		{
		}

		public virtual List<EffectProperties> GetProperties(IEnumerable<PropertyDescriptor> baseProperty)
		{
			return null;
		}
	}
}