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
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Provides cursors to UI components.
	/// </summary>
	public static class CursorProvider {

		/// <summary>
		/// Registers a custom cursor that can be used with SetCursor.
		/// </summary>
		/// <param name="fileName">The file name of the cursor resource.</param>
		/// <returns>Id of the cursor.</returns>
		public static int RegisterCursor(string fileName) {
			if (fileName == null) throw new ArgumentNullException("fileName");
			byte[] resource = null;
			FileStream stream = new FileStream(fileName, FileMode.Open);
			try {
				resource = new byte[stream.Length];
				stream.Read(resource, 0, resource.Length);
			} finally {
				stream.Close();
				stream.Dispose();
			}
			return RegisterCursorResource(resource);
		}


		/// <summary>
		/// Registers a custom cursor that can be used with SetCursor.
		/// </summary>
		/// <param name="resourceAssembly">Assembly containing the cursor resource.</param>
		/// <param name="resourceName">The name of the cursor resource.</param>
		/// <returns>Id of the cursor.</returns>
		public static int RegisterCursor(Assembly resourceAssembly, string resourceName) {
			if (resourceAssembly == null) throw new ArgumentNullException("resourceAssembly");
			if (resourceName == null) throw new ArgumentNullException("resourceName");
			byte[] resource = null;
			Stream stream = resourceAssembly.GetManifestResourceStream(resourceName);
			try {
				resource = new byte[stream.Length];
				stream.Read(resource, 0, resource.Length);
			} finally {
				stream.Close();
				stream.Dispose();
			}
			return RegisterCursorResource(resource);
		}


		/// <summary>
		/// Registers a custom cursor that can be used with SetCursor.
		/// </summary>
		/// <param name="resource">The cursor resource.</param>
		/// <returns>Id of the cursor.</returns>
		public static int RegisterCursor(byte[] resource) {
			if (resource == null) throw new ArgumentNullException("resource");
			return RegisterCursorResource(resource);
		}


		/// <summary>
		/// Returns all registered cursors.
		/// CursorId 0 means the system's default cursor which is not stored as resource.
		/// </summary>
		public static IEnumerable<int> CursorIDs {
			get { return registeredCursors.Keys; }
		}


		/// <summary>
		/// Returns the resource associated with the given cursorID. 
		/// CursorId 0 means the system's default cursor which is not stored as resource.
		/// </summary>
		/// <param name="cursorID">ID of the cursor returned by the RegisterCursor method.</param>
		/// <returns></returns>
		public static byte[] GetResource(int cursorID) {
			if (cursorID == DefaultCursorID) return null;
			return registeredCursors[cursorID];
		}


		/// <summary>
		/// The default cursor id.
		/// </summary>
		public const int DefaultCursorID = 0;


		private static int RegisterCursorResource(byte[] resource) {
			// Check if the resource was registered
			foreach (KeyValuePair<int, byte[]> item in registeredCursors) {
				if (item.Value.Length == resource.Length) {
					bool equal = true;
					for (int i = item.Value.Length - 1; i >= 0; --i) {
						if (item.Value[i] != resource[i]) {
							equal = false;
							break;
						}
					}
					if (equal) return item.Key;
				}
			}
			// Register resource
			int cursorId = registeredCursors.Count + 1;
			registeredCursors.Add(cursorId, resource);
			return cursorId;
		}


		private static Dictionary<int, byte[]> registeredCursors = new Dictionary<int, byte[]>();
	}

}
