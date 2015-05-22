/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace System.Windows.Controls.WpfPropertyGrid.PropertyEditing
{
	/// <summary>
	/// Specifies the source of value exception
	/// </summary>
	public enum ValueExceptionSource
	{
		/// <summary>
		/// Exception occurred during a Get operation
		/// </summary>
		Get,

		/// <summary>
		/// Exception occurred during a Set operation
		/// </summary>
		Set
	}
}