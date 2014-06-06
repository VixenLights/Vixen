using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using System.Xml;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace VixenModules.Effect.Nutcracker
{
	[DataContract, Serializable]
	public class NutcrackerModuleData : ModuleDataModelBase
	{
		private NutcrackerData _nutcrackerData = null;

		public NutcrackerModuleData()
		{
			_nutcrackerData = new NutcrackerData();
		}

		[DataMember]
		public NutcrackerData NutcrackerData
		{
			get
			{
				if (_nutcrackerData == null) {
					_nutcrackerData = new NutcrackerData();
				}
				return _nutcrackerData;
			}
			set { _nutcrackerData = value; }
		}

		public override IModuleDataModel Clone()
		{
			using (var stream = new MemoryStream()) {
				var ds = new DataContractSerializer(GetType());
				using (XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(stream))
				{
					ds.WriteObject(w, this);
				}

				using (var effectDataIn = new MemoryStream(stream.ToArray()))
				{
					using (XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(effectDataIn, XmlDictionaryReaderQuotas.Max))
					{
						return (IModuleDataModel) ds.ReadObject(r);
					}
				}
			}
			
			
		}
	}
}