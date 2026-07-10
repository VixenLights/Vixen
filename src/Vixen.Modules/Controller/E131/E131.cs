// =====================================================================
// E131.cs - common E1.31 classes
// written from the ground up based on the protocol specifications
// from E1.31 published documentation available. in no way is
// this claimed to be an official implementation endorsed or
// certified in any way.
// mainly a collection of classes used to build, send, receive,
// and manipulate the E1.31 protocol in a logical vs. physical
// format. the three layers are represented as C# classes without
// concerns of actual transport formatting. the PhyBuffer member
// converts the values to/from a byte array for transport in
// network normal byte ordering and packing.
// this is not the most efficient way. it tries to be object
// oriented instead of efficient to cleanly implement the
// protocol.
// there are other ways through interop marshalling that may
// be more efficient for the 'conversion' between logical vs.
// physical but they may then be less efficient for manipulation
// of the resultant data within C# with its System.Object based
// reference variable etc. and there is always the big-endian
// vs. little-endian conversions to take into account.
// however once built, if a copy of the phybuffer is preserved,
// it only needs to be 'patched' in two places (sequence # and slots)
// and reused to send another packet of the same format.
// version 1.0.0.0 - 1 june 2010
// =====================================================================

// =====================================================================
// Copyright (c) 2010 Joshua 1 Systems Inc. All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright notice, this list of
//       conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright notice, this list
//       of conditions and the following disclaimer in the documentation and/or other materials
//       provided with the distribution.
// THIS SOFTWARE IS PROVIDED BY JOSHUA 1 SYSTEMS INC. "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Joshua 1 Systems Inc.
// =====================================================================

namespace VixenModules.Controller.E131
{
	using System.Linq;

	/// <summary>
	///   A collection of common functions and constants.
	/// </summary>
	public abstract class E131Base
	{
		public abstract byte[] PhyBuffer { get; set; }

		public override string ToString()
		{
			var txt = string.Empty;
			var buffer = PhyBuffer;
			return buffer == null
			       	? string.Empty
			       	: buffer.Aggregate(txt, (current, val) => current + (val.ToString("X2") + ' '));
		}
	}
}