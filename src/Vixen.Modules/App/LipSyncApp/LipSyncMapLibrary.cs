﻿using Common.Controls;
using NLog;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.LipSyncApp
{
	public class LipSyncMapLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<string, LipSyncMapData>>
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private LipSyncMapStaticData _staticData;
		static int uniqueKeyIndex = 0;

		public override void Loading()
		{
			MigrateMapsToProperties();
		}

		public override void Unloading()
		{
		}

		public override Vixen.Sys.IApplication Application
		{
			set { }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _staticData; }
			set { _staticData = value as LipSyncMapStaticData; }
		}

		private LipSyncMapData _defaultMap;

		public LipSyncMapData DefaultMapping
		{
			get
			{
				
				if (_defaultMap == null)
				{
					foreach (LipSyncMapData dataItem in Library.Values)
					{
						if (dataItem.IsDefaultMapping)
						{
							_defaultMap = dataItem;
							break;
						}
					}

					if (_defaultMap == null)
					{
						_defaultMap = Library.FirstOrDefault().Value;
					}
				}
				return _defaultMap;
			}

			set
			{
				_defaultMap = value;
				foreach (LipSyncMapData dataItem in Library.Values)
				{
					dataItem.IsDefaultMapping = false;
				}
				_defaultMap.IsDefaultMapping = true;
			}
		}

		
		public string DefaultMappingName
		{
			get
			{
				return DefaultMapping!=null?DefaultMapping.LibraryReferenceName:string.Empty;
			}

			set
			{
				string newDefaultName = (string)value;

				if (_staticData.Library.ContainsKey(newDefaultName))
				{
					DefaultMapping = _staticData.Library[newDefaultName];
				}
			}
		}

		public bool IsDefaultMapping(string compareName)
		{
			return DefaultMapping.LibraryReferenceName.Equals(compareName);
		}

		public Dictionary<string, LipSyncMapData> Library
		{
			get { return _staticData.Library; }
		}

		public IEnumerator<KeyValuePair<string, LipSyncMapData>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		public bool Contains(string name)
		{
			return Library.ContainsKey(name);
		}

		public LipSyncMapData GetMapping(string name)
		{
			if (Library.ContainsKey(name))
				return Library[name];
			else
				return null;
		}

		public string AddMapping(bool insertNew, string name, LipSyncMapData mapping, bool isMatrix)
		{
			string mapName = name;

			if (insertNew)
			{
				if (string.IsNullOrWhiteSpace(mapName) == true)
				{
					mapName = "New Map";
				}
				else
				{
					mapName = name;
				}
				while (Library.Keys.Contains(mapName) == true)
				{
					mapName = string.Format(mapName + "({0})", ++uniqueKeyIndex);
				}
			}

			bool inLibrary = Contains(mapName);
			if (inLibrary)
			{
				Library[mapName].IsCurrentLibraryMapping = false;
			}
			mapping.IsCurrentLibraryMapping = true;
			mapping.LibraryReferenceName = mapName;
			mapping.IsMatrix = isMatrix;
			Library[mapName] = (insertNew) ? (LipSyncMapData)mapping.Clone() : mapping;

			return mapName;
		}

		public bool RemoveMapping(string name, bool removeFileData = false)
		{
			bool retVal = false;
			bool doRemove = true;
			LipSyncMapData origMapping = GetMapping(name);

			if (origMapping != null)
			{
				Library[name].IsCurrentLibraryMapping = false;
				if (IsDefaultMapping(name) == true)
				{
					_defaultMap = null;
				}

				if (removeFileData && origMapping.IsMatrix)
				{
					LipSyncMapMatrixEditor editor = new LipSyncMapMatrixEditor(origMapping);
					doRemove = editor.RemoveMappingFiles();
				}

				if (doRemove)
				{
					retVal = Library.Remove(name);
				}
			}
			return retVal;
		}

		private bool EditMatrixMapping(string name, LipSyncMapData newMapping)
		{
			bool retVal = false;
			bool doRemove = true;

			LipSyncMapMatrixEditor editor = new LipSyncMapMatrixEditor(newMapping);
			
			var parent = System.Windows.Forms.Application.OpenForms.Cast<Form>().FirstOrDefault(x => x.Name.Equals("LipSyncMapSelector"));
			if (editor.ShowDialog(parent) == DialogResult.OK)
			{
				if ((name.Equals(editor.LibraryMappingName) == false) &&
					(this.Contains(editor.LibraryMappingName) == true))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Overwrite existing " +
							editor.LibraryMappingName + " mapping?",
							"Map exists", true, false);
					messageBox.ShowDialog();

					doRemove = (messageBox.DialogResult == DialogResult.OK) ? true : false;
				}

				if (doRemove == true)
				{
					RemoveMapping(name);
				}

				AddMapping(!doRemove, editor.LibraryMappingName, editor.MapData, true);
				retVal = true;
			}

			return retVal;
		}

		public bool RenameLibraryMapping(string oldName, string newName)
		{
			bool retVal = false;
			LipSyncMapData origMapping = GetMapping(oldName);

			if (origMapping != null)
			{
				LipSyncMapData newMapping = new LipSyncMapData(origMapping);
				if ((newMapping != null) && (newName != ""))
				{
					string newTmpName = CloneLibraryMapping(oldName,newName);
					RemoveMapping(oldName, true);
					retVal = true;
				}
			}
			return retVal;
		}

		public string CloneLibraryMapping(string name, string newName = null)
		{
			bool success = false;
			string newTmpName = (newName == null) ? name : newName;

			LipSyncMapData origMapping = GetMapping(name);

			if (origMapping != null)
			{
				LipSyncMapData newMapping = new LipSyncMapData(origMapping);
				newName = AddMapping(true, newTmpName, newMapping, newMapping.IsMatrix);

				if (newName != "")
				{
					if (newMapping.IsMatrix)
					{
						LipSyncMapMatrixEditor editor = new LipSyncMapMatrixEditor(origMapping);
						success = editor.CloneMappingFiles(newName);
					}
				}
			}
			return (success) ? newName : "";
		}

		public bool EditLibraryMapping(string name)
		{
			bool retVal = false;
			LipSyncMapData origMapping = GetMapping(name);

			if (origMapping != null)
			{
				LipSyncMapData newMapping = new LipSyncMapData(origMapping);

				if (newMapping.IsMatrix == false)
				{
					Logging.Error($"Trying to edit a string map: {name}!");
				}
				else
				{
					retVal = EditMatrixMapping(name, newMapping);
				}
			}
			return retVal;
		}

		public bool MigrateMapsToProperties()
		{
			bool migrated = false;
			if (_staticData.NeedsStringMapMigration)
			{
				_staticData.MigrateMaps();
				migrated = true;
			}

			return migrated;
		}

	}
}
