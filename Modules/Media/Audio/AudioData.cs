using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Module.Media;
using System.Reflection;
using System.IO;

namespace VixenModules.Media.Audio
{
	[DataContract]
	public class AudioData : ModuleDataModelBase
	{
		[DataMember]
		public string FilePath
		{
			get {

				if (!System.IO.File.Exists(filePath) && !string.IsNullOrWhiteSpace(relativeAudioPath)) {
					filePath =  System.IO.Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName, relativeAudioPath);
				}

				return filePath; 
			
			}
			set
			{
				filePath=value;

				if (System.IO.File.Exists(filePath) && string.IsNullOrWhiteSpace(relativeAudioPath))
					relativeAudioPath=	Vixen.Utility.PathUtility.MakeRelativePath(new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName, filePath);
			}
		}
		string filePath;
		//    <d2p1:FilePath>C:\Users\gizmo_000\Music\Christmas Playlist\Glee - Deck the Rooftop.mp3</d2p1:FilePath>
  
		[DataMember]
		public string RelativeAudioPath { get { return relativeAudioPath; } set { relativeAudioPath = value; } }
		string relativeAudioPath;

		public override IModuleDataModel Clone()
		{
			AudioData result = new AudioData();
			result.FilePath = FilePath;
			return result;
		}
	}
}