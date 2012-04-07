using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Generator {
	public class ColorCommandGenerator : Generator {
		override public void Handle(ICombinator<float> obj) {
			Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
		}

		override public void Handle(ICombinator<Color> obj) {
			Value = new ColorValue(obj.Value);
		}

		override public void Handle(ICombinator<long> obj) {
			Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
		}

		override public void Handle(ICombinator<double> obj) {
			Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
		}
	}
	//public class ColorCommandGenerator : IGenerator, IAnyCombinatorHandler {
	//    public void GenerateCommand(ICombinator combinator) {
	//        combinator.Dispatch(this);
	//    }

	//    public ICommand Value { get; private set; }

	//    public void Handle(ICombinator<float> obj) {
	//        Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
	//    }

	//    public void Handle(ICombinator<DateTime> obj) {
	//        // Ignored
	//    }

	//    public void Handle(ICombinator<Color> obj) {
	//        Value = new ColorValue(obj.Value);
	//    }

	//    public void Handle(ICombinator<long> obj) {
	//        Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
	//    }

	//    public void Handle(ICombinator<double> obj) {
	//        //// 0-1 representing 0-100%
	//        //byte value = (byte)(obj.Value * byte.MaxValue);
	//        //Value = new ColorValue(Helper.ConvertToGrayscale(value));
	//        Value = new ColorValue(Helper.ConvertToGrayscale(obj.Value));
	//    }

	//    //private Color _ConvertToGrayscale<T>(ICombinator<T> combinator) {
	//    //    // Convert to grayscale, capping at 255.
	//    //    byte value = _ConvertToByte(combinator);
	//    //    return _ConvertToGrayscale(value);
	//    //}

	//    //private Color _ConvertToGrayscale(byte value) {
	//    //    return Color.FromArgb(value, value, value);
	//    //}

	//    //private byte _ConvertToByte<T>(ICombinator<T> combinator) {
	//    //    int i = Convert.ToInt32(combinator.Value);
	//    //    i = Math.Max(byte.MinValue, i);
	//    //    i = Math.Min(byte.MaxValue, i);
	//    //    return (byte)i;
	//    //}
	//}
}
