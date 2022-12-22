﻿using System.Reflection;
using Vixen.Sys.Attribute;

namespace Vixen.Sys
{
	public static class Paths
	{
		// System- or user-defined parent directory of "Vixen" directory.
		private static string _dataRootPath;

		private const string VIXEN_DATA_DIR = "Vixen 3";

		static Paths()
		{
			// Basing the binary directory on Vixen.dll instead of the current application's
			// appdomain.  They will likely be the same thing, but at least we'll be based
			// on the actual binary this way.
			BinaryRootPath = Path.GetDirectoryName(VixenSystem.AssemblyFileName);
		}

		public static string BinaryRootPath { get; private set; }

		public static string ModuleRootPath
		{
			get { return Modules.Directory; }
		}

		public static string DataRootPath
		{
			get
			{
				if (_dataRootPath == null) {
					_dataRootPath = DefaultDataRootPath;
				}
				return _dataRootPath;
			}
			set
			{
				if (value == null) value = DefaultDataRootPath;
				value = value.TrimEnd(Path.DirectorySeparatorChar);

				// Want to be sure the directory can exist before making it the
				// data directory.
				if (Helper.EnsureDirectory(value)) {
					_dataRootPath = value;
					_BuildDataDirectories();
				}
				else {
					throw new IOException("Specified data path does not exist and could not be created.");
				}
			}
		}

		public static string DefaultDataRootPath
		{
			get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), VIXEN_DATA_DIR); }
		}

		public static string ModuleDataFilesPath
		{
			get { return Path.Combine(DataRootPath, "Module Data Files"); }
		}

		private static void _BuildDataDirectories()
		{
			Helper.EnsureDirectory(_dataRootPath);

			Type attributeType = typeof (DataPathAttribute);
			// Iterate types in the system assembly.
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				AddDataPaths(type, attributeType);
			}
		}

		internal static void AddDataPaths(Type owningType, Type pathAttributeType)
		{
			// Iterate static members of the type, public and private.
			// They must be static because we won't have an instance to pull values from.
			bool isWriteable = false;
			foreach (
				MemberInfo mi in
					owningType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public |
					                      BindingFlags.Static)) {
				string path = null;

				// Does the member have an appropriate attribute?
				if (mi.GetCustomAttributes(pathAttributeType, false).Any()) {
					// Get the value from the property or field.
					switch (mi.MemberType) {
						case MemberTypes.Property:
							PropertyInfo pi = (PropertyInfo) mi;
							if (pi.CanRead && pi.PropertyType == typeof (string)) {
								path = pi.GetValue(null, null) as string;
								isWriteable = pi.CanWrite;
							}
							break;
						case MemberTypes.Field:
							FieldInfo fi = (FieldInfo) mi;
							if (fi.FieldType == typeof (string)) {
								path = fi.GetValue(null) as string;
								isWriteable = !fi.IsInitOnly;
							}
							break;
					}
				}

				if (path == null) continue;

				// Value may or may not be qualified.
				if (!Path.IsPathRooted(path)) {
					path = Path.Combine(ModuleDataFilesPath, path);
				}

				// Ensure the path's presence.
				// Make sure the path exists before writing it back
				// in case the module's descriptor does something
				// with the directory upon it being set.
				Helper.EnsureDirectory(path);

				if (!isWriteable) continue;

				// Write back the resolved path.
				if (mi is PropertyInfo) {
					((PropertyInfo) mi).SetValue(null, path, null);
				}
				else if (mi is FieldInfo) {
					((FieldInfo) mi).SetValue(null, path);
				}
			}
		}
	}
}