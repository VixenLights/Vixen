using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using Vixen.Attributes;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace Launcher
{
	public class Module : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;
		private Data _data;

		public Module()
		{
			_data = new Data();
		}

		protected override void TargetNodesChanged()
		{
			
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();

			var value = new CommandValue(new StringCommand(string.Format("{0}|{1},{2}", "Launcher", _data.Executable, _data.Arguments)));

			foreach (var node in TargetNodes)
			{
				foreach (var elementNode in node.GetLeafEnumerator())
				{
					IIntent i = new CommandIntent(value, TimeSpan);
					_elementData.AddIntentForElement(elementNode.Element.Id, i, TimeSpan.Zero);	
				}
			}
				
		}

		protected override Vixen.Sys.EffectIntents _Render()
		{
			return _elementData;
		}
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as Data;
				IsDirty = true;
			}
		}
		public override bool ForceGenerateVisualRepresentation { get { return true; } }
	
		public override void GenerateVisualRepresentation(System.Drawing.Graphics g, System.Drawing.Rectangle clipRectangle)
		{
			try {

				string DisplayValue = string.Format("Launcher - {0}", Description);

				Font AdjustedFont =  Vixen.Common.Graphics.GetAdjustedFont(g, DisplayValue, clipRectangle, "Vixen.Fonts.DigitalDream.ttf", 48);
				using (var StringBrush = new SolidBrush(Color.White)) {
					using (var backgroundBrush = new SolidBrush(Color.DarkBlue)) {
						g.FillRectangle(backgroundBrush, clipRectangle);
					}
					g.DrawString(DisplayValue, AdjustedFont, StringBrush, clipRectangle.X + 2, 2);
					//base.GenerateVisualRepresentation(g, clipRectangle);
				}

			} catch (Exception e) {

				Console.WriteLine(e.ToString());
			}
		}

		[Value]
		[ProviderCategory(@"Config")]
		[DisplayName(@"Description")]
		[Description(@"Sets the description.")]
		[PropertyOrder(1)]
		public string Description
		{
			get
			{
				return _data.Description;
			}
			set
			{
				_data.Description=value;
				IsDirty=true;
				OnPropertyChanged();
			}
		}
		[Value]
		[ProviderCategory(@"Config")]
		[DisplayName(@"Executable")]
		[Description(@"Sets the executable.")]
		[PropertyOrder(2)]
		[PropertyEditor("FilePathEditor")]
		public string Executable
		{
			get
			{
				return _data.Executable;
			}
			set
			{
				_data.Executable=value;
				IsDirty=true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config")]
		[DisplayName(@"Arguments")]
		[Description(@"Sets the arguments to use on the executable.")]
		[PropertyOrder(3)]
		public string Arguments
		{
			get { return _data.Arguments; }
			set
			{
				_data.Arguments=value;
				IsDirty=true;
				OnPropertyChanged();
			}
		}

	

	}
}
