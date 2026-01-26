using System.Diagnostics;
using System.Windows;

using Common.Controls;

using Orc.Theming;

using Vixen.Sys;
using Vixen.Sys.Managers;

using VixenApplication.SetupDisplay.ViewModels;

using VixenModules.App.Modeling;

using WPFCommon.Extensions;

using Timer = System.Threading.Timer;


namespace VixenApplication.SetupDisplay.Views
{
	/// <summary>
	/// Code Behind file for the SetupDisplayWindow.
	/// This class handles events for the Setup Display Window.
	/// </summary>
	public partial class SetupDisplayWindow : Window
	{
		#region Fields

		private readonly Timer _nodeRefreshTimer;
				
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
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

		#endregion

		#region Private Methods

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
						
		#endregion
	}
}
