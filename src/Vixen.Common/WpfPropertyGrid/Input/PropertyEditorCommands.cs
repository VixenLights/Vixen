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
using System.Windows.Input;

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  ///Provides a standard set of property editor related commands.
  /// </summary>
  public static class PropertyEditorCommands
  {
    private static readonly Type ThisType = typeof(PropertyEditorCommands);
        
    private static readonly RoutedUICommand _ShowDialogEditor = new RoutedUICommand("Show Dialog Editor", "ShowDialogEditorCommand", ThisType);
    private static readonly RoutedUICommand _ShowExtendedEditor = new RoutedUICommand("Show Extended Editor", "ShowExtendedEditorCommand", ThisType);

    /// <summary>
    /// Defines a command to show dialog editor for a property.
    /// </summary>    
    public static RoutedUICommand ShowDialogEditor
    {
      get { return _ShowDialogEditor; }
    }

    /// <summary>
    /// Defines a command to show extended editor for a property.
    /// </summary>
    public static RoutedUICommand ShowExtendedEditor
    {
      get { return _ShowExtendedEditor; }
    }
  }
}
