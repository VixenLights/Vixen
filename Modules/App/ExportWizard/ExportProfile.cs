using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Vixen.Annotations;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	[DataContract]
	public class ExportProfile:ICloneable, INotifyPropertyChanged
	{
		private string _name;

		public ExportProfile(string name, string format, string sequenceFolder, string audioFolder, bool enableCompression = false)
		{
			SequenceFiles = new List<string>();
			Interval = 50;
			Format = format;
			OutputFolder = sequenceFolder;
			AudioOutputFolder = audioFolder;
			Id = Guid.NewGuid();
			Name = name;
			EnableCompression = enableCompression;
			SyncronizeControllerInfo();
		}

		private ExportProfile()
		{
			
		}

		[DataMember]
		public string Name				
		{
			get { return _name; }
			set
			{
				_name = value; 
				OnPropertyChanged();
			}
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public List<string> SequenceFiles { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public string Format { get; set; }

		[DataMember]
		public string OutputFolder { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IncludeAudio { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RenameAudio { get; set; }

		[DataMember]
		public string AudioOutputFolder { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool CreateUniverseFile { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool BackupUniverseFile { get; set; }

		[DataMember]
		public string FalconOutputFolder { get; set; }

		[DataMember]
		public List<Controller> Controllers { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool EnableCompression { get; set; }

		public bool IsFalconFormat => Format.Contains("Falcon");

		public bool IsFalcon2xFormat => Format.Contains("Falcon Player Sequence 2.6+");

		public bool IsFalconEffectFormat => Format.Equals("Falcon Player Effect");
		
		public override string ToString()
		{
			return Name;
		}

		internal void SyncronizeControllerInfo()
		{

			if (Controllers == null)
			{
				Controllers = Export.CreateControllerInfo(true);
				return;
			}
			//get our current list
			var currentControllerInfo = Export.CreateControllerInfo(false);

			Dictionary<Guid, Controller> lookup = Controllers.ToDictionary(x => x.Id);

			foreach (var controllerExportInfo in currentControllerInfo)
			{
				Controller controllerInfo;
				if (lookup.TryGetValue(controllerExportInfo.Id, out controllerInfo))
				{
					controllerExportInfo.IsActive = controllerInfo.IsActive;
					controllerExportInfo.Index = controllerInfo.Index;
				}

			}

			//re-index
			int index = 0;
			foreach (var controllerExportInfo in currentControllerInfo.OrderBy(x => x.Index))
			{
				controllerExportInfo.Index = index++;
			}

			Controllers = currentControllerInfo;
		}

		public object Clone()
		{
			var data = new ExportProfile()
			{
				Name = Name,
				Id = Id,
				SequenceFiles = SequenceFiles.ToList(),
				Controllers = Controllers.Select(x => x.Clone() as Controller).ToList(),
				Format = Format,
				Interval = Interval,
				OutputFolder = OutputFolder,
				IncludeAudio = IncludeAudio,
				RenameAudio = RenameAudio,
				AudioOutputFolder = AudioOutputFolder,
				FalconOutputFolder = FalconOutputFolder,
				CreateUniverseFile = CreateUniverseFile,
				BackupUniverseFile = BackupUniverseFile,
				EnableCompression = EnableCompression
				
			};
			return data;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			//try to map old ones to the new format. Best effort here.
			if (IsFalconFormat && string.IsNullOrEmpty(FalconOutputFolder))
			{
				if (OutputFolder.EndsWith("sequences") || OutputFolder.EndsWith("sequences/"))
				{
					var path = Directory.GetParent(OutputFolder).FullName;
					//if(Directory.Exists(path))
					//{
						FalconOutputFolder = path;
					//}
				}
			}
		}
	}
}
