namespace Common.Messages
{
	/// <summary>Associates a broadcast channel name with the message type it carries.</summary>
	/// <typeparam name="TMessage">The message type published and received on this channel.</typeparam>
	/// <remarks>
	/// Declare channel constants as <c>static readonly BroadcastChannel&lt;T&gt;</c> instead of plain
	/// strings so that callers get a compile-time guarantee that the message type matches the channel.
	/// The raw channel name is available via <see cref="Name"/> for interop with the untyped overloads.
	/// </remarks>
	public sealed class BroadcastChannel<TMessage> where TMessage : class
	{
		/// <summary>Gets the string name of the channel.</summary>
		public string Name { get; }

		/// <summary>Initializes a new <see cref="BroadcastChannel{TMessage}"/> with the given name.</summary>
		/// <param name="name">The unique channel name.</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null" />, empty, or contains only whitespace.</exception>
		internal BroadcastChannel(string name)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name);
			Name = name;
		}
	}
}
