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

using System.Windows.Data;

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
  /// <summary>
  /// Defines a content presenter control for a Category editor.
  /// </summary>
  public class CategoryEditorContentPresenter : ContentPresenter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryEditorContentPresenter"/> class.
    /// </summary>
    public CategoryEditorContentPresenter()
    {
      var contentBinding = new Binding
      {
        RelativeSource = RelativeSource.Self,
        Path = new PropertyPath("(0).(1)", new[] { GridEntryContainer.ParentContainerProperty, GridEntryContainer.EntryProperty })
      };

      var contentTemplateBinding = new Binding
      {
        RelativeSource = RelativeSource.Self,
        Path = new PropertyPath("(0).EditorTemplate", new[] { GridEntryContainer.ParentContainerProperty })
      };

      SetBinding(ContentProperty, contentBinding);
      SetBinding(ContentTemplateProperty, contentTemplateBinding); 
    }
  }
}
