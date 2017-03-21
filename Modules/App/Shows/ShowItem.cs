using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Vixen.Extensions;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.Shows
{
	public enum ShowItemType
	{
		All,
		Startup,
		Background,
		Sequential,
		Input,
		Shutdown
	}

	public enum ActionType
	{
		//Show,
		Launch,
		Sequence,
		//WebPage,
		Pause
	}

	[DataContract,
	Serializable]
	public class ShowItem
	{
		public ShowItem(ShowItemType showType, string itemName, Guid currenShowID)
		{
			ItemType = showType;
			Name = itemName;
			CurrentShowID = CurrentShowID;
		}

		[DataMember]
		public ShowItemType ItemType { get; set; }
		[DataMember]
		public ActionType Action { get; set; }
		[DataMember]
		public string Name { get; set; }

		// Sequence
		/// <summary>
		/// The path to the sequence relative to the profile sequence folder. This is now a private holder.
		/// </summary>
		[DataMember]
		private string Sequence_FileName { get; set; }

		/// <summary>
		/// Full path to the sequence in the profile. This should be contained in the profile sequence folder.
		/// </summary>
		public string SequencePath
		{
			get
			{
				return SequenceService.SequenceDirectory + Path.DirectorySeparatorChar + Sequence_FileName;
			}
			set
			{
				if (!value.StartsWith(SequenceService.SequenceDirectory))
				{
					throw new ArgumentException("Path is not rooted in the profile sequence folder.");
				}
				FileSystemInfo sequenceDirectory = new DirectoryInfo(SequenceService.SequenceDirectory);
				FileSystemInfo showPath = new FileInfo(value);
				Sequence_FileName = sequenceDirectory.GetRelativePathTo(showPath);
			}
		}

		// Launch
		[DataMember(EmitDefaultValue = false)]
		public string Launch_ProgramName { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public string Launch_CommandLine { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public bool Launch_ShowCommandWindow { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public bool Launch_WaitForExit { get; set; }

		// Website
		[DataMember(EmitDefaultValue = false)]
		public string Website_URL { get; set; }

		// Pause
		[DataMember(EmitDefaultValue = false)]
		public int Pause_Seconds { get; set; }

		// Show
		[DataMember(EmitDefaultValue = false)]
		public Guid Show_ShowID;
		[DataMember(EmitDefaultValue = false)]
		public bool Show_StopCurrentShow;

		[DataMember]
		public int ItemOrder { get; set; }

		public Guid CurrentShowID { get; set; }

		[NonSerialized]
		TypeEditorBase currentEditor = null;
		public TypeEditorBase Editor
		{
			get
			{
				switch (Action)
				{
					case ActionType.Sequence:
						currentEditor = new SequenceTypeEditor(this);
						break;
					case ActionType.Launch:
						currentEditor = new LaunchTypeEditor(this);
						break;
					//case ActionType.WebPage:
					//	currentEditor = new WebPageTypeEditor(this);
					//	break;
					case ActionType.Pause:
						currentEditor = new PauseTypeEditor(this);
						break;
					//case ActionType.Show:
					//	currentEditor = new ShowTypeEditor(this, CurrentShowID);
					//	break;
				}
				return currentEditor;
			}
			set
			{
				currentEditor = value;
			}
		}

		public static bool diableShowAction;

		[NonSerialized]
		public Action currentAction = null;
		public Action GetAction()
		{
			if (currentAction == null)
			{
				switch (Action)
				{
					case ActionType.Sequence:
						currentAction = new SequenceAction(this);
						break;
					case ActionType.Launch:
						currentAction = new LaunchAction(this);
						break;
					//case ActionType.WebPage:

					//	break;
					case ActionType.Pause:
						currentAction = new PauseAction(this);
						break;
					//case ActionType.Show:

					//	break;
				}
			}
			return currentAction;
		}

		public object Clone()
		{
			return ObjectCopier.Clone(this);
		}

		public void ClearAction()
		{
			if (currentAction != null)
			{
				currentAction.Dispose();
				currentAction = null;
			}
			currentAction = null;
		}
	}
}
