using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Module.Media
{
	public abstract class MediaModuleInstanceBase : ModuleInstanceBase, IMediaModuleInstance,
	                                                IEqualityComparer<IMediaModuleInstance>,
	                                                IEquatable<IMediaModuleInstance>,
	                                                IEqualityComparer<MediaModuleInstanceBase>,
	                                                IEquatable<MediaModuleInstanceBase>
	{
		public abstract string MediaFilePath { get; set; }

		public abstract void LoadMedia(TimeSpan startTime);

		/// <inheritdoc />
		public abstract ITiming TimingSource { get; }
		public abstract int CurrentPlaybackDeviceIndex { get; set; }

		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		public abstract void Start();

		public abstract void Stop();

		public abstract void Pause();

		public abstract void Resume();

		public bool Equals(IMediaModuleInstance x, IMediaModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IMediaModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(MediaModuleInstanceBase x, MediaModuleInstanceBase y)
		{
			return Equals(x as IMediaModuleInstance, y as IMediaModuleInstance);
		}

		public int GetHashCode(MediaModuleInstanceBase obj)
		{
			return GetHashCode(obj as IMediaModuleInstance);
		}

		public bool Equals(MediaModuleInstanceBase other)
		{
			return Equals(other as IMediaModuleInstance);
		}
	}
}