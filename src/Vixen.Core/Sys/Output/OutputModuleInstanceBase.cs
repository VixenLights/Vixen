using Vixen.Module;

namespace Vixen.Sys.Output
{
	public abstract class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModule
	{
		/// <summary>
		/// If overriding this, please also override Start and Stop.
		/// </summary>
		public virtual bool IsRunning { get; private set; }

		/// <summary>
		/// If overriding this, please also override Pause and Resume.
		/// </summary>
		public virtual bool IsPaused { get; private set; }

		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		/// <summary>
		/// If overriding this, please also override Stop and IsRunning.
		/// </summary>
		public virtual void Start()
		{
			IsRunning = true;
		}

		/// <summary>
		/// If overriding this, please also override Start and IsRunning.
		/// </summary>
		public virtual void Stop()
		{
			IsRunning = false;
		}

		/// <summary>
		/// If overriding this, please also override Resume and IsPaused.
		/// </summary>
		public virtual void Pause()
		{
			IsPaused = true;
		}

		/// <summary>
		/// If overriding this, please also override Pause and IsPaused.
		/// </summary>
		public virtual void Resume()
		{
			IsPaused = false;
		}

		public virtual int UpdateInterval
		{
			get { return ((IOutputModuleDescriptor) Descriptor).UpdateInterval; }
		}

		public virtual IOutputDeviceUpdateSignaler UpdateSignaler
		{
			get { return null; }
		}
	}
}