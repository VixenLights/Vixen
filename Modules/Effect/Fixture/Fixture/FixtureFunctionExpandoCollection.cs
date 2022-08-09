using System;
using System.Collections.Generic;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains a collection of fixture function expando objects.
	/// </summary>
	public class FixtureFunctionExpandoCollection : ExpandoObjectObservableCollection<IFixtureFunctionExpando, FixtureFunctionExpando>		
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionExpandoCollection() :
			base("FixtureFunctions")
		{
		}

		#endregion

		#region IExpandoObjectCollection

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public override int GetMinimumItemCount()
		{
			// The fixture function collection starts out empty
			return 0;
		}

		#endregion
	}
}
