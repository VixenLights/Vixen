using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.App {
    /// <summary>
    /// Application-level module.
    /// Couldn't call it IApplication because it's already used for the AOM.
    /// </summary>
    public interface IApp {
        void Loading();
        void Unloading();
        IApplication Application { set; }
    }
}
