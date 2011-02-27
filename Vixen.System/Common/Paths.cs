using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Vixen.Common {
    static class Paths {
        // System- or user-defined parent directory of "Vixen" directory.
        static private string _dataRootPath = null;

        private const string VIXEN_DIR = "Vixen";

        static Paths() {
            BinaryRootPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        static public string BinaryRootPath { get; private set; }

        static public string DataRootPath {
            get {
                if(_dataRootPath == null) {
                    _dataRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), VIXEN_DIR);
                }
                return _dataRootPath;
            }
            set {
                if(!value.EndsWith(Path.DirectorySeparatorChar + VIXEN_DIR)) {
                    value += Path.DirectorySeparatorChar + VIXEN_DIR;
                }
                // Want to be sure the directory can exist before making it the
                // data directory.
                if(Helper.EnsureDirectory(value)) {
                    _dataRootPath = value;
                    _BuildDataDirectories();
                }
            }
        }

		static public string ModuleFilesPath {
			get { return Path.Combine(DataRootPath, "Module Data Files"); }
		}

        static private void _BuildDataDirectories() {
            Helper.EnsureDirectory(_dataRootPath);

            Type attributeType = typeof(DataPathAttribute);
			//string path;
            // Iterate types in the system assembly.
            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				AddDataPaths(type, attributeType);
            }
        }

		static internal void AddDataPaths(Type owningType, Type pathAttributeType) {
			// Iterate static members of the type, public and private.
			// They must be static because we won't have an instance to pull values from.
			string path;
			bool isWriteable = false;
			foreach(MemberInfo mi in owningType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
				path = null;

				// Does the member have an appropriate attribute?
				if(mi.GetCustomAttributes(pathAttributeType, false).Length != 0) {
					// Get the value from the property or field.
					if(mi.MemberType == MemberTypes.Property) {
						PropertyInfo pi = mi as PropertyInfo;
						if(pi.CanRead && pi.PropertyType == typeof(string)) {
							path = pi.GetValue(null, null) as string;
							isWriteable = pi.CanWrite;
						}
					} else if(mi.MemberType == MemberTypes.Field) {
						FieldInfo fi = mi as FieldInfo;
						if(fi.FieldType == typeof(string)) {
							path = fi.GetValue(null) as string;
							isWriteable = true;
						}
					}
				}

				// Ensure the path's presence.
				if(path != null) {
					// Value may or may not be qualified.
					if(!Path.IsPathRooted(path)) {
						// Assume it to be a data path; parent it to the ModuleData directory.
						if(isWriteable) {
							//path = Path.Combine(DataRootPath, path);
							path = Path.Combine(ModuleFilesPath, path);
							// Write back the resolved path.
							if(mi is PropertyInfo) {
								(mi as PropertyInfo).SetValue(null, path, null);
							} else if(mi is FieldInfo) {
								(mi as FieldInfo).SetValue(null, path);
							}
						} else {
							// If it's not a qualified path, the implementor needs to know the
							// resulting qualified path, so the declaration needs to be writeable.
							// Otherwise, no path for you.
							path = null;
						}
					}
					if(path != null) {
						Helper.EnsureDirectory(path);
					}
				}
			}
		}
    }
}
