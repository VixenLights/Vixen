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
  /// Provides a standard set of property grid related commands.
  /// </summary>
  public static class PropertyGridCommands
  {
    #region Fields
    private static readonly Type ThisType = typeof(PropertyGridCommands);
    #endregion
        
    #region Commands

    private static readonly RoutedUICommand _ShowFilter = new RoutedUICommand("Show Filter", "ShowFilter", ThisType);
    private static readonly RoutedUICommand _HideFilter = new RoutedUICommand("Hide Filter", "HideFilter", ThisType);
    private static readonly RoutedUICommand _ToggleFilter = new RoutedUICommand("Toggle Filter", "ToggleFilter", ThisType);
    private static readonly RoutedUICommand _ResetFilter = new RoutedUICommand("Reset Filter", "ResetFilter", ThisType);
    private static readonly RoutedUICommand _Reload = new RoutedUICommand("Reload", "Reload", ThisType);
    private static readonly RoutedUICommand _ShowReadOnlyProperties = new RoutedUICommand("Show Read-Only Properties", "ShowReadOnlyProperties", ThisType);
    private static readonly RoutedUICommand _HideReadOnlyProperties = new RoutedUICommand("Hide Read-Only Properties", "HideReadOnlyProperties", ThisType);
    private static readonly RoutedUICommand _ToggleReadOnlyProperties = new RoutedUICommand("Toggle Read-Only Properties", "ToggleReadOnlyProperties", ThisType);
    private static readonly RoutedUICommand _ShowAttachedProperties = new RoutedUICommand("Show Attached Properties", "ShowAttachedProperties", ThisType);
    private static readonly RoutedUICommand _HideAttachedProperties = new RoutedUICommand("Hide Attached Properties", "HideAttachedProperties", ThisType);
    private static readonly RoutedUICommand _ToggleAttachedProperties = new RoutedUICommand("Toggle Attached Properties", "ToggleAttachedProperties", ThisType);
    private static readonly RoutedUICommand _ClosePropertyTab = new RoutedUICommand("Close Property Tab", "ClosePropertyTabCommand", ThisType);

    /// <summary>
    /// Represents a command for the control to show property filter box.
    /// </summary>
    public static RoutedUICommand ShowFilter { get { return _ShowFilter; } }

    /// <summary>
    /// Represents a command for the control to hide property filter box.
    /// </summary>
    public static RoutedUICommand HideFilter { get { return _HideFilter; } }

    /// <summary>
    /// Represents a command for the control to toggle visibility of property filter box.
    /// </summary>
    public static RoutedUICommand ToggleFilter { get { return _ToggleFilter; } }

    /// <summary>
    /// Represents a command that resets current grid filter.
    /// </summary>
    public static RoutedUICommand ResetFilter { get { return _ResetFilter; } }    

    /// <summary>
    /// Represents a command that reloads current grid properties.
    /// </summary>
    public static RoutedUICommand Reload { get { return _Reload; } }
        
    /// <summary>
    /// Represents a command for the control to show all read-only properties.
    /// </summary>
    public static RoutedUICommand ShowReadOnlyProperties { get { return _ShowReadOnlyProperties; } }

    /// <summary>
    /// Represetns a command for the control to hide all read-only properties.
    /// </summary>
    public static RoutedUICommand HideReadOnlyProperties { get { return _HideReadOnlyProperties; } }

    /// <summary>
    /// Represents a command for the control to toggle visibility of read-only properties.
    /// </summary>
    public static RoutedUICommand ToggleReadOnlyProperties { get { return _ToggleReadOnlyProperties; } }

    /// <summary>
    /// Represents a command for the control to show all attached properties.
    /// </summary>
    public static RoutedUICommand ShowAttachedProperties { get { return _ShowAttachedProperties; } }

    /// <summary>
    /// Represents a command for the control to hide all attached properties.
    /// </summary>
    public static RoutedUICommand HideAttachedProperties { get { return _HideAttachedProperties; } }

    /// <summary>
    /// Represents a command for the control to toggle visibility of attached properties.
    /// </summary>
    public static RoutedUICommand ToggleAttachedProperties { get { return _ToggleAttachedProperties; } }

    /// <summary>
    /// Represents a command for the control to close active property tab.
    /// </summary>
    public static RoutedUICommand ClosePropertyTab { get { return _ClosePropertyTab; } }

    #endregion
  }
}
