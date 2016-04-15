using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevel : BaseEffect
	{
		private SetLevelData _data;
		private EffectIntents _elementData = null;

		public SetLevel()
		{
			_data = new SetLevelData();
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();
			
			foreach (ElementNode node in TargetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
				{
					_elementData = RenderNode(node);
					//_elementData = IntentBuilder.ConvertToStaticArrayIntents(_elementData, TimeSpan, IsDiscrete());
					
				}
			}
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SetLevelData;
				IsDirty = true;
			}
		}

		#region Layer

		public override byte Layer
		{
			get { return _data.Layer; }
			set
			{
				_data.Layer = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		[Value]
		[ProviderCategory(@"Brightness",2)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyEditor("LevelEditor")]
		public double IntensityLevel
		{
			get { return _data.level; }
			set
			{
				_data.level = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color",1)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		public Color Color
		{
			get
			{
				//CheckForInvalidColorData();
				return _data.color;
			}
			set
			{
				_data.color = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		private bool IsDiscrete { get; set; }

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				IsDiscrete = true;
				if (!validColors.Contains(_data.color.ToArgb()))
				{
					//Our color is not valid for any elements we have.
					//Set a default color 
					Color = validColors.First();
				}
			}
			else
			{
				IsDiscrete = false;
			}
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private EffectIntents RenderNode(ElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			foreach (ElementNode elementNode in node.GetLeafEnumerator())
			{
				var intent = IsDiscrete ? CreateDiscreteIntent(Color, (float) HSV.FromRGB(Color).V * IntensityLevel, TimeSpan) : CreateIntent(Color, (float) HSV.FromRGB(Color).V *IntensityLevel, TimeSpan);
				effectIntents.AddIntentForElement(elementNode.Element.Id, intent, TimeSpan.Zero);
			}

			return effectIntents;
		}
		
	}
}