﻿using Vixen.Module;
using Vixen.Services;

namespace Vixen.Sys
{
	internal class ModuleStore
	{
		public static readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		public static readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public ModuleStore()
		{
			TypeData = new ModuleStaticDataSet();
			InstanceData = new ModuleLocalDataSet();
		}

		public string LoadedFilePath { get; set; }

		public ModuleStaticDataSet TypeData { get; set; }

		public ModuleLocalDataSet InstanceData { get; set; }

		public void Save()
		{
			FileService.Instance.SaveModuleStoreFile(this);
		}
	}
}