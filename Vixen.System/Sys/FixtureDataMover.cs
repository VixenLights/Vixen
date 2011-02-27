using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	class FixtureDataMover : ITypedDataMover {
		public string TypeOfData {
			get { return "Fixtures"; }
		}

		public bool CanHandle(object obj) {
			return obj is IFixtureContainer;
		}

		public void Copy(object source, object destination, bool overwrite) {
			// Yes, a misbehaving UI will cause problems if objects are not correctly typed.
			IFixtureContainer fixtureSource = source as IFixtureContainer;
			IFixtureContainer fixtureDestination = destination as IFixtureContainer;
			Fixture[] fixtures; // Want an already-enumerated set since the collections may be modified.

			if(!overwrite) {
				// Only copy fixtures that do not already exist.
				fixtures = fixtureSource.Fixtures.Except(fixtureDestination.Fixtures).ToArray();
			} else {
				fixtures = fixtureSource.Fixtures.ToArray();
				foreach(Fixture fixture in fixtures) {
					fixtureDestination.RemoveFixture(fixture);
				}
			}

			foreach(Fixture fixture in fixtures) {
				fixtureDestination.InsertFixture(fixture, overwrite);
			}
		}

		public void Move(object source, object destination, bool overwrite) {
			IFixtureContainer fixtureSource = source as IFixtureContainer;
			IFixtureContainer fixtureDestination = destination as IFixtureContainer;

			Copy(source, destination, overwrite);
			foreach(Fixture fixture in fixtureSource.Fixtures.ToArray()) {
				fixtureSource.RemoveFixture(fixture);
			}
		}
	}
}
