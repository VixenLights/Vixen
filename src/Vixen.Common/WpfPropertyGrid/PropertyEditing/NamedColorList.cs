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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media;

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Provides a list of colors that is possible to bind to UI
  /// </summary>
  // For more details see http://shevaspace.spaces.live.com/blog/cns!FD9A0F1F8DD06954!435.entry
  public sealed class NamedColorList: ObservableCollection<NamedColor>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedColorList"/> class.
    /// </summary>
    public NamedColorList()
    {
      NamedColor nc;
      const MethodAttributes inclusiveAttributes = MethodAttributes.Static | MethodAttributes.Public;

      foreach (var pi in typeof(Colors).GetProperties())
      {
        if (pi.PropertyType != typeof(Color)) continue;

        var mi = pi.GetGetMethod();
        if ((mi == null) || ((mi.Attributes & inclusiveAttributes) != inclusiveAttributes)) continue;

        nc = new NamedColor(pi.Name, (Color)pi.GetValue(null, null));
        Add(nc);
      }

    }
  }
}
