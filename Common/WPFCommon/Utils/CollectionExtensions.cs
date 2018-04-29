using System.Collections.Generic;

namespace Common.WPFCommon.Utils
{
	public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                collection.Add(value);
            }
        }
    }
}
