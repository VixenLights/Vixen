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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VixenModules.Editor.EffectEditor.Design
{
	/// <summary>
	/// Specifies a property name presenter.
	/// </summary>
	public sealed class PropertyNameTextBlock : TextBlock
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNameTextBlock"/> class.
		/// </summary>
		public PropertyNameTextBlock()
		{
			TextTrimming = TextTrimming.CharacterEllipsis;
			TextWrapping = TextWrapping.NoWrap;
			TextAlignment = TextAlignment.Right;
			VerticalAlignment = VerticalAlignment.Center;
			ClipToBounds = true;

			KeyboardNavigation.SetIsTabStop(this, false);
		}
	}
}