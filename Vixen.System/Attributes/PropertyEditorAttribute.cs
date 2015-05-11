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

using System;

namespace Vixen.Attributes
{
  /// <summary>
  /// Specifies the editor to use to change the property.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class PropertyEditorAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the type of the editor.
    /// </summary>
    /// <value>The type of the editor.</value>
    public string EditorType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyEditorAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    public PropertyEditorAttribute(string editorType)
    {
      if (string.IsNullOrEmpty(editorType)) throw new ArgumentNullException("editorType");
      EditorType = editorType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyEditorAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    public PropertyEditorAttribute(Type editorType)
      : this(editorType.AssemblyQualifiedName)
    {
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
      return EditorType.GetHashCode();
    }

    /// <summary>
    /// Returns a value that indicates whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">An <see cref="T:System.Object"/> to compare with this instance or null.</param>
    /// <returns>
    /// true if <paramref name="obj"/> equals the type and value of this instance; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj == this) return true;

      var attribute = obj as PropertyEditorAttribute;
      return (attribute != null && attribute.EditorType == EditorType);
    }
  }
}
