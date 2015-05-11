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
  /// Specifies the order of property.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class PropertyOrderAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>The order.</value>
    public int Order { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/> class.
    /// </summary>
    /// <param name="order">The order.</param>
    public PropertyOrderAttribute(int order)
    {
      Order = order;
    }
  }
}