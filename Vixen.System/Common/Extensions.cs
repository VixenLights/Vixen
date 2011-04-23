using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Vixen.Common
{
	static public class Extensions
	{
		static public bool ElementsExistWithin(this XmlReader reader, string name)
		{
            bool result = !reader.IsEmptyElement;
			reader.ReadStartElement(name);
			return result;
		}

        static public IEnumerable<Type> FindImplementationsWithin(this Type type, Assembly assembly) {
            // Must be an interface.
            if(type.IsInterface) {
                return
                    from t in assembly.GetTypes()
                    from i in t.GetInterfaces()
                    where i.Name == type.Name
                    select t;
            }
            return Enumerable.Empty<Type>();
        }

		static public IEnumerable<Type> FindConcreteImplementationsWithin(this Type type, Assembly assembly) {
			// Must be an interface.
			if(type.IsInterface) {
				return
					from t in assembly.GetTypes()
					where !t.IsAbstract
					from i in t.GetInterfaces()
					where i.Name == type.Name
					select t;
			}
			return Enumerable.Empty<Type>();
		}

        static public bool ImplementsInterface(this Type type, Type interfaceType) {
            return type.GetInterface(interfaceType.Name) != null;
        }

		static public IEnumerable<Type> GetAttributedInterfaces(this Assembly assembly, Type attributeType) {
			return assembly.GetTypes().SelectMany(x => x.GetAttributedInterfaces(attributeType));
		}

		static public IEnumerable<Type> GetAttributedInterfaces(this Type type, Type attributeType) {
			return
				from iface in type.GetInterfaces()
				where iface.GetCustomAttributes(attributeType, true).Length > 0
				select iface;
		}
		
		static public Type GetAttributedInterface(this Type type, Type attributeType) {
			return GetAttributedInterfaces(type, attributeType).FirstOrDefault();
        }

		static public IEnumerable<Type> GetAttributedTypes(this Assembly assembly, Type attributeType) {
			return
				from type in assembly.GetTypes()
				where type.GetCustomAttributes(attributeType, false).Length > 0
				select type;
		}

		static public XElement GetXElement(this XmlNode node) {
			XDocument xDoc = new XDocument();
			using(XmlWriter xmlWriter = xDoc.CreateWriter())
				node.WriteTo(xmlWriter);
			return xDoc.Root;
		}

		static public XmlNode GetXmlNode(this XElement element) {
			using(XmlReader xmlReader = element.CreateReader()) {
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc;
			}
		}

		static public IEnumerable<T> AsEnumerable<T>(this T item) {
			yield return item;
		}

	}
}
