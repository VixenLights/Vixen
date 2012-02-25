using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	class SystemContext : FilePackage {
		private const int VERSION = 1;
		private const string TEMP_DIRECTORY_NAME = "VixenContext";

		public SystemContext()
			: base(VixenSystem.SystemConfig.Identity) {
		}

		public SystemContext(Guid sourceIdentity)
			: base(sourceIdentity) {
		}

		public string ContextName { get; set; }

		public string ContextDescription { get; set; }

		public void AddFile(string filePath) {
			string destinationPath;

			if(filePath.StartsWith(Paths.BinaryRootPath)) {
				// Add file paths relative to the installation.
				destinationPath = filePath.Substring(Paths.BinaryRootPath.Length + 1);
			} else {
				// Anything not sourced from Modules goes into the target root.
				// (Currently only UserData.xml)
				destinationPath = Path.GetFileName(filePath);
			}
	
			NewContextFile contextFile = new NewContextFile(filePath, destinationPath);
			AddFile(contextFile);
		}

		public void Save(string targetFilePath) {
			FileSerializer<SystemContext> serializer = SerializerFactory.Instance.CreateSystemContextSerializer();
			serializer.Write(this, targetFilePath);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextFilePath"></param>
		/// <returns>Root of the exploded context.</returns>
		public string Explode(string contextFilePath) {
			// Get/Create a directory for the context.
			string contextRoot = Path.Combine(Path.GetTempPath(), TEMP_DIRECTORY_NAME);
			Directory.CreateDirectory(contextRoot);

			// Copy the current context to the new directory.
			int rootPathLength = Paths.BinaryRootPath.Length;
			if(!Paths.BinaryRootPath.EndsWith("\\")) rootPathLength++;

			// We want the resulting relative path to not start with a slash.
			foreach(string directory in Directory.GetDirectories(Paths.BinaryRootPath, "*.*", SearchOption.AllDirectories)) {
				string relativePath = directory.Substring(rootPathLength);
				string destinationPath = Path.Combine(contextRoot, relativePath);
				Directory.CreateDirectory(destinationPath);
			}
			foreach(string filePath in Directory.GetFiles(Paths.BinaryRootPath, "*.*", SearchOption.AllDirectories)) {
				string relativePath = filePath.Substring(rootPathLength);
				string destinationPath = Path.Combine(contextRoot, relativePath);
				File.Copy(filePath, destinationPath, true);
			}

			foreach(IPackageFileContent file in this) {
				string destinationFilePath = Path.Combine(contextRoot, file.FilePath);
				if(File.Exists(destinationFilePath)) File.Delete(destinationFilePath);
				File.WriteAllBytes(destinationFilePath, file.FileContent);
			}

			return contextRoot;
		}

		public int Version {
			get { return VERSION; }
		}

		static public SystemContext PackageSystemContext(string targetFilePath) {
			SystemContext context = new SystemContext();

			// Add the system data files.
			string systemConfigFilePath = _PrepSystemConfig();
			context.AddFile(new NewContextFile(systemConfigFilePath, Path.GetFileName(VixenSystem.SystemConfig.LoadedFilePath)));
			string moduleStoreFilePath = _PrepModuleStore();
			context.AddFile(new NewContextFile(moduleStoreFilePath, Path.GetFileName(VixenSystem.ModuleStore.LoadedFilePath)));

			// Add all binaries under the "Modules" directory.
			foreach(string moduleFilePath in Directory.GetFiles(Modules.Directory, "*.*", SearchOption.AllDirectories)) {
				context.AddFile(moduleFilePath);
			}

			return context;
		}

		static public SystemContext UnpackageSystemContext(string contextFilePath) {
			FileSerializer<SystemContext> serializer = SerializerFactory.Instance.CreateSystemContextSerializer();
			SerializationResult<SystemContext> result = serializer.Read(contextFilePath);
			return result.Object;
		}

		static private string _PrepSystemConfig() {
			// The user data needs a flag set to state that it's a context copy and
			// therefore should be the one used, not the one in the user's data branch.

			// Flush the system data.
			VixenSystem.SystemConfig.Save();
			
			// Load the system config into a new instance.
			FileSerializer<SystemConfig> serializer = SerializerFactory.Instance.CreateSystemConfigSerializer();
			SerializationResult<SystemConfig> result = serializer.Read(VixenSystem.SystemConfig.LoadedFilePath);
			SystemConfig contextUserData = result.Object;

			// Set the context flag.
			contextUserData.IsContext = true;
			
			// Save to a temp file.
			string tempFilePath = Path.GetTempFileName();
			serializer.Write(contextUserData, tempFilePath);

			return tempFilePath;
		}

		static private string _PrepModuleStore() {
			VixenSystem.ModuleStore.Save();
			return VixenSystem.ModuleStore.LoadedFilePath;
		}

		#region NewContextFile
		class NewContextFile : IPackageFileContent {
			private string _sourceFilePath;

			public NewContextFile(string filePath, string destinationPath) {
				if(string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
				if(destinationPath == null) throw new ArgumentNullException("destinationPath");
				if(!File.Exists(filePath)) throw new InvalidOperationException(filePath + "does not exist.");

				_sourceFilePath = filePath;
				FilePath = destinationPath;
			}

			public string FilePath { get; private set; }

			public byte[] FileContent {
				get { return File.ReadAllBytes(_sourceFilePath); }
			}
		}
		#endregion
	}
}
