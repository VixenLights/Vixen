namespace VixenModules.OutputFilter.TaggedFilter.Outputs
{
	/// <summary>
	/// Maintains a tagged filter output.
	/// </summary>
	public class TaggedFilterOutput : TaggedFilterOutputBase<Filters.TaggedFilter> 
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tag">Tag applicable to the output</param>
		public TaggedFilterOutput(string tag) : base(tag) 
		{	
		}

		#endregion
	}
}