using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Property
{
	public interface IProperty : IHasSetup
	{
		IElementNode Owner { get; set; }

		/// <summary>
		/// Set or reset the property's values to a property-specific default.
		/// </summary>
		void SetDefaultValues();

		/// <summary>
		/// Clones the property-specific values of sourceProperty so that they're appropriate for the local values.
		/// </summary>
		void CloneValues(IProperty sourceProperty);
	}
}