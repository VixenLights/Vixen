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
using System.Windows.Markup;

namespace System.Windows.Controls.WpfPropertyGrid.Themes
{
  public class KaxamlTheme : ResourceDictionary, IComponentConnector
  {
    private bool isInitialized;

    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(KaxamlTheme), "Style"); }
    }

    public KaxamlTheme()
    {
      InitializeComponent();
    }

    #region IComponentConnector Members

    public void Connect(int connectionId, object target)
    {
      this.isInitialized = true;
    }

    public void InitializeComponent()
    {
      if (!isInitialized)
      {
        this.isInitialized = true;
        Uri resourceLocator = new Uri("/System.Windows.Controls.WpfPropertyGrid.Themes;component/Kaxaml/Theme.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
      }
    }

    #endregion
  }
}
