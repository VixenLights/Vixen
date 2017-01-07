using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Setup
{
	internal interface ISetupControllersControl
	{
		event EventHandler<ControllerSelectionEventArgs> ControllerSelectionChanged;
		// TODO: should parameterize what controllers have changed...
		event EventHandler ControllersChanged;

		ControllersAndOutputsSet SelectedControllersAndOutputs { get; set; }

		Control SetupControllersControl { get; }
		DisplaySetup MasterForm { get; set; }

		void UpdatePatching();

		void UpdateScrollPosition();
	}

	public class ControllerSelectionEventArgs : EventArgs
	{
		public ControllersAndOutputsSet ControllersAndOutputs;

		public ControllerSelectionEventArgs(ControllersAndOutputsSet controllersAndOutputs)
		{
			ControllersAndOutputs = controllersAndOutputs;
		}
	}

	public class ControllersAndOutputsSet : Dictionary<IControllerDevice, HashSet<int>>
	{
	}


	// ~~~~~~~  This was one way I was thinking of doing it; now that it's a treeview,
	//			it makes more sense to just track the individual outputs
	//public class ControllerChannelsEventArgs : EventArgs
	//{
	//    public List<ControllerChannelRange> ControllerChannelRanges;

	//    public ControllerChannelsEventArgs(IEnumerable<ControllerChannelRange> ranges)
	//    {
	//        ControllerChannelRanges = new List<ControllerChannelRange>(ranges);
	//    }
	//}

	//public class ControllerChannelRange
	//{
	//    public IOutputDevice Controller;
	//    public int RangeLow;
	//    public int RangeHigh;
	//}

}
