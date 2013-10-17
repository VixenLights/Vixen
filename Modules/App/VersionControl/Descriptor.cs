using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.App;

namespace VersionControl
{
    public class Descriptor : AppModuleDescriptorBase
    {
        Guid _TypeId = new Guid("{C2D86EAC-618A-4066-B6E2-1E259C2959F6}");

        public override string TypeName
        {
            get { return "Vixen Version Control"; }
        }

        public override Guid TypeId
        {
            get { return _TypeId; }
        }

        public override string Author
        {
            get { return "Darren McDaniel"; }
        }

        public override string Description
        {
            get { return "Vixen Version Control"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override Type ModuleClass
        {
            get { return typeof(Module); }
        }
        public override Type ModuleStaticDataClass
        {
            get { return typeof(Data); }
        }
    }
}
