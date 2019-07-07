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
using System.Windows;
using System.Windows.Input;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Input;
using VixenModules.Editor.EffectEditor.Internal;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor
{
	public partial class EffectPropertyEditorGrid
	{
		/// <summary>
		///     Initializes the command bindings.
		/// </summary>
		protected void InitializeCommandBindings()
		{
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ResetFilter, OnResetFilterCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.Reload, OnReloadCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowReadOnlyProperties, OnShowReadOnlyPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideReadOnlyProperties, OnHideReadOnlyPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleReadOnlyProperties,
				OnToggleReadOnlyPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowAttachedProperties, OnShowAttachedPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideAttachedProperties, OnHideAttachedPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleAttachedProperties,
				OnToggleAttachedPropertiesCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ShowFilter, OnShowFilterCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.HideFilter, OnHideFilterCommand));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.ToggleFilter, OnToggleFilterCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowDialogEditor, OnShowDialogEditor));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.TogglePreview, OnTogglePreviewCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.AddCollectionItem, OnAddCollectionItemCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowGradientLevelCurveEditor, OnShowGradientLevelCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowGradientLevelGradientEditor, OnShowGradientLevelGradientCommand));

			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterXCurveEditor, OnShowEmitterXCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterYCurveEditor, OnShowEmitterYCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterFlowCurveEditor, OnShowEmitterFlowCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterVelocityCurveEditor, OnShowEmitterVelocityCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterSizeCurveEditor, OnShowEmitterSizeCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterLifetimeCurveEditor, OnShowEmitterLifetimeCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterGradientEditor, OnShowEmitterGradientCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterVelocityXCurveEditor, OnShowEmitterVelocityXCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterVelocityYCurveEditor, OnShowEmitterVelocityYCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterAngleCurveEditor, OnShowEmitterAngleCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterOscillationSpeedCurveEditor, OnShowEmitterOscillationSpeedCurveCommand));
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.ShowEmitterBrightnessCurveEditor, OnShowEmitterBrightnessCurveCommand));
		}

		#region Commands

		private void OnResetFilterCommand(object sender, ExecutedRoutedEventArgs e)
		{
			PropertyFilter = string.Empty;
		}

		private void OnReloadCommand(object sender, ExecutedRoutedEventArgs e)
		{
			DoReload();
		}

		private void OnShowReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowReadOnlyProperties = true;
		}

		private void OnHideReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowReadOnlyProperties = false;
		}

		private void OnToggleReadOnlyPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowReadOnlyProperties = !ShowReadOnlyProperties;
		}

		private void OnShowAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowAttachedProperties = true;
		}

		private void OnHideAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowAttachedProperties = false;
		}

		private void OnToggleAttachedPropertiesCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ShowAttachedProperties = !ShowAttachedProperties;
		}

		private void OnShowFilterCommand(object sender, ExecutedRoutedEventArgs e)
		{
			PropertyFilterVisibility = Visibility.Visible;
		}

		private void OnHideFilterCommand(object sender, ExecutedRoutedEventArgs e)
		{
			PropertyFilterVisibility = Visibility.Collapsed;
		}

		private void OnToggleFilterCommand(object sender, ExecutedRoutedEventArgs e)
		{
			PropertyFilterVisibility = (PropertyFilterVisibility == Visibility.Visible)
				? Visibility.Collapsed
				: Visibility.Visible;
		}

		private void OnAddCollectionItemCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var value = e.Parameter as PropertyItemValue;
			if (value != null)
			{
				value.AddItemToCollection();
			}
		}

		// TODO: refactoring needed
		private void OnShowDialogEditor(object sender, ExecutedRoutedEventArgs e)
		{
			var value = e.Parameter as PropertyItemValue;
			if (value != null)
			{
				ShowDialogEditor(value);
			}
			else
			{
				var collectionItem = e.Parameter as CollectionItemValue;
				if (collectionItem != null)
				{
					var grid = sender as EffectPropertyEditorGrid;
					if (grid != null)
					{
						Editors.Editor editor = grid.GetEditors().GetEditor(collectionItem.ItemType);
						ShowDialogEditor(collectionItem, editor);		
					}
					
				}
			}
		}

		private void OnShowGradientLevelGradientCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var collectionItem = e.Parameter as CollectionItemValue;
			if (collectionItem != null)
			{
				var grid = sender as EffectPropertyEditorGrid;
				if (grid != null)
				{
					Editors.Editor editor = grid.GetEditors().GetEditor(typeof(ColorGradient));
					GradientLevelPair glp = collectionItem.Value as GradientLevelPair;
					if (glp != null)
					{
						var newValue = editor.ShowDialog(collectionItem.ParentProperty, glp.ColorGradient, this);
						if (newValue is ColorGradient)
						{
							var newGradientLevelPair = new GradientLevelPair((ColorGradient)newValue, glp.Curve);
							collectionItem.Value = newGradientLevelPair;
						}
					}
				}
			}
		}

		private void OnShowGradientLevelCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var collectionItem = e.Parameter as CollectionItemValue;
			if (collectionItem != null)
			{
				var grid = sender as EffectPropertyEditorGrid;
				if (grid != null)
				{
					Editors.Editor editor = grid.GetEditors().GetEditor(typeof(Curve));
					GradientLevelPair glp = collectionItem.Value as GradientLevelPair;
					if (glp != null)
					{
						var newValue = editor.ShowDialog(collectionItem.ParentProperty, glp.Curve, this);
						if (newValue is Curve)
						{
							var newGradientLevelPair = new GradientLevelPair(glp.ColorGradient, (Curve)newValue);
							collectionItem.Value = newGradientLevelPair;
						}
					}
					
				}

			}
		}

		private void OnShowEmitterGradientCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var collectionItem = e.Parameter as CollectionItemValue;
			if (collectionItem != null)
			{
				var grid = sender as EffectPropertyEditorGrid;
				if (grid != null)
				{
					Editors.Editor editor = grid.GetEditors().GetEditor(typeof(ColorGradient)); 
					IEmitter glp = collectionItem.Value as IEmitter;
					if (glp != null)
					{
						var newValue = editor.ShowDialog(collectionItem.ParentProperty, glp.Color, this);
						if (newValue is ColorGradient)
						{
							IEmitter emit = glp.CreateInstanceForClone();
							emit.Color = (ColorGradient)newValue;
							collectionItem.Value = emit;
						}
					}
				}
			}
		}

	

		private void OnShowEmitterXCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.X = curve; }, (emitter) => { return emitter.X; });
		}

		private void OnShowEmitterYCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.Y = curve; }, (emitter) => { return emitter.Y; });
		}

		private void OnShowEmitterFlowCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.Flow = curve; }, (emitter) => { return emitter.Flow; });
		}

		private void OnShowEmitterVelocityCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.ParticleVelocity = curve; }, (emitter) => { return emitter.ParticleVelocity; });
		}

		private void OnShowEmitterLifetimeCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.Lifetime = curve; }, (emitter) => { return emitter.Lifetime; });
		}

		private void OnShowEmitterSizeCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.SourceSize = curve; }, (emitter) => { return emitter.SourceSize; });
		}

		private void OnShowEmitterVelocityXCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.VelocityX = curve; }, (emitter) => { return emitter.VelocityX; });
		}

		private void OnShowEmitterVelocityYCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.VelocityY = curve; }, (emitter) => { return emitter.VelocityY; });
		}

		private void OnShowEmitterAngleCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.Angle = curve; }, (emitter) => { return emitter.Angle; });
		}

		private void OnShowEmitterOscillationSpeedCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.OscillationSpeed = curve; }, (emitter) => { return emitter.OscillationSpeed; });
		}

		private void OnShowEmitterBrightnessCurveCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnShowEmitterCurveCommand(sender, e, (emitter, curve) => { emitter.Brightness = curve; }, (emitter) => { return emitter.Brightness; });
		}


		private void OnShowEmitterCurveCommand(object sender, ExecutedRoutedEventArgs e, Action<IEmitter, Curve> setCurve, Func<IEmitter, Curve> getCurve)
		{
			CollectionItemValue collectionItem = e.Parameter as CollectionItemValue;

			if (collectionItem != null)
			{
				EffectPropertyEditorGrid grid = sender as EffectPropertyEditorGrid;
				if (grid != null)
				{
					Editors.Editor editor = grid.GetEditors().GetEditor(typeof(Curve));
					IEmitter emitter = collectionItem.Value as IEmitter;

					if (emitter != null)
					{
						var newValue = editor.ShowDialog(collectionItem.ParentProperty, getCurve(emitter), this);

						if (newValue is Curve)
						{
							IEmitter newEmitter = emitter.CreateInstanceForClone();
							setCurve(newEmitter, (Curve)newValue);							
							collectionItem.Value = newEmitter;
						}
					}
				}
			}
		}


		private void ShowDialogEditor(PropertyItemValue value)
		{
			var property = value.ParentProperty;
			if (property == null) return;

			var editor = property.Editor;
			// TODO: Finish DialogTemplate implementation
			if (editor != null && !value.ParentProperty.IsReadOnly) // && editor.HasDialogTemplate)
			{
				value.Value = editor.ShowDialog(value.ParentProperty, value.Value, this);
			}	
		}

		private void ShowDialogEditor(CollectionItemValue value, Editors.Editor editor)
		{
			// TODO: Finish DialogTemplate implementation
			
			value.Value = editor.ShowDialog(value.ParentProperty, value.Value, this);
			
		}

		
		private void OnTogglePreviewCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnPreviewStateChanged((bool) e.Parameter);
		}

		#endregion
	}
}