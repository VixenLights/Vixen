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
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Input;

namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Frequently used types cache used performance optimization.
  /// </summary>
  public static class KnownTypes
  {
    public static class Collections
    {
      public static readonly Type IList = typeof(IList);
    }

    public static class Attributes
    {
      public static readonly Type EditorBrowsableAttribute = typeof(EditorBrowsableAttribute);
      public static readonly Type MergablePropertyAttribute = typeof(MergablePropertyAttribute);
      public static readonly Type PropertyEditorAttribute = typeof(PropertyEditorAttribute);
      public static readonly Type CategoryEditorAttribute = typeof(CategoryEditorAttribute);
      public static readonly Type NotifyParentPropertyAttribute = typeof(NotifyParentPropertyAttribute);
    }

    public static class Wpf
    {
      public static readonly Type Geometry = typeof(Geometry);
      public static readonly Type CornerRadius = typeof(CornerRadius);
      public static readonly Type Point3D = typeof(Point3D);
      public static readonly Type Point4D = typeof(Point4D);
      public static readonly Type Point3DCollection = typeof(Point3DCollection);
      public static readonly Type Matrix3D = typeof(Matrix3D);
      public static readonly Type Quaternion = typeof(Quaternion);
      public static readonly Type Rect3D = typeof(Rect3D);
      public static readonly Type Size3D = typeof(Size3D);
      public static readonly Type Vector3D = typeof(Vector3D);
      public static readonly Type Vector3DCollection = typeof(Vector3DCollection);
      public static readonly Type PointCollection = typeof(PointCollection);
      public static readonly Type VectorCollection = typeof(VectorCollection);
      public static readonly Type Point = typeof(Point);
      public static readonly Type Rect = typeof(Rect);
      public static readonly Type Size = typeof(Size);
      public static readonly Type Thickness = typeof(Thickness);
      public static readonly Type Vector = typeof(Vector);
      public static readonly Type FontStretch = typeof(FontStretch);
      public static readonly Type FontStyle = typeof(FontStyle);
      public static readonly Type FontWeight = typeof(FontWeight);
      public static readonly Type FontFamily = typeof(FontFamily);
      public static readonly Type Cursor = typeof(Cursor);
      public static readonly Type Brush = typeof(Brush);
    }

    public static class Wpg
    {
      public static readonly Type Editor = typeof(Editor);
    }
  }
}
