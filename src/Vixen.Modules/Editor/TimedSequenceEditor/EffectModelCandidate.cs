using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Vixen.Annotations;
using Vixen.Module;
using Vixen.Module.Effect;

#nullable enable

namespace VixenModules.Editor.TimedSequenceEditor
{
	/// <summary>
	/// Class to hold effect data to allow it to be placed on the clipboard and be reconstructed when later pasted
	/// </summary>
	[Serializable]
	public class EffectModelCandidate
	{
		private readonly string? _moduleDataClass;
		private readonly byte[]? _effectData;

		public EffectModelCandidate(IEffectModuleInstance effect)
		{
			LayerId = Guid.Empty;
			LayerName = String.Empty;
			
			_moduleDataClass = effect.Descriptor.ModuleDataClass.AssemblyQualifiedName;

			// If a valid module data class name was retrieved then...
			if (!string.IsNullOrEmpty(_moduleDataClass))
			{
				Type? moduleDataType = Type.GetType(_moduleDataClass);

				if (moduleDataType != null)
				{
					DataContractSerializer ds = new DataContractSerializer(moduleDataType);

					TypeId = effect.Descriptor.TypeId;
					MemoryStream memoryStream = new MemoryStream();
					using (XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(memoryStream))
					{
						ds.WriteObject(w, effect.ModuleData);
					}

					_effectData = memoryStream.ToArray();
				}
			}
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan Duration { get; set; }
		public Guid TypeId { get; private set; }
		public Guid LayerId { get; set; }
		public Guid LayerTypeId { get; set; }
		public string LayerName { get; set; }

		
		public IModuleDataModel? GetEffectData() 
		{
			IModuleDataModel? effectData = null;

			// If the efffect module data class name has been captured then...
			if (!string.IsNullOrEmpty(_moduleDataClass))
			{
				Type? effectDataType = Type.GetType(_moduleDataClass);

				if (effectDataType != null)
				{
					DataContractSerializer ds = new DataContractSerializer(effectDataType);

					if (_effectData != null)
					{
						MemoryStream effectDataIn = new MemoryStream(_effectData);

						using (XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(effectDataIn, XmlDictionaryReaderQuotas.Max))
						{
							effectData = (IModuleDataModel?)ds.ReadObject(r);
						}
					}
				}
			}

			return effectData;
		}
	}
}