using Vixen.Module.Editor;

namespace Vixen.Sys
{
	/// <summary>
	/// Application Object Model for client applications.
	/// </summary>
	public interface IApplication
	{
		Guid ApplicationId { get; }
		IEditorUserInterface ActiveEditor { get; }
		IEditorUserInterface[] AllEditors { get; }
		AppCommand AppCommands { get; }
		
		/// <summary>
		/// Flag to control whether the Vixen System should use
		/// the real time render engine or the flat file engine.
		/// </summary>
		/// <remarks>This flag is being used to determine if the application is the Vixen Sequencer or the Vixen Player</remarks>
		bool UseFlatFileEngine { get; }
		
		///// <summary>
		///// Notifies the application that it needs to be refreshed.
		///// </summary>
		//void Refresh();
		//event KeyEventHandler KeyDown;
		//event KeyEventHandler KeyUp;
		//event MouseEventHandler MouseDown;
		//event MouseEventHandler MouseMove;
		//event MouseEventHandler MouseUp;
	}
}