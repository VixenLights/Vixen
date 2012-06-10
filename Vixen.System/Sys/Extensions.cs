using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Vixen.Sys
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

		// http://stackoverflow.com/questions/3793/best-way-to-get-innerxml-of-an-xelement
		static public string InnerXml(this XElement element) {
			XmlReader reader = element.CreateReader();
			reader.MoveToContent();
			return reader.ReadInnerXml();
		}

		static public IEnumerable<T> AsEnumerable<T>(this T item) {
			yield return item;
		}

		static public void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> values) {
			foreach(T value in values) {
				hashSet.Add(value);
			}
		}

		static public T DynamicCast<T>(this object value) {
			dynamic val = value;
			return (T)val;
		}

		static public bool ContainsString(this string source, string value, StringComparison comparison = StringComparison.Ordinal) {
			return source.IndexOf(value, comparison) >= 0;
		}

		static public void Raise(this MulticastDelegate thisEvent, object sender, EventArgs e) {
			//AsyncCallback callback = new AsyncCallback(EndAsynchronousEvent);

			foreach(Delegate d in thisEvent.GetInvocationList()) {
				EventHandler uiMethod = d as EventHandler;
				if(uiMethod != null) {
					ISynchronizeInvoke target = d.Target as ISynchronizeInvoke;
					if(target != null) target.BeginInvoke(uiMethod, new[] { sender, e });
					else uiMethod.BeginInvoke(sender, e, null, uiMethod);
				}
			}
		}

		static public IEnumerable<T> NotNull<T>(this IEnumerable<T> values)
			where T : class {
			return values.Where(x => x != null);
		}

		static public T[] SubArray<T>(this T[] data, int index, int length) {
			T[] result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}

		static public T[] SubArray<T>(this T[] data, int index) {
			int length = data.Length - index;
			return SubArray(data, index, length);
		}

		/// <summary>
		/// Returns a color of highest-wins of each component.
		/// </summary>
		static public Color Combine(this Color color, Color otherColor) {
			return Color.FromArgb(
				Math.Max(color.R, otherColor.R),
				Math.Max(color.G, otherColor.G),
				Math.Max(color.B, otherColor.B));
		}

		static public Channel[] GetChannels(this IEnumerable<ChannelNode> nodes) {
			return nodes.SelectMany(x => x.GetChannelEnumerator()).ToArray();
		}
	}
}
