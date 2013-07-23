using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;

namespace VixenModules.App.WebServer.HTTP
{
	class BufferedConsumer : IDataConsumer
	{
		List<ArraySegment<byte>> buffer = new List<ArraySegment<byte>>();
		Action<string> resultCallback;
		Action<Exception> errorCallback;

		public BufferedConsumer(Action<string> resultCallback, Action<Exception> errorCallback)
		{
			this.resultCallback = resultCallback;
			this.errorCallback = errorCallback;
		}
		public bool OnData(ArraySegment<byte> data, Action continuation)
		{
			// since we're just buffering, ignore the continuation. 
			// TODO: place an upper limit on the size of the buffer. 
			// don't want a client to take up all the RAM on our server! 
			buffer.Add(data);
			return false;
		}
		public void OnError(Exception error)
		{
			errorCallback(error);
		}

		public void OnEnd()
		{
			// turn the buffer into a string. 
			// 
			// (if this isn't what you want, you could skip 
			// this step and make the result callback accept 
			// List<ArraySegment<byte>> or whatever) 
			// 
			var str = buffer
				.Select(b => Encoding.UTF8.GetString(b.Array, b.Offset, b.Count))
				.Aggregate((result, next) => result + next);

			resultCallback(str);
		}
	}
}
