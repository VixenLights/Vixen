using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Common.ValueTypes;
using Vixen.Attributes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Liquid;
using VixenModules.Effect.Morph;
using VixenModules.Effect.Wave;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

namespace VixenModules.Editor.EffectEditor
{
	/// <summary>
	///     Frequently used types cache used performance optimization.
	/// </summary>
	public static class KnownTypes
	{
		public static class Collections
		{
			public static readonly Type List = typeof (IList);
		}

		public static class Attributes
		{
			public static readonly Type EditorBrowsableAttribute = typeof (EditorBrowsableAttribute);
			public static readonly Type MergablePropertyAttribute = typeof (MergablePropertyAttribute);
			public static readonly Type PropertyEditorAttribute = typeof (PropertyEditorAttribute);
			public static readonly Type CategoryEditorAttribute = typeof (CategoryEditorAttribute);
			public static readonly Type NotifyParentPropertyAttribute = typeof (NotifyParentPropertyAttribute);
		}

		public static class Vixen
		{
			public static readonly Type Color = typeof (Color);
			public static readonly Type Curve = typeof (Curve);
			public static readonly Type ColorGradient = typeof (ColorGradient);
			public static readonly Type Percentage = typeof (Percentage);
			public static readonly Type Emitter = typeof(IEmitter);
			public static readonly Type Waveform = typeof(IWaveform);
			public static readonly Type PolygonContainer = typeof(PolygonContainer);
			public static readonly Type MorphPolygon = typeof(IMorphPolygon);
		}

		public static class Wpf
		{
			public static readonly Type FontStretch = typeof (FontStretch);
			public static readonly Type FontStyle = typeof (FontStyle);
			public static readonly Type FontWeight = typeof (FontWeight);
			public static readonly Type FontFamily = typeof (FontFamily);
			public static readonly Type Cursor = typeof (Cursor);
			public static readonly Type Brush = typeof (Brush);
			public static readonly Type Integer = typeof (int);
			public static readonly Type Double = typeof(double);
		}

		public static class Windows
		{
			public static readonly Type Font = typeof(Font);
			
		}

		public static class Editors
		{
			public static readonly Type Editor = typeof (EffectEditor.Editors.Editor);
		}
	}
}