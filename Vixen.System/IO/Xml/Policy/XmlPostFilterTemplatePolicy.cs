using System.Linq;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Policy {
	class XmlPostFilterTemplatePolicy : PostFilterTemplatePolicy {
		private PostFilterTemplate _template;
		private XElement _content;

		private const string ELEMENT_FILTER_COLLECTIONS = "FilterCollections";

		public XmlPostFilterTemplatePolicy() {
		}

		public XmlPostFilterTemplatePolicy(PostFilterTemplate template, XElement content) {
			_template = template;
			_content = content;
		}

		protected override void WriteModuleDataSet() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			XElement element = serializer.WriteObject(_template.DataSet);
			_content.Add(element);
		}

		protected override void WriteOutputFilterCollections() {
			XmlPostFilterCollectionSerializer serializer = new XmlPostFilterCollectionSerializer();
			XElement element = new XElement(ELEMENT_FILTER_COLLECTIONS,
				_template.OutputFilters.Select(serializer.WriteObject));
			_content.Add(element);
		}

		protected override void ReadModuleDataSet() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			_template.DataSet = serializer.ReadObject(_content);
		}

		protected override void ReadOutputFilterCollections() {
			_template.ClearOutputFilters();
			
			XElement element = _content.Element(ELEMENT_FILTER_COLLECTIONS);
			if(element != null) {
				XmlPostFilterCollectionSerializer serializer = new XmlPostFilterCollectionSerializer();
				_template.AddOutputFilters(element.Elements().Select(serializer.ReadObject).NotNull());
			}
		}
	}
}
