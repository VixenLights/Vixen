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


		//Vixen 3 Beat Mark Collection Import routine 2-7-2014 JMB
		public static void ImportVixen3Beats(ObservableCollection<IMarkCollection> collections)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = ".v3m";
			openFileDialog.Filter = @"Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{

				_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
				var xdoc = XDocument.Load(openFileDialog.FileName);
				if (xdoc.Root != null)
				{
					Type type;
					bool migrate = false;
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
						Logging.Error($"Could not determine type of Vixen Mark import file. Type {xdoc.Root.Name.LocalName} Namspace {xdoc.Root.Name.NamespaceName}");
						string msg = "There was an error importing the Vixen Marks.";
						var messageBox = new MessageBoxForm(msg, "Vixen Marks Import Error", MessageBoxButtons.OK, SystemIcons.Error);
						messageBox.ShowDialog();
						return;
					}

					using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
					{
						try
						{
							DataContractSerializer ser = CreateSerializer(type, migrate);
							var markCollections = ser.ReadObject(reader);
							if (!migrate)
							{
								var imc = markCollections as List<IMarkCollection>;
								if (imc != null && collections.Any(x => x.IsDefault))
								{
									//make sure any imported are not default becasue we have a set and there will
									//be a default there.
									imc.ForEach(x => x.IsDefault = false);
									
								}

								collections.AddRange(imc);
							}
							else
							{
								MigrateMarkCollections(collections, (List<Sequence.Timed.MarkCollection>) markCollections);
							}

							if (!collections.Any(x => x.IsDefault))
							{
								SetDefaultCollection(collections);
							}
						}
						catch (Exception e)
						{
							Logging.Error(e, "Unable to import V3 Marks");
						}

					}
				}
				
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

		private static void MigrateMarkCollections(ObservableCollection<IMarkCollection> collections, List<Sequence.Timed.MarkCollection> oldCollections)
		{
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
				collections.Add(lmc);
			}

			if (!collections.Any(x => x.IsDefault))
			{
				SetDefaultCollection(collections);
			}
		}

		private static void SetDefaultCollection(ICollection<IMarkCollection> collections)
		{
			//Set one of them active
			if (!collections.Any()) return;
			var mc = collections.FirstOrDefault(x => x.IsVisible);
			if (mc != null)
			{
				mc.IsDefault = true;
			}
			else
			{
				collections.First().IsDefault = true;
			}
		}

		public static void LoadBarLabels(ObservableCollection<IMarkCollection> collections)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = ".txt";
			openFileDialog.Filter = @"Audacity Bar Labels|*.txt|All Files|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
				try
				{
					String everything;
					using (StreamReader sr = new StreamReader(openFileDialog.FileName))
					{
						everything = sr.ReadToEnd();
					}
					// Remove the \r so we're just left with a \n (allows importing of Sean's Audacity beat marks
					everything = everything.Replace("\r", "");
					string[] lines = everything.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
					if (lines.Any())
					{
						var mc = CreateNewCollection(Color.Yellow, "Audacity Marks");
						foreach (string line in lines)
						{
							string endTimeMark = "0";
							string text = string.Empty;
							string[] lineParts;
							if (line.IndexOf("\t") > 0)
							{
								lineParts = line.Split('\t');
							}
							else
							{
								lineParts = line.Trim().Split(' ');
							}

							var startTimeMark = lineParts[0].Trim();
							if (lineParts.Length > 1)
							{
								endTimeMark = lineParts[1].Trim();
							}

							if (lineParts.Length > 2)
							{
								text = lineParts[2].Trim();
							}

							TimeSpan startTime = TimeSpan.FromSeconds(Convert.ToDouble(startTimeMark)); 
							TimeSpan endTime = TimeSpan.FromSeconds(Convert.ToDouble(endTimeMark));
							TimeSpan duration = TimeSpan.Zero;
							if (endTime > TimeSpan.Zero)
							{
								duration = endTime - startTime;
							}
							mc.AddMark(new Mark(startTime)
							{
								Duration = duration,
								Text = text
							});
						}

						collections.Add(mc);
						if (!collections.Any(x => x.IsDefault))
						{
							SetDefaultCollection(collections);
						}
					}
				}
				catch (Exception ex)
				{
					string msg = "There was an error importing the Audacity bar marks.";
					Logging.Error(ex, msg);
					var messageBox = new MessageBoxForm(msg, "Audacity Import Error", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
			}
		}

		public static void LoadBeatLabels(ObservableCollection<IMarkCollection> collections)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = ".txt";
			openFileDialog.Filter = @"Audacity Beat Labels|*.txt|All Files|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = _lastFolder;
			openFileDialog.FileName = "";
			var colors = new List<Color>
			{
				Color.Yellow,Color.Gold, Color.Goldenrod, Color.SaddleBrown,Color.CadetBlue,Color.BlueViolet
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
				try
				{
					String file;
					using (var sr = new StreamReader(openFileDialog.FileName))
					{
						file = sr.ReadToEnd();
					}
					if (file.Any())
					{
						const string pattern = @"(\d*\.\d*)\s(\d*\.\d*)\s(\d)";
						MatchCollection matches = Regex.Matches(file, pattern);
						int numBeats = Convert.ToInt32(matches.Cast<Match>().Max(x => x.Groups[3].Value));
						var marks = new List<MarkCollection>(numBeats);
						for (int i = 0; i < numBeats; i++)
						{
							marks.Add(CreateNewCollection(colors[i], $"Audacity Beat {i + 1} Marks"));
						}

						foreach (Match match in matches)
						{
							TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(match.Groups[1].Value));
							int beatNumber = Convert.ToInt32(match.Groups[3].Value);
							marks[beatNumber - 1].AddMark(new Mark(time));
						}
						
						collections.AddRange(marks);
					}

					if (!collections.Any(x => x.IsDefault))
					{
						SetDefaultCollection(collections);
					}
				}
				catch (Exception ex)
				{
					string msg = "There was an error importing the Audacity beat marks.";
					Logging.Error(ex, msg);
					var messageBox = new MessageBoxForm(msg, "Audacity Import Error", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
			}
		}

		private static MarkCollection CreateNewCollection(Color color, string name = "New Collection")
		{
			MarkCollection newCollection = new MarkCollection();
			newCollection.Name = name;
			newCollection.Decorator.Color = color;
			
			return newCollection;
		}

		public static void LoadXTiming(ObservableCollection<IMarkCollection> collections)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = ".txt";
			openFileDialog.Filter = @"xTiming|*.xTiming|xTiming xml|*.xTiming.xml|All Files|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(openFileDialog.FileName);
				LoadXTimingTracks(xmlDoc, collections);
            }
		}

        private static void LoadXTimingTracks(XmlDocument xmlDoc, ICollection<IMarkCollection> collections)
        {
            try
            {
               
                XmlNode timingGroups = xmlDoc.SelectSingleNode("/timings");
                if (timingGroups != null)
                {
                    //We have multiples
                    var timingNodes = timingGroups.SelectNodes("timing");
                    foreach (XmlNode timingNode in timingNodes)
                    {
                        ProcessTiming(timingNode, collections);
                    }
                }
                else
                {
                    XmlNode timingNode = xmlDoc.SelectSingleNode("/timing");
                    if (timingNode != null)
                    {
                        ProcessTiming(timingNode, collections);
                    }
                }

                if (!collections.Any(x => x.IsDefault))
                {
                    SetDefaultCollection(collections);
                }
            }
            catch (Exception ex)
            {
                string msg = "There was an error importing the Audacity bar marks.";
                Logging.Error(ex, msg);
                var messageBox = new MessageBoxForm(msg, "Audacity Import Error", MessageBoxButtons.OK, SystemIcons.Error);
                messageBox.ShowDialog();
            }
        }

        private static void ProcessTiming(XmlNode timingNode,  ICollection<IMarkCollection> collections)
		{
			if (timingNode == null)
			{
				return;
			}
			var name = timingNode.Attributes?.GetNamedItem("name").Value;
			var effectLayers = timingNode.SelectNodes("EffectLayer");
			if (effectLayers != null)
			{
				int counter = 1;
				bool lipSyncTrack = effectLayers.Count > 1;
				foreach (XmlNode effectLayer in effectLayers)
				{
					var collectionName = $"{name ?? "xTiming"} - {counter}";
					var mc = CreateNewCollection(Color.Brown, collectionName);
					if (lipSyncTrack)
					{
						switch (counter)
						{
							case 1:
								mc.CollectionType = MarkCollectionType.Phrase;
								mc.Name = $"{name ?? "xTiming"} - Phrase";
								break;
							case 2:
								mc.CollectionType = MarkCollectionType.Word;
								mc.LinkedMarkCollectionId = collections.Last().Id;
								mc.Name = $"{name ?? "xTiming"} - Word";
								break;
							case 3:
								mc.CollectionType = MarkCollectionType.Phoneme;
								mc.LinkedMarkCollectionId = collections.Last().Id;
								mc.Name = $"{name ?? "xTiming"} - Phoneme";
								break;
						}
					}

					mc.ShowMarkBar = true; //We have labels, so make sure they are seen.
					var effects = effectLayer?.SelectNodes("Effect");
					if (effects != null)
					{
						//iterate the marks
						foreach (XmlNode effect in effects)
						{
							var label = effect.Attributes?.GetNamedItem("label").Value;
							var startTime = effect.Attributes?.GetNamedItem("starttime").Value;
							var endTime = effect.Attributes?.GetNamedItem("endtime").Value;
							if(startTime == endTime) continue; //Due to some odd reason there can be zero length labels. Right/Wrong we are going to skip those.
							var mark = new Mark(TimeSpan.FromMilliseconds(Convert.ToDouble(startTime)));
							mark.Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(endTime)) - mark.StartTime;
							mark.Text = label;
							mc.AddMark(mark);
						}
					}

					if (mc.Marks.Any())
					{
						counter++;
						collections.Add(mc);
					}
				}
			}
		}

		public static void ImportPapagayoTracks(ICollection<IMarkCollection> markCollection)
		{
			FileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = @"Papagayo files (*.pgo)|*.pgo|All files (*.*)|*.*";
			openDialog.FilterIndex = 1;
			openDialog.InitialDirectory = _lastFolder;
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			PapagayoDoc papagayoFile = new PapagayoDoc();
			string fileName = openDialog.FileName;
			_lastFolder = Path.GetDirectoryName(openDialog.FileName);
			papagayoFile.Load(fileName);
			var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			int rownum = 0;
			foreach (string voice in papagayoFile.VoiceList)
			{
				var phraseCollection = new MarkCollection();
				phraseCollection.Name = $"{fileWithoutExtension} {voice} Phrases";
				phraseCollection.ShowMarkBar = true;
				phraseCollection.Decorator.Color = Color.FromArgb(205,242,162);
				var wordCollection = new MarkCollection();
				wordCollection.Name = $"{fileWithoutExtension} {voice} Words";
				wordCollection.ShowMarkBar = true;
				wordCollection.Decorator.Color = Color.FromArgb(242,205,162);
				var phonemeCollection = new MarkCollection();
				phonemeCollection.Name = $"{fileWithoutExtension} {voice} Phonemes";
				phonemeCollection.ShowMarkBar = true;
				phonemeCollection.Decorator.Color = Color.FromArgb(235,185,210);
				var phrases = papagayoFile.PhraseList(voice);
				foreach (var phrase in phrases)
				{
					var mark = new Mark(TimeSpan.FromMilliseconds(phrase.StartMS));
					mark.Duration = TimeSpan.FromMilliseconds(phrase.DurationMS);
					mark.Text = phrase.Text;
					phraseCollection.AddMark(mark);
					foreach (var word in phrase.Words)
					{
						mark = new Mark(TimeSpan.FromMilliseconds(word.StartMS));
						mark.Duration = TimeSpan.FromMilliseconds(word.EndMS) - mark.StartTime;
						mark.Text = word.Text;
						wordCollection.AddMark(mark);
						foreach (var phoneme in word.Phonemes)
						{
							mark = new Mark(TimeSpan.FromMilliseconds(phoneme.StartMS));
							mark.Duration = TimeSpan.FromMilliseconds(phoneme.EndMS) - mark.StartTime;
							mark.Text = phoneme.TypeName;
							phonemeCollection.AddMark(mark);
						}
					}
				}

				phraseCollection.CollectionType = MarkCollectionType.Phrase;
				markCollection.Add(phraseCollection);
				wordCollection.CollectionType = MarkCollectionType.Word;
				wordCollection.LinkedMarkCollectionId = phraseCollection.Id;
				markCollection.Add(wordCollection);
				phonemeCollection.CollectionType = MarkCollectionType.Phoneme;
				phonemeCollection.LinkedMarkCollectionId = wordCollection.Id;
				markCollection.Add(phonemeCollection);


				//phonemeCollection = new MarkCollection();
				//phonemeCollection.Name = $"{fileWithoutExtension} {voice} Phonemes Coalesced";
				//phonemeCollection.ShowMarkBar = true;
				//phonemeCollection.Decorator.Color = Color.FromArgb(245, 75, 210); ;
				//foreach (var phoneme in papagayoFile.PhonemeList(voice))
				//{
				//	var mark = new Mark(TimeSpan.FromMilliseconds(phoneme.StartMS));
				//	mark.Duration = TimeSpan.FromMilliseconds(phoneme.EndMS) - mark.StartTime;
				//	mark.Text = phoneme.TypeName;
				//	phonemeCollection.AddMark(mark);
				//}
				//phonemeCollection.CollectionType = MarkCollectionType.Phoneme;
				//markCollection.Add(phonemeCollection);
				rownum++;
			}

			string displayStr = rownum + " Voices imported to clipboard as Mark Collctions\n\n";
			int j = 1;
			foreach (string voiceStr in papagayoFile.VoiceList)
			{
				displayStr += "Row #" + j + " - " + voiceStr + "\n";
				j++;
			}
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm(displayStr, @"Papagayo Import", false, false);
			messageBox.ShowDialog();
		}

        public static async void ImportSingingFacesTracks(ICollection<IMarkCollection> markCollection)
        {
            VendorInventoryWindow viw = new VendorInventoryWindow();
            var result = viw.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if (viw.ViewModel is VendorInventoryWindowViewModel vm)
                {
                    if (vm.SelectedSong != null)
                    {
                        var timing = await vm.GetSelectedSongTiming();
                        var xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(timing);
						LoadXTimingTracks(xmlDoc, markCollection);
                    }
                    
                }
            }
        }

		//Beat Mark Collection Export routine 2-7-2014 JMB
		//In the audacity section, if the MarkCollections.Count = 1 then we assume the collection is bars and iMarkCollection++
		//Otherwise its beats, at least from the information I have studied, and we do not iMarkCollection++ to keep the collections together properly.
		public static async Task ExportMarkCollections(MarkExportType exportType, IList<ExportableMarkCollection> collections)
		{
			var saveFileDialog = new SaveFileDialog();
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

					DataContractSerializer ser = CreateSerializer(typeof(List<IMarkCollection>), false);
					var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
					ser.WriteObject(writer, collections.Select(x => x.MarkCollection));
					writer.Close();
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
					string name = saveFileDialog.FileName;

					await using StreamWriter file = new StreamWriter(name);
					foreach (string bm in beatMarks.OrderBy(x => x))
					{
						await file.WriteLineAsync(bm);
					}
				}
			}

			if (exportType == MarkExportType.PangolinBeyond)
			{
				//Create a list of marks
				var markRecords = new List<MarkRecord>();
				foreach (var emc in collections)
				{
					//Convert to Hex and remove the leading #
					var color = ToHex(emc.MarkCollection.Decorator.Color).Substring(1);
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
						? mr.StartTime.ToString(@"hh\:mm\:ss\:fff")
						: mr.StartTime.ToString(@"mm\:ss\:fff");
					beatMarks.Add($"M{markNum},{mr.Text},{timeText},{mr.Color}");
					markNum++;
				}

				saveFileDialog.DefaultExt = ".csv";
				saveFileDialog.Filter = @"CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					string name = saveFileDialog.FileName;

					await using StreamWriter file = new StreamWriter(name);
					foreach (string bm in beatMarks)
					{
						await file.WriteLineAsync(bm);
					}
				}
			}
		}

		//This should be in an extension class, but the one in Vixen.Core adds a conflict with the AddRange in Catel.Collections
		//Need to find a better way to handle that and refactor this later.
		public static string ToHex(Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
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
