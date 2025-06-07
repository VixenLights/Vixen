#nullable enable
using Vixen.Module.Property;
using Vixen.Sys;

namespace VixenModules.Property.Order {
	public class OrderModule : PropertyModuleInstanceBase {

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private OrderData _data;

		public OrderModule() {
			_data = new OrderData();
		}

		public override void SetDefaultValues()
        {
            _data = new();
        }

		public int Order
		{
			get { return _data.Order; }
			set { _data.Order = value; }
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get {
				return _data;
			}
			set {
				_data = (OrderData)value;
			}
		}

		public static int GetOrderForElement(ElementNode element)
		{
			int p;
			OrderModule module = element.Properties.Get(OrderDescriptor.ModuleId) as OrderModule;
			if (module != null)
			{
				p = module.Order;
			}
			else
			{
				p = 0;
			}
			
			return p;
		}

        public static void AddUpdatePatchingOrder(IEnumerable<ElementNode> nodes)
        {
            int count = 1;
            foreach (ElementNode node in nodes)
            {
				OrderModule? order = null;
                if (node.Properties.TryGetValue(OrderDescriptor.ModuleId, out var propertyModule))
                {
                    if (propertyModule is OrderModule orderModule)
                    {
                        order = orderModule;
                    }
                }
                else
                {
                    order = node.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
                }
                
                if (order != null)
                {
                    order.Order = count;
                }

                count++;
            }
}

		public static void AddPatchingOrder(IEnumerable<ElementNode> nodes)
		{
			AddUpdatePatchingOrder(nodes);
		}

		public static void AddPatchingOrder(IEnumerable<ElementNode> nodes, int zigZagEvery)
		{
			AddUpdatePatchingOrder(nodes, zigZagEvery);
		}

        public static void AddUpdatePatchingOrder(IEnumerable<ElementNode> nodes, int zigZagEvery)
        {
            int totalCtr = 1;
            int groupCtr = 0;
            bool fwdIndicator = true;
            int grpStart = 1;
            foreach (ElementNode node in nodes)
            {
                OrderModule? order = null;
				if(node.Properties.TryGetValue(OrderDescriptor.ModuleId, out var propertyModule ))
                {
                    if (propertyModule is OrderModule orderModule)
                    {
                        order = orderModule;
                    }
                }
                else
				{
                    order = node.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
				}
               
                if (order != null)
                {
                    order.Order = fwdIndicator ? totalCtr : grpStart - groupCtr;
                }
                groupCtr++;

                if (groupCtr >= zigZagEvery)
                {
                    fwdIndicator = !fwdIndicator;
                    groupCtr = 0;
                    if (!fwdIndicator)
                    {
                        grpStart = totalCtr + zigZagEvery;
                    }
                }
                totalCtr++;
            }
        }

        /// <inheritdoc />
        public override void CloneValues(IProperty sourceProperty)
        {
            var source = sourceProperty as OrderModule;
            if (source == null)
            {
                Logging.Error(
                    "OrderModule: trying to CloneValues from another property, but it's not a OrderModule!");
                return;
            }

            _data.Order = source.Order;
        }

        #region Overrides of PropertyModuleInstanceBase

        /// <inheritdoc />
        public override bool HasSetup => true;
		
        public override bool Setup() {
            using (SetupForm setupForm = new SetupForm(_data)) {
                if (setupForm.ShowDialog() == DialogResult.OK) {
                    _data.Order = setupForm.Order;
                    return true;
                }
                return false;
            }
        }

        #endregion

	}
}
