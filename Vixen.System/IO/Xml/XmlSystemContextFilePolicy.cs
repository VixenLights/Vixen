using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemContextFilePolicy : SystemContextFilePolicy {
		private SystemContext _context;
		private XElement _content;

		private const string ELEMENT_SOURCE_IDENTITY = "SourceIdentity";
		private const string ELEMENT_CONTEXT_NAME = "Name";
		private const string ELEMENT_CONTEXT_DESCRIPTION = "Description";

		public XmlSystemContextFilePolicy() {
			// Used when wanting just the current version of the sequence file.
		}

		public XmlSystemContextFilePolicy(SystemContext context, XElement content) {
			_context = context;
			_content = content;
		}

		protected override void WriteSourceIdentity() {
			_content.Add(new XElement(ELEMENT_SOURCE_IDENTITY, _context.SourceIdentity.ToString()));
		}

		protected override void WriteContextName() {
			_content.Add(new XElement(ELEMENT_CONTEXT_NAME, _context.ContextName));
		}

		protected override void WriteContextDescription() {
			_content.Add(new XElement(ELEMENT_CONTEXT_DESCRIPTION, _context.ContextDescription));
		}

		protected override void WriteFiles() {
			XmlCompressedFileCollectionSerializer compressedFileCollectionSerializer = new XmlCompressedFileCollectionSerializer();
			_content.Add(compressedFileCollectionSerializer.WriteObject(_context));
		}

		protected override void ReadSourceIdentity() {
			XElement identityElement = _content.Element(ELEMENT_SOURCE_IDENTITY);
			if(identityElement != null) {
				_context.SourceIdentity = Guid.Parse(identityElement.Value);
			} else {
				throw new Exception("Source identity not found.");
			}
		}

		protected override void ReadContextName() {
			XElement nameElement = _content.Element(ELEMENT_CONTEXT_NAME);
			if(nameElement != null) {
				_context.ContextName = nameElement.Value;
			} else {
				throw new Exception("Context name not found.");
			}
		}

		protected override void ReadContextDescription() {
			XElement descriptionElement = _content.Element(ELEMENT_CONTEXT_NAME);
			if(descriptionElement != null) {
				_context.ContextDescription = descriptionElement.Value;
			} else {
				throw new Exception("Context description not found.");
			}
		}

		protected override void ReadFiles() {
			XmlCompressedFileCollectionSerializer compressedFileCollectionSerializer = new XmlCompressedFileCollectionSerializer();
			IEnumerable<IPackageFileContent> files = compressedFileCollectionSerializer.ReadObject(_content);
			foreach(IPackageFileContent file in files) {
				_context.AddFile(file);
			}
		}
	}
}
