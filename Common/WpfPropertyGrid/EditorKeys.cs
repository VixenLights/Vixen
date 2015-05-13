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
  /// Contains a set of editor keys.
  /// </summary>
  public static class EditorKeys
  {
    private static readonly Type ThisType = typeof(EditorKeys);
    private static readonly ComponentResourceKey _NamedColorEditorKey = new ComponentResourceKey(ThisType, "NamedColorEditor");
	private static readonly ComponentResourceKey _ColorEditorKey = new ComponentResourceKey(ThisType, "ColorEditor");
	private static readonly ComponentResourceKey _CurveEditorKey = new ComponentResourceKey(ThisType, "CurveEditor");
	private static readonly ComponentResourceKey _GradientEditorKey = new ComponentResourceKey(ThisType, "GradientEditor");
    private static readonly ComponentResourceKey _PasswordEditorKey = new ComponentResourceKey(ThisType, "PasswordEditor");
    private static readonly ComponentResourceKey _DefaultEditorKey = new ComponentResourceKey(ThisType, "DefaultEditor");
    private static readonly ComponentResourceKey _BooleanEditorKey = new ComponentResourceKey(ThisType, "BooleanEditor");
    private static readonly ComponentResourceKey _DoubleEditorKey = new ComponentResourceKey(ThisType, "DoubleEditor");
    private static readonly ComponentResourceKey _EnumEditorKey = new ComponentResourceKey(ThisType, "EnumEditor");
	private static readonly ComponentResourceKey _ComboBoxEditorKey = new ComponentResourceKey(ThisType, "ComboBoxEditor");
    private static readonly ComponentResourceKey _SliderEditorKey = new ComponentResourceKey(ThisType, "SliderEditor");
	private static readonly ComponentResourceKey _SliderDoubleEditorKey = new ComponentResourceKey(ThisType, "SliderDoubleEditor");
	private static readonly ComponentResourceKey _SliderLevelEditorKey = new ComponentResourceKey(ThisType, "SliderLevelEditor");
	private static readonly ComponentResourceKey _SliderPercentageEditorKey = new ComponentResourceKey(ThisType, "SliderPercentageEditor");
    private static readonly ComponentResourceKey _FontFamilyEditorKey = new ComponentResourceKey(ThisType, "FontFamilyEditor");
    private static readonly ComponentResourceKey _BrushEditorKey = new ComponentResourceKey(ThisType, "BrushEditor");
    private static readonly ComponentResourceKey _DefaultCategoryEditorKey = new ComponentResourceKey(ThisType, "DefaultCategoryEditor");
    private static readonly ComponentResourceKey _ComplexPropertyEditorKey = new ComponentResourceKey(ThisType, "ComplexPropertyEditor");

    /// <summary>
    /// Gets the NamedColor editor key.
    /// </summary>
    /// <value>The named color editor key.</value>
    public static ComponentResourceKey NamedColorEditorKey { get { return _NamedColorEditorKey; } }

	/// <summary>
	/// Gets the Curve editor key.
	/// </summary>
	/// <value>The named color editor key.</value>
	public static ComponentResourceKey CurveEditorKey { get { return _CurveEditorKey; } }

	/// <summary>
	/// Gets the Gradient editor key.
	/// </summary>
	/// <value>The named color editor key.</value>
	public static ComponentResourceKey GradientEditorKey { get { return _GradientEditorKey; } }

	/// <summary>
	/// Gets the NamedColor editor key.
	/// </summary>
	/// <value>The named color editor key.</value>
	public static ComponentResourceKey ColorEditorKey { get { return _ColorEditorKey; } }

    /// <summary>
    /// Gets the password editor key.
    /// </summary>
    /// <value>The password editor key.</value>
    public static ComponentResourceKey PasswordEditorKey { get { return _PasswordEditorKey; } }

    /// <summary>
    /// Gets the default editor key.
    /// </summary>
    /// <value>The default editor key.</value>
    public static ComponentResourceKey DefaultEditorKey { get { return _DefaultEditorKey; } }

    /// <summary>
    /// Gets the boolean editor key.
    /// </summary>
    /// <value>The boolean editor key.</value>
    public static ComponentResourceKey BooleanEditorKey { get { return _BooleanEditorKey; } }

    /// <summary>
    /// Gets the double editor key.
    /// </summary>
    /// <value>The double editor key.</value>
    public static ComponentResourceKey DoubleEditorKey { get { return _DoubleEditorKey; } }

    /// <summary>
    /// Gets the enum editor key.
    /// </summary>
    /// <value>The enum editor key.</value>
    public static ComponentResourceKey EnumEditorKey { get { return _EnumEditorKey; } }

	/// <summary>
	/// Gets the combobox editor key.
	/// </summary>
	/// <value>The enum editor key.</value>
	public static ComponentResourceKey ComboBoxEditorKey { get { return _ComboBoxEditorKey; } }

    /// <summary>
    /// Gets the slider editor key.
    /// </summary>
    /// <value>The slider editor key.</value>
    public static ComponentResourceKey SliderEditorKey { get { return _SliderEditorKey; } }


	/// <summary>
	/// Gets the slider double editor key.
	/// </summary>
	/// <value>The slider editor key.</value>
	public static ComponentResourceKey SliderDoubleEditorKey { get { return _SliderDoubleEditorKey; } }

	/// <summary>
	/// Gets the slider level editor key.
	/// </summary>
	/// <value>The slider editor key.</value>
	public static ComponentResourceKey SliderLevelEditorKey { get { return _SliderLevelEditorKey; } }

	/// <summary>
	/// Gets the slider percentage editor key.
	/// </summary>
	/// <value>The slider editor key.</value>
	public static ComponentResourceKey SliderPercentageEditorKey { get { return _SliderPercentageEditorKey; } }

    /// <summary>
    /// Gets the font family editor key.
    /// </summary>
    /// <value>The font family editor key.</value>
    public static ComponentResourceKey FontFamilyEditorKey { get { return _FontFamilyEditorKey; } }

    /// <summary>
    /// Gets the brush editor key.
    /// </summary>
    /// <value>The brush editor key.</value>
    public static ComponentResourceKey BrushEditorKey { get { return _BrushEditorKey; } }

    /// <summary>
    /// Gets the default category editor key.
    /// </summary>
    /// <value>The default category editor key.</value>
    public static ComponentResourceKey DefaultCategoryEditorKey { get { return _DefaultCategoryEditorKey; } }

    /// <summary>
    /// Gets the default complex property editor key.
    /// </summary>
    public static ComponentResourceKey ComplexPropertyEditorKey { get { return _ComplexPropertyEditorKey; } }
  }
}
