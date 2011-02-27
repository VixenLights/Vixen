using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface IFixtureContainer {
		IEnumerable<Fixture> Fixtures { get; }
		void InsertFixture(Fixture fixture, bool overwrite = false);
		void RemoveFixture(Fixture fixture);
	}
}
