using System.Drawing;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	internal readonly record struct PangolinBeyondMarkRecord(string Text, TimeSpan StartTime, Color Color);
}
