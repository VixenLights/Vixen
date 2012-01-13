namespace VixenModules.Output.E131
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Vixen.Module;
    using VixenModules.Output.E131.Model;

    [DataContract]
    public class Data : ModuleDataModelBase
    {
        [DataMember]
        public Guid SenderId { get; set; }

        [DataMember]
        public ObservableCollection<UniverseEntry> Universes { get; set; }

        public override IModuleDataModel Clone()
        {
            return MemberwiseClone() as IModuleDataModel;
        }

        [DataMember]
        public int EventRepeatCount { get; set; }

        [DataMember]
        public bool DisplayWarnings { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Universes == null)
            {
                Universes = new ObservableCollection<UniverseEntry>();
            }

            if (SenderId == Guid.Empty)
            {
                SenderId = new Guid();
            }
        }
    }
}
