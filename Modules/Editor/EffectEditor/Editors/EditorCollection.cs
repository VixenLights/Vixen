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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Liquid;
using VixenModules.Effect.Morph;
using VixenModules.Effect.Wave;

namespace VixenModules.Editor.EffectEditor.Editors
{
	/// <summary>
	///     Defines a collection of value Editors (Type, Category and Property editors).
	/// </summary>
	public class EditorCollection : Collection<Editor>
	{
		private static readonly string EditorNamespace = typeof(Editor).Namespace;
		private static readonly Dictionary<Type, Editor> Cache = new Dictionary<Type, Editor>
		{
			{typeof (bool), new TypeEditor(typeof (bool), EditorKeys.BooleanEditorKey)},
			{KnownTypes.Wpf.Integer, new TypeEditor(KnownTypes.Wpf.Integer, EditorKeys.IntegerEditorKey)},
			{KnownTypes.Wpf.Double, new TypeEditor(KnownTypes.Wpf.Double, EditorKeys.DoubleEditorKey)},
			{KnownTypes.Vixen.Color, new ColorTypeEditor()},
			{KnownTypes.Vixen.Curve, new CurveEditor()},
			{KnownTypes.Vixen.ColorGradient, new GradientTypeEditor()},
			{typeof(List<ColorGradient>), new TypeEditor(typeof(List<ColorGradient>),EditorKeys.ColorGradientPaletteEditorKey)},
			{typeof(List<Color>), new TypeEditor(typeof(List<Color>),EditorKeys.ColorPaletteEditorKey)},
			{typeof(List<GradientLevelPair>), new TypeEditor(typeof(List<GradientLevelPair>),EditorKeys.GradientLevelPairEditorKey)},
			{typeof(List<string>), new TypeEditor(typeof(List<string>),EditorKeys.StringCollectionEditorKey)},
			{KnownTypes.Vixen.Percentage, new TypeEditor(KnownTypes.Vixen.Percentage, EditorKeys.SliderPercentageEditorKey)},
			{typeof (Enum), new TypeEditor(typeof (Enum), EditorKeys.EnumEditorKey)},
			{KnownTypes.Windows.Font, new FontEditor() },
			{typeof(IList<IEmitter>), new TypeEditor(typeof(IList<IEmitter>), EditorKeys.IEmitterEditorKey)},
			{KnownTypes.Vixen.Emitter, new TypeEditor()},
			{typeof(IList<IWaveform>), new TypeEditor(typeof(IList<IWaveform>), EditorKeys.IWaveformEditorKey)},
			{KnownTypes.Vixen.Waveform, new TypeEditor()},
			{typeof(IList<IMorphPolygon>), new TypeEditor(typeof(IList<IMorphPolygon>), EditorKeys.IMorphPolygonEditorKey)},
			{KnownTypes.Vixen.MorphPolygon, new TypeEditor()},
			{KnownTypes.Vixen.PolygonContainer, new PolygonContainerTypeEditor()},			
		};

		/// <summary>
		///     Finds the type editor.
		/// </summary>
		/// <param name="editedType">Edited type.</param>
		/// <returns>Editor for Type</returns>
		public TypeEditor FindTypeEditor(Type editedType)
		{
			if (editedType == null) throw new ArgumentNullException("editedType");

			return this
				.OfType<TypeEditor>()
				.FirstOrDefault(item => item.EditedType.IsAssignableFrom(editedType));
		}

		/// <summary>
		///     Finds the property editor.
		/// </summary>
		/// <param name="declaringType">Declaring type.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>Editor for Property</returns>
		public PropertyEditor FindPropertyEditor(Type declaringType, string propertyName)
		{
			if (declaringType == null) throw new ArgumentNullException("declaringType");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return this
				.OfType<PropertyEditor>()
				.Where(item => item.DeclaringType.IsAssignableFrom(declaringType))
				.FirstOrDefault(item => item.PropertyName == propertyName);
		}

		/// <summary>
		///     Finds the category editor.
		/// </summary>
		/// <param name="declaringType">Declaring type.</param>
		/// <param name="categoryName">Name of the category.</param>
		/// <returns>Editor for Category</returns>
		public CategoryEditor FindCategoryEditor(Type declaringType, string categoryName)
		{
			if (declaringType == null) throw new ArgumentNullException("declaringType");
			if (string.IsNullOrEmpty(categoryName)) throw new ArgumentNullException("categoryName");

			return this
				.OfType<CategoryEditor>()
				.Where(item => item.DeclaringType.IsAssignableFrom(declaringType))
				.FirstOrDefault(item => item.CategoryName == categoryName);
		}

