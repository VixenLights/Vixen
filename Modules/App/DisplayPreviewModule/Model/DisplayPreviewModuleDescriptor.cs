namespace VixenModules.App.DisplayPreview.Model
{
    using System;
    using System.Collections.Generic;
    using Vixen.Module.App;
    using Vixen.Sys;
    using VixenModules.Property.RGB;

    public class DisplayPreviewModuleDescriptor : AppModuleDescriptorBase
    {
        static DisplayPreviewModuleDescriptor()
        {
            ModulePath = "DisplayPreview";
        }

        [ModuleDataPath]
        public static string ModulePath { get; set; }

        public override string TypeName
        {
            get
            {
                return "Vixen Display Preview";
            }
        }

        public override Guid TypeId
        {
            get
            {
                return new Guid("BC0FBE6E-2E5F-4058-A311-C553EC156642");
            }
        }

        public override Type ModuleClass
        {
            get
            {
                return typeof(DisplayPreviewModuleInstance);
            }
        }

        public override Type ModuleStaticDataClass
        {
            get
            {
                return typeof(DisplayPreviewModuleDataModel);
            }
        }

        public override string Author
        {
            get
            {
                return "Erik Mathisen";
            }
        }

        public override string Description
        {
            get
            {
                return "A module that allows you to build a virtual mock of your display, and preview what the display will look like during sequence playback.";
            }
        }

        public override string Version
        {
            get
            {
                return "1.0";
            }
        }

        public override Guid[] Dependencies
        {
            get
            {
                var deps = new List<Guid>(base.Dependencies) { RGBDescriptor.ModuleID };
                return deps.ToArray();
            }
        }
    }
}
