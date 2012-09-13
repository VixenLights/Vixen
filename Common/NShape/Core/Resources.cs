/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Reflection;
using System.Resources;


namespace Dataweb.Utilities {

	/* The following naming scheme should be applied to resource string names:
	 * <Entity name>_<string name>
	 * Entity name is a name that describes the entity which defines the string.
	 * String name is a unique name within this class. */

	/// <summary>
	/// Loads strings from the resource of the library.
	/// </summary>
	static class Resources {

		public static string GetString(string name) {
			if (name == null) throw new ArgumentNullException("name");
			EnsureResourceManager();
			return resourceManager.GetString(name);
		}


		public static string FormatString(string formatName, object arg0) {
			if (formatName == null) throw new ArgumentNullException("formatName");
			EnsureResourceManager();
			return string.Format(resourceManager.GetString(formatName), arg0);
		}


		public static string FormatString(string formatName, object arg0, object arg1) {
			if (formatName == null) throw new ArgumentNullException("formatName");
			EnsureResourceManager();
			return string.Format(resourceManager.GetString(formatName), arg0, arg1);
		}


		public static string FormatString(string formatName, object arg0, object arg1, object arg2) {
			if (formatName == null) throw new ArgumentNullException("formatName");
			EnsureResourceManager();
			return string.Format(resourceManager.GetString(formatName), arg0, arg1, arg2);
		}


		public static string FormatString(string formatName, params object[] args) {
			if (formatName == null) throw new ArgumentNullException("formatName");
			EnsureResourceManager();
			return string.Format(resourceManager.GetString(formatName), args);
		}


		static Resources() {
			// Nothing to do yet.
		}


		// Makes sure the resource manager is isInitialized.
		private static void EnsureResourceManager() {
			if (resourceManager == null) {
				resourceManager = new ResourceManager("Dataweb.NShape.Properties.Resources", Assembly.GetExecutingAssembly());
			}
		}


		// Holds a resource manager for the default resources of the current library
		private static ResourceManager resourceManager;

	}

}