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

		public static void ConsumerFinished(string sequenceFile)
		{
			
			SequenceEntry entry = null;
			if (ActiveSequences.TryGetValue(sequenceFile, out entry))
			{
				lock (entry)
				{
					entry.RemoveConsumer();
					if (!entry.HasConsumers())
					{
						SequenceEntry junk = null;
						ActiveSequences.TryRemove(sequenceFile, out junk);
						DisposeSequence(entry.Sequence);
					}
				}
			}
		}

		public static async Task<SequenceEntry> GetSequenceAsync(string sequenceFile)
		{
			//Returning the entry with a placeholder to the sequence facilitates parallel loading
			SequenceEntry entry = null;
			if (ActiveSequences.TryGetValue(sequenceFile, out entry))
			{
				lock (entry)
				{
					entry.AddConsumer();
					return entry;
				}
			}

			entry = await LoadSequenceAsync(sequenceFile);
			if (entry != null)
			{
				entry.AddConsumer();
			}
			return entry;
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
					entry.Sequence = sequence;
					entry.SequenceLoading = false;
					return entry;
				}
				catch (Exception e)
				{
					Logging.Error("Error loading sequence!", e);
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
			private long _consumerCount;

			public ISequence Sequence { get; set; }

			public long ConsumerCount
			{
				get { return Interlocked.Read(ref _consumerCount); } 
			}

			public bool SequenceLoading { get; internal set; }

			internal void AddConsumer()
			{
				Interlocked.Increment(ref _consumerCount);
			}

			internal void RemoveConsumer()
			{
				if (ConsumerCount > 0)
				{
					Interlocked.Decrement(ref _consumerCount);
				}

			}

			public bool HasConsumers()
			{
				return ConsumerCount > 0;
			}
		}

	}

	
}
