using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;

namespace VixenModules.App.WebServer.HTTP
{
	class BufferedProducer : IDataProducer
	{
		ArraySegment<byte> data;

		public BufferedProducer(string data) : this(data, Encoding.UTF8) { }
		public BufferedProducer(string data, Encoding encoding) : this(encoding.GetBytes(data)) { }
		public BufferedProducer(byte[] data) : this(new ArraySegment<byte>(data)) { }
		public BufferedProducer(ArraySegment<byte> data)
		{
			this.data = data;
		}

		public IDisposable Connect(IDataConsumer channel)
		{
			// null continuation, consumer must swallow the data immediately.
			channel.OnData(data, null);
			channel.OnEnd();
			return null;
		}
	}
}
