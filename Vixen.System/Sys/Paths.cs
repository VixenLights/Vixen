using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
    static public class Paths {
        // System- or user-defined parent directory of "Vixen" directory.
        static private string _dataRootPath = null;

        private const string VIXEN_DATA_DIR = "Vixen 3";

        static Paths() {
			// Basing the binary directory on Vixen.dll instead of the current application's
			// appdomain.  They will likely be the same thing, but at least we'll be based
			// on the actual binary this way.
			BinaryRootPath = Path.GetDirectoryName(VixenSystem.AssemblyFileName);
		}

        static public string BinaryRootPath { get; private set; }

		static public string ModuleRootPath {
			get { return Modules.Directory; }
		}

        static public string DataRootPath {
            get {
                if(_dataRootPath == null) {
                    _dataRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), VIXEN_DATA_DIR);
                }
                return _dataRootPath;
            }
            set {
                if(!value.EndsWith(Path.DirectorySeparatorChar + VIXEN_DATA_DIR)) {
                    value += Path.DirectorySeparatorChar + VIXEN_DATA_DIR;
                }
                // Want to be sure the directory can exist before making it the
                // data directory.
                if(Helper.EnsureDirectory(value)) {
                    _dataRootPath = value;
                    _BuildDataDirectories();
                }
            }
        }

		static public string ModuleDataFilesPath {
			get { return Path.Combine(DataRootPath, "Module Data Files"); }
		}

        static private void _BuildDataDirectories() {
            Helper.EnsureDirectory(_dataRootPath);

            Type attributeType = typeof(DataPathAttribute);
            // Iterate types in the system assembly.
            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				AddDataPaths(type, attributeType);
            }
        }

		static internal void AddDataPaths(Type owningType, Type pathAttributeType) {
			// Iterate static members of the type, public and private.
			// They must be static because we won't have an instance to pull values from.
			bool isWriteable = false;
			foreach(MemberInfo mi in owningType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
				string path = null;

				// Does the member have an appropriate attribute?
				if(mi.GetCustomAttributes(pathAttributeType, false).Any()) {
					// Get the value from the property or field.
					switch(mi.MemberType) {
						case MemberTypes.Property:
							PropertyInfo pi = (PropertyInfo)mi;
							if(pi.CanRead && pi.PropertyType == typeof(string)) {
								path = pi.GetValue(null, null) as string;
								isWriteable = pi.CanWrite;
							}
							break;
						case MemberTypes.Field:
							FieldInfo fi = (FieldInfo)mi;
							if(fi.FieldType == typeof(string)) {
								path = fi.GetValue(null) as string;
								isWriteable = true;
							}
							break;
					}
				}

				if(path == null) continue;

				// Value may or may not be qualified.
				if(!Path.IsPathRooted(path)) {
					path = Path.Combine(ModuleDataFilesPath, path);
				}

				// Ensure the path's presence.
				// Make sure the path exists before writing it back
				// in case the module's descriptor does something
				// with the directory upon it being set.
				Helper.EnsureDirectory(path);

				if(!isWriteable) continue;

				// Write back the resolved path.
				if(mi is PropertyInfo) {
					((PropertyInfo)mi).SetValue(null, path, null);
				} else if(mi is FieldInfo) {
					((FieldInfo)mi).SetValue(null, path);
				}
			}
		}
    }
}
