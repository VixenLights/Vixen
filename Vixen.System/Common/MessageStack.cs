using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common
{
    public static class MessageStack
    {
        private static Queue<string> scriptingRuntimeMessages = new Queue<string>();
        public static Queue<string> ScriptingRuntimeMessages
        {
            get { return scriptingRuntimeMessages; }
            set { scriptingRuntimeMessages = value; }
        }

    }
}
