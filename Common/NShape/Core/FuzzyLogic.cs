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

namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Basic operations for fuzzy logic
	/// </summary>
	internal class Fuzzy {

		internal static byte Or(byte v1, byte v2) {
			return Math.Max(v1, v2);
		}


		internal static byte Or(byte v1, byte v2, byte v3) {
			return Math.Max(Math.Max(v1, v2), v3);
		}


		internal static byte And(byte v1, byte v2) {
			return Math.Min(v1, v2);
		}


		internal static byte And(byte v1, byte v2, byte v3) {
			return Math.Min(Math.Min(v1, v2), v3);
		}


		/// <summary>
		/// Bildet einen Float-Wert auf einen Fuzzy-Wert linear ab.
		/// </summary>
		/// <param name="value">Zum Fuzzy-Wert proportionaler Float-Wert</param>
		/// <param name="falseValue">Untergrenze, ab der der Fuzyy-Wert 0 wird</param>
		/// <param name="trueValue">Obergrenze, ab der der Fuzzy-Wert 100 wird</param>
		/// <returns>Fuzzy-Wert</returns>
		internal static byte MapToFuzzy(double value, float falseValue, float trueValue) {
			byte result;
			if (value <= falseValue)
				result = 0;
			else if (value >= trueValue)
				result = 100;
			else
				result = (byte)(100.0 * (value - falseValue) / (trueValue - falseValue));
			return result;
		}

	}

}
