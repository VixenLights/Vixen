namespace Vixen.Sys.Props.HelperTools
{
	public class PropParameters : Dictionary<string, object>
	{
		/// <summary>
		/// Set up a Prop Parameter database
		/// </summary>
		public PropParameters()
		{
		}

		/// <summary>
		/// Set up a Prop Parameter database
		/// </summary>
		/// <param name="key">Specifies the primary key</param>
		/// <param name="value">Specifies the data associated with this key</param>
		public PropParameters(string key, object value)
		{
			Update(key, value);
		}

		/// <summary>
		/// Adds or updates the value associated with the key
		/// </summary>
		/// <param name="key">Specifies the primary key</param>
		/// <param name="value">Specifies the data associated with this key</param>
		/// <returns></returns>
		public PropParameters Update(string key, object value)
		{
			if (ContainsKey(key))
			{
				this[key] = value;
			}
			else
			{
				Add(key, value);
			}
			return this;
		}

		/// <summary>
		/// Returns the value associated with the key
		/// </summary>
		/// <param name="key">Specifies the primary key</param>
		/// <returns>Returns the <see cref="object"/> (or data) associated with the key</returns>
		public object Get(string key)
		{
			if (ContainsKey(key))
			{
				return this[key];
			}
			else
			{
				return null;
			}
		}
	}
}
