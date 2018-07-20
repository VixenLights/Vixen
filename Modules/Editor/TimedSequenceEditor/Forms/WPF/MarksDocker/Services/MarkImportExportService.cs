using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Catel.Collections;
using Common.Controls;
using NLog;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	public class MarkImportExportService
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private static string _lastFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


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


				var xdoc = XDocument.Load(openFileDialog.FileName);
				if (xdoc.Root != null)
				{
					Type type;
					bool migrate = false;
					if (xdoc.Root.Name.NamespaceName.Equals("http://schemas.datacontract.org/2004/07/VixenModules.App.Marks"))
					{
						type = typeof(List<MarkCollection>);
					}
					else if (xdoc.Root.Name.NamespaceName.Equals("http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed")
					)
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
							DataContractSerializer ser = new DataContractSerializer(type);
							var markCollections = ser.ReadObject(reader);
							if (!migrate)
							{
								var imc = markCollections as List<MarkCollection>;
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

		private static void MigrateMarkCollections(ObservableCollection<IMarkCollection> collections, List<Sequence.Timed.MarkCollection> oldCollections)
		{
			foreach (var markCollection in oldCollections)
			{
				var lmc = new MarkCollection();
				lmc.Name = markCollection.Name;
				lmc.Level = markCollection.Level;
				lmc.ShowGridLines = markCollection.Enabled;
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

		private static void SetDefaultCollection(ObservableCollection<IMarkCollection> collections)
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
							string mark;
							if (line.IndexOf("\t") > 0)
							{
								mark = line.Split('\t')[0].Trim();
							}
							else
							{
								mark = line.Trim().Split(' ')[0].Trim();
							}

							TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(mark));
							mc.AddMark(new Mark(time));
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
				try
				{
					var xmlDoc = new XmlDocument();
					xmlDoc.Load(openFileDialog.FileName);
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
		}

		private static void ProcessTiming(XmlNode timingNode,  ObservableCollection<IMarkCollection> collections)
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
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			PapagayoDoc papagayoFile = new PapagayoDoc();
			string fileName = openDialog.FileName;
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
				markCollection.Add(phraseCollection);
				markCollection.Add(wordCollection);
				markCollection.Add(phonemeCollection);


				phonemeCollection = new MarkCollection();
				phonemeCollection.Name = $"{fileWithoutExtension} {voice} Phonemes Coalesced";
				phonemeCollection.ShowMarkBar = true;
				phonemeCollection.Decorator.Color = Color.FromArgb(245, 75, 210); ;
				foreach (var phoneme in papagayoFile.PhonemeList(voice))
				{
					var mark = new Mark(TimeSpan.FromMilliseconds(phoneme.StartMS));
					mark.Duration = TimeSpan.FromMilliseconds(phoneme.EndMS) - mark.StartTime;
					mark.Text = phoneme.TypeName;
					phonemeCollection.AddMark(mark);
				}
				markCollection.Add(phonemeCollection);
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


		//Beat Mark Collection Export routine 2-7-2014 JMB
		//In the audacity section, if the MarkCollections.Count = 1 then we assume the collection is bars and iMarkCollection++
		//Other wise its beats, at least from the information I have studied, and we do not iMarkCollection++ to keep the collections together properly.
		public static void ExportMarkCollections(string exportType, ObservableCollection<IMarkCollection> collections)
		{
			var saveFileDialog = new SaveFileDialog();
			if (exportType == "vixen3")
			{
				saveFileDialog.DefaultExt = ".v3m";
				saveFileDialog.Filter = @"Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					var xmlsettings = new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "\t"
					};

					DataContractSerializer ser = new DataContractSerializer(typeof(List<IMarkCollection>));
					var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
					ser.WriteObject(writer, collections);
					writer.Close();
				}
			}

			if (exportType == "audacity")
			{
				int iMarkCollection = 0;
				List<string> beatMarks = new List<string>();
				foreach (MarkCollection mc in collections)
				{
					iMarkCollection++;
					foreach (Mark mark in mc.Marks)
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

					using (StreamWriter file = new StreamWriter(name))
					{
						foreach (string bm in beatMarks.OrderBy(x => x))
						{
							file.WriteLine(bm);
						}
					}
				}
			}
		}
	}
}
