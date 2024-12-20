using System.Reflection;
using Vixen.Sys;
using System.Collections.Concurrent;
using WPFApplication = System.Windows.Application;

/****************************************************************************************
 * How to use Broadcasting
 * 
 * To send a message:
 *   Broadcast.Transmit<'Type of variable'>("'Name of channel'", 'Variable');
 *     where:
 *       Broadcast is a static class
 *       'Type of variable' is any legal C# identifier that is of type 'Variable'
 *       'Name of Channel' is a any String that identifies the channel to broadcast 
 *                         similar to a TV broadcast channel
 *       'Variable' contains the actual value to transmit and is of the same type
 *                  as 'Type of variable'
 * 
 * To receive a message
 *   Broadcast.AddReceiver<'Type of variable'>("'Name of channel'", 'Callback');
 *     where:
 *       Broadcast is a static class
 *       'Type of variable' is any legal C# identifier that is of type variable in the
 *                          callback's parameter
 *       'Name of Channel' is a any String that identifies the channel to receive the
 *                         broadcast similar to a TV broadcast channel
 *       'Callback' contains the method name of the callback which is in the form of
 *                  void 'Callback'('Type of Variable' variable)
 * 
 * <=============>  IMPORTANT  <=============>
 * Prior to the receiver's method being dismissed (i.e. the class falls out of scope), you MUST 
 * remove the Receiver or else a memory leak will occur
 * 
 * To remove a message receiver
 *   Broadcast.RemoveReceiver<'Type of variable'>("'Name of channel'", 'Callback');
 *     where:
 *       Broadcast is a static class
 *       'Type of variable' is any legal C# identifier that is of type variable in the
 *                          callback's parameter
 *       'Name of Channel' is a any String that identifies the channel to receive the
 *                         broadcast similar to a TV broadcast channel
 *       'Callback' contains the method name of the callback which is in the form of
 *                  void 'Callback'('Type of Variable' variable)
 *           
 * -------------------------------------------------------------------------
 * 
 * Notes:
 *   1) You can use any amount of Transmits and establish any number of Receivers in an 'n to n' relationship.
 *           
 *      Example:
 *        void NumberReceiver(int valueReceived)
 *        {
 *          Console.WriteLine($"Number received: {valueReceived}.");
 *        }
 *        Broadcast.AddReceiver<int>("Number of elements", NumberReceiver);
 *
 *        int valueSent = 123;
 *        Broadcast.Transmit<int>("Number of elements", valueSent);
 *
 *        Broadcast.RemoveReceiver<int>("Number of elements", NumberReceiver);
 *
 *     This will transmit the number 123 to all receivers listening on the channel "Number of Elements".
 * 
 *   2) A channel can only be established for it's 'Type'. For example, the above channel,
 *      "Number of Elements" is linked to an Integer, therefore you cannot establish a channel,
 *      "Number of Elements" with the type of String.  Instead, you would have to name the
 *      channel something like, "Number of Elements in Text"
 * 
 ****************************************************************************************/
namespace Common.Broadcast
{
	public static class Broadcast
	{
		// List of return values
		public enum Result
		{
			SUCCESS,
			NO_RECEIVERS_FOUND,
			RECEIVER_NOT_FOUND,
			RECEIVER_EXISTS,
			CHANNEL_NOT_MATCHING,
			CHANNEL_NOT_FOUND
		}

		private class Frequency
		{
			struct MethodData
			{
				public MethodInfo _callbackData;
				public Action<object> _callback;
			}
			struct Modulation
			{
				public Type _type;
				public List<MethodData> _methodData;
			}

			private readonly ConcurrentDictionary<String, Modulation> _network = new();

			public Result Subscribe<T>(String channel, Action<T> callback)
			{
				Result result = Result.SUCCESS;
				Modulation encoders;

				// If this channel has not been opened prior, then create a channel
				if (!_network.TryGetValue(channel, out encoders))
				{
					encoders = new Modulation();
					encoders._type = typeof(T);
					encoders._methodData = new List<MethodData>();
					_network[channel] = encoders;
				}

				// Validate that the channel is for the correct Type
				if (encoders._type != typeof(T))
					result = Result.CHANNEL_NOT_MATCHING;
				else
				{
					// Iterate through all the Callbacks, checking to see if this one has
					// already been added
					bool existing = false;
					foreach (var methodData in encoders._methodData)
					{
						if (methodData._callbackData == callback.GetMethodInfo())
						{
							existing = true;
							result = Result.RECEIVER_EXISTS;
							break;
						}
					}

					// If the Callback is not already added, then add the Callback
					if (!existing)
					{
						MethodData methodData = new();

						methodData._callbackData = callback.GetMethodInfo();
						methodData._callback = obj => callback((T)obj);
						encoders._methodData.Add(methodData);
					}
				}

				return result;
			}

