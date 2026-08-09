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

		/// <summary>
		/// Writes the saved defaults for the given effect types to a file, in the same binary format used for the
		/// primary store. The written entries carry their already-scrubbed payload bytes exactly as captured by
		/// <see cref="SaveDefault"/>; nothing is re-serialized or re-scrubbed for export.
		/// </summary>
		/// <param name="path">The file to write. Overwritten if it already exists.</param>
		/// <param name="effectTypeIds">The effect type <c>TypeId</c>s whose saved defaults should be exported.
		/// Any id with no saved default is silently skipped.</param>
		public void Export(string path, IEnumerable<Guid> effectTypeIds)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (effectTypeIds == null)
			{
				throw new ArgumentNullException(nameof(effectTypeIds));
			}

			EffectDefaultsStore store;
			lock (_loadLock)
			{
				EnsureLoadedLocked();
				store = BuildExportStore(_entriesByTypeId, effectTypeIds);
			}

			DataContractSerializer serializer = GetOrAddSerializer(typeof(EffectDefaultsStore));
			byte[] bytes = WriteBinary(serializer, store);
			File.WriteAllBytes(path, bytes);
		}

		/// <summary>
		/// Builds the subset <see cref="EffectDefaultsStore"/> that <see cref="Export"/> writes to disk, containing
		/// only the requested entries. Factored out from <see cref="Export"/> so it can be exercised without any
		/// disk or singleton-store dependency.
		/// </summary>
		/// <param name="entries">The current entries, keyed by effect type <c>TypeId</c>.</param>
		/// <param name="effectTypeIds">The effect type <c>TypeId</c>s to include. Any id not present in
		/// <paramref name="entries"/> is silently skipped.</param>
		/// <returns>A new store containing only the requested entries.</returns>
		internal static EffectDefaultsStore BuildExportStore(IReadOnlyDictionary<Guid, EffectDefaultEntry> entries, IEnumerable<Guid> effectTypeIds)
		{
			var store = new EffectDefaultsStore();
			foreach (Guid typeId in effectTypeIds)
			{
				if (entries.TryGetValue(typeId, out EffectDefaultEntry entry))
				{
					store.Entries.Add(entry);
				}
			}
			return store;
		}

		/// <summary>
		/// Reads a file previously written by <see cref="Export"/> and merges its entries into the current store,
		/// then persists the merged store to disk. Every entry in the imported file is upserted by
		/// <c>TypeId</c>; entries already in the current store that are not present in the imported file are left
		/// untouched.
		/// </summary>
		/// <param name="path">The file to import, previously written by <see cref="Export"/>.</param>
		/// <param name="mode">The merge strategy to use. Only <see cref="ImportMode.Overwrite"/> is currently
		/// supported.</param>
		/// <returns>How many entries were newly added versus how many overwrote an existing entry.</returns>
		public EffectDefaultsImportResult Import(string path, ImportMode mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (mode != ImportMode.Overwrite)
			{
				throw new ArgumentOutOfRangeException(nameof(mode), mode, "Only ImportMode.Overwrite is currently supported.");
			}

			byte[] bytes = File.ReadAllBytes(path);
			DataContractSerializer serializer = GetOrAddSerializer(typeof(EffectDefaultsStore));
			var importedStore = (EffectDefaultsStore)ReadBinary(serializer, bytes);

			EffectDefaultsImportResult result;

			lock (_loadLock)
			{
				EnsureLoadedLocked();
				result = MergeEntries(_entriesByTypeId, importedStore.Entries);
				PersistLocked();
			}

			Reload();

			return result;
		}

		/// <summary>
		/// Upserts each entry from an imported file into the current entries, by <c>TypeId</c>. Factored out from
		/// <see cref="Import"/> so the merge counting logic can be exercised without any disk or singleton-store
		/// dependency.
		/// </summary>
		/// <param name="entries">The current entries, keyed by effect type <c>TypeId</c>. Mutated in place.</param>
		/// <param name="importedEntries">The entries read from the imported file.</param>
		/// <returns>How many entries were newly added versus how many overwrote an existing entry.</returns>
		internal static EffectDefaultsImportResult MergeEntries(Dictionary<Guid, EffectDefaultEntry> entries, IEnumerable<EffectDefaultEntry> importedEntries)
		{
			var result = new EffectDefaultsImportResult();
			foreach (EffectDefaultEntry entry in importedEntries)
			{
				if (entries.ContainsKey(entry.TypeId))
				{
					result.Overwritten++;
				}
				else
				{
					result.Imported++;
				}

				entries[entry.TypeId] = entry;
			}
			return result;
		}

		/// <summary>
		/// Writes the current store to a file as readable, indented XML, for troubleshooting. The dump is produced
		/// from the exact same in-memory <see cref="EffectDefaultsStore"/> object graph used for the binary store
		/// file, through a plain <see cref="XmlWriter"/> instead of a binary one, so it can never drift out of
		/// sync with what is actually stored.
		/// </summary>
		/// <param name="path">The file to write. Overwritten if it already exists.</param>
		public void WriteDiagnosticDump(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			EffectDefaultsStore store;
			lock (_loadLock)
			{
				EnsureLoadedLocked();
				store = new EffectDefaultsStore { Entries = _entriesByTypeId.Values.ToList() };
			}

			DataContractSerializer serializer = GetOrAddSerializer(typeof(EffectDefaultsStore));
			WriteIndentedXml(serializer, store, path);
		}

		/// <summary>
		/// Serializes <paramref name="value"/> to <paramref name="path"/> as indented XML. Factored out from
		/// <see cref="WriteDiagnosticDump"/> so it can be exercised without any disk-path or singleton-store
		/// dependency beyond the destination file itself.
		/// </summary>
		/// <param name="serializer">A <see cref="DataContractSerializer"/> for <paramref name="value"/>'s runtime
		/// type.</param>
		/// <param name="value">The object graph to serialize.</param>
		/// <param name="path">The file to write. Overwritten if it already exists.</param>
		internal static void WriteIndentedXml(DataContractSerializer serializer, object value, string path)
		{
			using XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings { Indent = true });
			serializer.WriteObject(writer, value);
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
