using Orc.Theming;
using System.Diagnostics;
using System.Windows;
using Vixen.Sys;
using Vixen.Sys.Managers;
using VixenApplication.SetupDisplay.ViewModels;
using VixenModules.App.Modeling;
using WPFCommon.Extensions;
using Timer = System.Threading.Timer;

namespace VixenApplication.SetupDisplay.Views
{
	public partial class SetupDisplayWindow: Window
	{
		private readonly Timer _nodeRefreshTimer;
        public SetupDisplayWindow()
		{
            ThemeManager.Current.SynchronizeTheme();
			InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            DataContext = new SetupDisplayViewModel();
            _nodeRefreshTimer = new Timer(Timer_Elapsed, null, 2000, Timeout.Infinite);
            NodeManager.NodesChanged += NodeManager_NodesChanged;
            ElementTree.ExportDiagram = ExportWireDiagram;
		}

        private void Timer_Elapsed(object? state)
        {
	        if (ElementTree.InvokeRequired)
	        {
		        ElementTree.Invoke(ElementTree.PopulateNodeTree);
	        }
			Debug.WriteLine("Refreshed");
		}

		private void NodeManager_NodesChanged(object? sender, EventArgs e)
		{
			_nodeRefreshTimer.Change(2000, Timeout.Infinite);
		}

		private void ExportWireDiagram(ElementNode node, bool flip = false)
		{
			ElementModeling.ElementsToSvg(node, flip);
		}
	}
}
