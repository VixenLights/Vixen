using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public class ColorBreakdownModule : OutputFilterModuleInstanceBase
	{
		private ColorBreakdownData _data;
		private ColorBreakdownOutput[] _outputs;

		public override void Handle(IntentsDataFlowData obj)
		{
			foreach (ColorBreakdownOutput output in Outputs) {
				output.ProcessInputData(obj);
			}
		}

		public override DataFlowType InputDataType
		{
			get { return DataFlowType.MultipleIntents; }
		}

		public override DataFlowType OutputDataType
		{
			get { return DataFlowType.SingleCommand; }
		}

		public override IDataFlowOutput[] Outputs
		{
			get { return _outputs; }
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (ColorBreakdownData) value;
				_CreateOutputs();
			}
		}

		public List<ColorBreakdownItem> BreakdownItems
		{
			get { return _data.BreakdownItems; }
			set
			{
				_data.BreakdownItems = value;
				_CreateOutputs();
			}
		}

		public bool MixColors
		{
			get { return _data.MixColors; }
			set
			{
				_data.MixColors = value;
				_CreateOutputs();
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (ColorBreakdownSetup setup = new ColorBreakdownSetup(_data)) {
				if (setup.ShowDialog() == DialogResult.OK) {
					_data.BreakdownItems = setup.BreakdownItems;
					_CreateOutputs();
					return true;
				}
			}
			return false;
		}

		private void _CreateOutputs()
		{
			// Initialize the RGB to RGBW converter to null
			// If this converter is null then the logic below will use normal RGB filtering
			RGBToRGBWConverter rgbToRGBWConverter = null;

			// If mixing colors AND
			// the break down contains four colors then...
			if (_data.MixColors && 
			    _data.BreakdownItems.Count == 4)
			{
				// Verify the colors are in the expected order
				Debug.Assert(_data.BreakdownItems[0].Color == Color.Red);
				Debug.Assert(_data.BreakdownItems[1].Color == Color.Lime);
				Debug.Assert(_data.BreakdownItems[2].Color == Color.Blue);
				Debug.Assert(_data.BreakdownItems[3].Color == Color.White);

				// Create the RGB to RGBW converter
				rgbToRGBWConverter = new RGBToRGBWConverter();
			}
			
			// Create the outputs for the specified break down items
			_outputs = _data.BreakdownItems.Select(x => new ColorBreakdownOutput(x, _data.MixColors, rgbToRGBWConverter)).ToArray();
		}
	}
}