using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	public enum EffectGroups
	{
		Basic,
		Pixel,
		Device
	}

	[Serializable]
	public abstract class EffectModuleDescriptorBase : ModuleDescriptorBase, IEffectModuleDescriptor,
													   IEqualityComparer<IEffectModuleDescriptor>,
													   IEquatable<IEffectModuleDescriptor>,
													   IEqualityComparer<EffectModuleDescriptorBase>,
													   IEquatable<EffectModuleDescriptorBase> {
		protected EffectModuleDescriptorBase() {
			PropertyDependencies = new Guid[0];
		}
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public abstract EffectGroups EffectGroup { get; }

		public virtual bool SupportsMedia
		{
			get
			{
				return false;
			}
		}

		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string EffectName { get; }

		public abstract ParameterSignature Parameters { get; }

		public virtual Guid[] PropertyDependencies { get; private set; }

		public virtual Image GetRepresentativeImage(int desiredWidth, int desiredHeight) {
			try {

				//Default to Null image
				var resources = this.Assembly.GetManifestResourceNames().ToList();
				//resources.ToList().ForEach(a => Console.WriteLine(a)); 
				int maxDimension = Math.Max(desiredWidth, desiredHeight);
				if (maxDimension <= 16) {
					var resName = resources.FirstOrDefault(r => r.ContainsString(".Effect16.", StringComparison.CurrentCultureIgnoreCase));
					if (!string.IsNullOrWhiteSpace(resName))
						return Image.FromStream(this.Assembly.GetManifestResourceStream(resName));
					return null;
				}
				if (maxDimension <= 48) {
					var resName = resources.FirstOrDefault(r => r.ContainsString(".Effect48.", StringComparison.CurrentCultureIgnoreCase));
					if (!string.IsNullOrWhiteSpace(resName))
						return Image.FromStream(this.Assembly.GetManifestResourceStream(resName));
					return null;
				}
				else {
					var resName = resources.FirstOrDefault(r => r.ContainsString(".Effect64.", StringComparison.CurrentCultureIgnoreCase));
					if (!string.IsNullOrWhiteSpace(resName))
						return Image.FromStream(this.Assembly.GetManifestResourceStream(resName));
					return null;
				}
			}
			catch (Exception e) {
				Logging.Error(e.Message, e);
				return null;
			}
		}
		
		public bool Equals(IEffectModuleDescriptor x, IEffectModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IEffectModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(EffectModuleDescriptorBase x, EffectModuleDescriptorBase y) {
			return Equals(x as IEffectModuleDescriptor, y as IEffectModuleDescriptor);
		}

		public int GetHashCode(EffectModuleDescriptorBase obj) {
			return GetHashCode(obj as IEffectModuleDescriptor);
		}

		public bool Equals(EffectModuleDescriptorBase other) {
			return Equals(other as IEffectModuleDescriptor);
		}
	}
}