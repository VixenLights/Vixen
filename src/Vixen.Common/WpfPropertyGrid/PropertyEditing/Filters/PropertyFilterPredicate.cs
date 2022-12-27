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
using System.Globalization;

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Property filter predicate.
  /// </summary>
  public class PropertyFilterPredicate
  {
    /// <summary>
    /// Gets or sets the match text.
    /// </summary>
    /// <value>The match text.</value>
    public string MatchText { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyFilterPredicate"/> class.
    /// </summary>
    /// <param name="matchText">The match text.</param>
    public PropertyFilterPredicate(string matchText)
    {
      if (matchText == null) throw new ArgumentNullException("matchText");
      MatchText = matchText.ToUpper(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Matches the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if target matches predicate; otherwise, <c>false</c>.</returns>
    public virtual bool Match(string target)
    {
      return ((target != null) && target.ToUpper(CultureInfo.CurrentCulture).Contains(MatchText));
    }    
  }
}
