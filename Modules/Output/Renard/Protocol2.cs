using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

//TODO: This.
namespace VixenModules.Output.Renard {
	//[ProtocolVersion(2)]
	class Protocol2 {
		//private byte[] _p2Packet;
		//private byte[] _p2Zeroes;
		//private int _outputCount;

		//public Protocol2() {
		//    // 1 byte per channel * 8 channels = 8 + 3 control bytes = 11
		//    _p2Packet = new byte[11];
		//    _p2Packet[0] = 0x00;
		//    _p2Zeroes = new byte[8];
		//}

		//public void StartPacket(int outputCount) {
		//    _outputCount = outputCount;
		//}

		//public int ChainIndex { get; set; }

		//public void Add(LightingValueCommand command) {
		//    int startChannel, endChannel;
		//    byte picIndex = 0x80;
		//    int arrayIndex, arrayIndex2;
		//    byte offsetByte;
		//    byte[] usedBytes = new byte[8];
		//    byte bottomValue, topValue;

		//    // One whole PIC at a time.
		//    for(startChannel = 0; startChannel < channelCount; startChannel += 8) {
		//        endChannel = Math.Min(startChannel + 7, channelCount - 1);
		//        _p2Packet[1] = picIndex++;

		//        // Not all pins of the last PIC may be used, so clear the data portion 
		//        // of the packet if it's the last PIC.
		//        if(endChannel >= channelCount - 1) {
		//            _p2Zeroes.CopyTo(_p2Packet, 3);
		//        }

		//        // Track the possible offset bytes that are used by packet values
		//        Array.Clear(usedBytes, 0, 8); // favoring size over speed for this

		//        // Scan the values for this PIC for the offset byte
		//        // Need to find a value between 1 and 8 (inclusive)
		//        // The offset needs to be a value that, when added to any channel value,
		//        // keeps it from being 0 (including wrap-around).
		//        for(arrayIndex = startChannel; arrayIndex <= endChannel; arrayIndex++) {
		//            Command command = outputStates[arrayIndex];
		//            if(!(command is Lighting.Monochrome.SetLevel)) continue;
		//            byte level = (byte)(command as Lighting.Monochrome.SetLevel).Level;

		//            bottomValue = level;
		//            topValue = (byte)(0 - bottomValue);
		//            if(bottomValue >= 1 && bottomValue <= 8) {
		//                usedBytes[bottomValue - 1] = 1;
		//            } else if(topValue >= 1 && topValue <= 8) {
		//                usedBytes[topValue - 1] = 1;
		//            }
		//        }
		//        offsetByte = (byte)(1 + Array.IndexOf(usedBytes, (byte)0));
		//        _p2Packet[2] = offsetByte;

		//        // Copy values to the packet, adding the offset byte
		//        for(arrayIndex = startChannel, arrayIndex2 = 3; arrayIndex <= endChannel; arrayIndex++, arrayIndex2++) {
		//            Command command = outputStates[arrayIndex];
		//            if(!(command is Lighting.Monochrome.SetLevel)) continue;
		//            byte level = (byte)(command as Lighting.Monochrome.SetLevel).Level;
		//            _p2Packet[arrayIndex2] = (byte)(level - offsetByte);
		//        }

		//        //if(IsRunning) {
		//        //    _port.Write(_p2Packet, 0, arrayIndex2);
		//        //}
		//    }
		//}

		//public int PacketSize {
		//    get { return arrayIndex2; }
		//}

		//public byte[] FinishPacket() {
		//    return _packet;
		//}
	}
}
