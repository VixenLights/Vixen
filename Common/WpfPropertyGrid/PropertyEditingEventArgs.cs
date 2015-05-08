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

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Encapsulates a method that contains an object and PropertyEditingEventArgs event arguments.
  /// </summary>
  public delegate void PropertyEditingEventHandler(object sender, PropertyEditingEventArgs e);

  public class PropertyEditingEventArgs : RoutedEventArgs
  {
    /// <summary>
    /// Gets property descriptor.
    /// </summary>
    // TODO: Replace with my wrapper?
    public PropertyDescriptor PropertyDescriptor { get; private set; }

    public PropertyEditingEventArgs(RoutedEvent routedEvent, object source, PropertyDescriptor propertyDescriptor)
      : base(routedEvent, source)
    {
      PropertyDescriptor = propertyDescriptor;
    }
  }
}
