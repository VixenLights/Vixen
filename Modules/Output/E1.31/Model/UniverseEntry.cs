namespace VixenModules.Output.E131.Model
{
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class UniverseEntry
    {
        public UniverseEntry(bool isActive, int universeNumber, int startIndex, int size, string unicast, string multicast, int ttl)
        {
            IsActive = isActive;
            UniverseNumber = universeNumber;
            StartIndex = startIndex;
            Size = size;
            Unicast = unicast;
            MulticastNicId = multicast;
            Ttl = ttl;
            Socket = null;
            DestIpEndPoint = null;
            PhyBuffer = null;
            EventRepeatCount = 0;
            PacketCount = 0;            
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the universe is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///   Gets or sets the destination end point
        /// </summary>
        public IPEndPoint DestIpEndPoint { get; set; }

        /// <summary>
        ///   Gets or sets how many identical pkts to skip (0 = none)
        /// </summary>
        public int EventRepeatCount { get; set; }

        public string InfoToText
        {
            get
            {
                var text = new StringBuilder();
                text.Append(" Univ=");
                text.Append(UniverseNumber.ToString());
                text.Append(" Start=");
                text.Append((StartIndex + 1).ToString());
                text.Append(" Size=");
                text.Append(Size.ToString());
                if (Unicast != null)
                {
                    text.Append(" Unicast");
                }

                if (MulticastNicId != null)
                {
                    text.Append(" Multicast");
                }

                text.Append(" TTL=");
                text.Append(Ttl.ToString());
                return text.ToString();
            }
        }

        /// <summary>
        ///   Gets the Multicast NIC ID (if not null)
        /// </summary>
        public string MulticastNicId { get; private set; }

        /// <summary>
        ///   Gets or sets the Physical buffer
        /// </summary>
        public byte[] PhyBuffer { get; set; }

        /// <summary>
        ///   Gets or sets the packet count per universe
        /// </summary>
        public int PacketCount { get; set; }

        public string RowUnivToText
        {
            get
            {
                var text = new StringBuilder();
                text.Append(" Univ=");
                text.Append(UniverseNumber.ToString());
                return text.ToString();
            }
        }

        /// <summary>
        ///   Gets or set the number of slots
        /// </summary>
        public int Size { get; private set; }

        public Socket Socket { get; set; }

        /// <summary>
        ///   Gets the zero based starting slot.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        ///   Gets the time to live
        /// </summary>
        public int Ttl { get; private set; }

        /// <summary>
        ///   Gets the Unicast IP Address
        /// </summary>
        public string Unicast { get; private set; }

        /// <summary>
        ///   Gets the Universe Number
        /// </summary>
        public int UniverseNumber { get; private set; }
    }
}
