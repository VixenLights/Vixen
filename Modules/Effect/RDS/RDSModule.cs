using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.RDS
{
	public class RDSModule : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;
		private RDSData _data;

		public RDSModule()
		{
			_data = new RDSData();

		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			CommandValue value = new CommandValue(new StringCommand(string.Format("{0}|{1}", "RDS", _data.Title)));

			foreach (IElementNode node in TargetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;
				 

				IIntent i = new CommandIntent(value, TimeSpan);
				_elementData.AddIntentForElement(node.Element.Id, i, TimeSpan.Zero);
			}
		}

		[Value]
		[Category(@"Effect Text")]
		[DisplayName(@"RDS Text")]
		[Description(@"Text to send")]
		public string Title
		{
			get { return _data.Title; }
			set
			{
				_data.Title=value;
				IsDirty=true;
				OnPropertyChanged();
			}
		}

		[Value]
		[Browsable(false)]
		public string Artist
		{
			get
			{
				return _data.Artist;
			}
			set
			{
				_data.Artist=value;
				IsDirty=true;
				OnPropertyChanged();
			}
		}



		public override void GenerateVisualRepresentation(System.Drawing.Graphics g, System.Drawing.Rectangle clipRectangle)
		{	
			try {

			string DisplayValue = string.Format("RDS - {0}", Title);

			Font AdjustedFont =  Vixen.Common.Graphics.GetAdjustedFont(g, DisplayValue, clipRectangle,"Vixen.Fonts.DigitalDream.ttf", 48);
			using (var StringBrush = new SolidBrush(Color.White)) {
				using (var backgroundBrush = new SolidBrush(Color.DarkGray)) {
					g.FillRectangle(backgroundBrush, clipRectangle);
				}
				g.DrawString(DisplayValue, AdjustedFont, StringBrush, clipRectangle.X + 2, 2);
				//base.GenerateVisualRepresentation(g, clipRectangle);
			}
		
			} catch (Exception e) {

				Console.WriteLine(e.ToString());
			}
		}
		public override bool ForceGenerateVisualRepresentation { get { return true; } }

		protected override void TargetNodesChanged()
		{
			//Nothing to do
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
				_data = value as RDSData;
				IsDirty = true;
			}
		}
	}
}
