﻿/*
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
using System.Globalization;

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Specifies a generic font type converter that provides standard values collection.
  /// </summary>
  public abstract class FontConverterDecorator : TypeConverter
  {
    private readonly TypeConverter _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontConverterDecorator"/> class.
    /// </summary>
    /// <param name="converter">The original converter.</param>
    protected FontConverterDecorator(TypeConverter converter)
    {
      _converter = converter;
    }

    #region TypeConverter implementation
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return _converter.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return _converter.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
    /// <returns>
    /// An <see cref="T:System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return _converter.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
    /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
    /// <returns>
    /// An <see cref="T:System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="destinationType"/> parameter is null.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return _converter.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <returns>
    /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.
    /// </returns>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }
    #endregion
  }
}