		/// <summary>
		///     Gets the property editor by attributes.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <returns>Editor for Property</returns>
		public static Editor GetPropertyEditorByAttributes(AttributeCollection attributes)
		{
			if (attributes == null) return null;

			var attribute = attributes[KnownTypes.Attributes.PropertyEditorAttribute] as PropertyEditorAttribute;
			if (attribute == null) return null;

			try
			{
				var editorType = Type.GetType(attribute.EditorType)??Type.GetType(string.Format("{0}.{1}",EditorNamespace, attribute.EditorType));
				if (editorType == null || !KnownTypes.Editors.Editor.IsAssignableFrom(editorType)) return null;
				return (Editor) Activator.CreateInstance(editorType);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		///     Gets the category editor by attributes.
		/// </summary>
		/// <param name="declaringType">Type of the declaring.</param>
		/// <param name="categoryName">Name of the category.</param>
		/// <returns>Editor for Category</returns>
		public static Editor GetCategoryEditorByAttributes(Type declaringType, string categoryName)
		{
			if (declaringType == null || string.IsNullOrEmpty(categoryName)) return null;

			var name = categoryName.ToUpperInvariant();

			var attribute = declaringType
				.GetCustomAttributes(KnownTypes.Attributes.CategoryEditorAttribute, true)
				.OfType<CategoryEditorAttribute>()
				.FirstOrDefault(attr => attr.CategoryName == name);

			if (attribute == null) return null;

			try
			{
				var editorType = Type.GetType(attribute.EditorType);
				if (editorType == null || !KnownTypes.Editors.Editor.IsAssignableFrom(editorType)) return null;
				return (Editor) Activator.CreateInstance(editorType);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		///     Gets the editor.
		/// </summary>
		/// <param name="categoryItem">The category item.</param>
		/// <returns>Editor for Category</returns>
		public Editor GetEditor(CategoryItem categoryItem)
		{
			if (categoryItem == null) throw new ArgumentNullException("categoryItem");

			if (categoryItem.Owner == null)
				return null;

			var declaringObject = ObjectServices.GetUnwrappedObject(categoryItem.Owner.SelectedObject);
			if (declaringObject == null)
				return null;

			var declaringType = declaringObject.GetType();

			Editor editor = FindCategoryEditor(declaringType, categoryItem.Name);
			if (editor != null) return editor;

			editor = GetCategoryEditorByAttributes(declaringType, categoryItem.Name);
			if (editor != null) return editor;

			return new CategoryEditor(declaringType, categoryItem.Name, EditorKeys.DefaultCategoryEditorKey);
		}

		/// <summary>
		///     Gets the editor.
		/// </summary>
		/// <param name="propertyItem">The property item.</param>
		/// <returns>Editor for Property</returns>
		public Editor GetEditor(PropertyItem propertyItem)
		{
			if (propertyItem == null) throw new ArgumentNullException("propertyItem");

			Editor editor;

			if (propertyItem.IsReadOnly)
			{
				return new TypeEditor(propertyItem.PropertyType, EditorKeys.LabelEditorKey);
			}

			if (propertyItem.Attributes != null)
			{
				editor = GetPropertyEditorByAttributes(propertyItem.Attributes);
				if (editor != null) return editor;
			}

			if (propertyItem.Component != null && !string.IsNullOrEmpty(propertyItem.Name))
			{
				var declaringObject = ObjectServices.GetUnwrappedObject(propertyItem.Owner.SelectedObject);
				editor = FindPropertyEditor(declaringObject.GetType(), propertyItem.Name);
				if (editor != null) return editor;
			}

			var hasType = propertyItem.PropertyType != null;

			if (hasType)
			{
				editor = FindTypeEditor(propertyItem.PropertyType);
				if (editor != null) return editor;
			}

			if (hasType)
			{
				foreach (var cachedEditor in Cache)
				{
					if (cachedEditor.Key.IsAssignableFrom(propertyItem.PropertyType))
						return cachedEditor.Value;
				}
				
			}

			if (propertyItem.PropertyValue.HasSubProperties)
				return new TypeEditor(propertyItem.PropertyType, EditorKeys.ComplexPropertyEditorKey);

			if (hasType)
			{
				return new TypeEditor(propertyItem.PropertyType, EditorKeys.DefaultEditorKey);
			}

			return null;
		}

		public Editor GetEditor(Type declaringType)
		{
			Editor editor = FindTypeEditor(declaringType);
			if (editor != null) return editor;

			foreach (var cachedEditor in Cache)
			{
				if (cachedEditor.Key.IsAssignableFrom(declaringType))
					return cachedEditor.Value;
			}

			return new TypeEditor(declaringType, EditorKeys.DefaultEditorKey);
		}
	}
}