			public Result Unsubscribe<T>(String channel, Action<T> callback)
			{
				Result result = Result.SUCCESS;
				Modulation encoders;

				// Find the channel 
				if (_network.TryGetValue(channel, out encoders))
				{
					// Validate that the channel is for the correct Type
					if (encoders._type != typeof(T))
						result = Result.CHANNEL_NOT_MATCHING;
					else
					{
						// Iterate through all the Callbacks to find the correct one, then
						// remove that Callback
						bool existing = false;
						foreach (var methodData in encoders._methodData)
						{
							if (methodData._callbackData == callback.GetMethodInfo())
							{
								encoders._methodData.Remove(methodData);
								existing = true;
								break;
							}
						}

						if (!existing)
						{
							result = Result.RECEIVER_NOT_FOUND;
						}
						// If there are no more Callbacks, then also remove the channel
						else if (encoders._methodData.Count == 0)
						{
							_network.TryRemove(channel, out var Mod);
						}
					}
				}
				else
					result = Result.CHANNEL_NOT_FOUND;

				return result;
			}

			public Result Publish<T>(String channel, T message)
			{
				Result result = Result.SUCCESS;
				Modulation encoders;

				// Find the channel 
				if (_network.TryGetValue(channel, out encoders))
				{
					// Validate that the channel is for the correct Type
					if (encoders._type != typeof(T))
						result = Result.CHANNEL_NOT_MATCHING;
					else
					{
						// Iterate through all the Callbacks to find the correct one, then
						// run that Callback
						bool existing = false;
						foreach (var methodData in encoders._methodData)
						{
							// We're either in the application thread or the GUI thread
							if (VixenSystem.UIThread == System.Threading.Thread.CurrentThread)
								methodData._callback(message);
							else
								WPFApplication.Current.Dispatcher.Invoke(methodData._callback, message);
							existing = true;
						}

						if (!existing)
							result = Result.RECEIVER_NOT_FOUND;
					}
				}

				else
					result = Result.CHANNEL_NOT_FOUND;

				return result;
			}
		}

		private static readonly Frequency _transmission = new();
		static ManualResetEventSlim mres1;

		static Broadcast()
		{
			mres1 = new ManualResetEventSlim(false);
		}

		/// <summary>
		/// Transmit a message of a specified type to all that are receiving on the channel
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="channel">Specify the message channel</param>
		/// <param name="message">Specify the message content</param>
		/// <returns>
		///   <list type="bullet">
		///     <item>
		///       <term>SUCCESS</term>
		///       <description>Transmission completed successfully</description>
		///     </item>
		///     <item>
		///       <term>RECEIVER_NOT_FOUND</term>
		///       <description>No receiver for the specified channel and type was found</description>
		///     </item>
		///     <item>
		///       <term>CHANNEL_NOT_MATCHING</term>
		///       <description>No channel is registered for a message of type T</description>
		///     </item>
		///   </list>
		/// </returns>
		public static Result Transmit<T>(String channel, T message)
		{
			return _transmission.Publish(channel, message);
		}

		/// <summary>
		/// Identify the receiver of a message of a specified type to all that are receiving on channel "channel"
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="channel">Specify the message channel</param>
		/// <param name="callback">Specify the message receiver</param>
		/// <returns>
		///   <list type="bullet">
		///     <item>
		///       <term>SUCCESS</term>
		///       <description>Action completed successfully</description>
		///     </item>
		///     <item>
		///       <term>RECEIVER_EXISTS</term>
		///       <description>The receiver already exists</description>
		///     </item>
		///     <item>
		///       <term>CHANNEL_NOT_MATCHING</term>
		///       <description>No channel is registered for a message of type T</description>
		///     </item>
		///     <item>
		///       <term>CHANNEL_NOT_FOUND</term>
		///       <description>No receiver for the specified channel was found</description>
		///     </item>
		///   </list>
		/// </returns>
		public static Result AddReceiver<T>(String channel, Action<T> callback)
		{
			return _transmission.Subscribe(channel, callback);
		}

		/// <summary>
		/// Transmit a message "message" of type "T" to all that are receiving on channel "channel"
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="channel">Specify the message channel</param>
		/// <param name="message">Specify the message content</param>
		/// <returns>
		///   <list type="bullet">
		///     <item>
		///       <term>SUCCESS</term>
		///       <description>Action completed successfully</description>
		///     </item>
		///     <item>
		///       <term>RECEIVER_NOT_FOUND</term>
		///       <description>No receiver for the specified channel and type was found</description>
		///     </item>
		///     <item>
		///       <term>CHANNEL_NOT_MATCHING</term>
		///       <description>No channel is registered for a message of type T</description>
		///     </item>
		///     <item>
		///       <term>CHANNEL_NOT_FOUND</term>
		///       <description>No receiver for the specified channel was found</description>
		///     </item>
		///   </list>
		/// </returns>
		public static Result RemoveReceiver<T>(String channel, Action<T> callback)
		{
			return _transmission.Unsubscribe(channel, callback);
		}
	}
}
