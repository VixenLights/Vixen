using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Dispatch;
using Vixen.Commands;

namespace Vixen.Export
{
    class ExportCommandHandler : CommandDispatch
    {
        public byte Value { get; private set; }

        public void Reset()
        {
            Value = 0;
        }

        public override void Handle(_8BitCommand obj)
        {
            Value = obj.CommandValue;
        }
    }
}
