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

using System.ComponentModel;

namespace System.Windows.Controls.WpfPropertyGrid.Converters
{
	/// <summary>
	/// Extended <see cref="FontStyleConverter"/> that provides standard values collection.
	/// </summary>
	public sealed class FontStyleConverterDecorator : FontConverterDecorator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FontStyleConverterDecorator"/> class.
		/// </summary>
		public FontStyleConverterDecorator() : base(new FontStyleConverter())
		{
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
		/// <returns>
		/// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
		/// </returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(
				new[]
				{
					FontStyles.Italic,
					FontStyles.Normal,
					FontStyles.Oblique
				});
		}
	}
}