using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Xml;
using NLog;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Services.EffectDefaults
{
	/// <summary>
	/// Manages the per-profile store of saved effect defaults: user-captured <c>ModuleData</c> settings that are
	/// applied to newly created instances of an effect type in place of the module's built-in constructor
	/// defaults. See the effect defaults ExecPlan at
	/// <c>docs/plans/effects/vix-3964-effect-default-settings.md</c> for the full design.
	/// </summary>
	/// <remarks>
	/// Saved defaults are stored profile-scoped in a single binary <see cref="DataContractSerializer"/>-encoded
	/// file at <see cref="Directory"/>\<see cref="FileName"/>. The store is loaded lazily on first access and is
	/// automatically reloaded whenever <see cref="Sys.Paths.DataRootPath"/> changes (for example, after switching
	/// profiles), so a stale cache is never served across a profile switch. All access to the in-memory store
	/// (<see cref="_entriesByTypeId"/> and <see cref="_loadedRootPath"/>) is serialized through
	/// <see cref="_loadLock"/>, including reads, so that a profile switch can never be observed mid-reload.
	/// </remarks>
	public sealed class EffectDefaultsService
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// The directory under the current profile's data root where the effect defaults store file lives. This
		/// member is scanned and auto-created at startup by <c>Vixen.Sys.Paths</c> because it is decorated with
		/// <c>[DataPath]</c> and declared on a type inside the <c>Vixen.Core</c> assembly.
		/// </summary>
		[DataPath] public static readonly string Directory = Path.Combine(Paths.DataRootPath, "Effect Defaults");

		/// <summary>
		/// The file name of the effect defaults store within <see cref="Directory"/>.
		/// </summary>
		public const string FileName = "EffectDefaults.vfd";

		private static readonly Lazy<EffectDefaultsService> LazyInstance = new(() => new EffectDefaultsService());

		private readonly ConcurrentDictionary<Type, DataContractSerializer> _serializers = new();
		private readonly object _loadLock = new();

		private Dictionary<Guid, EffectDefaultEntry> _entriesByTypeId;
		private string _loadedRootPath;

		private EffectDefaultsService()
		{
		}

		/// <summary>
		/// Gets the shared instance of the effect defaults service.
		/// </summary>
		public static EffectDefaultsService Instance => LazyInstance.Value;

		private static string FilePath => Path.Combine(Directory, FileName);

		/// <summary>
		/// Determines whether a saved default exists for the given effect type.
		/// </summary>
		/// <param name="effectTypeId">The effect type's descriptor <c>TypeId</c>.</param>
		/// <returns><see langword="true"/> if a saved default exists for <paramref name="effectTypeId"/>.</returns>
		public bool HasDefault(Guid effectTypeId)
		{
			lock (_loadLock)
			{
				EnsureLoadedLocked();
				return _entriesByTypeId.ContainsKey(effectTypeId);
			}
		}

		/// <summary>
		/// Creates a fresh <see cref="IModuleDataModel"/> instance from the saved default for the given effect
		/// module, if one exists. This never throws: any failure (no saved default, the effect type is no longer
		/// installed, a type mismatch, or a deserialization error) is caught, logged, and results in
		/// <see langword="null"/>, which callers must treat as "fall back to the effect's built-in constructor
		/// defaults."
		/// </summary>
		/// <param name="effectModule">The effect instance being created, used to determine its type.</param>
		/// <returns>A freshly deserialized <see cref="IModuleDataModel"/> if a valid saved default exists;
		/// otherwise <see langword="null"/>.</returns>
		public IModuleDataModel CreateDefaultData(IModuleInstance effectModule)
		{
			if (effectModule == null)
			{
				return null;
			}

			lock (_loadLock)
			{
				try
				{
					EnsureLoadedLocked();
					if (!_entriesByTypeId.TryGetValue(effectModule.TypeId, out EffectDefaultEntry entry))
					{
						return null;
					}

					Type dataType = Modules.GetDescriptorById(effectModule.TypeId)?.ModuleDataClass;
					if (dataType == null)
					{
						Logging.Warn("Effect default for type {0} could not be applied because that effect type is not currently installed.", effectModule.TypeId);
						return null;
					}

					if (!string.IsNullOrEmpty(entry.DataModelTypeName) && dataType.FullName != entry.DataModelTypeName)
					{
						Logging.Warn("Effect default for type {0} was saved for data model '{1}' but the installed data model is now '{2}'; ignoring the saved default.", effectModule.TypeId, entry.DataModelTypeName, dataType.FullName);
						return null;
					}

					DataContractSerializer serializer = GetOrAddSerializer(dataType);
					return (IModuleDataModel)ReadBinary(serializer, entry.Payload);
				}
				catch (Exception ex)
				{
					Logging.Warn(ex, "Failed to create default ModuleData for effect type {0}; falling back to built-in defaults.", effectModule.TypeId);
					return null;
				}
			}
		}

		/// <summary>
		/// Captures the given effect's current <c>ModuleData</c> as the new saved default for its effect type,
		/// replacing any existing saved default for that type. The live effect's <c>ModuleData</c> is never
		/// mutated: the capture pipeline serializes it, deserializes an independent copy, scrubs sequence-scoped
		/// members (see <see cref="EffectDefaultScrubber"/>) from the copy, and stores the scrubbed copy.
		/// </summary>
		/// <param name="effect">The effect instance whose current settings should become the new default for its
		/// effect type.</param>
		public void SaveDefault(IEffectModuleInstance effect)
		{
			if (effect == null)
			{
				throw new ArgumentNullException(nameof(effect));
			}

			Type dataType = effect.Descriptor.ModuleDataClass;
			DataContractSerializer serializer = GetOrAddSerializer(dataType);
			byte[] payloadBytes = CaptureScrubbedPayload(serializer, effect.ModuleData);

			var entry = new EffectDefaultEntry
			{
				TypeId = effect.Descriptor.TypeId,
				EffectName = effect.Descriptor.TypeName,
				DataModelTypeName = dataType.FullName,
				SavedUtc = DateTime.UtcNow,
				Payload = payloadBytes
			};

			lock (_loadLock)
			{
				EnsureLoadedLocked();
				_entriesByTypeId[entry.TypeId] = entry;
				PersistLocked();
			}
		}

		/// <summary>
		/// Deletes the saved default for the given effect type, if one exists. Does not alter any effect instance
		/// currently open in the editor; it only affects effects created after the deletion.
		/// </summary>
		/// <param name="effectTypeId">The effect type's descriptor <c>TypeId</c>.</param>
		/// <returns><see langword="true"/> if a saved default existed and was removed; otherwise
		/// <see langword="false"/>.</returns>
		public bool ClearDefault(Guid effectTypeId)
		{
			lock (_loadLock)
			{
				EnsureLoadedLocked();
				bool removed = _entriesByTypeId.Remove(effectTypeId);
				if (removed)
				{
					PersistLocked();
				}
				return removed;
			}
		}

		/// <summary>
		/// Forces the in-memory store to be reloaded from disk on next access, discarding any cached state. Used
		/// after an out-of-band change to the store file, such as an import.
		/// </summary>
		public void Reload()
		{
			lock (_loadLock)
			{
				LoadLocked();
			}
		}

		/// <summary>
		/// Gets a read-only summary of every currently saved effect default.
		/// </summary>
		/// <returns>One <see cref="EffectDefaultSummary"/> per saved default.</returns>
		public IReadOnlyCollection<EffectDefaultSummary> GetSummaries()
		{
			lock (_loadLock)
			{
				EnsureLoadedLocked();
				return _entriesByTypeId.Values
					.Select(entry => new EffectDefaultSummary(entry.TypeId, entry.EffectName, entry.SavedUtc, Modules.GetDescriptorById(entry.TypeId) != null))
					.ToList();
			}
		}

		// Callers must hold _loadLock.
		private void EnsureLoadedLocked()
		{
			if (_entriesByTypeId != null && _loadedRootPath == Paths.DataRootPath)
			{
				return;
			}

			LoadLocked();
		}

		// Callers must hold _loadLock.
		private void LoadLocked()
		{
			var entries = new Dictionary<Guid, EffectDefaultEntry>();

			try
			{
				if (File.Exists(FilePath))
				{
					byte[] bytes = File.ReadAllBytes(FilePath);
					DataContractSerializer serializer = GetOrAddSerializer(typeof(EffectDefaultsStore));
					var store = (EffectDefaultsStore)ReadBinary(serializer, bytes);
					foreach (EffectDefaultEntry entry in store.Entries)
					{
						entries[entry.TypeId] = entry;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Warn(ex, "Failed to load the effect defaults store at '{0}'; continuing with an empty store.", FilePath);
			}

			_entriesByTypeId = entries;
			_loadedRootPath = Paths.DataRootPath;
		}

		// Callers must hold _loadLock.
		private void PersistLocked()
		{
			System.IO.Directory.CreateDirectory(Directory);

			var store = new EffectDefaultsStore { Entries = _entriesByTypeId.Values.ToList() };
			DataContractSerializer serializer = GetOrAddSerializer(typeof(EffectDefaultsStore));
			byte[] bytes = WriteBinary(serializer, store);

			string tempPath = Path.Combine(Directory, FileName + ".tmp");
			File.WriteAllBytes(tempPath, bytes);

			if (File.Exists(FilePath))
			{
				File.Replace(tempPath, FilePath, null);
			}
			else
			{
				File.Move(tempPath, FilePath);
			}
		}

		private DataContractSerializer GetOrAddSerializer(Type type)
		{
			return _serializers.GetOrAdd(type, t => new DataContractSerializer(t));
		}

		/// <summary>
		/// Runs the capture pipeline described in the effect defaults ExecPlan's Decision Log: serialize the live
		/// data, deserialize an independent copy, scrub the copy's <c>[ExcludeFromEffectDefault]</c> members, and
		/// serialize the scrubbed copy. This never mutates <paramref name="liveData"/>, including any object it
		/// shares by reference with other live state (for example, a <c>Curve</c> shared via
		/// <c>PulseData.CreateInstanceForClone</c>), because the copy scrubbed here is a fresh object graph with
		/// no references in common with <paramref name="liveData"/>.
		/// </summary>
		/// <param name="serializer">A <see cref="DataContractSerializer"/> for <paramref name="liveData"/>'s
		/// runtime type.</param>
		/// <param name="liveData">The live effect's current <c>ModuleData</c>. Never modified.</param>
		/// <returns>The scrubbed copy, serialized with <paramref name="serializer"/>.</returns>
		internal static byte[] CaptureScrubbedPayload(DataContractSerializer serializer, IModuleDataModel liveData)
		{
			byte[] liveBytes = WriteBinary(serializer, liveData);
			var copy = (IModuleDataModel)ReadBinary(serializer, liveBytes);
			EffectDefaultScrubber.Scrub(copy);
			return WriteBinary(serializer, copy);
		}

		internal static byte[] WriteBinary(DataContractSerializer serializer, object value)
		{
			using var stream = new MemoryStream();
			using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
			{
				serializer.WriteObject(writer, value);
			}
			return stream.ToArray();
		}

		internal static object ReadBinary(DataContractSerializer serializer, byte[] bytes)
		{
			using var stream = new MemoryStream(bytes);
			using XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
			return serializer.ReadObject(reader);
		}
	}
}
