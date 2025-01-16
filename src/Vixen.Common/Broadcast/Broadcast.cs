using System.Reflection;
using Vixen.Sys;
using System.Collections.Concurrent;
using WPFApplication = System.Windows.Application;
using CommunityToolkit.Mvvm.Messaging;

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
 *   Broadcast.AddReceiver<'Type of variable'>('Callback's class', "'Name of channel'", 'Callback');
 *     where:
 *       Broadcast is a static class
 *       'Callback's class' is the class where the callback is instantiated
 *       'Type of variable' is any legal C# identifier that is of type variable in the
 *                          callback's parameter
 *       'Name of Channel' is a any String that identifies the channel to receive the
 *                         broadcast similar to a TV broadcast channel
 *       'Callback' contains the method name of the callback which is in the form of
 *                  void 'Callback'('Type of Variable' variable)
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
 *        Broadcast.AddReceiver<int>(this, "Number of elements", NumberReceiver);
 *
 *        int valueSent = 123;
 *        Broadcast.Transmit<int>("Number of elements", valueSent);
 *
 *        Broadcast.RemoveReceiver<int>(this, "Number of elements");
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
		/// <summary>
		/// Transmit a message of a specified type to all that are receiving on the channel
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="channel">Specify the message channel</param>
		/// <param name="message">Specify the message content</param>
		public static void Transmit<T>(String channel, T message) where T : class
		{
			if (VixenSystem.UIThread == System.Threading.Thread.CurrentThread)
				WeakReferenceMessenger.Default.Send<T, String>(message, channel);
			else
				WPFApplication.Current.Dispatcher.Invoke( (Action)(() => WeakReferenceMessenger.Default.Send<T, String>(message, channel) ));
		}

		/// <summary>
		/// Identify the receiver of a message of a specified type to all that are receiving on channel "channel"
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="source">Receiver's object</param>
		/// <param name="channel">Specify the message channel</param>
		/// <param name="callback">Specify the message receiver</param>
		public static void AddReceiver<T>(Object source, String channel, Action<T> callback) where T:class
		{
			if (!WeakReferenceMessenger.Default.IsRegistered<T, String>(source, channel))
				WeakReferenceMessenger.Default.Register<T, String>(source, channel, (r, m) => { callback(m); });
		}

		/// <summary>
		/// Transmit a message "message" of type "T" to all that are receiving on channel "channel"
		/// </summary>
		/// <typeparam name="T">Specify the type of the message</typeparam>
		/// <param name="source">Receiver's object</param>
		/// <param name="channel">Specify the message channel</param>
		public static void RemoveReceiver<T>(Object source, String channel) where T : class
		{
			WeakReferenceMessenger.Default.Unregister<T, String>(source, channel);
		}
	}
}
