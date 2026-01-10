using System.ComponentModel;

namespace Vixen.Module.Effect
{
	public interface IEffectModuleInstance : IEffect, IModuleInstance
	{
		enum SpecialFilters
		{
			None,
			LiquidUseOneColorList
		}

		 bool ForceGenerateVisualRepresentation { get;   }

		void MarkDirty();
		void Removing();

        /// <summary>
        /// Returns the number of sub-effects
        /// </summary>
        [Browsable(false)]
        int CountOfSubEffects { 
			get { return 0; }
			set { }
		}

		/// <summary>
		/// Returns a sub-effect, by index.
		/// </summary>
		/// <param name="index">Specifies which sub-effect to access</param>
		/// <returns>The sub-effect, specified by index</returns>
		virtual object GetSubEffect(int index)
		{
			return null;
		}

		/// <summary>
		/// Refresh the sub-effect's MVVM bindings.
		/// </summary>
		virtual void UpdateNotifyContentChanged()
		{
		}

		/// <summary>
		/// Gets the properties for a sub-effect like Emitter in Liquid and Waves in Wave.
		/// </summary>
		/// <param name="index">Specifies which sub0-effect to access</param>
		/// <param name="propertyType">Specifies the Property Type to search for</param>
		/// <param name="specialFilters">Specifies a filter value that modifies the returned Property List</param>
		/// <returns>Returns all the properties that are of type Property Type</returns>
		virtual dynamic GetSubEffectProperties(int index, Type propertyType, SpecialFilters specialFilters = SpecialFilters.None)
		{
			return null;
		}

        /// <summary>
        /// Gets the sub-effect's name
        /// </summary>
        /// <param name="index">Specifies which sub-effect to access</param>
        /// <returns>Returns the sub-effect's name</returns>
		public virtual string GetSubEffectName(int index)
		{
			return string.Empty;
		}
	}
}