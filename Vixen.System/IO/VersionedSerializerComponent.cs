using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.IO {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of the object being serialized.</typeparam>
	/// <typeparam name="U">Type of what the object is being serialized to.</typeparam>
	//*** get rid of this
	abstract class VersionedSerializerComponent<T, U> : ISerializer<T, U> 
		where T : class
		where U : class {
		protected abstract int ComponentVersion { get; }
		protected abstract int ObjectVersion { get; }

		protected delegate U ObjectMigrationAction(U serializedObject);

		abstract public U WriteObject(T value);

		public T ReadObject(U serializedObject) {
			if(_MigrationNeeded()) {
				Backup(ObjectVersion);
				serializedObject = _Migrate(serializedObject, ObjectVersion, ComponentVersion);
			}

			return DoReadObject(serializedObject);
		}

		abstract protected T DoReadObject(U serializedObject);
		abstract protected void Backup(int objectVersion);

		private U _Migrate(U serializedObject, int fromVersion, int toVersion) {
			var migrations = GetNecessaryMigrations(fromVersion, toVersion);
			return migrations.Aggregate(serializedObject, (migratingObject, migrationAction) => migrationAction(migratingObject));
		}

		private bool _MigrationNeeded() {
			switch(Comparer<int>.Default.Compare(ComponentVersion, ObjectVersion)) {
				case -1: // component version < object version
					throw new Exception("This file was created by a newer version and cannot be loaded by this version.");
				case 1: // component version > object version
					return true;
			}
			return false;
		}

		//Each component to have its own version?
		abstract protected IEnumerable<ObjectMigrationAction> GetNecessaryMigrations(int fromVersion, int toVersion);
	}
}
