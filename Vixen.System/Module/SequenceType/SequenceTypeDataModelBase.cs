using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Execution;
using Vixen.Module.Media;
using Vixen.Module.SequenceType.Surrogate;
using Vixen.Sys;

namespace Vixen.Module.SequenceType {
	[DataContract(Namespace = "")]
	public class SequenceTypeDataModelBase : ModuleDataModelBase, ISequenceTypeDataModel {
		public SequenceTypeDataModelBase() {
			Media = new MediaCollection();
			LocalDataSet = new ModuleLocalDataSet();
			_InitDataStreams();
		}

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public TimeSpan Length { get; set; }

		[DataMember]
		private SelectedTimingProviderSurrogate _selectedTimingProviderSurrogate;
		public SelectedTimingProvider SelectedTimingProvider { get; set; }

		[DataMember]
		private IModuleDataModel[] _dataModels;
		public ModuleLocalDataSet LocalDataSet { get; set; }

		[DataMember]
		private MediaSurrogate[] _mediaSurrogates;
		public List<IMediaModuleInstance> Media { get; set; }

		[DataMember]
		private EffectNodeSurrogate[] _effectNodeSurrogates;
		public DataStream EffectData { get; set; }

		[DataMember]
		private FilterNodeSurrogate[] _filterNodeSurrogates;
		public DataStream SequenceFilterData { get; set; }

		public DataStreams DataStreams { get; private set; }

		public override IModuleDataModel Clone() {
			throw new NotImplementedException();
			SequenceTypeDataModelBase newInstance = new SequenceTypeDataModelBase();
			newInstance.Version = Version;
			newInstance.Length = Length;
			newInstance.SelectedTimingProvider = new SelectedTimingProvider(SelectedTimingProvider.ProviderType, SelectedTimingProvider.SourceName);
			newInstance.Media = new MediaCollection(Media);
			//newInstance.EffectData = 
			//newInstance.SequenceFilterData = 

			return newInstance;
		}

		private void _InitDataStreams() {
			DataStreams = new DataStreams();
			EffectData = DataStreams.GetDataStream(null);
			SequenceFilterData = DataStreams.CreateStream("SequenceFilter");
		}

		[OnSerializing]
		void SurrogateWrite(StreamingContext context) {
			_selectedTimingProviderSurrogate = new SelectedTimingProviderSurrogate(SelectedTimingProvider);
			if(Media != null) {
				_mediaSurrogates = Media.Select(x => new MediaSurrogate(x)).ToArray();
			}
			if(LocalDataSet != null) {
				_dataModels = LocalDataSet.DataModels.ToArray();
			}
			if(EffectData != null) {
				_effectNodeSurrogates = EffectData.Select(x => new EffectNodeSurrogate((IEffectNode)x)).ToArray();
			}
			if(SequenceFilterData != null) {
				_filterNodeSurrogates = SequenceFilterData.Select(x => new FilterNodeSurrogate((ISequenceFilterNode)x)).ToArray();
			}
		}

		[OnDeserialized]
		void SurrogateRead(StreamingContext context) {
			SelectedTimingProvider = _selectedTimingProviderSurrogate.CreateSelectedTimingProvider();
			Media = new MediaCollection(_mediaSurrogates.Select(x => x.CreateMedia()));

			// Rehydrate the local module data store.
			LocalDataSet = new ModuleLocalDataSet();
			LocalDataSet.DataModels = _dataModels;

			// Rehydrate the modules.
			IEffectNode[] effectNodes = _effectNodeSurrogates.Select(x => x.CreateEffectNode()).ToArray();
			ISequenceFilterNode[] sequenceFilterNodes = _filterNodeSurrogates.Select(x => x.CreateFilterNode()).ToArray();

			// Connect them to their respective data from the data store.
			// This was previously being done by adding the data to the sequence after loading the data.
			foreach(var effectNode in effectNodes) {
				LocalDataSet.AssignModuleInstanceData(effectNode.Effect);
			}
			foreach(var sequenceFilterNode in sequenceFilterNodes) {
				LocalDataSet.AssignModuleInstanceData(sequenceFilterNode.Filter);
			}

			// Get the modules back into their collections.
			_InitDataStreams();
			EffectData.AddData(effectNodes);
			SequenceFilterData.AddData(sequenceFilterNodes);
		}

	}
}
