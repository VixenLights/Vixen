using System.Net;
using Xunit;
using System.Net.Sockets;
using Vixen.Commands;
using DdpController = VixenModules.Output.DDP.DDP;
using VixenModules.Output.DDP;

namespace Vixen.Tests.DDP;

public class DDPPacketTests
{
	[Fact]
	public void UpdateState_SinglePacket_EncodesSpecHeaderAndPayload()
	{
		using var receiver = new UdpClient(new IPEndPoint(IPAddress.Loopback, 4048));
		var controller = new DdpController
		{
			ModuleData = new DDPData { Address = IPAddress.Loopback.ToString() },
			OutputCount = 3
		};
		controller.Start();

		try
		{
			controller.UpdateState(0, [new _8BitCommand(0x12), new _8BitCommand(0x34), new _8BitCommand(0x56)]);
			var remote = new IPEndPoint(IPAddress.Any, 0);
			var packet = receiver.Receive(ref remote);

			Assert.Equal(new byte[] { 0x41, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x12, 0x34, 0x56 }, packet);
		}
		finally
		{
			controller.Stop();
		}
	}

	[Fact]
	public void UpdateState_MultiplePackets_OffsetsAndPushFlagMatchSpec()
	{
		using var receiver = new UdpClient(new IPEndPoint(IPAddress.Loopback, 4048));
		var controller = new DdpController
		{
			ModuleData = new DDPData { Address = IPAddress.Loopback.ToString() },
			OutputCount = 1441
		};
		controller.Start();

		try
		{
			var states = Enumerable.Range(0, 1441).Select(i => (ICommand)new _8BitCommand((byte)(i & 0xff))).ToArray();
			controller.UpdateState(0, states);

			var remote = new IPEndPoint(IPAddress.Any, 0);
			var firstPacket = receiver.Receive(ref remote);
			var secondPacket = receiver.Receive(ref remote);

			Assert.Equal(1450, firstPacket.Length);
			Assert.Equal((byte)0x40, firstPacket[0]);
			Assert.Equal((byte)0x00, firstPacket[4]);
			Assert.Equal((byte)0x00, firstPacket[5]);
			Assert.Equal((byte)0x00, firstPacket[6]);
			Assert.Equal((byte)0x00, firstPacket[7]);
			Assert.Equal((byte)0x05, firstPacket[8]);
			Assert.Equal((byte)0xa0, firstPacket[9]);
			Assert.Equal((byte)0x41, secondPacket[0]);
			Assert.Equal((byte)0x00, secondPacket[4]);
			Assert.Equal((byte)0x00, secondPacket[5]);
			Assert.Equal((byte)0x05, secondPacket[6]);
			Assert.Equal((byte)0xa0, secondPacket[7]);
			Assert.Equal((byte)0x00, secondPacket[8]);
			Assert.Equal((byte)0x01, secondPacket[9]);
			Assert.Equal((byte)0x9f, firstPacket[^1]);
			Assert.Equal((byte)0xa0, secondPacket[10]);
		}
		finally
		{
			controller.Stop();
		}
	}
}
