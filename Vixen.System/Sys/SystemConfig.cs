using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Services;
using Vixen.Sys.Attribute;
using Vixen.Sys.Output;

namespace Vixen.Sys {
	class SystemConfig {
		private string _alternateDataPath;
		private IEnumerable<Channel> _channels;
		private IEnumerable<ChannelNode> _nodes;
		private IEnumerable<IOutputDevice> _controllers;
		private IEnumerable<IOutputDevice> _previews;
		private IEnumerable<IOutputDevice> _smartControllers;
		private IEnumerable<ChannelOutputPatch> _channelPatching;
		private IEnumerable<ControllerLink> _controllerLinking;
		private List<Guid> _disabledControllers;

		[DataPath]
		static public readonly string Directory = Path.Combine(Paths.DataRootPath, "SystemData");
		public const string FileName = "SystemConfig.xml";
		static public readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public SystemConfig() {
			Identity = Guid.NewGuid();
			_disabledControllers = new List<Guid>();
			IsPreviewThreaded = true; // opt-out
			AllowFilterEvaluation = true; // opt-out
		}

		public string LoadedFilePath { get; set; }

		public Guid Identity { get; set; }

		// Doing it this way means that Channels and Nodes will never be null.
		public IEnumerable<Channel> Channels {
			get {
				if(_channels == null) {
					_channels = new Channel[0];
				}
				return _channels;
			}
			set { _channels = value; }
		}

		public IEnumerable<ChannelNode> Nodes {
			get {
				if(_nodes == null) {
					_nodes = new ChannelNode[0];
				}
				return _nodes;
			}
			set { _nodes = value; }
		}

		public IEnumerable<IOutputDevice> Controllers {
			get {
				if(_controllers == null) {
					_controllers = new IOutputDevice[0];
				}
				return _controllers;
			}
			set { _controllers = value; }
		}

		public IEnumerable<IOutputDevice> Previews {
			get {
				if(_previews == null) {
					_previews = new IOutputDevice[0];
				}
				return _previews;
			}
			set { _previews = value; }
		}

		public IEnumerable<IOutputDevice> SmartControllers {
			get {
				if(_smartControllers == null) {
					_smartControllers = new IOutputDevice[0];
				}
				return _smartControllers;
			}
			set { _smartControllers = value; }
		}

		public IEnumerable<ChannelOutputPatch> ChannelPatching {
			get {
				if(_channelPatching == null) {
					_channelPatching = new ChannelOutputPatch[0];
				}
				return _channelPatching;
			}
			set { _channelPatching = value; }
		}

		public IEnumerable<ControllerLink> ControllerLinking {
			get {
				if(_controllerLinking == null) {
					_controllerLinking = new ControllerLink[0];
				}
				return _controllerLinking;
			}
			set { _controllerLinking = value; }
		}

		public IEnumerable<IOutputDevice> DisabledControllers {
			get { return _disabledControllers.Select(x => _controllers.FirstOrDefault(y => y.Id == x)).NotNull(); }
			set { _disabledControllers = new List<Guid>(value.Select(x => x.Id)); }
		}

		public bool IsContext { get; set; }

		public bool IsPreviewThreaded { get; set; }

		public string AlternateDataPath {
			get { return _alternateDataPath; }
			set {
				Paths.DataRootPath = value;
				if(Paths.DataRootPath == value) {
					// Data root path is the path that we specified; the set
					// did not fail.
					_alternateDataPath = value;
				}
			}
		}

		public bool AllowFilterEvaluation { get; set; }

		static public SystemConfig Load(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateSystemConfigSerializer();
			ISerializationResult result = serializer.Read(filePath);
			return (SystemConfig)result.Object;
		}

		public void Save() {
			Save(LoadedFilePath ?? Path.Combine(Directory, FileName));
		}

		public void Save(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateSystemConfigSerializer();
			serializer.Write(this, filePath);
		}
	}
}
