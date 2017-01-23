using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys.Attribute;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	internal class SystemConfig
	{
		public static int DEFAULT_UPDATE_INTERVAL = 50;

		private IEnumerable<Element> _elements;
		private IEnumerable<ElementNode> _nodes;
		private IEnumerable<IOutputDevice> _controllers;
		private IEnumerable<IOutputDevice> _previews;
		//private IEnumerable<IOutputDevice> _smartControllers;
		private IEnumerable<IOutputFilterModuleInstance> _filters;
		private IEnumerable<DataFlowPatch> _dataFlow;
		private List<Guid> _disabledDevicesIds;

		[DataPath] public static readonly string Directory = Path.Combine(Paths.DataRootPath, "SystemData");
		public const string FileName = "SystemConfig.xml";
		public static readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public SystemConfig()
		{
			Identity = Guid.NewGuid();
			_disabledDevicesIds = new List<Guid>();
			//*** is not in the data
			IsPreviewThreaded = true; // opt-out
			AllowFilterEvaluation = true; // opt-out
			DefaultUpdateInterval = DEFAULT_UPDATE_INTERVAL;
		}

		public string LoadedFilePath { get; set; }

		public Guid Identity { get; set; }

		// Doing it this way means that Elements and Nodes will never be null.
		public IEnumerable<Element> Elements
		{
			get
			{
				if (_elements == null) {
					_elements = new Element[0];
				}
				return _elements;
			}
			set { _elements = value; }
		}

		public IEnumerable<ElementNode> Nodes
		{
			get
			{
				if (_nodes == null) {
					_nodes = new ElementNode[0];
				}
				return _nodes;
			}
			set { _nodes = value; }
		}

		public IEnumerable<IOutputDevice> OutputControllers
		{
			get
			{
				if (_controllers == null) {
					_controllers = new IOutputDevice[0];
				}
				return _controllers;
			}
			set { _controllers = value; }
		}

		public IEnumerable<IOutputDevice> Previews
		{
			get
			{
				if (_previews == null) {
					_previews = new IOutputDevice[0];
				}
				return _previews;
			}
			set { _previews = value; }
		}

		//public IEnumerable<IOutputDevice> SmartOutputControllers
		//{
		//	get
		//	{
		//		if (_smartControllers == null) {
		//			_smartControllers = new IOutputDevice[0];
		//		}
		//		return _smartControllers;
		//	}
		//	set { _smartControllers = value; }
		//}

		//classes to handle each of these responsibilities?
		public IEnumerable<IOutputFilterModuleInstance> Filters
		{
			get
			{
				if (_filters == null) {
					_filters = new IOutputFilterModuleInstance[0];
				}
				return _filters;
			}
			set { _filters = value; }
		}

		public IEnumerable<DataFlowPatch> DataFlow
		{
			get
			{
				if (_dataFlow == null) {
					_dataFlow = new DataFlowPatch[0];
				}
				return _dataFlow;
			}
			set { _dataFlow = value; }
		}

		public IEnumerable<IOutputDevice> DisabledDevices
		{
			get { return _GetDisabledDevices(_disabledDevicesIds); }
			set { _disabledDevicesIds = new List<Guid>(value.Select(x => x.Id)); }
		}

		public IEnumerable<Guid> DisabledDeviceIds
		{
			get { return _disabledDevicesIds.ToArray(); }
			set { _disabledDevicesIds = new List<Guid>(value); }
		}

		public bool IsContext { get; set; }

		public bool IsPreviewThreaded { get; set; }

		public bool AllowFilterEvaluation { get; set; }

		public int DefaultUpdateInterval { get; set; }

		public void Save()
		{
			FileService.Instance.SaveSystemConfigFile(this);
		}

		private IEnumerable<IOutputDevice> _GetDisabledDevices(IEnumerable<Guid> deviceIds)
		{
			return deviceIds.Select(x =>
			                        _controllers.FirstOrDefault(y => y.Id == x) ??
			                        //_smartControllers.FirstOrDefault(y => y.Id == x) ??
			                        _previews.FirstOrDefault(y => y.Id == x)).Where(x => x != null);
		}
	}
}