using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Vixen.Extensions
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

	    public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> values)
	    {
		    foreach (T value in values)
		    {
			    collection.Remove(value);
		    }
	    }

		public static int RemoveAll<T>(this ICollection<T> coll, Func<T, bool> condition)
		{
			var itemsToRemove = coll.Where(condition).ToList();

			foreach (var itemToRemove in itemsToRemove)
			{
				coll.Remove(itemToRemove);
			}

			return itemsToRemove.Count;
		}

	    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
	    {
		    var sortableList = new List<T>(collection);
		    sortableList.Sort(comparison);

		    for (int i = 0; i < sortableList.Count; i++)
		    {
			    collection.Move(collection.IndexOf(sortableList[i]), i);
		    }
	    }

	    public static void Sort<T>(this ObservableCollection<T> collection)
	    {
		    var sortableList = new List<T>(collection);
		    sortableList.Sort();

		    for (int i = 0; i < sortableList.Count; i++)
		    {
			    collection.Move(collection.IndexOf(sortableList[i]), i);
		    }
	    }

	}
}
