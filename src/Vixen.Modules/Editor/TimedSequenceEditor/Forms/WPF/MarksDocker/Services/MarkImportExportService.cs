using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Catel.Collections;
using Common.Controls;
using NLog;
using TimedSequenceEditor.Forms.WPF.MarksDocker.Services;
using Vixen.Marks;
using Vixen.Sys;
using VixenModules.App.Marks;
using VixenModules.App.TimingTrackBrowser.ViewModels;
using VixenModules.App.TimingTrackBrowser.Views;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	public class MarkImportExportService
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private static string _lastFolder = Paths.DataRootPath;

		/// <summary>
		/// Materializes mark collections from a Pangolin Beyond CSV file.
		/// </summary>
		/// <remarks>
		/// Parses the complete file before prompting for import options or changing the active timed sequence.
		/// Files with zero or one source color import directly with their source color; files with multiple source colors
		/// prompt for the collection arrangement.
		/// Cancelling either import prompt leaves the collection set unchanged.
		/// </remarks>
		/// <returns>A task whose result contains detached mark collection candidates, or a cancellation/failure status.</returns>
		internal static async Task<MarkCollectionImportResult> ImportPangolinBeyondMarksAsync()
		{
			using var openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".csv",
				Filter = @"Pangolin Beyond CSV (*.csv)|*.csv|All Files (*.*)|*.*",
				FilterIndex = 0,
				InitialDirectory = _lastFolder
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.PangolinBeyond);
			}

			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			try
			{
				var csv = await File.ReadAllTextAsync(openFileDialog.FileName);
				if (!PangolinBeyondMarkParser.TryParse(csv, out var records, out var error))
				{
					Logging.Error("Unable to import Pangolin Beyond marks from {FileName}: {Error}", openFileDialog.FileName, error);
					await ShowPangolinBeyondImportErrorAsync();
					return MarkCollectionImportResult.Failed(MarkCollectionImportType.PangolinBeyond);
				}

				var importMode = PangolinBeyondImportMode.GroupByColor;
				if (RequiresPangolinBeyondColorChoice(records))
				{
					using var choiceDialog = new MessageBoxForm(
						"Create a Mark Collection for each Beyond color?",
						"Pangolin Beyond Import",
						MessageBoxButtons.YesNoCancel,
						SystemIcons.Question);
					var choice = GetPangolinBeyondImportMode(await choiceDialog.ShowDialogAsync());
					if (choice is null)
					{
						return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.PangolinBeyond);
					}

					importMode = choice.Value;
				}

				var replacementColor = Color.Empty;
				if (importMode == PangolinBeyondImportMode.SingleCollection)
				{
					using var colorPicker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();
					if (await colorPicker.ShowDialogAsync() != DialogResult.OK)
					{
						return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.PangolinBeyond);
					}

					replacementColor = colorPicker.Color.ToRGB().ToArgb();
				}

				return MaterializePangolinBeyondMarks(records, importMode, replacementColor);
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "Unable to import Pangolin Beyond marks from {FileName}", openFileDialog.FileName);
				await ShowPangolinBeyondImportErrorAsync();
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.PangolinBeyond);
			}
		}

		internal static PangolinBeyondImportMode? GetPangolinBeyondImportMode(DialogResult dialogResult)
		{
			return dialogResult switch
			{
				DialogResult.OK or DialogResult.Yes => PangolinBeyondImportMode.GroupByColor,
				DialogResult.No => PangolinBeyondImportMode.SingleCollection,
				_ => null
			};
		}

		internal static bool RequiresPangolinBeyondColorChoice(IReadOnlyList<PangolinBeyondMarkRecord> records)
		{
			ArgumentNullException.ThrowIfNull(records);

			return records.Select(record => record.Color).Distinct().Skip(1).Any();
		}

		internal static MarkCollectionImportResult MaterializePangolinBeyondMarks(
			IReadOnlyList<PangolinBeyondMarkRecord> records,
			PangolinBeyondImportMode? importMode,
			Color replacementColor)
		{
			ArgumentNullException.ThrowIfNull(records);
			if (importMode is null)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.PangolinBeyond);
			}

			var importedCollections = PangolinBeyondMarkCollectionFactory.CreateCollections(records, importMode.Value, replacementColor);
			return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.PangolinBeyond, importedCollections);
		}

		private static async Task ShowPangolinBeyondImportErrorAsync()
		{
			const string message = "There was an error importing the Pangolin Beyond marks.";
			using var messageBox = new MessageBoxForm(message, "Pangolin Beyond Import Error", MessageBoxButtons.OK, SystemIcons.Error);
			await messageBox.ShowDialogAsync();
		}

		/// <summary>
		/// Materializes mark collections from a Vixen 3 mark collection file.
		/// </summary>
		/// <returns>A task whose result contains detached mark collection candidates, or a cancellation/failure status.</returns>
		internal static async Task<MarkCollectionImportResult> ImportVixen3BeatsAsync()
		{
			using var openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".v3m",
				Filter = @"Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*",
				FilterIndex = 0,
				InitialDirectory = _lastFolder
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.Vixen3);
			}

			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			try
			{
				var documentText = await File.ReadAllTextAsync(openFileDialog.FileName);
				var xdoc = XDocument.Parse(documentText);
				if (xdoc.Root is null)
				{
					return MarkCollectionImportResult.Failed(MarkCollectionImportType.Vixen3);
				}

				Type type;
				var migrate = false;
				if (xdoc.Root.Name.NamespaceName.Equals("http://schemas.datacontract.org/2004/07/" + typeof(IMarkCollection)))
				{
					type = typeof(List<IMarkCollection>);
				}
				else if (xdoc.Root.Name.NamespaceName.Equals("http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed"))
				{
					type = typeof(List<Sequence.Timed.MarkCollection>);
					migrate = true;
				}
				else
				{
					Logging.Error("Could not determine type of Vixen Mark import file. Type {Type} Namespace {Namespace}", xdoc.Root.Name.LocalName, xdoc.Root.Name.NamespaceName);
					await ShowVixenImportErrorAsync();
					return MarkCollectionImportResult.Failed(MarkCollectionImportType.Vixen3);
				}

				using var reader = XmlReader.Create(new StringReader(documentText));
				var serializer = CreateSerializer(type, migrate);
				var markCollections = serializer.ReadObject(reader);
				var candidates = migrate
					? MigrateMarkCollections((List<Sequence.Timed.MarkCollection>)markCollections)
					: (markCollections as List<IMarkCollection> ?? []).ToList();
				return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.Vixen3, candidates);
			}
			catch (Exception exception)
			{
				Logging.Error(exception, "Unable to import V3 Marks");
				await ShowVixenImportErrorAsync();
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.Vixen3);
			}
		}

		private static DataContractSerializer CreateSerializer(Type type, bool legacy)
		{
			if (legacy)
			{
				return new DataContractSerializer(type);
			}

			return new DataContractSerializer(type, "ArrayOfIMarkCollection", "http://schemas.datacontract.org/2004/07/" + typeof(IMarkCollection), new[] { typeof(MarkCollection), typeof(Mark), typeof(MarkDecorator) });

		}

		private static IReadOnlyList<IMarkCollection> MigrateMarkCollections(List<Sequence.Timed.MarkCollection> oldCollections)
		{
			var migratedCollections = new List<IMarkCollection>(oldCollections.Count);
			foreach (var markCollection in oldCollections)
			{
				var lmc = new MarkCollection();
				lmc.Name = markCollection.Name;
				lmc.Level = markCollection.Level;
				lmc.ShowGridLines = markCollection.Enabled;
				lmc.ShowTailGridLines = false;
				lmc.Locked = false;
				lmc.Decorator = new MarkDecorator
				{
					Color = markCollection.MarkColor,
					IsBold = markCollection.Bold,
					IsSolidLine = markCollection.SolidLine
				};
				markCollection.Marks.ForEach(x => lmc.AddMark(new Mark(x)));
				migratedCollections.Add(lmc);
			}

			return migratedCollections;
		}

		private static async Task ShowVixenImportErrorAsync()
		{
			using var messageBox = new MessageBoxForm("There was an error importing the Vixen Marks.", "Vixen Marks Import Error", MessageBoxButtons.OK, SystemIcons.Error);
			await messageBox.ShowDialogAsync();
		}

		internal static async Task<MarkCollectionImportResult> LoadBarLabelsAsync()
		{
			using var openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".txt",
				Filter = @"Audacity Bar Labels|*.txt|All Files|*.*",
				FilterIndex = 0,
				InitialDirectory = _lastFolder
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.BarLabels);
			}

			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			try
			{
				var everything = (await File.ReadAllTextAsync(openFileDialog.FileName)).Replace("\r", string.Empty);
				var lines = everything.Split('\n', StringSplitOptions.RemoveEmptyEntries);
				if (!lines.Any())
				{
					return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.BarLabels, []);
				}

				var markCollection = CreateNewCollection(Color.Yellow, "Audacity Marks");
				foreach (var line in lines)
				{
					var endTimeMark = "0";
					var text = string.Empty;
					var lineParts = line.IndexOf('\t') > 0 ? line.Split('\t') : line.Trim().Split(' ');
					var startTimeMark = lineParts[0].Trim();
					if (lineParts.Length > 1)
					{
						endTimeMark = lineParts[1].Trim();
					}
					if (lineParts.Length > 2)
					{
						text = lineParts[2].Trim();
					}

					var startTime = TimeSpan.FromSeconds(Convert.ToDouble(startTimeMark));
					var endTime = TimeSpan.FromSeconds(Convert.ToDouble(endTimeMark));
					markCollection.AddMark(new Mark(startTime)
					{
						Duration = endTime > TimeSpan.Zero ? endTime - startTime : TimeSpan.Zero,
						Text = text
					});
				}

				return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.BarLabels, [markCollection]);
			}
			catch (Exception exception)
			{
				await ShowImportErrorAsync(exception, "There was an error importing the Audacity bar marks.", "Audacity Import Error");
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.BarLabels);
			}
		}

		internal static async Task<MarkCollectionImportResult> LoadBeatLabelsAsync()
		{
			using var openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".txt",
				Filter = @"Audacity Beat Labels|*.txt|All Files|*.*",
				FilterIndex = 0,
				InitialDirectory = _lastFolder
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.BeatLabels);
			}

			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			try
			{
				var file = await File.ReadAllTextAsync(openFileDialog.FileName);
				if (!file.Any())
				{
					return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.BeatLabels, []);
				}

				const string pattern = @"(\d*\.\d*)\s(\d*\.\d*)\s(\d)";
				var matches = Regex.Matches(file, pattern);
				var numberOfBeats = Convert.ToInt32(matches.Max(match => match.Groups[3].Value));
				var colors = new[] { Color.Yellow, Color.Gold, Color.Goldenrod, Color.SaddleBrown, Color.CadetBlue, Color.BlueViolet };
				var collections = Enumerable.Range(0, numberOfBeats)
					.Select(index => CreateNewCollection(colors[index], $"Audacity Beat {index + 1} Marks"))
					.ToList();
				foreach (Match match in matches)
				{
					var time = TimeSpan.FromSeconds(Convert.ToDouble(match.Groups[1].Value));
					var beatNumber = Convert.ToInt32(match.Groups[3].Value);
					collections[beatNumber - 1].AddMark(new Mark(time));
				}

				return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.BeatLabels, collections);
			}
			catch (Exception exception)
			{
				await ShowImportErrorAsync(exception, "There was an error importing the Audacity beat marks.", "Audacity Import Error");
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.BeatLabels);
			}
		}

		private static MarkCollection CreateNewCollection(Color color, string name = "New Collection")
		{
			MarkCollection newCollection = new MarkCollection();
			newCollection.Name = name;
			newCollection.Decorator.Color = color;
			
			return newCollection;
		}

		internal static async Task<MarkCollectionImportResult> LoadXTimingAsync()
		{
			using var openFileDialog = new OpenFileDialog { DefaultExt = ".txt", Filter = @"xTiming|*.xTiming|xTiming xml|*.xTiming.xml|All Files|*.*", FilterIndex = 0, InitialDirectory = _lastFolder };
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.XTiming);
			}

			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			try
			{
				var documentText = await File.ReadAllTextAsync(openFileDialog.FileName);
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(documentText);
				return MaterializeXTimingTracks(xmlDocument, MarkCollectionImportType.XTiming);
			}
			catch (Exception exception)
			{
				await ShowImportErrorAsync(exception, "There was an error importing the xTiming marks.", "xTiming Import Error");
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.XTiming);
			}
		}

		internal static MarkCollectionImportResult MaterializeXTimingTracks(XmlDocument xmlDocument, MarkCollectionImportType importType)
		{
			ArgumentNullException.ThrowIfNull(xmlDocument);
			var candidates = new List<IMarkCollection>();
			foreach (var timingNode in GetTimingNodes(xmlDocument))
			{
				candidates.AddRange(MaterializeTiming(timingNode));
			}

			return MarkCollectionImportResult.Succeeded(importType, candidates);
		}

		private static IEnumerable<XmlNode> GetTimingNodes(XmlDocument xmlDocument)
		{
			var timingGroups = xmlDocument.SelectSingleNode("/timings");
			if (timingGroups?.SelectNodes("timing") is { } timingNodes)
			{
				return timingNodes.Cast<XmlNode>();
			}

			return xmlDocument.SelectSingleNode("/timing") is { } timingNode ? [timingNode] : [];
		}

		private static IReadOnlyList<IMarkCollection> MaterializeTiming(XmlNode timingNode)
		{
			var name = timingNode.Attributes?.GetNamedItem("name")?.Value ?? "xTiming";
			var effectLayers = timingNode.SelectNodes("EffectLayer");
			if (effectLayers is null)
			{
				return [];
			}

			var candidates = new List<IMarkCollection>();
			var lipSyncTrack = effectLayers.Count > 1;
			var layerNumber = 1;
			foreach (XmlNode effectLayer in effectLayers)
			{
				var collection = CreateNewCollection(Color.Brown, $"{name} - {layerNumber}");
				if (lipSyncTrack)
				{
					ConfigureTimingCollection(collection, name, layerNumber, candidates);
				}

				collection.ShowMarkBar = true;
				var effects = effectLayer.SelectNodes("Effect");
				if (effects is not null)
				{
					foreach (XmlNode effect in effects)
					{
						var startTime = effect.Attributes?.GetNamedItem("starttime")?.Value;
						var endTime = effect.Attributes?.GetNamedItem("endtime")?.Value;
						if (startTime is null || endTime is null || startTime == endTime)
						{
							continue;
						}

						var mark = new Mark(TimeSpan.FromMilliseconds(Convert.ToDouble(startTime)))
						{
							Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(endTime)) - TimeSpan.FromMilliseconds(Convert.ToDouble(startTime)),
							Text = effect.Attributes?.GetNamedItem("label")?.Value
						};
						collection.AddMark(mark);
					}
				}

				if (collection.Marks.Any())
				{
					candidates.Add(collection);
					layerNumber++;
				}
			}

			return candidates;
		}

		private static void ConfigureTimingCollection(MarkCollection collection, string name, int layerNumber, IReadOnlyList<IMarkCollection> candidates)
		{
			switch (layerNumber)
			{
				case 1:
					collection.CollectionType = MarkCollectionType.Phrase;
					collection.Name = $"{name} - Phrase";
					break;
				case 2:
					collection.CollectionType = MarkCollectionType.Word;
					collection.Name = $"{name} - Word";
					if (candidates.LastOrDefault() is { } wordParent)
					{
						collection.LinkedMarkCollectionId = wordParent.Id;
					}
					break;
				case 3:
					collection.CollectionType = MarkCollectionType.Phoneme;
					collection.Name = $"{name} - Phoneme";
					if (candidates.LastOrDefault() is { } phonemeParent)
					{
						collection.LinkedMarkCollectionId = phonemeParent.Id;
					}
					break;
			}
		}

		internal static async Task<MarkCollectionImportResult> ImportPapagayoTracksAsync()
		{
			using FileDialog openDialog = new OpenFileDialog { Filter = @"Papagayo files (*.pgo)|*.pgo|All files (*.*)|*.*", FilterIndex = 1, InitialDirectory = _lastFolder };
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.Papagayo);
			}
			_lastFolder = Path.GetDirectoryName(openDialog.FileName);
			try
			{
				var documentBytes = await File.ReadAllBytesAsync(openDialog.FileName);
				var papagayoFile = new PapagayoDoc();
				using var documentStream = new MemoryStream(documentBytes);
				using var documentReader = new StreamReader(documentStream);
				papagayoFile.Load(documentReader);
				var fileName = Path.GetFileNameWithoutExtension(openDialog.FileName);
				var candidates = new List<IMarkCollection>();
				foreach (var voice in papagayoFile.VoiceList)
				{
					candidates.AddRange(MaterializePapagayoVoice(papagayoFile, fileName, voice));
				}

				await ShowPapagayoImportSummaryAsync(papagayoFile.VoiceList);
				return MarkCollectionImportResult.Succeeded(MarkCollectionImportType.Papagayo, candidates);
			}
			catch (Exception exception)
			{
				await ShowImportErrorAsync(exception, "There was an error importing the Papagayo marks.", "Papagayo Import Error");
				return MarkCollectionImportResult.Failed(MarkCollectionImportType.Papagayo);
			}
		}

		internal static async Task<MarkCollectionImportResult> ImportSingingFacesTracksAsync()
		{
			var vendorInventoryWindow = new VendorInventoryWindow();
			if (vendorInventoryWindow.ShowDialog() == true && vendorInventoryWindow.ViewModel is VendorInventoryWindowViewModel { SelectedSong: not null } viewModel)
			{
				try
				{
					var timing = await viewModel.GetSelectedSongTiming();
					var xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(timing);
					return MaterializeXTimingTracks(xmlDocument, MarkCollectionImportType.SingingFaces);
				}
				catch (Exception exception)
				{
					await ShowImportErrorAsync(exception, "There was an error importing the Singing Faces timing marks.", "Singing Faces Import Error");
					return MarkCollectionImportResult.Failed(MarkCollectionImportType.SingingFaces);
				}
			}

			return MarkCollectionImportResult.Cancelled(MarkCollectionImportType.SingingFaces);
		}

		private static IReadOnlyList<IMarkCollection> MaterializePapagayoVoice(PapagayoDoc papagayoFile, string fileName, string voice)
		{
			var phraseCollection = CreateNewCollection(Color.FromArgb(205, 242, 162), $"{fileName} {voice} Phrases");
			phraseCollection.ShowMarkBar = true;
			phraseCollection.CollectionType = MarkCollectionType.Phrase;
			var wordCollection = CreateNewCollection(Color.FromArgb(242, 205, 162), $"{fileName} {voice} Words");
			wordCollection.ShowMarkBar = true;
			wordCollection.CollectionType = MarkCollectionType.Word;
			wordCollection.LinkedMarkCollectionId = phraseCollection.Id;
			var phonemeCollection = CreateNewCollection(Color.FromArgb(235, 185, 210), $"{fileName} {voice} Phonemes");
			phonemeCollection.ShowMarkBar = true;
			phonemeCollection.CollectionType = MarkCollectionType.Phoneme;
			phonemeCollection.LinkedMarkCollectionId = wordCollection.Id;

			foreach (var phrase in papagayoFile.PhraseList(voice))
			{
				phraseCollection.AddMark(new Mark(TimeSpan.FromMilliseconds(phrase.StartMS)) { Duration = TimeSpan.FromMilliseconds(phrase.DurationMS), Text = phrase.Text });
				foreach (var word in phrase.Words)
				{
					wordCollection.AddMark(new Mark(TimeSpan.FromMilliseconds(word.StartMS)) { Duration = TimeSpan.FromMilliseconds(word.EndMS - word.StartMS), Text = word.Text });
					foreach (var phoneme in word.Phonemes)
					{
						phonemeCollection.AddMark(new Mark(TimeSpan.FromMilliseconds(phoneme.StartMS)) { Duration = TimeSpan.FromMilliseconds(phoneme.EndMS - phoneme.StartMS), Text = phoneme.TypeName });
					}
				}
			}

			return [phraseCollection, wordCollection, phonemeCollection];
		}

		private static async Task ShowPapagayoImportSummaryAsync(IReadOnlyCollection<string> voices)
		{
			var display = $"{voices.Count} voices imported as Mark Collections\n\n" + string.Join("\n", voices.Select((voice, index) => $"Row #{index + 1} - {voice}"));
			MessageBoxForm.msgIcon = SystemIcons.Information;
			using var messageBox = new MessageBoxForm(display, "Papagayo Import", false, false);
			await messageBox.ShowDialogAsync();
		}

		private static async Task ShowImportErrorAsync(Exception exception, string message, string title)
		{
			Logging.Error(exception, message);
			using var messageBox = new MessageBoxForm(message, title, MessageBoxButtons.OK, SystemIcons.Error);
			await messageBox.ShowDialogAsync();
		}

		//Beat Mark Collection Export routine 2-7-2014 JMB
		//In the audacity section, if the MarkCollections.Count = 1 then we assume the collection is bars and iMarkCollection++
		//Otherwise its beats, at least from the information I have studied, and we do not iMarkCollection++ to keep the collections together properly.
		/// <summary>
		/// Exports the selected Mark Collections to the requested file format.
		/// </summary>
		/// <param name="exportType">One of the enumeration values that specifies the output format.</param>
		/// <param name="collections">The Mark Collections and export options to write.</param>
		/// <returns>A task that represents the asynchronous export operation.</returns>
		public static async Task ExportMarkCollectionsAsync(MarkExportType exportType, IList<ExportableMarkCollection> collections)
		{
			using var saveFileDialog = new SaveFileDialog();
			if (exportType == MarkExportType.Vixen)
			{
				saveFileDialog.DefaultExt = ".v3m";
				saveFileDialog.Filter = @"Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
				saveFileDialog.InitialDirectory = _lastFolder;
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);
					var xmlsettings = new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "\t"
					};

					try
					{
						var serializer = CreateSerializer(typeof(List<IMarkCollection>), false);
						using var stream = new MemoryStream();
						using (var writer = XmlWriter.Create(stream, xmlsettings))
						{
							serializer.WriteObject(writer, collections.Select(x => x.MarkCollection).ToList());
						}
						await File.WriteAllBytesAsync(saveFileDialog.FileName, stream.ToArray());
					}
					catch (Exception ex)
					{
						Logging.Error(ex,"An exception occured trying to export the mark collection");
					}
					
				}
			}

			if (exportType == MarkExportType.Audacity)
			{
				int iMarkCollection = 0;
				List<string> beatMarks = new List<string>();
				foreach (IMarkCollection mc in collections.Select(x => x.MarkCollection))
				{
					iMarkCollection++;
					foreach (IMark mark in mc.Marks)
					{
						beatMarks.Add(mark.StartTime.TotalSeconds.ToString("0000.000") + "\t" + mark.StartTime.TotalSeconds.ToString("0000.000") + "\t" + iMarkCollection);
						if (collections.Count == 1)
							iMarkCollection++;
					}
				}

				saveFileDialog.DefaultExt = ".txt";
				saveFileDialog.Filter = @"Audacity Marks (*.txt)|*.txt|All Files (*.*)|*.*";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					await File.WriteAllLinesAsync(saveFileDialog.FileName, beatMarks.OrderBy(mark => mark));
				}
			}

			if (exportType == MarkExportType.PangolinBeyond)
			{
				//Create a list of marks
				var markRecords = new List<MarkRecord>();
				foreach (var emc in collections)
				{
					//Convert to Hex and remove the leading #
					var color = ToBGRHex(emc.MarkCollection.Decorator.Color).Substring(1);
					foreach (IMark mark in emc.MarkCollection.Marks)
					{
						var markText = emc.IsTextIncluded ? mark.Text.Replace(',', ' ') : string.Empty;
						markRecords.Add(new MarkRecord(mark.StartTime,markText,color));
					}
				}

				var orderedMarks = markRecords.OrderBy(x => x.StartTime).ToList();

				var beatMarks = new List<string>(orderedMarks.Count + 1);
				//Add the required header
				beatMarks.Add("#,Name,Start,Color");
				int markNum = 1;
				foreach (var mr in orderedMarks)
				{
					var timeText = mr.StartTime.Hours > 0
						? mr.StartTime.ToString(@"hh\:mm\:ss\.fff")
						: mr.StartTime.ToString(@"mm\:ss\.fff");
					beatMarks.Add($"M{markNum},{mr.Text},{timeText},{mr.Color}");
					markNum++;
				}

				saveFileDialog.DefaultExt = ".csv";
				saveFileDialog.Filter = @"CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					await File.WriteAllLinesAsync(saveFileDialog.FileName, beatMarks);
				}
			}
		}

		//This should be in an extension class, but the one in Vixen.Core adds a conflict with the AddRange in Catel.Collections
		//Need to find a better way to handle that and refactor this later.
		public static string ToHex(Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}
		
		public static string ToBGRHex(Color color)
		{
			return $"#{color.B:X2}{color.G:X2}{color.R:X2}";
		}

	}

	record struct MarkRecord(TimeSpan StartTime, string Text, string Color)
	{

	}

	public enum MarkExportType
	{
		Vixen,
		Audacity,
		PangolinBeyond
	}
}
