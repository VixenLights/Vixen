using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using System.IO;
using System.Xml;
using Vixen.IO;
using Vixen.Module;

namespace Vixen.Sys {
	public class ProfileDefinition : Definition, IFixtureContainer, IModuleDataContainer {
		private LinkedList<Fixture> _fixtures;

		private const string DIRECTORY_NAME = "Profile";
		private const string FILE_EXT = ".pro";

		/// <summary>
		/// Do not use.  Use ProfileDefinition.NewDefinition instead.
		/// </summary>
		public ProfileDefinition() {
			// Since one member has to be in here (ModuleData), they will both be.
			_fixtures = new LinkedList<Fixture>();
			ModuleDataSet = new ModuleDataSet();
		}

		[DataPath]
		static protected readonly string _profileDefinitionDirectory = Path.Combine(Definition._definitionDirectory, DIRECTORY_NAME);

		static public ProfileDefinition NewDefinition(string name, IModuleDataSet moduleDataSet, params Fixture[] fixtures) {
			ProfileDefinition profileDefinition = new ProfileDefinition();
			profileDefinition.ModuleDataSet = moduleDataSet ?? profileDefinition.ModuleDataSet;
			profileDefinition._fixtures = new LinkedList<Fixture>(fixtures);
			profileDefinition.Save(Path.Combine(_profileDefinitionDirectory, name + FILE_EXT));
			return profileDefinition;
		}

		static public ProfileDefinition Get(string name) {
			return Definition._GetInstance<ProfileDefinition>(_profileDefinitionDirectory, name, FILE_EXT);
		}

		static public IEnumerable<ProfileDefinition> GetAll() {
			return Definition._GetAll<ProfileDefinition>(_profileDefinitionDirectory, FILE_EXT);
		}

		public void InsertFixture(Fixture fixture, bool overwrite = false) {
			if(overwrite && _fixtures.Contains(fixture)) {
				if(fixture.Equals(_fixtures.First)) {
					_fixtures.Remove(fixture);
					_fixtures.AddFirst(fixture);
				} else {
					LinkedListNode<Fixture> prior = _fixtures.Find(fixture);
					_fixtures.Remove(fixture);
					_fixtures.AddAfter(prior, fixture);
				}
			} else {
				_fixtures.AddLast(fixture);
			}
		}

		public void RemoveFixture(Fixture fixture) {
			_fixtures.Remove(fixture);
		}

		public IEnumerable<Fixture> Fixtures {
			get { return _fixtures; }
		}

		public IModuleDataSet ModuleDataSet { get; private set; }

		protected override void ReadAttributes(XmlReader reader) { }

		protected override void ReadBody(XmlReader reader) {
			// This is duplicated from a Sequence, but that's to keep it independent.
			// Just because that changes doesn't mean this should.

			// Module data
			reader.ReadStartElement("ModuleData");
			ModuleDataSet.Deserialize(reader.ReadOuterXml());
			reader.ReadEndElement(); // ModuleData

			// Fixtures
			if(reader.ElementsExistWithin("Fixtures")) { // Container element for child entity
				FixtureReader fixtureReader = new FixtureReader();
				while(fixtureReader.Read(reader)) {
					InsertFixture(fixtureReader.Fixture);
				}
				reader.ReadEndElement(); // Fixtures
			}
		}

		protected override void WriteAttributes(XmlWriter writer) { }

		protected override void WriteBody(XmlWriter writer) {
			// Module data
			writer.WriteStartElement("ModuleData");
			writer.WriteRaw(ModuleDataSet.Serialize());
			writer.WriteEndElement(); // ModuleData

			// Fixtures
			FixtureWriter fixtureWriter = new FixtureWriter();
			writer.WriteStartElement("Fixtures");
			foreach(Fixture fixture in Fixtures) {
				fixtureWriter.Fixture = fixture;
				fixtureWriter.Write(writer);
			}
			writer.WriteEndElement(); // Fixtures
		}
	}
}
