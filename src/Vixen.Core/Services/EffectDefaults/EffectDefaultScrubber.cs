using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using Vixen.Sys.Attribute;

namespace Vixen.Services.EffectDefaults
{
	/// <summary>
	/// Walks a deserialized effect <c>ModuleData</c> object graph and resets every member decorated with
	/// <see cref="ExcludeFromEffectDefaultAttribute"/> back to its type's default value, in place. This is used
	/// by <see cref="EffectDefaultsService"/> as part of capturing a saved effect default: sequence-scoped values
	/// such as a Mark Collection identifier must never be carried from the sequence a default was saved in into
	/// an unrelated sequence. The walk never modifies library links (for example on a <c>Curve</c> or
	/// <c>ColorGradient</c>) — saved defaults intentionally keep those live.
	/// </summary>
	public static class EffectDefaultScrubber
	{
		private const int MaxDepth = 32;

		private static readonly Type[] LeafTypes =
		{
			typeof(string),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Color)
		};

		/// <summary>
		/// Resets every <see cref="ExcludeFromEffectDefaultAttribute"/>-decorated member reachable from
		/// <paramref name="root"/> to its type's default value. Safe to call on <see langword="null"/> (no-op).
		/// </summary>
		/// <param name="root">The root of the object graph to scrub, typically a freshly deserialized copy of an
		/// effect's <c>ModuleData</c>. The graph is mutated in place.</param>
		public static void Scrub(object root)
		{
			Walk(root, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
		}

		private static void Walk(object node, int depth, HashSet<object> visited)
		{
			if (node == null || depth > MaxDepth)
			{
				return;
			}

			Type nodeType = node.GetType();
			if (!nodeType.IsValueType)
			{
				if (visited.Contains(node))
				{
					return;
				}
				visited.Add(node);
			}

			if (node is IEnumerable enumerable && node is not string)
			{
				foreach (object element in enumerable)
				{
					Walk(element, depth + 1, visited);
				}
				return;
			}

			foreach (PropertyInfo property in nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (property.GetIndexParameters().Length > 0 || property.GetCustomAttribute<DataMemberAttribute>() == null)
				{
					continue;
				}

				if (property.GetCustomAttribute<ExcludeFromEffectDefaultAttribute>() != null)
				{
					if (property.CanWrite)
					{
						property.SetValue(node, DefaultValue(property.PropertyType));
					}
					continue;
				}

				if (!property.CanRead)
				{
					continue;
				}

				object value = property.GetValue(node);
				if (value == null || IsLeafType(property.PropertyType))
				{
					continue;
				}

				Walk(value, depth + 1, visited);
			}

			foreach (FieldInfo field in nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				if (field.GetCustomAttribute<DataMemberAttribute>() == null)
				{
					continue;
				}

				if (field.GetCustomAttribute<ExcludeFromEffectDefaultAttribute>() != null)
				{
					field.SetValue(node, DefaultValue(field.FieldType));
					continue;
				}

				object value = field.GetValue(node);
				if (value == null || IsLeafType(field.FieldType))
				{
					continue;
				}

				Walk(value, depth + 1, visited);
			}
		}

		private static bool IsLeafType(Type type)
		{
			return type.IsPrimitive || type.IsEnum || LeafTypes.Contains(type);
		}

		private static object DefaultValue(Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}
