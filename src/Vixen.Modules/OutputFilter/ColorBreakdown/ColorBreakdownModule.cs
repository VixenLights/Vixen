using System.Diagnostics;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public class ColorBreakdownModule : OutputFilterModuleInstanceBase
	{
		private ColorBreakdownData _data;
		private IDataFlowOutput[] _outputs;

		public override void Handle(IntentsDataFlowData obj)
		{
			foreach (ColorBreakdownOutputBase output in Outputs) {
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

		public bool _16Bit
		{
			get { return _data._16Bit; }
			set
			{
				_data._16Bit = value;
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
				Debug.Assert(_data.BreakdownItems[0].Color.ToArgb() == Color.Red.ToArgb());
				Debug.Assert(_data.BreakdownItems[1].Color.ToArgb() == Color.Lime.ToArgb());
				Debug.Assert(_data.BreakdownItems[2].Color.ToArgb() == Color.Blue.ToArgb());
				Debug.Assert(_data.BreakdownItems[3].Color.ToArgb() == Color.White.ToArgb());

				// Create the RGB to RGBW converter
				rgbToRGBWConverter = new RGBToRGBWConverter();
			}

			// If 16 bit color support is needed then...
			if (_data._16Bit)
			{
				// Create the outputs for the specified break down items 
				// This output is going to produce 16 bit command values in the range (0-65535)
				_outputs = _data.BreakdownItems
					.Select(x => new _16BitColorBreakDownOutput(x, _data.MixColors, rgbToRGBWConverter)).ToArray();
			}
			else
			{
				// Create the outputs for the specified break down items
				// This output is going to produce 8 bit command values in the range (0-255)
				_outputs = _data.BreakdownItems
					.Select(x => new _8BitColorBreakdownOutput(x, _data.MixColors, rgbToRGBWConverter)).ToArray();
			}
		}
	}
}