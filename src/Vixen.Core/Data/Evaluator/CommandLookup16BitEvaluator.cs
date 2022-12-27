using Vixen.Commands;

namespace Vixen.Data.Evaluator
{
	/// <summary>
	/// This class improves performance by creating re-useable 16 bit commands.
	/// </summary>
	public class CommandLookup16BitEvaluator
	{
		#region Private Properties

		/// <summary>
		/// Dictionary of 16-bit commands indexed by the command ushort value.
		/// </summary>
		private static Dictionary<ushort, _16BitCommand> CommandLookup { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		static CommandLookup16BitEvaluator()
		{
			// Create the dictionary
			CommandLookup = new Dictionary<ushort, _16BitCommand>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets a cached command for the specified value.
		/// </summary>
		/// <param name="value">Value to retrieve the command for</param>
		/// <returns>16 Bit command wrapping the specified value</returns>
		public static _16BitCommand GetCommand(ushort value)
		{
			// If the dictionary does NOT contain the specified value then...
			if (!CommandLookup.ContainsKey(value))
			{
				// Add the command to the dictionary
				CommandLookup.Add((ushort)value, new _16BitCommand(value));
			}
			
			// Return the cached command
			return CommandLookup[value];
		}

		#endregion
	}
}
