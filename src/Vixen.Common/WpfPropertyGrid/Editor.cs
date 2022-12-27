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
namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// A base class for all value editors.
  /// </summary>
  public abstract class Editor : DependencyObject
  {
    private static readonly Type ThisType = typeof(Editor);
    /// <summary>
    /// Identifies the <see cref="InlineTemplate"/> property. This is a dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineTemplateProperty =
      DependencyProperty.Register("InlineTemplate", typeof(object), ThisType, new PropertyMetadata(null), ValidateTemplateValue);

    /// <summary>
    /// Gets or sets the inline template. Can be either DataTemplate or ComponentResourceKey object.
    /// </summary>
    /// <value>The inline template.</value>
    public object InlineTemplate
    {
      get { return GetValue(InlineTemplateProperty); }
      set { SetValue(InlineTemplateProperty, value); }
    }
                
    /// <summary>
    /// Identifies the <see cref="ExtendedTemplate"/> property. This is a dependency property.
    /// </summary>
    public static readonly DependencyProperty ExtendedTemplateProperty =
      DependencyProperty.Register("ExtendedTemplate", typeof(object), ThisType, new PropertyMetadata(null), ValidateTemplateValue);

    /// <summary>
    /// Gets or sets the extended template. Can be either DataTemplate or ComponentResourceKey object.
    /// </summary>
    /// <value>The extended template.</value>
    public object ExtendedTemplate
    {
      get { return GetValue(ExtendedTemplateProperty); }
      set { SetValue(ExtendedTemplateProperty, value); }
    }
                
    /// <summary>
    /// Idenfities the <see cref="DialogTemplate"/>. This is a dependency property.
    /// </summary>
    public static readonly DependencyProperty DialogTemplateProperty =
      DependencyProperty.Register("DialogTemplate", typeof(object), ThisType, new PropertyMetadata(null), ValidateTemplateValue);

    /// <summary>
    /// Gets or sets the dialog template.
    /// </summary>
    /// <value>The dialog template.</value>
    public object DialogTemplate
    {
      get { return GetValue(DialogTemplateProperty); }
      set { SetValue(DialogTemplateProperty, value); }
    }

    private static bool ValidateTemplateValue(object value)
    {
      if (value == null) return true;
      if (value is DataTemplate || value is ComponentResourceKey) return true;
      return false;
    }

    /// <summary>
    /// Shows the dialog for editing property value.
    /// </summary>
    /// <param name="propertyValue">The property value.</param>
    /// <param name="commandSource">The command source.</param>
    public virtual void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
    {
    }
  }
}
