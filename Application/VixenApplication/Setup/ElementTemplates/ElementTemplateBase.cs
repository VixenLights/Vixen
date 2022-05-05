using Common.Controls;
using System.Collections.Generic;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	/// <summary>
	/// Base class for an element template.
	/// </summary>
	public abstract class ElementTemplateBase : BaseForm
	{
		#region IElementTamplate

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual bool ConfigureColor => true;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual bool ConfigureDimming => true;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual bool Cancelled => false;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual IEnumerable<ElementNode> GetElementsToDelete()
		{
			// By default there are no element nodes to delete
			return new List<ElementNode>();
		}

		#endregion
	}
}
