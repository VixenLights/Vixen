//-----------------------------------------------------------------
//
// UniverseEntry - a class to keep all in memory info together
//
//-----------------------------------------------------------------

namespace VixenModules.Controller.E131
{
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
	public class UniverseEntry
	{
        public UniverseEntry() { }

        //Deprecated - Kept for transition
		public UniverseEntry(
			int rowNum, bool active, int universe, int start, int size, string unicast, string multicast, int ttl)
		{
			this.RowNum = rowNum;
			this.Active = active;
			this.Universe = universe;
			this.Start = start;
			this.Size = size;
			this.Unicast = unicast; //deprecated
			this.Multicast = multicast; //deprecated
			this.Ttl = ttl; //deprecated

			this.Socket = null;
			this.DestIpEndPoint = null;
			this.PhyBuffer = null;
			this.EventRepeatCount = 0;
			this.EventSuppressCount = 0;

			this.seqNum = 0;

			this.PktCount = 0;
			this.SlotCount = 0;
		}

        public UniverseEntry(
        int rowNum, bool active, int universe, int start, int size)
        {
            this.RowNum = rowNum;
            this.Active = active;
            this.Universe = universe;
            this.Start = start;
            this.Size = size;
            this.Unicast = null; //deprecated
            this.Multicast = null; //deprecated
            this.Ttl = 64; //deprecated

            this.Socket = null;
            this.DestIpEndPoint = null;
            this.PhyBuffer = null;
            this.EventRepeatCount = 0;
            this.EventSuppressCount = 0;

            this.seqNum = 0;

            this.PktCount = 0;
            this.SlotCount = 0;
        }

        public UniverseEntry(
        int rowNum, bool active, int universe, int start, int size, string unicast, string multicast)
        {
            this.RowNum = rowNum;
            this.Active = active;
            this.Universe = universe;
            this.Start = start;
            this.Size = size;
            this.Unicast = unicast; //deprecated
            this.Multicast = multicast; //deprecated
            this.Ttl = 64; //deprecated

            this.Socket = null;
            this.DestIpEndPoint = null;
            this.PhyBuffer = null;
            this.EventRepeatCount = 0;
            this.EventSuppressCount = 0;

            this.seqNum = 0;

            this.PktCount = 0;
            this.SlotCount = 0;
        }

		/// <summary>
		///   Gets or sets a value indicating whether the universe is active.
		/// </summary>
        [DataMember]
        public bool Active { get; set; }

		/// <summary>
		///   Gets or sets the destination end point
		/// </summary>
		public IPEndPoint DestIpEndPoint { get; set; }

		/// <summary>
		///   Gets or sets how many identical pkts are being sent (0 = all)
		/// </summary>
        [DataMember]
        public int EventRepeatCount { get; set; }

		/// <summary>
		///   Gets or sets how many identical pkts are being suppressed (0 = none)
		/// </summary>
		[DataMember]
		public int EventSuppressCount { get; set; }

		public string InfoToText
		{
			get
			{
				var text = new StringBuilder();
				text.Append("Row ");
				text.Append(this.RowNum.ToString());
				text.Append(":");
				text.Append(" Univ=");
				text.Append(this.Universe.ToString());
				text.Append(" Start=");
				text.Append((this.Start + 1).ToString());
				text.Append(" Size=");
				text.Append(this.Size.ToString());
				if (this.Unicast != null) {
					text.Append(" Unicast");
				}

				if (this.Multicast != null) {
					text.Append(" Multicast");
				}

				text.Append(" TTL=");
				text.Append(this.Ttl.ToString());
				return text.ToString();
			}
		}

		/// <summary>
		///   Gets or sets the seqNum
		/// </summary>
        [DataMember]
        public byte seqNum { get; set; }

		/// <summary>
		///   Gets the Multicast NIC ID (if not null)
		/// </summary>
        [DataMember]
        public string Multicast { get; /*private*/ set; }

		/// <summary>
		///   Gets or sets the Physical buffer
		/// </summary>
		public byte[] PhyBuffer { get; set; }

		/// <summary>
		///   Gets or sets the packet count per universe
		/// </summary>
		public int PktCount { get; set; }

		public string RowUnivToText
		{
			get
			{
				var text = new StringBuilder();
				text.Append("Row ");
				text.Append(this.RowNum.ToString());
				text.Append(":");
				text.Append(" Univ=");
				text.Append(this.Universe.ToString());
				return text.ToString();
			}
		}

		/// <summary>
		///   Gets or set the number of slots
		/// </summary>
        [DataMember]
        public int Size { get; private set; }

		/// <summary>
		///   Gets or sets the slot count per universe
		/// </summary>
     
        public long SlotCount { get; set; }

		public Socket Socket { get; set; }

		/// <summary>
		///   Gets the zero based starting slot.
		/// </summary>
        [DataMember]
        public int Start { get; private set; }

		public string StatsToText
		{
			get
			{
				var text = new StringBuilder();
				text.Append("Row ");
				text.Append(this.RowNum.ToString());
				text.Append(":");
				text.Append(" Univ=");
				text.Append(this.Universe.ToString());
				text.Append("  Packets=");
				text.Append(this.PktCount.ToString());
				text.Append("  Slots=");
				text.Append(this.SlotCount.ToString());
				return text.ToString();
			}
		}

		/// <summary>
		///   Gets the time to live. Deprecated.
		/// </summary>
        [DataMember]
        public int Ttl { get; private set; }

		/// <summary>
		///   Gets the Unicast IP Address. Deprecated.
		/// </summary>
        [DataMember]
        public string Unicast { get; private set; }

		/// <summary>
		///   Gets the Universe Number.
		/// </summary>
        [DataMember]
		public int Universe { get; private set; }

		/// <summary>
		///   Gets or sets the row number (1-x)
		/// </summary>
		private int RowNum { get; set; }
	}
}