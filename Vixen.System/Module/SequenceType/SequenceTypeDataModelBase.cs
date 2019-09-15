using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Execution;
using Vixen.Module.Media;
using Vixen.Module.SequenceType.Surrogate;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Module.SequenceType
{
	[DataContract(Namespace = "")]
	public class SequenceTypeDataModelBase : ModuleDataModelBase, ISequenceTypeDataModel
	{
		public SequenceTypeDataModelBase()
		{
			Media = new MediaCollection();
			LocalDataSet = new ModuleLocalDataSet();
			SequenceLayers = new SequenceLayers();
			_InitDataStreams();
		}

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public TimeSpan Length { get; set; }

		[DataMember] private SelectedTimingProviderSurrogate _selectedTimingProviderSurrogate;
		public SelectedTimingProvider SelectedTimingProvider { get; set; }

		[DataMember]
		private IModuleDataModel[] _layerMixingFilterDataModels;

		[DataMember] private IModuleDataModel[] _dataModels;
		public ModuleLocalDataSet LocalDataSet { get; set; }

		[DataMember] private MediaSurrogate[] _mediaSurrogates;
		public List<IMediaModuleInstance> Media { get; set; }

		[DataMember] private EffectNodeSurrogate[] _effectNodeSurrogates;
		public DataStream EffectData { get; set; }

		[DataMember] private FilterNodeSurrogate[] _filterNodeSurrogates;
		public DataStream SequenceFilterData { get; set; }

		public DataStreams DataStreams { get; private set; }

		[DataMember]
		public SequenceLayers SequenceLayers { get; set; }

		[DataMember]
		private LayerMixingFilterSurrogate[] _layerMixingFilterSurrogates;

		public override IModuleDataModel Clone()
		{
			throw new NotImplementedException();
		}

		private void _InitDataStreams()
		{
			DataStreams = new DataStreams();
			EffectData = DataStreams.GetDataStream(null);
			SequenceFilterData = DataStreams.CreateStream("SequenceFilter");
		}

		[OnSerializing]
		private void SurrogateWrite(StreamingContext context)
		{
			// while saving module instance data, load the instance IDs of the modules in the sequence into a set. Then, when saving the
			// data set for all the modules, we can check if the module data is actually *being used* by anything; if not, we can remove it.
			HashSet<Guid> activeInstances = new HashSet<Guid>();

			_selectedTimingProviderSurrogate = new SelectedTimingProviderSurrogate(SelectedTimingProvider);
			if (Media != null) {
				_mediaSurrogates = Media.Select(x => new MediaSurrogate(x)).ToArray();
				Media.ForEach(x => activeInstances.Add(x.InstanceId));
			}
			if (EffectData != null) {
				_effectNodeSurrogates = EffectData.Select(x => new EffectNodeSurrogate((IEffectNode) x)).ToArray();
				foreach (IDataNode dataNode in EffectData) {
					activeInstances.Add(((IEffectNode) dataNode).Effect.InstanceId);
				}
			}

			if (SequenceLayers != null)
			{
				_layerMixingFilterSurrogates = SequenceLayers.Layers.Where(x => x.LayerMixingFilter != null).Select(x => new LayerMixingFilterSurrogate(x)).ToArray();
				ModuleLocalDataSet data = new ModuleLocalDataSet();
				foreach (var layer in SequenceLayers.Layers)
				{
					if (layer != null)
					{
						data.AssignModuleInstanceData(layer.LayerMixingFilter);
					}
				}
				_layerMixingFilterDataModels = data.DataModels.ToArray();
			}

			if (LocalDataSet != null)
			{
				_dataModels = LocalDataSet.DataModels.Where(x => activeInstances.Contains(x.ModuleInstanceId)).ToArray();
			}

			if (SequenceFilterData != null) {
				_filterNodeSurrogates = SequenceFilterData.Select(x => new FilterNodeSurrogate((ISequenceFilterNode) x)).ToArray();
			}

			
		}

		[OnDeserialized]
		private void SurrogateRead(StreamingContext context)
		{
			SelectedTimingProvider = _selectedTimingProviderSurrogate.CreateSelectedTimingProvider();
			Media = new MediaCollection(_mediaSurrogates.Select(x => x.CreateMedia()));

			// Rehydrate the local module data store.
			LocalDataSet = new ModuleLocalDataSet();
			LocalDataSet.DataModels = _dataModels;

			// Rehydrate the modules.
			var elementNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);
			IEffectNode[] effectNodes = _effectNodeSurrogates.Select(x => x.CreateEffectNode(elementNodes)).ToArray();
			// weed out effects without nodes..
			//effectNodes = effectNodes.Where(x => x.Effect.TargetNodes.Count() != 0).ToArray();

			ISequenceFilterNode[] sequenceFilterNodes = _filterNodeSurrogates.Select(x => x.CreateFilterNode()).ToArray();

			// Connect them to their respective data from the data store.
			// This was previously being done by adding the data to the sequence after loading the data.
			foreach (var effectNode in effectNodes) {
				LocalDataSet.AssignModuleInstanceData(effectNode.Effect);
			}
			foreach (var sequenceFilterNode in sequenceFilterNodes) {
				LocalDataSet.AssignModuleInstanceData(sequenceFilterNode.Filter);
			}

			if (SequenceLayers != null)
			{
				var layerMixingFilterModels = new ModuleLocalDataSet { DataModels = _layerMixingFilterDataModels };
				//Bring in the layers
				foreach (var layer in SequenceLayers.Layers)
				{
					var surrogate = _layerMixingFilterSurrogates.FirstOrDefault(x => x.LayerReferenceId == layer.Id);
					if (surrogate != null)
					{
						layer.LayerMixingFilter = surrogate.CreateLayerMixingFilter();
						layerMixingFilterModels.AssignModuleInstanceData(layer.LayerMixingFilter);
					}
					
				}
				
			}
			else
			{
				SequenceLayers = new SequenceLayers();
			}
			
			// Get the modules back into their collections.
			_InitDataStreams();
			EffectData.AddData(effectNodes);
			SequenceFilterData.AddData(sequenceFilterNodes);
			
		}
	}
}