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
using System.Windows.Input;

namespace VixenModules.Editor.EffectEditor.Input
{
	/// <summary>
	///Provides a standard set of property editor related commands.
	/// </summary>
	public static class PropertyEditorCommands
	{
		private static readonly Type ThisType = typeof (PropertyEditorCommands);

		private static readonly RoutedUICommand _ShowDialogEditor = new RoutedUICommand("Show Dialog Editor",
			"ShowDialogEditorCommand", ThisType);

		private static readonly RoutedUICommand _AddCollectionItem = new RoutedUICommand("Add Collection Item",
			"AddCollectionItemCommand", ThisType);

		private static readonly RoutedUICommand _RemoveCollectionItem = new RoutedUICommand("Remove Collection Item",
			"RemoveCollectionItemCommand", ThisType);

		private static readonly RoutedUICommand _ShowExtendedEditor = new RoutedUICommand("Show Extended Editor",
			"ShowExtendedEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowGradientLevelCurveEditor = new RoutedUICommand("Show Gradient Level Curve Editor",
			"ShowGradientLevelCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowGradientLevelGradientEditor = new RoutedUICommand("Show Gradient Level Gradient Editor",
			"ShowGradientLevelGradientEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterXCurveEditor = new RoutedUICommand("Show Emitter X Curve Editor",
			"ShowEmitterXCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterYCurveEditor = new RoutedUICommand("Show Emitter Y Curve Editor",
			"ShowEmitterYCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterFlowCurveEditor = new RoutedUICommand("Show Emitter Flow Curve Editor",
			"ShowEmitterFlowCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterVelocityCurveEditor = new RoutedUICommand("Show Emitter ParticleVelocity Curve Editor",
			"ShowEmitterVelocityCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterSizeCurveEditor = new RoutedUICommand("Show Emitter Size Curve Editor",
			"ShowEmitterSizeCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterLifetimeCurveEditor = new RoutedUICommand("Show Emitter Lifetime Curve Editor",
			"ShowEmitterLifetimeCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterGradientEditor = new RoutedUICommand("Show Emitter Gradient Editor",
			"ShowEmitterGradientEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterVelocityXCurveEditor = new RoutedUICommand("Show Emitter ParticleVelocity X Curve Editor",
			"ShowEmitterVelocityYCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterVelocityYCurveEditor = new RoutedUICommand("Show Emitter ParticleVelocity Y Curve Editor",
			"ShowEmitterVelocityXCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterAngleCurveEditor = new RoutedUICommand("Show Emitter Angle Curve Editor",
			"ShowEmitterAngleCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterOscillationSpeedCurveEditor = new RoutedUICommand("Show Emitter Oscillation Speed Curve Editor",
			"ShowEmitterOscillationSpeedCurveEditorCommand", ThisType);

		private static readonly RoutedUICommand _ShowEmitterBrightnessCurveEditor = new RoutedUICommand("Show Emitter Brightness Curve Editor",
			"ShowEmitterBrightnessCurveEditorCommand", ThisType);

		/// <summary>
		/// Defines a command to show dialog editor for a property.
		/// </summary>    
		public static RoutedUICommand ShowDialogEditor
		{
			get { return _ShowDialogEditor; }
		}

		/// <summary>
		/// Defines a command to show dialog editor for a GradientLevelPair property.
		/// </summary>    
		public static RoutedUICommand ShowGradientLevelCurveEditor
		{
			get { return _ShowGradientLevelCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterXCurveEditor
		{
			get { return _ShowEmitterXCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterYCurveEditor
		{
			get { return _ShowEmitterYCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterFlowCurveEditor
		{
			get { return _ShowEmitterFlowCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterVelocityCurveEditor
		{
			get { return _ShowEmitterVelocityCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterLifetimeCurveEditor
		{
			get { return _ShowEmitterLifetimeCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterVelocityXCurveEditor
		{
			get { return _ShowEmitterVelocityXCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterVelocityYCurveEditor
		{
			get { return _ShowEmitterVelocityYCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterSizeCurveEditor
		{
			get { return _ShowEmitterSizeCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterAngleCurveEditor
		{
			get { return _ShowEmitterAngleCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterGradientEditor
		{
			get { return _ShowEmitterGradientEditor; }
		}

		public static RoutedUICommand ShowEmitterOscillationSpeedCurveEditor
		{
			get { return _ShowEmitterOscillationSpeedCurveEditor; }
		}

		public static RoutedUICommand ShowEmitterBrightnessCurveEditor
		{
			get { return _ShowEmitterBrightnessCurveEditor; }
		}

		/// <summary>
		/// Defines a command to show dialog editor for a GradientLevelPair property.
		/// </summary>    
		public static RoutedUICommand ShowGradientLevelGradientEditor
		{
			get { return _ShowGradientLevelGradientEditor; }
		}

		/// <summary>
		/// Defines a command to add a itme to a collection property.
		/// </summary>    
		public static RoutedUICommand AddCollectionItem
		{
			get { return _AddCollectionItem; }
		}

		/// <summary>
		/// Defines a command to add a itme to a collection property.
		/// </summary>    
		public static RoutedUICommand RemoveCollectionItem
		{
			get { return _RemoveCollectionItem; }
		}

		/// <summary>
		/// Defines a command to show extended editor for a property.
		/// </summary>
		public static RoutedUICommand ShowExtendedEditor
		{
			get { return _ShowExtendedEditor; }
		}
	}
}