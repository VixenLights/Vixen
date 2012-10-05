using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Execution;
using Vixen.Services;
using Vixen.Sys;

namespace BaseSequence {
	public class Sequence : ISequence {
		/// <summary>
		/// Use this to set the sequence's length when the sequence is untimed.
		/// </summary>
		static public readonly TimeSpan Forever = TimeSpan.MaxValue;

		public Sequence() {
			FilePath = "";
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
		}

		public ISequenceTypeDataModel SequenceData { get; set; }

		private bool _DataListener(IEffectNode effectNode) {
			SequenceData.EffectData.AddData(effectNode);
			SequenceData.LocalDataSet.AssignModuleInstanceData(effectNode.Effect);
			// Do not cancel the event.
			return false;
		}

		public void Save(string filePath) {
			SequenceService.Instance.Save(this, filePath);
		}

		public void Save() {
			Save(FilePath);
		}

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public TimeSpan Length {
			get { return SequenceData.Length; }
			set { SequenceData.Length = value; }
		}

		public string FilePath { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(IEffectNode effectNode) {
			InsertDataListener.InsertData(effectNode);
		}

		public void InsertData(IEnumerable<IEffectNode> effectNodes) {
			InsertDataListener.InsertData(effectNodes);
		}

		public bool RemoveData(IEffectNode effectNode) {
			return SequenceData.EffectData.RemoveData(effectNode);
		}

		#region IHasMedia
		public void AddMedia(IEnumerable<IMediaModuleInstance> modules) {
			foreach(IMediaModuleInstance module in modules) {
				AddMedia(module);
			}
		}

		public void AddMedia(IMediaModuleInstance module) {
			SequenceData.LocalDataSet.AssignModuleInstanceData(module);
			SequenceData.Media.Add(module);
		}

		public IMediaModuleInstance AddMedia(string filePath) {
			IMediaModuleInstance media = MediaService.Instance.GetMedia(filePath);
			if(media != null) {
				AddMedia(media);
			}
			return media;
		}

		public bool RemoveMedia(IMediaModuleInstance module) {
			SequenceData.LocalDataSet.RemoveModuleInstanceData(module);
			return SequenceData.Media.Remove(module);
		}

		public IEnumerable<IMediaModuleInstance> GetAllMedia() {
			return SequenceData.Media;
		}

		public void ClearMedia() {
			SequenceData.Media.Clear();
		}
		#endregion

		#region IHasSequenceFilters
		public void AddSequenceFilters(IEnumerable<ISequenceFilterNode> filterNodes) {
			foreach(SequenceFilterNode filterNode in filterNodes) {
				AddSequenceFilter(filterNode);
			}
		}

		public void AddSequenceFilter(ISequenceFilterNode sequenceFilterNode) {
			SequenceData.LocalDataSet.AssignModuleInstanceData(sequenceFilterNode.Filter);
			SequenceData.SequenceFilterData.AddData(sequenceFilterNode);
		}

		public bool RemoveSequenceFilter(ISequenceFilterNode sequenceFilterNode) {
			SequenceData.LocalDataSet.RemoveModuleInstanceData(sequenceFilterNode.Filter);
			return SequenceData.SequenceFilterData.RemoveData(sequenceFilterNode);
		}

		public void ClearSequenceFilters() {
			foreach(ISequenceFilterNode filterNode in GetAllSequenceFilters()) {
				RemoveSequenceFilter(filterNode);
			}
		}
		#endregion

		public SelectedTimingProvider SelectedTimingProvider {
			get { return SequenceData.SelectedTimingProvider; }
			set { SequenceData.SelectedTimingProvider = value; }
		}

		public ITiming GetTiming() {
			TimingProviders timingProviders = new TimingProviders(this);
			return timingProviders.GetTimingSource(SelectedTimingProvider);
		}

		public IEnumerable<ISequenceFilterNode> GetAllSequenceFilters() {
			return SequenceData.SequenceFilterData.Cast<ISequenceFilterNode>();
		}

		public override string ToString() {
			return Name;
		}
	}
}
