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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Dataweb.NShape;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NShapeTest {

	[TestClass]
	public class ExceptionTest {

		[TestMethod]
		public void TestExceptionSerialization() {
			const string errorMessage = "Exception Serialization Test Message";
			const string innerErrorMessage = "Inner Exception Error Message";

			PerformExceptionTest(new NShapeException(errorMessage, new Exception(innerErrorMessage)));
			PerformExceptionTest(new NShapeSecurityException(Permission.Security));
			PerformExceptionTest(new NShapeInternalException(errorMessage, new Exception(innerErrorMessage)));
			PerformExceptionTest(new NShapeUnsupportedValueException(this));
			//PerformExceptionTest(new NShapeInterfaceNotSupportedException());
			//PerformExceptionTest(new NShapeMappingNotSupportedException());
			PerformExceptionTest(new NShapePropertyNotSetException(this, "Property"));
			PerformExceptionTest(new LoadLibraryException("Dataweb.NShape.GeneralShapes"));
		}


		private void PerformExceptionTest<TException>(TException exception) where TException : Exception {
			// Save the full ToString() value, including the exception message and stack trace.
			string exceptionToString = exception.ToString();

			// Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
			BinaryFormatter formatter = new BinaryFormatter();
			using (MemoryStream memStream = new MemoryStream()) {
				// "Save" object state
				formatter.Serialize(memStream, exception);
				// Re-use the same stream for de-serialization
				memStream.Seek(0, 0);
				// Replace the original exception with de-serialized one
				exception = (TException)formatter.Deserialize(memStream);
			}
			// Double-check that the exception message and stack trace (owned by the base Exception) are preserved
			Assert.AreEqual(exceptionToString, exception.ToString(), string.Format("{0}.ToString() failed after serialization.", typeof(TException).Name));
		}

	}
}
