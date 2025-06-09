#nullable enable

using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Services;

namespace Vixen.Sys.Props
{
    public abstract class BaseLightProp : BaseProp
    {
        private StringTypes _stringType;

        protected BaseLightProp(string name, PropType propType): base(name, propType)
        {
			StringType = StringTypes.Pixel;
		}

		[PropertyOrder(1)]
        [DisplayName("String Type")]
        public StringTypes StringType
        {
            get => _stringType;
            set => SetProperty(ref _stringType, value);
        }

        protected void AddStringElements(ElementNode node, int count, int nodesPerString, int namingIndex = 0)
        {
            for (int i = namingIndex; i < count + namingIndex; i++)
            {
                string stringName = $"{AutoPropStringName} {i + 1}";
                ElementNode stringNode = ElementNodeService.Instance.CreateSingle(node, stringName, true, false);

                if (StringType == StringTypes.Pixel)
                {
                    AddNodeElements(stringNode, nodesPerString);
                }
            }
        }
	}

}
