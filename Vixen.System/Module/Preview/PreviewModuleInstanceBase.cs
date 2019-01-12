using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Preview
{
	public abstract class PreviewModuleInstanceBase : OutputModuleInstanceBase, IPreviewModuleInstance,
	                                                  IEqualityComparer<IPreviewModuleInstance>,
	                                                  IEquatable<IPreviewModuleInstance>,
	                                                  IEqualityComparer<PreviewModuleInstanceBase>,
	                                                  IEquatable<PreviewModuleInstanceBase>
	{
		protected abstract IThreadBehavior ThreadBehavior { get; }

		//protected Vixen.Preview.PreviewElementIntentStates ElementStates { get; private set; }

		public override void Start()
		{
			ThreadBehavior.Start();
		}

		public override void Stop()
		{
			ThreadBehavior.Stop();
		}

		public override bool IsRunning
		{
			get { return ThreadBehavior.IsRunning; }
		}

		public void UpdateState(/*Vixen.Preview.PreviewElementIntentStates elementIntentStates*/)
		{
			// Get the data referenced locally so we can get off this thread if need be.
			//ElementStates = elementIntentStates;
			if(IsRunning)
			{
				ThreadBehavior.BeginInvoke(Update);
			}
		}

		/// <inheritdoc />
		public void PlayerStarted()
		{
			if (IsRunning)
			{
				ThreadBehavior.BeginInvoke(PlayerActivatedImpl);
			}
		}

		/// <inheritdoc />
		public void PlayerEnded()
		{
			if (IsRunning)
			{
				ThreadBehavior.BeginInvoke(PlayerDeactivatedImpl);
			}
		}

		protected abstract void PlayerActivatedImpl();

		protected abstract void PlayerDeactivatedImpl();

		protected abstract void Update();

		#region Equality

		public bool Equals(IPreviewModuleInstance x, IPreviewModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreviewModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IPreviewModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(PreviewModuleInstanceBase x, PreviewModuleInstanceBase y)
		{
			return Equals(x as IPreviewModuleInstance, y as IPreviewModuleInstance);
		}

		public int GetHashCode(PreviewModuleInstanceBase obj)
		{
			return GetHashCode(obj as IPreviewModuleInstance);
		}

		public bool Equals(PreviewModuleInstanceBase other)
		{
			return Equals(other as IPreviewModuleInstance);
		}

		public virtual string Name { get; set; }

		#endregion
	}
}