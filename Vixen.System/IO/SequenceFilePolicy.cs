namespace Vixen.IO {
	abstract class SequenceFilePolicy : IFilePolicy {
		virtual public void Write() {
			WriteLength();
			WriteTimingSource();
			WriteModuleData();
			WriteEffectNodes();
			WriteFilterNodes();
		}

		protected abstract void WriteLength();
		protected abstract void WriteTimingSource();
		protected abstract void WriteModuleData();
		protected abstract void WriteEffectNodes();
		protected abstract void WriteFilterNodes();

		virtual public void Read() {
			ReadLength();
			ReadTimingSource();
			ReadModuleData();
			ReadEffectNodes();
			ReadFilterNodes();
			ReadMediaData();
		}

		protected abstract void ReadLength();
		protected abstract void ReadTimingSource();
		protected abstract void ReadModuleData();
		protected abstract void ReadEffectNodes();
		protected abstract void ReadFilterNodes();
		protected abstract void ReadMediaData();

		public abstract int GetVersion();
	}
}
