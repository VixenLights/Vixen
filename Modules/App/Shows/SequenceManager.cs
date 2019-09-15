using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.Shows
{
	public static class SequenceManager
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private static readonly ConcurrentDictionary<string, SequenceEntry> ActiveSequences = new ConcurrentDictionary<string, SequenceEntry>();
		private static readonly ConcurrentDictionary<string, SequenceEntry> RetiredSequences = new ConcurrentDictionary<string, SequenceEntry>();

		public static void ConsumerFinished(string sequenceFile, Guid id)
		{
			SequenceEntry entry = null;
			if (ActiveSequences.TryGetValue(sequenceFile, out entry))
			{
				lock (entry)
				{
					entry.RemoveConsumer(id);
					if (!entry.HasConsumers())
					{
						SequenceEntry junk = null;
						ActiveSequences.TryRemove(sequenceFile, out junk);
						DisposeSequence(entry.Sequence);
					}
				}
			}else if (RetiredSequences.TryGetValue(sequenceFile, out entry))
			{
				lock (entry)
				{
					entry.RemoveConsumer(id);
					if (!entry.HasConsumers())
					{
						SequenceEntry junk = null;
						RetiredSequences.TryRemove(sequenceFile, out junk);
						DisposeSequence(entry.Sequence);
					}
				}
			}
		}

		/// <summary>
		/// Gets a copy of the sequence and registers the consumer as a user
		/// </summary>
		/// <param name="sequenceFile"></param>
		/// <param name="id">Unique Id of a consumer</param>
		/// <returns></returns>
		public static async Task<SequenceEntry> GetSequenceAsync(string sequenceFile, Guid id)
		{
			//Returning the entry with a placeholder to the sequence facilitates parallel loading
			if (ActiveSequences.TryGetValue(sequenceFile, out var entry))
			{
				if (IsSequenceStale(sequenceFile))
				{
					ActiveSequences.TryRemove(sequenceFile, out entry);
					entry.RemoveConsumer(id);
					if (entry.HasConsumers())
					{
						RetiredSequences.TryAdd(sequenceFile, entry);
					}
					entry = await LoadSequenceAsync(sequenceFile);
				}

				lock (entry)
				{
					entry.AddConsumer(id);
					return entry;
				}
			}

			entry = await LoadSequenceAsync(sequenceFile);
			entry?.AddConsumer(id);
			return entry;
		}

		private static DateTime SequenceLastModified(string sequenceFile)
		{
			if (ActiveSequences.TryGetValue(sequenceFile, out var entry))
			{
				return entry.SequenceLastModified;
			}

			return DateTime.MaxValue;
		}

		public static bool IsSequenceStale(string sequenceFile)
		{
			return SequenceLastModified(sequenceFile) < LastModifiedTime(sequenceFile);
		}

		private static DateTime LastModifiedTime(string file)
		{
			if (System.IO.File.Exists(file))
			{
				return System.IO.File.GetLastWriteTime(file);
			}

			return DateTime.MaxValue;
		}

		public static async Task PreLoadSequenceAsync(string sequenceFile)
		{
			if (ActiveSequences.ContainsKey(sequenceFile))
			{
				return;
			}

			await LoadSequenceAsync(sequenceFile);

		}

		private static async Task<SequenceEntry> LoadSequenceAsync(string sequenceFile)
		{
			var entry = new SequenceEntry()
			{
				SequenceLoading = true
			};
			
			if (ActiveSequences.TryAdd(sequenceFile, entry))
			{
				try
				{
					var sequence = await Task.Run(() => SequenceService.Instance.Load(sequenceFile));
					entry.SequenceLastModified = LastModifiedTime(sequenceFile);
					entry.Sequence = sequence;
					entry.SequenceLoading = false;
					return entry;
				}
				catch (Exception e)
				{
					Logging.Error(e, "Error loading sequence!");
				}
			}
			
			return null;
		}

		private static void DisposeSequence(ISequence sequence)
		{
			//ISequence needs to implement Disposable
			var tSequence = (sequence as TimedSequence);
			if (tSequence != null)
			{
				tSequence.Dispose();
			}
		}

		public class SequenceEntry
		{
			private readonly HashSet<Guid> _consumerIds = new HashSet<Guid>();

			public ISequence Sequence { get; set; }

			public DateTime SequenceLastModified { get; set; }

			public long ConsumerCount => _consumerIds.Count;

			public bool SequenceLoading { get; internal set; }

			internal void AddConsumer(Guid id)
			{
				if (!_consumerIds.Contains(id))
				{
					_consumerIds.Add(id);
				}
			}

			internal bool RemoveConsumer(Guid id)
			{
				return _consumerIds.Remove(id);
			}

			public bool HasConsumers()
			{
				return _consumerIds.Count>0;
			}
		}

	}

	
}
