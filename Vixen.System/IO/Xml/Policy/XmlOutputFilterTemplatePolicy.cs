using System.Linq;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Module;
using Vixen.Sys;

namespace Vixen.IO.Xml.Policy {
	class XmlOutputFilterTemplatePolicy : OutputFilterTemplatePolicy {
		private OutputFilterTemplate _template;
		private XElement _content;

		private const string ELEMENT_FILTER_COLLECTIONS = "FilterCollections";

		public XmlOutputFilterTemplatePolicy() {
		}

		public XmlOutputFilterTemplatePolicy(OutputFilterTemplate template, XElement content) {
			_template = template;
			_content = content;
		}

		protected override void WriteModuleDataSet() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			XElement element = serializer.WriteObject(_template.DataSet);
			_content.Add(element);
		}

		protected override void WriteOutputFilterCollections() {
			XmlOutputFilterCollectionSerializer serializer = new XmlOutputFilterCollectionSerializer();
			XElement element = new XElement(ELEMENT_FILTER_COLLECTIONS,
				_template.OutputFilters.Select(serializer.WriteObject));
			_content.Add(element);
		}

		protected override void ReadModuleDataSet() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			_template.DataSet = serializer.ReadObject(_content);
		}

		protected override void ReadOutputFilterCollections() {
			// We don't want to wipe out the dataset we just loaded, but we do want the
			// filter collection to leave.
			var dataSet = (ModuleLocalDataSet)_template.DataSet.Clone();
			_template.ClearOutputFilters();
			_template.DataSet = dataSet;
			
			XElement element = _content.Element(ELEMENT_FILTER_COLLECTIONS);
			if(element != null) {
				XmlOutputFilterCollectionSerializer serializer = new XmlOutputFilterCollectionSerializer();
				_template.AddOutputFilters(element.Elements().Select(serializer.ReadUnwrappedCollection).NotNull());
			}
		}
	}
}
