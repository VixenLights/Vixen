using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.App.VirtualEffect
{
	public class VirtualEffectLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<Guid, VirtualEffect>>
	{
		private VirtualEffectLibraryData _data = new VirtualEffectLibraryData();

		public override Vixen.Sys.IApplication Application
		{
			set { }
		}

		public override void Loading()
		{
		}

		public override void Unloading()
		{
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as VirtualEffectLibraryData; }
		}

		#region VirtualEffect public members

		public Dictionary<Guid, VirtualEffect> Library
		{
			get { return _data.Library; }
		}

		public void addEffect(Guid virtualEffectGuid, String Name, Guid EffectGuid, object[] parameters)
		{
			Library.Add(virtualEffectGuid, new VirtualEffect(Name, EffectGuid, parameters));
		}

		public void removeEffect(Guid virtualEffectGuid)
		{
			Library.Remove(virtualEffectGuid);
		}

		public VirtualEffect GetVirtualEffect(Guid virtualEffectId)
		{
			if (this.ContainsEffect(virtualEffectId)) {
				return Library[virtualEffectId];
			}
			else {
				return null;
			}
		}

		public bool ContainsEffect(Guid effectId)
		{
			return Library.ContainsKey(effectId);
		}

		#endregion

		public IEnumerator<KeyValuePair<Guid, VirtualEffect>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}
	}
}