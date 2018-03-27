using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Xml;
using Common.Controls;
using Common.Controls.ControlsEx.ValueControls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using Vixen;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.App;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Video;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using Vixen.Sys.State;
using VixenModules.Analysis.BeatsAndBars;
using VixenModules.App.ColorGradients;
using VixenModules.Editor.EffectEditor;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Windows.Forms.Timer;
using VixenModules.Property.Color;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		#region Event Handlers
		#endregion

	}
}
