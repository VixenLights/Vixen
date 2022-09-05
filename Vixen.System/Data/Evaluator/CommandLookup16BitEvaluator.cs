using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Data.Evaluator
{
	/// <summary>
	/// This class improves performance by creating re-useable 16 bit commands.
	/// </summary>
	public class CommandLookup16BitEvaluator
	{
		#region Public Properties

		/// <summary>
		/// Dictionary of 16-bit commands indexed by the command ushort value.
		/// </summary>
		public static Dictionary<ushort, _16BitCommand> CommandLookup { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		static CommandLookup16BitEvaluator()
		{
			// Create the dictionary
			CommandLookup = new Dictionary<ushort, _16BitCommand>();

			// Loop over all possible ushort values
			for (int i = 0; i <= ushort.MaxValue; i++)
			{
				// Add the specified value to the dictionary
				CommandLookup.Add((ushort)i, new _16BitCommand((ushort)i));
			}
		}

		#endregion
	}
}
