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
  /// Specifies a range.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class NumberRangeAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum { get; private set; }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>The maximum.</value>
    public double Maximum { get; private set; }

    /// <summary>
    /// Gets or sets the tick.
    /// </summary>
    /// <value>The tick.</value>
    public double Tick { get; private set; }

    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>The precision.</value>
    public double Precision { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberRangeAttribute"/> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    /// <param name="tick">The tick.</param>
    public NumberRangeAttribute(double minimum, double maximum, double tick)
      : this(minimum, maximum, tick, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberRangeAttribute"/> class.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    /// <param name="tick">The tick.</param>
    /// <param name="precision">The precision.</param>
    public NumberRangeAttribute(double minimum, double maximum, double tick, double precision)
    {
      Minimum = minimum;
      Maximum = maximum;
      Tick = tick;
      Precision = precision;
    }
  }
}
