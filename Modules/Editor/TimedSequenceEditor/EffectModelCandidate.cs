using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Vixen.Module;
using Vixen.Module.Effect;

namespace VixenModules.Editor.TimedSequenceEditor
{
	/// <summary>
	/// Class to hold effect data to allow it to be placed on the clipboard and be reconstructed when later pasted
	/// </summary>
	[Serializable]
	public class EffectModelCandidate
	{
		private readonly Type _moduleDataClass;
		private readonly MemoryStream _effectData;

		public EffectModelCandidate(IEffectModuleInstance effect)
		{
			LayerId = Guid.Empty;
			_moduleDataClass = effect.Descriptor.ModuleDataClass;
			DataContractSerializer ds = new DataContractSerializer(_moduleDataClass);

			TypeId = effect.Descriptor.TypeId;
			_effectData = new MemoryStream();
			using (XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(_effectData))
				ds.WriteObject(w, effect.ModuleData);
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan Duration { get; set; }
		public Guid TypeId { get; private set; }
		public Guid LayerId { get; set; }
		public Guid LayerTypeId { get; set; }
		public string LayerName { get; set; }

		public IModuleDataModel GetEffectData()
		{
			DataContractSerializer ds = new DataContractSerializer(_moduleDataClass);
			MemoryStream effectDataIn = new MemoryStream(_effectData.ToArray());
			using (XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(effectDataIn, XmlDictionaryReaderQuotas.Max))
				return (IModuleDataModel)ds.ReadObject(r);
		}
	}
}