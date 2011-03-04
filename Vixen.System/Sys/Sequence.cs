using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Common;
using Vixen.IO;
using CommandStandard;
using Vixen.Sequence;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Input;

namespace Vixen.Sys {
	#region Static Sequence class
	static class Sequence {
		/// <summary>
		/// Gets a new instance.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		static public Vixen.Module.Sequence.ISequenceModuleInstance Get(string fileName) {
			// Get the sequence module manager.
			Vixen.Module.Sequence.SequenceModuleManagement manager = Server.Internal.GetModuleManager<Vixen.Module.Sequence.ISequenceModuleInstance, Vixen.Module.Sequence.SequenceModuleManagement>();
			// Get an instance of the appropriate sequence module.
			Vixen.Module.Sequence.ISequenceModuleInstance instance = manager.Get(fileName);

			return instance;
		}

		/// <summary>
		/// Loads an existing instance.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		static public Vixen.Module.Sequence.ISequenceModuleInstance Load(string fileName) {
			// Get an instance of the appropriate sequence module.
			Vixen.Module.Sequence.ISequenceModuleInstance instance = Get(fileName);
			// Load the sequence.
			instance.Load(fileName);

			return instance;
		}

		static public string[] GetAllFileNames() {
			// We can't assume where all of the sequence file types will exist, so to provide
			// this functionality we will have to do the following:

			// Iterate all of the sequence type descriptors and build a set of file types.
			HashSet<string> fileTypes = new HashSet<string>();
			foreach(Vixen.Module.Sequence.ISequenceModuleDescriptor descriptor in Modules.GetModuleDescriptors("Sequence")) {
				fileTypes.Add(descriptor.FileExtension);
			}
			// Find all files of those types in the data branch.
			return fileTypes.SelectMany(x => Directory.GetFiles(Paths.DataRootPath, "*" + x, SearchOption.AllDirectories)).ToArray();
		}
	}
	#endregion

	/// <summary>
	/// Base class for any sequence implementation.
	/// </summary>
	/// <typeparam name="Reader">Reader for the sequence type.</typeparam>
	/// <typeparam name="Writer">Writer for the sequence type</typeparam>
	/// <typeparam name="SequenceType">The sequence type being implemented as you read this.</typeparam>
	abstract public class Sequence<Reader, Writer, SequenceType> : ISequence
		where Reader : ISequenceReader, new()
		where Writer : ISequenceWriter, new()
		where SequenceType : class, ISequence {
		// Going with a linked list because List<> excels at random access while
		// a LinkedList<> accells at sequential access. 
		private LinkedList<Fixture> _fixtures = new LinkedList<Fixture>();
		private IntervalCollection _intervals = new IntervalCollection();
		private int _length;
		// Input channels are not in fixtures.  Fixtures are currently output-only.
		private List<InputChannel> _inputChannels = new List<InputChannel>();

		/// <summary>
		/// Use this to set the sequence's length when the sequence is untimed.
		/// </summary>
		public const int Forever = int.MaxValue;

		protected Sequence() {
			ModuleDataSet = new ModuleDataSet();
			Name = "Unnamed sequence";
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			// Default behavior is to use the generic timer.
			TimingSourceId = Guid.Empty;
			Data = new CommandNodeIntervalSync(this, _inputChannels, _intervals);
			RuntimeBehaviors = Server.ModuleManagement.GetAllRuntimeBehavior();
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
				ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private void _DataListener(InsertDataParameters parameters) {
			Data.AddCommand(new CommandNode(parameters.Command, parameters.Channels, parameters.StartTime, parameters.TimeSpan));
		}

		abstract protected string Directory { get; }
		// Cannot perform late-bound operations on generic types.
		//[DataPath]
		//static private readonly string _directory = Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName">Must be a qualified file path.</param>
		/// <returns></returns>
		public void Load(string fileName) {
			Reader reader = new Reader();
			reader.Read(fileName, this);
		}

		public void Save(string fileName) {
			if(string.IsNullOrWhiteSpace(fileName)) throw new InvalidOperationException("A name is required.");
			string filePath = Path.Combine(this.Directory, Path.GetFileName(fileName));
			Writer writer = new Writer();
			writer.Write(filePath, this);
			this.FileName = filePath;
		}

		public void Save() {
			Save(FileName);
		}

		public bool Masked {
			get { return Fixtures.All(x => x.Masked); }
			set {
				foreach(Fixture fixture in Fixtures) {
					fixture.Masked = value;
				}
			}
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
			fixture.ParentSequence = this;
		}

		public void RemoveFixture(Fixture fixture) {
			_fixtures.Remove(fixture);
		}

		public IEnumerable<Fixture> Fixtures {
			get { return _fixtures; }
		}

		/// <summary>
		/// Set to the file name when deserialized and serialized. 
		/// </summary>
		public string Name { get; set; }

		public IModuleDataSet ModuleDataSet { get; private set; }

		public int Length {
			get { return _length; }
			set {
				if(value != _length) {
					int oldLength = _length;
					_length = value;

					// Update the interval collection.
					if(value != Forever) {
						if(oldLength == Forever) {
							// Forever -> Length
							// Reset the interval collection with the new length.
							Data.TimingInterval = Data.TimingInterval;
						} else {
							// Length -> Length
							if(value > oldLength) {
								Data.AddIntervals(value - oldLength, Data.TimingInterval);
							} else if(value < oldLength) {
								Data.RemoveIntervals(value, oldLength);
							}
						}
					} else {
						// Length -> Forever (the only possibility)
						Data.ClearIntervals();
					}
				}
			}
		}

		public string FileName { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(InsertDataParameters parameters) {
			InsertData(parameters.Channels, parameters.StartTime, parameters.TimeSpan, parameters.Command);
		}

		public void InsertData(OutputChannel[] channels, int startTime, int timeSpan, Command command) {
			InsertDataListener.InsertData(channels, startTime, timeSpan, command);
		}

		public IEnumerable<OutputChannel> OutputChannels {
			get {
				return
					from fixture in Fixtures
					from channel in fixture.Channels
					select channel;
			}
		}

		/// <summary>
		/// Default behavior is a full update.
		/// </summary>
		virtual public UpdateBehavior UpdateBehavior {
			get { return UpdateBehavior.FullUpdate; }
		}

		public bool IsUntimed {
			get { return Length == Forever; }
			set { Length = value ? Forever : 0; }
		}

		public Guid TimingSourceId { get; set; }

		public CommandNodeIntervalSync Data { get; private set; }

		// Every sequence will get a collection of all available runtime behaviors.
		public IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; private set; }
	}
}
