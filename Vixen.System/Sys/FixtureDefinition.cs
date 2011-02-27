using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Common;
using System.Xml;

using Vixen.IO;

namespace Vixen.Sys {
	// Output only for now.  May make generic for input and output fixtures.
	public class FixtureDefinition : Definition {
		private LinkedList<OutputChannel> _channels = new LinkedList<OutputChannel>();

		private const string DIRECTORY_NAME = "Fixture";
		private const string FILE_EXT = ".fix";

		/// <summary>
		/// Do not use.  Use FixtureDefinition.NewDefinition instead.
		/// </summary>
		public FixtureDefinition() { }

		[DataPath]
		static protected readonly string _fixtureDefinitionDirectory = Path.Combine(Definition._definitionDirectory, DIRECTORY_NAME);

		static public FixtureDefinition NewDefinition(string name, params OutputChannel[] channels) {
			FixtureDefinition fixtureDefinition = new FixtureDefinition();
			foreach(OutputChannel channel in channels) {
				fixtureDefinition.InsertChannel(channel);
			}
			fixtureDefinition.Save(Path.Combine(_fixtureDefinitionDirectory, name + FILE_EXT));
			return fixtureDefinition;
		}

		static public FixtureDefinition Get(string name) {
			return Definition._GetInstance<FixtureDefinition>(_fixtureDefinitionDirectory, name, FILE_EXT);
		}

		static public IEnumerable<FixtureDefinition> GetAll() {
			return Definition._GetAll<FixtureDefinition>(_fixtureDefinitionDirectory, FILE_EXT);
		}

		// Keeping the same interface as a fixture so it will be familiar.
		public void InsertChannel(OutputChannel channel) {
			_channels.AddLast(channel);
		}

		public IEnumerable<OutputChannel> Channels {
			get { return _channels; }
		}

		public int ChannelCount {
			get { return _channels.Count; }
		}

		protected override void ReadAttributes(XmlReader reader) { }

		protected override void ReadBody(XmlReader reader) {
			// Channels
			if(reader.ElementsExistWithin("Channels")) { // Container element for child entity
				ChannelReader<OutputChannel> channelReader = new ChannelReader<OutputChannel>();
				while(channelReader.Read(reader)) {
					InsertChannel(channelReader.Channel);
				}
				reader.ReadEndElement(); // Channels
			}
		}

		protected override void WriteAttributes(XmlWriter writer) { }

		protected override void WriteBody(XmlWriter writer) {
			// Any fixtures based on this template should be cloned instances with
			// no data, so there shouldn't be any data persisted when this is done.
			ChannelWriter channelWriter = new ChannelWriter();
			writer.WriteStartElement("Channels");
			foreach(Channel channel in Channels) {
				channelWriter.Channel = channel;
				channelWriter.Write(writer);
			}
			writer.WriteEndElement(); // Channels
		}
	}
}
