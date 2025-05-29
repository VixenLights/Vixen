

namespace Vixen.Sys.Props
{
    public class Arch: Prop
    {
        private int _lightCount;

        public Arch():this("Arch 1", 25)
        {
            
        }

        public Arch(string name, int lightCount): this(name, lightCount, StringTypes.Pixel)
        {
            
        }

        public Arch(string name, int lightCount = 25, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
        {
            LightCount = lightCount;
            //TODO create default element structure
            //TODO create Preview model
        }

		public int LightCount
        {
            get => _lightCount;
            set => SetProperty(ref _lightCount, value);
        }
    }
}