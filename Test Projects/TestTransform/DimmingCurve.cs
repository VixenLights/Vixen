using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.IO;
using System.Xml.Linq;

// Using double values and percents because:
// * The commands will not assume a 256-level value range.  Percent is applicable to any range.
// * If integer percent values are used, 60% of the resolution is lost.

// This class will no do any implicit validation on the data.  It is up to the UI and
// the user to ensure that there aren't gaps over overlaps.

namespace TestTransform {
	public class DimmingCurve {
		private const string FILE_EXT = ".crv";
		private const string ELEMENT_ROOT = "DimmingCurve";
		private const string ELEMENT_ITEM = "Item";
		private const string ATTR_RANGE = "range";
		private const string ATTR_LEVEL = "level";

		//[DataPath]
		//static private string _directory = Path.Combine(Paths.DataRootPath, "DimmingCurve");

		// Using a list because I want the items ordered.
		private List<Item> _items = new List<Item>();

		private DimmingCurve(string name, bool createDefaultMapping) {
			Name = name;

			if(createDefaultMapping) {
				// Create a default identity mapping based on 256 discrete levels.
				double itemRange = (double)100 / 256; // 100 percent points divided into 256 parts.
				for(int i = 0; i < 256; i++) {
					_items.Add(new Item() { Start = i * itemRange, Range = itemRange, Level = i * itemRange });
				}
			}
		}

		/// <summary>
		/// Create a new dimming curve.
		/// </summary>
		/// <param name="name"></param>
		public DimmingCurve(string name)
			: this(name, true) {
		}

		/// <summary>
		/// Load an existing dimming curve.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		static public DimmingCurve Load(string fileName) {
			if(string.IsNullOrEmpty(fileName)) return null;
			if(!fileName.EndsWith(FILE_EXT)) {
				fileName = fileName + FILE_EXT;
			}

			DimmingCurve dimmingCurve = null;

			fileName = Path.Combine(Module._directory, fileName);

			if(File.Exists(fileName)) {
				dimmingCurve = new DimmingCurve(Path.GetFileNameWithoutExtension(fileName), false);
				//XElement root = Helper.LoadXml(fileName).Element("DimmingCurve");
				//XElement root = Helper.LoadXml(fileName);
				if(File.Exists(fileName)) {
					using(FileStream fileStream = new FileStream(fileName, FileMode.Open)) {
						using(StreamReader reader = new StreamReader(fileStream)) {
							XElement root = XElement.Load(reader);

							foreach(XElement itemElement in root.Elements("Item")) {
								dimmingCurve._items.Add(new Item() {
									Start = double.Parse(itemElement.Attribute("start").Value),
									Range = double.Parse(itemElement.Attribute("range").Value),
									Level = double.Parse(itemElement.Attribute("level").Value)
								});
							}
						}
					}
				}
			}

			return dimmingCurve;
		}

		static public DimmingCurve[] GetAll() {
			return
				(from fileName in Directory.GetFiles(Module._directory, "*" + FILE_EXT)
				 select Load(fileName)).ToArray();
		}

		public void Save() {
			string fileName = Path.Combine(Module._directory, Name);
			fileName = Path.ChangeExtension(fileName, FILE_EXT);

			XElement root = new XElement("DimmingCurve",
				(from item in _items
				 select new XElement("Item",
					 new XAttribute("start", item.Start),
					 new XAttribute("range", item.Range),
					 new XAttribute("level", item.Level))));

			root.Save(fileName);
		}

		public IEnumerable<Item> Items {
			get { return _items.ToArray(); }
		}

		public string Name { get; private set; }

		/// <summary>
		/// Inserts a new range into the dimming curve, replacing anything that it overlaps.
		/// </summary>
		/// <param name="rangeStart">Start of the range of the source value, 0 - 100%.</param>
		/// <param name="rangeWidth">Width of the range to be modified, 0 - 100%.</param>
		/// <param name="outputLevel">Value the range will be output at, 0 - 100%.</param>
		public void Insert(double rangeStart, double rangeWidth, double outputLevel) {
			// Want the index of the first item that is greater than rangeStart.
			// Not found means add to the end.
			int i = 0;
			for(; i < _items.Count; i++) {
				if(_items[i].Start >= rangeStart) break;
			}
			_items.Insert(i, new Item() { Start = rangeStart, Range = rangeWidth, Level = outputLevel });
			//(from item in _items.Select( (selectItem,index) => new { TheItem = selectItem, Index = index })
			// where item.TheItem.Start >= rangeStart
			// select new { FinalIndex = item.Index, FinalItem = new Item() { Start = rangeStart, Range = rangeWidth, Level = outputLevel } })
		}

		public bool Find(double inputLevel, out double outputLevel) {
			// Range is inclusive.
			Item item = _items.FirstOrDefault(x => x.Start <= inputLevel && (x.Start + x.Range) >= inputLevel);
			if(item != Item.Empty) {
				outputLevel = item.Level;
				return true;
			} else {
				outputLevel = inputLevel;
				return false;
			}
		}

		public void RemoveAt(int index) {
			_items.RemoveAt(index);
		}

		public void Remove(double rangeStart) {
			Item item = _items.First(x => x.Start == rangeStart);
			_items.Remove(item);
		}

		#region struct Item
		public struct Item : IEquatable<Item>, IComparable<Item> {
			public double Start;
			public double Range;
			public double Level;
			
			public bool Equals(Item other) {
				return this.Start == other.Start;
			}

			public int CompareTo(Item other) {
				return this.Start.CompareTo(other.Start);
			}

			public override bool Equals(object obj) {
				if(obj is Item) {
					return this.Equals((Item)obj);
				} else {
					return base.Equals(obj);
				}
			}

			public override int GetHashCode() {
				return Start.GetHashCode();
			}

			public static bool operator ==(Item left, Item right) {
				return left.Equals(right);
			}

			public static bool operator !=(Item left, Item right) {
				return !left.Equals(right);
			}

			public static readonly Item Empty = new Item() { Start = -1 };
		}
		#endregion
	}
}
