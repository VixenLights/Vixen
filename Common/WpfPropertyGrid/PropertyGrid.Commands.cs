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
  public partial class PropertyEditorGrid
  {
    /// <summary>
    /// Initializes the command bindings.
    /// </summary>
    protected void InitializeCommandBindings()
    {
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ResetFilter, OnResetFilterCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.Reload, OnReloadCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowReadOnlyProperties, OnShowReadOnlyPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideReadOnlyProperties, OnHideReadOnlyPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleReadOnlyProperties, OnToggleReadOnlyPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowAttachedProperties, OnShowAttachedPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideAttachedProperties, OnHideAttachedPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleAttachedProperties, OnToggleAttachedPropertiesCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowFilter, OnShowFilterCommand));
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideFilter, OnHideFilterCommand));      
      CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleFilter, OnToggleFilterCommand));
      CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowDialogEditor, OnShowDialogEditor));
    }

    #region Commands

    private void OnResetFilterCommand(object sender, ExecutedRoutedEventArgs e)
    {
      PropertyFilter = string.Empty;
    }

    private void OnReloadCommand(object sender, ExecutedRoutedEventArgs e)
    {
      DoReload();
    }

    private void OnShowReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowReadOnlyProperties = true;
    }

    private void OnHideReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowReadOnlyProperties = false;
    }

    private void OnToggleReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowReadOnlyProperties = !ShowReadOnlyProperties;
    }

    private void OnShowAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowAttachedProperties = true;
    }

    private void OnHideAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowAttachedProperties = false;
    }

    private void OnToggleAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
    {
      ShowAttachedProperties = !ShowAttachedProperties;
    }

    private void OnShowFilterCommand(object sender, ExecutedRoutedEventArgs e)
    {
      PropertyFilterVisibility = Visibility.Visible;
    }

    private void OnHideFilterCommand(object sender, ExecutedRoutedEventArgs e)
    {
      PropertyFilterVisibility = Visibility.Collapsed;
    }

    private void OnToggleFilterCommand(object sender, ExecutedRoutedEventArgs e)
    {
      PropertyFilterVisibility = (PropertyFilterVisibility == Visibility.Visible)
        ? Visibility.Collapsed
        : Visibility.Visible;
    }
       
    // TODO: refactoring needed
    private void OnShowDialogEditor(object sender, ExecutedRoutedEventArgs e)
    {
      var value = e.Parameter as PropertyItemValue;
      if (value == null) return;
      
      var property = value.ParentProperty;
      if (property == null) return;
     
      var editor = property.Editor;
      // TODO: Finish DialogTemplate implementation
      if (editor != null)// && editor.HasDialogTemplate)
        editor.ShowDialog(value, this);
    }
    #endregion
  }
}