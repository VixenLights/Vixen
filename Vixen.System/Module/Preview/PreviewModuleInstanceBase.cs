using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Preview {
	abstract public class PreviewModuleInstanceBase : OutputModuleInstanceBase, IPreviewModuleInstance, IEqualityComparer<IPreviewModuleInstance>, IEquatable<IPreviewModuleInstance>, IEqualityComparer<PreviewModuleInstanceBase>, IEquatable<PreviewModuleInstanceBase> {
		protected abstract IThreadBehavior ThreadBehavior { get; }

		public override void Start() {
			ThreadBehavior.Start();
		}

		public override void Stop() {
			ThreadBehavior.Stop();
		}

		public override bool IsRunning {
			get { return ThreadBehavior.IsRunning; }
		}

		abstract public IDataPolicy DataPolicy { get; }

		abstract public void UpdateState(ChannelCommands channelCommands);

		public bool Equals(IPreviewModuleInstance x, IPreviewModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreviewModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPreviewModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PreviewModuleInstanceBase x, PreviewModuleInstanceBase y) {
			return Equals(x as IPreviewModuleInstance, y as IPreviewModuleInstance);
		}

		public int GetHashCode(PreviewModuleInstanceBase obj) {
			return GetHashCode(obj as IPreviewModuleInstance);
		}

		public bool Equals(PreviewModuleInstanceBase other) {
			return Equals(other as IPreviewModuleInstance);
		}
	}
}
