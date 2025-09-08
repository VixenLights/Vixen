using Orc.Theming;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Vixen.Sys;
using Vixen.Sys.Managers;
using VixenApplication.SetupDisplay.ViewModels;
using VixenModules.App.Modeling;
using VixenModules.Editor.FixtureGraphics.WPF;
using WPFCommon.Extensions;
using Timer = System.Threading.Timer;

namespace VixenApplication.SetupDisplay.Views
{
	public partial class SetupDisplayWindow: Window
	{
		private readonly Timer _nodeRefreshTimer;

		/// <summary>
		/// Moving head.
		/// </summary>
		private MovingHeadWPF _movingHead = new MovingHeadWPF();

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

		#region Private Methods

		/// <summary>
		/// User control loaded event, used to draw background graphics.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e"><Event arguments/param>
		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// Draw moving head 
			DrawMovingHead((int)Math.Min(MainViewport.ActualWidth, MainViewport.ActualHeight), System.Drawing.Color.Purple, MainViewport);
		}

		#endregion

		/// <summary>
		/// Displays two moving head fixtures on the user control using the specified viewports.
		/// </summary>
		/// <param name="size">Width and height of the moving head</param>
		/// <param name="color">Color of the light beam</param>
		/// <param name="leftViewport">Left viewport to render in</param>
		/// <param name="rightViewport">Right viewport to render in</param>
		protected void DrawMovingHead(
			int size,
			Color color,		
			Viewport3D viewport)
		{
			// Configure the left moving head
			_movingHead.MovingHead.IncludeLegend = false;
			_movingHead.MovingHead.TiltAngle = 45.0;
			_movingHead.MovingHead.PanAngle = 35.0;
			_movingHead.MovingHead.BeamColorLeft = color;
			_movingHead.MovingHead.BeamColorRight = color;
			_movingHead.MovingHead.BeamLength = 20;
			_movingHead.MovingHead.Focus = 20;

			// Draw the moving head
			_movingHead.DrawFixtureNoBitmap(size, size, 1.0, 0, 0, viewport);

			(DataContext as SetupDisplayViewModel).IsThreeD = false;			
		}
	}
}
