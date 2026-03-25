using System.IO;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml;
using Vixen.Module;
using Vixen.Module.Effect;

#nullable enable

namespace VixenModules.Editor.TimedSequenceEditor
{
	/// <summary>
	/// Class to hold effect data to allow it to be placed on the clipboard and be reconstructed when later pasted
	/// </summary>
	public class EffectModelCandidate
	{
		[JsonInclude]
		private string? _moduleDataClass;
		[JsonInclude]
		private byte[]? _effectData;

		/// <summary>
		/// Initializes a new instance of the EffectModelCandidate class. This parameterless constructor is intended for use
		/// during deserialization. Do not use in code!
		/// </summary>
		/// <remarks>Some serialization frameworks require a public parameterless constructor to instantiate objects
		/// during the deserialization process. This constructor should not typically be used directly in application
		/// code.</remarks>
		public EffectModelCandidate()
		{
			
		}

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

		[JsonInclude] 
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