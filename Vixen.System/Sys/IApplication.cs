using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Common;
using Vixen.Module.Editor;

namespace Vixen.Sys {
    /// <summary>
    /// Application Object Model for client applications.
    /// </summary>
    public interface IApplication {
        Guid ApplicationId { get; }
		IEditor ActiveEditor { get; }
		IEditor[] AllEditors { get; }
        AppCommand AppCommands { get; }
        /// <summary>
        /// Notifies the application that it needs to be refreshed.
        /// </summary>
        void Refresh();
        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;
    }
}
