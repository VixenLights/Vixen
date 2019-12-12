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
			
			foreach (IElementNode node in TargetNodes) {
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
				CheckForInvalidColorData();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/set-level/"; }
		}

		#endregion

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				if (!validColors.Contains(_data.color.ToArgb()))
				{
					//Our color is not valid for any elements we have.
					//Set a default color 
					Color = validColors.First();
				}
			}
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private EffectIntents RenderNode(IElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			var leafs = node.GetLeafEnumerator();
			foreach (IElementNode elementNode in leafs)
			{
				if (HasDiscreteColors && IsElementDiscrete(elementNode))
				{
					IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode, false);
					if (!colors.Contains(Color))
					{
						continue;
					}
				}
				var intent = CreateIntent(leafs.First(), Color, IntensityLevel, TimeSpan);
				effectIntents.AddIntentForElement(elementNode.Element.Id, intent, TimeSpan.Zero);
			}

			return effectIntents;
		}
		
	}
}