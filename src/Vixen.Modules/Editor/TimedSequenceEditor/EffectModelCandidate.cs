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
		private readonly string _moduleDataClass;
		private readonly byte[] _effectData;

		public EffectModelCandidate(IEffectModuleInstance effect)
		{
			LayerId = Guid.Empty;
			_moduleDataClass = effect.Descriptor.ModuleDataClass.AssemblyQualifiedName;
			DataContractSerializer ds = new DataContractSerializer(Type.GetType(_moduleDataClass));

			TypeId = effect.Descriptor.TypeId;
			MemoryStream memoryStream = new MemoryStream();
			using (XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(memoryStream))
				ds.WriteObject(w, effect.ModuleData);

			_effectData = memoryStream.ToArray();
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan Duration { get; set; }
		public Guid TypeId { get; private set; }
		public Guid LayerId { get; set; }
		public Guid LayerTypeId { get; set; }
		public string LayerName { get; set; }

		public IModuleDataModel GetEffectData()
		{
			DataContractSerializer ds = new DataContractSerializer(Type.GetType(_moduleDataClass));
			MemoryStream effectDataIn = new MemoryStream(_effectData);
			using (XmlDictionaryReader r =
			       XmlDictionaryReader.CreateBinaryReader(effectDataIn, XmlDictionaryReaderQuotas.Max))
			{
				return (IModuleDataModel)ds.ReadObject(r);
			}
		}
	}
}