using NLog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VixenApplication.Setup
{
	public partial class DisplaySetup
	{
		private static readonly Logger CloseDiagnosticsLogging = LogManager.GetCurrentClassLogger();

		private const int WmClose = 0x0010;
		private const int WmDestroy = 0x0002;
		private const int WmNcDestroy = 0x0082;
		private const uint GwOwner = 4;
		private const uint GaRoot = 2;
		private const uint GaRootOwner = 3;
		private static readonly CloseDiagnosticsDisposalMode CloseDiagnosticsDisposalExperiment = CloseDiagnosticsDisposalMode.ControllerTreeClearNodesThenDispose;

		private readonly Guid _closeDiagnosticsRunId = Guid.NewGuid();
		private readonly long _closeDiagnosticsStartTimestamp = Stopwatch.GetTimestamp();
		private readonly List<CloseDiagnosticMarker> _closeDiagnosticMarkers = new(512);
		private readonly List<HandleCensus> _handleCensuses = new(6);
		private readonly List<TopLevelWindowInventory> _topLevelWindowInventories = new(32);
		private readonly List<ControllerTreeNodeCensus> _controllerTreeNodeCensuses = new(2);
		private long _closeDiagnosticsPreviousTimestamp;
		private int _closeDiagnosticsSequenceNumber;

		/// <inheritdoc />
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			CaptureUiThreadTopLevelWindowInventory();
			AttachManagedControlHandleDestroyedHandlers();
			BeginInvoke(new MethodInvoker(CaptureHandleCensus));
		}

		/// <inheritdoc />
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			RecordCloseDiagnosticMarker("OnFormClosing before base");
			base.OnFormClosing(e);
			RecordCloseDiagnosticMarker("OnFormClosing after base");
		}

		/// <inheritdoc />
		protected override void WndProc(ref Message m)
		{
			if (IsCloseDiagnosticMessage(m.Msg))
			{
				RecordCloseDiagnosticMarker(GetWndProcPhaseName(m.Msg, "before base"));
			}

			base.WndProc(ref m);

			if (IsCloseDiagnosticMessage(m.Msg))
			{
				RecordCloseDiagnosticMarker(GetWndProcPhaseName(m.Msg, "after base"));
			}
		}

		/// <inheritdoc />
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			RecordCloseDiagnosticMarker("OnFormClosed before base");
			base.OnFormClosed(e);
			RecordCloseDiagnosticMarker("OnFormClosed after base");
		}

		/// <inheritdoc />
		protected override void OnHandleDestroyed(EventArgs e)
		{
			RecordCloseDiagnosticMarker("OnHandleDestroyed before base");
			base.OnHandleDestroyed(e);
			RecordCloseDiagnosticMarker("OnHandleDestroyed after base");
		}

		internal void RecordShowDialogAsyncReturnedAndWriteDiagnostics()
		{
			RecordCloseDiagnosticMarker("ShowDialogAsync returned");
			WriteCloseDiagnostics();
		}

		private void RecordCloseDiagnosticMarker(string phaseName, ManagedControlIdentity? destroyedControl = null)
		{
			var timestamp = Stopwatch.GetTimestamp();
			var previousTimestamp = _closeDiagnosticsPreviousTimestamp;
			_closeDiagnosticsPreviousTimestamp = timestamp;

			_closeDiagnosticMarkers.Add(new CloseDiagnosticMarker(
				_closeDiagnosticsRunId,
				++_closeDiagnosticsSequenceNumber,
				phaseName,
				timestamp,
				timestamp - _closeDiagnosticsStartTimestamp,
				previousTimestamp == 0 ? 0 : timestamp - previousTimestamp,
				Environment.CurrentManagedThreadId,
				IsHandleCreated,
				IsHandleCreated ? Handle : IntPtr.Zero,
				DialogResult,
				destroyedControl?.ControlType,
				destroyedControl?.ControlName,
				destroyedControl?.OriginalHandle ?? IntPtr.Zero,
				destroyedControl?.ParentType,
				destroyedControl?.ParentName));
		}

		private void CaptureUiThreadTopLevelWindowInventory()
		{
			_topLevelWindowInventories.Clear();
			EnumThreadWindows(GetCurrentThreadId(), (windowHandle, _) =>
			{
				var control = Control.FromHandle(windowHandle);
				var threadId = GetWindowThreadProcessId(windowHandle, out var processId);
				_topLevelWindowInventories.Add(new TopLevelWindowInventory(
					windowHandle,
					GetWindowClassName(windowHandle),
					GetNativeWindowText(windowHandle),
					GetParent(windowHandle),
					GetWindow(windowHandle, GwOwner),
					GetAncestor(windowHandle, GaRoot),
					GetAncestor(windowHandle, GaRootOwner),
					processId,
					threadId,
					IsWindowVisible(windowHandle),
					IsWindowEnabled(windowHandle),
					control?.GetType().FullName ?? "<none>",
					control?.Name ?? "<none>"));
				return true;
			}, IntPtr.Zero);
		}

		private void AttachManagedControlHandleDestroyedHandlers()
		{
			foreach (var control in GetDescendantControls(this))
			{
				var parent = control.Parent;
				var identity = new ManagedControlIdentity(
					control.GetType().FullName ?? control.GetType().Name,
					control.Name,
					control.IsHandleCreated ? control.Handle : IntPtr.Zero,
					parent is null ? "<null>" : parent.GetType().FullName ?? parent.GetType().Name,
					parent?.Name ?? "<null>");

				control.HandleDestroyed += (_, _) => RecordCloseDiagnosticMarker("Managed control HandleDestroyed", identity);
			}
		}

		private void PerformCancelDisposalExperiment()
		{
			RecordCloseDiagnosticMarker("Cancel disposal experiment mode " + CloseDiagnosticsDisposalExperiment + " selected");
			switch (CloseDiagnosticsDisposalExperiment)
			{
				case CloseDiagnosticsDisposalMode.ControllerTreeOnly:
					PerformControllerTreeDisposalExperiment(clearNodesFirst: false);
					return;
				case CloseDiagnosticsDisposalMode.ControllerTreeClearNodesThenDispose:
					PerformControllerTreeDisposalExperiment(clearNodesFirst: true);
					return;
			}

			foreach (var (subtreeName, subtree) in GetCancelDisposalExperimentSubtrees())
			{
				if (subtree is null || subtree.IsDisposed)
				{
					RecordCloseDiagnosticMarker("Cancel disposal experiment " + subtreeName + " skipped");
					continue;
				}

				RecordCloseDiagnosticMarker("Cancel disposal experiment " + subtreeName + " before detach");
				subtree.Parent?.Controls.Remove(subtree);
				RecordCloseDiagnosticMarker("Cancel disposal experiment " + subtreeName + " after detach");
				RecordCloseDiagnosticMarker("Cancel disposal experiment " + subtreeName + " before disposal");
				subtree.Dispose();
				RecordCloseDiagnosticMarker("Cancel disposal experiment " + subtreeName + " after disposal");
			}
		}

		private void PerformControllerTreeDisposalExperiment(bool clearNodesFirst)
		{
			var controllerTree = _setupControllersSimple.ControllerTreeViewForCloseDiagnostics;
			if (controllerTree.IsDisposed)
			{
				RecordCloseDiagnosticMarker("Cancel controller tree disposal experiment skipped");
				return;
			}

			if (clearNodesFirst)
			{
				CaptureControllerTreeNodeCensus("before Nodes.Clear", controllerTree);
			}

			RecordCloseDiagnosticMarker("Cancel controller tree before detach");
			controllerTree.Parent?.Controls.Remove(controllerTree);
			RecordCloseDiagnosticMarker("Cancel controller tree after detach");

			if (clearNodesFirst)
			{
				RecordCloseDiagnosticMarker("Cancel controller tree before BeginUpdate");
				controllerTree.BeginUpdate();
				RecordCloseDiagnosticMarker("Cancel controller tree after BeginUpdate");
				RecordCloseDiagnosticMarker("Cancel controller tree before Nodes.Clear");
				controllerTree.Nodes.Clear();
				RecordCloseDiagnosticMarker("Cancel controller tree after Nodes.Clear");
				RecordCloseDiagnosticMarker("Cancel controller tree before EndUpdate");
				controllerTree.EndUpdate();
				RecordCloseDiagnosticMarker("Cancel controller tree after EndUpdate");
				CaptureControllerTreeNodeCensus("after Nodes.Clear", controllerTree);
			}

			RecordCloseDiagnosticMarker("Cancel controller tree before disposal");
			controllerTree.Dispose();
			RecordCloseDiagnosticMarker("Cancel controller tree after disposal");
		}

		private void CaptureControllerTreeNodeCensus(string phaseName, TreeView controllerTree)
		{
			_controllerTreeNodeCensuses.Add(new ControllerTreeNodeCensus(
				phaseName,
				controllerTree.Nodes.Count,
				controllerTree.GetNodeCount(true),
				CountOutputNodes(controllerTree.Nodes),
				controllerTree.IsHandleCreated,
				controllerTree.IsHandleCreated ? controllerTree.Handle : IntPtr.Zero));
		}

		private static int CountOutputNodes(TreeNodeCollection nodes)
		{
			var outputNodeCount = 0;
			foreach (TreeNode node in nodes)
			{
				if (node.Tag is int)
				{
					outputNodeCount++;
				}

				outputNodeCount += CountOutputNodes(node.Nodes);
			}

			return outputNodeCount;
		}

		private IEnumerable<(string SubtreeName, Control? Subtree)> GetCancelDisposalExperimentSubtrees() => CloseDiagnosticsDisposalExperiment switch
		{
			CloseDiagnosticsDisposalMode.CurrentPatching => [("CurrentPatching", _currentPatchingControl?.SetupPatchingControl)],
			CloseDiagnosticsDisposalMode.CurrentControllers => [("CurrentControllers", _currentControllersControl?.SetupControllersControl)],
			CloseDiagnosticsDisposalMode.CurrentElements => [("CurrentElements", _currentElementControl?.SetupElementsControl)],
			CloseDiagnosticsDisposalMode.AllActiveSubtrees =>
			[
				("CurrentPatching", _currentPatchingControl?.SetupPatchingControl),
				("CurrentControllers", _currentControllersControl?.SetupControllersControl),
				("CurrentElements", _currentElementControl?.SetupElementsControl)
			],
			_ => []
		};

		private void CaptureHandleCensus()
		{
			if (IsDisposed || !Visible)
			{
				return;
			}

			_handleCensuses.Clear();
			CaptureHandleCensus("DisplaySetup", this);
			CaptureHandleCensus("CurrentElements", _currentElementControl?.SetupElementsControl);
			CaptureHandleCensus("CurrentPatching", _currentPatchingControl?.SetupPatchingControl);
			CaptureHandleCensus("CurrentControllers", _currentControllersControl?.SetupControllersControl);
			CaptureHandleCensus("SetupPatchingSimple", _setupPatchingSimple);
			CaptureHandleCensus("SetupPatchingGraphical", _setupPatchingGraphical);
		}

		private void CaptureHandleCensus(string subtreeName, Control? root)
		{
			if (root is null)
			{
				return;
			}

			var descendantControls = GetDescendantControls(root).ToList();
			var nativeDescendantWindowCount = root.IsHandleCreated ? CountNativeDescendantWindows(root.Handle) : 0;
			var parent = root.Parent;
			_handleCensuses.Add(new HandleCensus(
				subtreeName,
				root.GetType().FullName ?? root.GetType().Name,
				root.Name,
				parent is null ? "<null>" : parent.GetType().FullName ?? parent.GetType().Name,
				parent?.Name ?? "<null>",
				descendantControls.Count,
				descendantControls.Count(control => control.IsHandleCreated),
				nativeDescendantWindowCount));
		}

		private static IEnumerable<Control> GetDescendantControls(Control root)
		{
			foreach (Control child in root.Controls)
			{
				yield return child;

				foreach (var descendant in GetDescendantControls(child))
				{
					yield return descendant;
				}
			}
		}

		private static int CountNativeDescendantWindows(IntPtr rootHandle)
		{
			var count = 0;
			EnumChildWindows(rootHandle, (_, _) =>
			{
				count++;
				return true;
			}, IntPtr.Zero);
			return count;
		}

		private static string GetWindowClassName(IntPtr windowHandle)
		{
			var className = new StringBuilder(256);
			GetClassName(windowHandle, className, className.Capacity);
			return className.ToString();
		}

		private static string GetNativeWindowText(IntPtr windowHandle)
		{
			var length = GetWindowTextLength(windowHandle);
			var windowText = new StringBuilder(length + 1);
			GetWindowText(windowHandle, windowText, windowText.Capacity);
			return windowText.ToString();
		}

		private void WriteCloseDiagnostics()
		{
			foreach (var marker in _closeDiagnosticMarkers)
			{
				CloseDiagnosticsLogging.Info(
					"DISPLAY_SETUP_CLOSE_DIAGNOSTICS RunId={RunId} DisposalMode={DisposalMode} SequenceNumber={SequenceNumber} Phase={Phase} RawTimestamp={RawTimestamp} ElapsedMilliseconds={ElapsedMilliseconds} PreviousMarkerDeltaMilliseconds={PreviousMarkerDeltaMilliseconds} ManagedThreadId={ManagedThreadId} IsHandleCreated={IsHandleCreated} Handle={Handle} DialogResult={DialogResult} DestroyedControlType={DestroyedControlType} DestroyedControlName={DestroyedControlName} DestroyedControlOriginalHandle={DestroyedControlOriginalHandle} DestroyedControlParentType={DestroyedControlParentType} DestroyedControlParentName={DestroyedControlParentName}",
					marker.RunId,
					CloseDiagnosticsDisposalExperiment,
					marker.SequenceNumber,
					marker.PhaseName,
					marker.RawTimestamp,
					ToMilliseconds(marker.ElapsedTimestampDelta),
					ToMilliseconds(marker.PreviousMarkerTimestampDelta),
					marker.ManagedThreadId,
					marker.IsHandleCreated,
					marker.Handle,
					marker.DialogResult,
					marker.DestroyedControlType ?? "<none>",
					marker.DestroyedControlName ?? "<none>",
					marker.DestroyedControlOriginalHandle,
					marker.DestroyedControlParentType ?? "<none>",
					marker.DestroyedControlParentName ?? "<none>");
			}

			foreach (var census in _handleCensuses)
			{
				CloseDiagnosticsLogging.Info(
					"DISPLAY_SETUP_CLOSE_DIAGNOSTICS RunId={RunId} DisposalMode={DisposalMode} CensusSubtree={CensusSubtree} ControlType={ControlType} ControlName={ControlName} ParentType={ParentType} ParentName={ParentName} ManagedDescendantControlCount={ManagedDescendantControlCount} HandleCreatedControlCount={HandleCreatedControlCount} NativeDescendantHwndCount={NativeDescendantHwndCount}",
					_closeDiagnosticsRunId,
					CloseDiagnosticsDisposalExperiment,
					census.SubtreeName,
					census.ControlType,
					census.ControlName,
					census.ParentType,
					census.ParentName,
					census.ManagedDescendantControlCount,
					census.HandleCreatedControlCount,
					census.NativeDescendantHwndCount);
			}

			foreach (var window in _topLevelWindowInventories)
			{
				CloseDiagnosticsLogging.Info(
					"DISPLAY_SETUP_CLOSE_DIAGNOSTICS RunId={RunId} DisposalMode={DisposalMode} TopLevelWindowHandle={TopLevelWindowHandle} ClassName={ClassName} WindowText={WindowText} ParentHandle={ParentHandle} OwnerHandle={OwnerHandle} RootHandle={RootHandle} RootOwnerHandle={RootOwnerHandle} ProcessId={ProcessId} ThreadId={ThreadId} IsVisible={IsVisible} IsEnabled={IsEnabled} ManagedControlType={ManagedControlType} ManagedControlName={ManagedControlName}",
					_closeDiagnosticsRunId,
					CloseDiagnosticsDisposalExperiment,
					window.Handle,
					window.ClassName,
					window.WindowText,
					window.ParentHandle,
					window.OwnerHandle,
					window.RootHandle,
					window.RootOwnerHandle,
					window.ProcessId,
					window.ThreadId,
					window.IsVisible,
					window.IsEnabled,
					window.ManagedControlType,
					window.ManagedControlName);
			}

			foreach (var census in _controllerTreeNodeCensuses)
			{
				CloseDiagnosticsLogging.Info(
					"DISPLAY_SETUP_CLOSE_DIAGNOSTICS RunId={RunId} DisposalMode={DisposalMode} ControllerTreeCensusPhase={ControllerTreeCensusPhase} RootNodeCount={RootNodeCount} TotalRecursiveNodeCount={TotalRecursiveNodeCount} OutputNodeCount={OutputNodeCount} IsHandleCreated={IsHandleCreated} Handle={Handle}",
					_closeDiagnosticsRunId,
					CloseDiagnosticsDisposalExperiment,
					census.PhaseName,
					census.RootNodeCount,
					census.TotalRecursiveNodeCount,
					census.OutputNodeCount,
					census.IsHandleCreated,
					census.Handle);
			}
		}

		private static bool IsCloseDiagnosticMessage(int message) => message is WmClose or WmDestroy or WmNcDestroy;

		private static string GetWndProcPhaseName(int message, string boundary) => message switch
		{
			WmClose => "WndProc WM_CLOSE " + boundary,
			WmDestroy => "WndProc WM_DESTROY " + boundary,
			WmNcDestroy => "WndProc WM_NCDESTROY " + boundary,
			_ => "WndProc unknown " + boundary
		};

		private static double ToMilliseconds(long timestampDelta) => timestampDelta * 1000d / Stopwatch.Frequency;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildWindowProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumThreadWindows(uint dwThreadId, EnumChildWindowProc lpfn, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		private delegate bool EnumChildWindowProc(IntPtr hWnd, IntPtr lParam);

		private sealed record CloseDiagnosticMarker(
			Guid RunId,
			int SequenceNumber,
			string PhaseName,
			long RawTimestamp,
			long ElapsedTimestampDelta,
			long PreviousMarkerTimestampDelta,
			int ManagedThreadId,
			bool IsHandleCreated,
			IntPtr Handle,
			DialogResult DialogResult,
			string? DestroyedControlType,
			string? DestroyedControlName,
			IntPtr DestroyedControlOriginalHandle,
			string? DestroyedControlParentType,
			string? DestroyedControlParentName);

		private sealed record ManagedControlIdentity(
			string ControlType,
			string ControlName,
			IntPtr OriginalHandle,
			string ParentType,
			string ParentName);

		private sealed record HandleCensus(
			string SubtreeName,
			string ControlType,
			string ControlName,
			string ParentType,
			string ParentName,
			int ManagedDescendantControlCount,
			int HandleCreatedControlCount,
			int NativeDescendantHwndCount);

		private sealed record TopLevelWindowInventory(
			IntPtr Handle,
			string ClassName,
			string WindowText,
			IntPtr ParentHandle,
			IntPtr OwnerHandle,
			IntPtr RootHandle,
			IntPtr RootOwnerHandle,
			uint ProcessId,
			uint ThreadId,
			bool IsVisible,
			bool IsEnabled,
			string ManagedControlType,
			string ManagedControlName);

		private sealed record ControllerTreeNodeCensus(
			string PhaseName,
			int RootNodeCount,
			int TotalRecursiveNodeCount,
			int OutputNodeCount,
			bool IsHandleCreated,
			IntPtr Handle);

		private enum CloseDiagnosticsDisposalMode
		{
			None,
			CurrentPatching,
			CurrentControllers,
			CurrentElements,
			AllActiveSubtrees,
			ControllerTreeOnly,
			ControllerTreeClearNodesThenDispose
		}
	}
}
