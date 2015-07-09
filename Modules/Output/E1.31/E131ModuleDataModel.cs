
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.Controller.E131;

namespace VixenModules.Output.E131
{
	[DataContract]
	public class E131ModuleDataModel : ModuleDataModelBase
	{
		public E131ModuleDataModel() {
			Universes = new List<UniverseEntry>();
            AutoPopulate = true;
            Priority = 100;
            Universes.Add(new UniverseEntry(0, true, 1, 1, 510));
		}
        
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Priority = 100;
        }
         
		[DataMember]
		public int OutputCount { get; set; }

		[DataMember]
		public bool Warnings { get; set; }

		[DataMember]
		public bool Statistics { get; set; }

		[DataMember]
		public int EventRepeatCount { get; set; }

		[DataMember]
		public int EventSuppressCount { get; set; }

		[DataMember]
		public List<UniverseEntry> Universes { get; set; }

        [DataMember]
        public bool AutoPopulate { get; set; }

        [DataMember]
        public bool Blind { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public string Unicast { get; set; }

        [DataMember]
        public string Multicast { get; set; }


		public override IModuleDataModel Clone()
		{
			return new E131ModuleDataModel
			{
				OutputCount = OutputCount,
				Universes = Universes,
                AutoPopulate = AutoPopulate,
                Blind = Blind,
                Priority = Priority,
                Unicast = Unicast,
                Multicast = Multicast,

				EventRepeatCount = EventRepeatCount,
				EventSuppressCount = EventSuppressCount,
				Statistics = Statistics,
				Warnings = Warnings
			};
		}
	}

   
}