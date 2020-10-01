using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.IO;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Alternating;
using VixenModules.Effect.Chase;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Fireworks;
using VixenModules.Effect.Snowflakes;
using VixenModules.Effect.Spin;
using VixenModules.Effect.LipSync;
using VixenModules.Effect.Liquid;
using VixenModules.Effect.Wipe;
using ZedGraph;
using Liquid;
using Vixen.Module.Preview;
using Vixen.Sys;

namespace VixenModules.Sequence.Timed
{
	/// <summary>
	/// Class to conduct sequence migrations
	/// The Version atribute in the descriptor for the sequence determines the current version and is
	/// used to determine if migration should occur
	/// </summary>
	public class TimedSequenceMigrator : IContentMigrator<XElement>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public TimedSequenceMigrator()
		{
			ValidMigrations = new[]
				{
									new MigrationSegment<XElement>(0, 1, _Version_0_to_1),
									new MigrationSegment<XElement>(1, 2, _Version_1_to_2),
									new MigrationSegment<XElement>(2, 3, _Version_2_to_3),
									new MigrationSegment<XElement>(3, 4, _Version_3_to_4),
									new MigrationSegment<XElement>(4, 5, _Version_4_to_5),
									new MigrationSegment<XElement>(5, 6, _Version_5_to_6),
									new MigrationSegment<XElement>(6, 7, _Version_6_to_7),
									new MigrationSegment<XElement>(7, 8, _Version_7_to_8),
				};
		}

		private static bool CanShowMessage()
		{
			//TODO Not thrilled with the use of this logic. This whole message thing needs to be refactored.
			if (VixenSystem.UIThread == Thread.CurrentThread)
			{
				return true;
			}

			return false;
		}

		public XElement MigrateContent(XElement content, int fromVersion, int toVersion)
		{
			IMigrationSegment<XElement> migrationSegment =
				ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
			if (migrationSegment == null) {
				throw new InvalidOperationException("Cannot migrate content from version " + fromVersion + " to version " +
				toVersion);
			}
			content = migrationSegment.Execute(content);
			ShowCompleteMessage(fromVersion, toVersion);
			
			return content;
		}

		object IContentMigrator.MigrateContent(object content, int fromVersion, int toVersion)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");

			return MigrateContent((XElement) content, fromVersion, toVersion);
		}

		public IEnumerable<IMigrationSegment<XElement>> ValidMigrations { get; private set; }

		IEnumerable<IMigrationSegment> IContentMigrator.ValidMigrations
		{
			get { return ValidMigrations; }
		}
		private static void ShowCompleteMessage(int fromVersion, int toVersion)
		{
			var msg =
				$"Migration from version {fromVersion} to {toVersion} is complete. You will need to save the sequence in the editor for the migration to persist or use it in a Vixen scheduled show.";
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(msg,
					"Sequence Upgrade", MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}
			else
			{
				Logging.Info(msg);
			}
		}


		private XElement _Version_0_to_1(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 0 to version 1. Changes include moving Nutcracker and Audio files to the common media folder.{0}{0}" +
						"These changes are not backward compatible", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			//  3/14/2015
			//Migrate full path name of the background image to just the filename. Code will now look 
 			//relative to the profile for the module path to the filenames
			//This is the first introduction of versioning for sequences so not versioned files are considered version 0
			//new files and migrated ones will have a version atribute in the root element
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed";
			namespaces.AddNamespace("", ns.NamespaceName);
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Nutcracker";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);
			XNamespace d1p1 = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			namespaces.AddNamespace("d1p1", d1p1.NamespaceName);

			//Fix the paths on the Nutcracker Picture filenames so they are now relative to the profile instead of full paths
			IEnumerable<XElement> fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Picture_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}

			//Fix the paths on the Nutcracker Picture filenames  so they are now relative to the profile instead of full paths
			fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:PictureTile_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				if (!IsNutcrackerResource(fileNameElement.Value))
				{
					string fileName = Path.GetFileName(fileNameElement.Value);
					fileNameElement.SetValue(fileName);	
				}
				
			}

			//Fix the paths on the Nutcracker Glediator filenames so they are now relative to the profile instead of full paths
			fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Glediator_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}


			//Fix the audio paths on Media so they are now relative to the profile instead of full paths
			fileNameElements =
				content.XPathSelectElements(
					"_mediaSurrogates/MediaSurrogate/FilePath",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string filePath = fileNameElement.Value;
				string fileName = Path.GetFileName(filePath);
				fileNameElement.SetValue(fileName);
				fileNameElement.Name = "FileName";
				string newPath = Path.Combine(MediaService.MediaDirectory, fileName);
				if (File.Exists(filePath) && !File.Exists(newPath))
				{
					File.Copy(filePath, newPath);	
				}
					
			}

			//Fix the provider source name
			IEnumerable<XElement> timingElements =
				content.XPathSelectElements(
					"_selectedTimingProviderSurrogate",
					namespaces);

			foreach (var timingElement in timingElements)
			{
				XElement type = timingElement.Element("ProviderType");
				if (type != null && type.Value.Equals("Media"))
				{
					XElement source = timingElement.Element("SourceName");
					if (source != null)
					{
						string fileName = Path.GetFileName(source.Value);
						source.SetValue(fileName);		
					}
					
				}
				
			}
			
			return content;
		}

		private XElement _Version_1_to_2(XElement content)
		{

			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 1 to version 2. Changes include upgrades to the Alternating effect to allow more than 2 colors.{0}{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			//This migration deals with changing the Alternating effect to a Multi Alternating
			//Style that allows N number of colors. 
			var namespaces = GetStandardNamespaces();
			//Add in our specific ones
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Alternating";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);
			
			
			//Find the Alternating effects.
			IEnumerable<XElement> alternatingElements =
				content.XPathSelectElements(
				"_dataModels/d1p1:anyType[@i:type = 'd2p1:AlternatingData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels",namespaces);

			foreach (var alternatingElement in alternatingElements.ToList())
			{
				//Find all the data points.
				XElement isStaticColor1 = alternatingElement.XPathSelectElement("d2p1:StaticColor1", namespaces);
				XElement isStaticColor2 = alternatingElement.XPathSelectElement("d2p1:StaticColor2", namespaces);
				XElement color1 = alternatingElement.XPathSelectElement("d2p1:Color1", namespaces);
				XElement level1 = alternatingElement.XPathSelectElement("d2p1:Level1", namespaces);
				XElement colorGradient1 = alternatingElement.XPathSelectElement("d2p1:ColorGradient1", namespaces);
				XElement curve1 = alternatingElement.XPathSelectElement("d2p1:Curve1", namespaces);
				XElement color2 = alternatingElement.XPathSelectElement("d2p1:Color2", namespaces);
				XElement level2 = alternatingElement.XPathSelectElement("d2p1:Level2", namespaces);
				XElement colorGradient2 = alternatingElement.XPathSelectElement("d2p1:ColorGradient2", namespaces);
				XElement curve2 = alternatingElement.XPathSelectElement("d2p1:Curve2", namespaces);
				XElement enable = alternatingElement.XPathSelectElement("d2p1:Enable", namespaces);
				XElement groupEffect = alternatingElement.XPathSelectElement("d2p1:GroupEffect", namespaces);
				XElement interval = alternatingElement.XPathSelectElement("d2p1:Interval", namespaces);
				XElement moduleInstanceId = alternatingElement.XPathSelectElement("ModuleInstanceId", namespaces);
				XElement moduleTypeId = alternatingElement.XPathSelectElement("ModuleTypeId", namespaces);

				//Determine which of the old types were used, colors vs gradients
				//The new only uses gradients and levels so take the two and build up the new object
				var gradientLevelPairs = new List<GradientLevelPair>();

				//Colors are used backwards in the old effect, so pick them off in reverse.
				if (isStaticColor2.Value.Equals("true"))
				{
					RGB c2 = DeSerializer<RGB>(color2);
					double l2 = DeSerializer<double>(level2);
					l2 *= 100;
					gradientLevelPairs.Add(new GradientLevelPair(c2,
						new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { l2, l2 }))));
				}
				else
				{
					ColorGradient cg2 = DeSerializer<ColorGradient>(colorGradient2);
					Curve c2 = DeSerializer<Curve>(curve2);
					gradientLevelPairs.Add(new GradientLevelPair(cg2, c2));
				}


				if (isStaticColor1.Value.Equals("true"))
				{
					RGB c1 = DeSerializer<RGB>(color1);
					double l1 = DeSerializer<double>(level1);
					l1 *= 100;
					gradientLevelPairs.Add(new GradientLevelPair(c1,
						new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {l1, l1}))));
				}
				else
				{
					ColorGradient cg1 = DeSerializer<ColorGradient>(colorGradient1);	
					Curve c1 = DeSerializer<Curve>(curve1);
					gradientLevelPairs.Add(new GradientLevelPair(cg1, c1));	
				}
				

				//Build the new data model
				AlternatingData data = new AlternatingData
				{
					Colors = gradientLevelPairs,
					ModuleInstanceId = DeSerializer<Guid>(moduleInstanceId),
					ModuleTypeId = DeSerializer<Guid>(moduleTypeId),
					EnableStatic = !DeSerializer<bool>(enable),
					GroupLevel = DeSerializer<int>(groupEffect),
					Interval = DeSerializer<int>(interval),
					IntervalSkipCount = 1
				};

				//Remove the old data model from the xml
				alternatingElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = {data};
				DataContainer dc = new DataContainer {_dataModels = dm};

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(AlternatingData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}

			return content;
		}

		private XElement _Version_2_to_3(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 2 to version 3. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}{0}Snowflakes and Fireworks now allow more color options as well as enhanced features.{0}" +
						"Snowflakes had a bug where the flakes only went one direction. This has been corrected, so you may see some different behavior than before. " +
						"You may need to set the string orientation to get them going the right direction. Please review them.{0}{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateSnowflakesFrom2To3(content);

			MigrateFireworksFrom2To3(content);

			return content;
		}

		private static void MigrateSnowflakesFrom2To3(XElement content)
		{
			//This migration deals with changing the Snowflake effect to accomodate multiple gradients instead of single colors for inner and outer
			//Getthe standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Snowflakes";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Snowflakes effects.
			IEnumerable<XElement> snowFlakeElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:SnowflakesData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var snowflakeElement in snowFlakeElements.ToList())
			{
				//Find all the data points we need to keep.
				XElement outerColor = snowflakeElement.XPathSelectElement("d2p1:OuterColor", namespaces);
				XElement centerColor = snowflakeElement.XPathSelectElement("d2p1:CenterColor", namespaces);
				XElement flakeCount = snowflakeElement.XPathSelectElement("d2p1:FlakeCount", namespaces);
				XElement level = snowflakeElement.XPathSelectElement("d2p1:LevelCurve", namespaces);
				XElement speed = snowflakeElement.XPathSelectElement("d2p1:Speed", namespaces);
				XElement snowFlakeType = snowflakeElement.XPathSelectElement("d2p1:SnowflakeType", namespaces);

				XElement moduleInstanceId = snowflakeElement.XPathSelectElement("ModuleInstanceId", namespaces);
				XElement moduleTypeId = snowflakeElement.XPathSelectElement("ModuleTypeId", namespaces);

				//Build up our new replacement model
				SnowflakesData snowflakesData = new SnowflakesData()
				{
					ModuleInstanceId = DeSerializer<Guid>(moduleInstanceId),
					ModuleTypeId = DeSerializer<Guid>(moduleTypeId),
					OutSideColor = new List<ColorGradient>(new[] {new ColorGradient(DeSerializer<Color>(outerColor))}),
					InnerColor = new List<ColorGradient>(new[] {new ColorGradient(DeSerializer<Color>(centerColor))}),
					FlakeCount = DeSerializer<int>(flakeCount),
					LevelCurve = DeSerializer<Curve>(level),
					Speed = DeSerializer<int>(speed),
					SnowflakeType = DeSerializer<SnowflakeType>(snowFlakeType),
					RandomSpeed = false,
					RandomBrightness = false,
					PointFlake45 = false
				};

				//Remove the old version
				snowflakeElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = {snowflakesData};
				DataContainer dc = new DataContainer {_dataModels = dm};

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(SnowflakesData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private static void MigrateFireworksFrom2To3(XElement content)
		{
			//This migration deals with changing the Fireworks effect to accomodate multiple gradients instead of miltiple colors
			//Get the standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Fireworks";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Snowflakes effects.
			IEnumerable<XElement> fireworksElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:FireworksData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var fireworkElement in fireworksElements.ToList())
			{
				//Find all the data points we need to keep.
				XElement colors = fireworkElement.XPathSelectElement("d2p1:Colors", namespaces);
				XElement explosions = fireworkElement.XPathSelectElement("d2p1:Explosions", namespaces);
				XElement particleFade = fireworkElement.XPathSelectElement("d2p1:ParticleFade", namespaces);
				XElement level = fireworkElement.XPathSelectElement("d2p1:LevelCurve", namespaces);
				XElement particles = fireworkElement.XPathSelectElement("d2p1:Particles", namespaces);
				XElement velocity = fireworkElement.XPathSelectElement("d2p1:Velocity", namespaces);

				XElement moduleInstanceId = fireworkElement.XPathSelectElement("ModuleInstanceId", namespaces);
				XElement moduleTypeId = fireworkElement.XPathSelectElement("ModuleTypeId", namespaces);

				var colorList = DeSerializer<List<Color>>(colors);
				List<ColorGradient> colorGradients = colorList.Select(color => new ColorGradient(color)).ToList();

				//Build up our new replacement model
				FireworksData data = new FireworksData()
				{
					ModuleInstanceId = DeSerializer<Guid>(moduleInstanceId),
					ModuleTypeId = DeSerializer<Guid>(moduleTypeId),
					Velocity = DeSerializer<int>(velocity),
					Explosions = DeSerializer<int>(explosions),
					ParticleFade = DeSerializer<int>(particleFade),
					Particles = DeSerializer<int>(particles),
					LevelCurve = DeSerializer<Curve>(level),
					ColorGradients = colorGradients,
					RandomParticles = false,
					RandomVelocity = false
				};

				//Remove the old version
				fireworkElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = { data };
				DataContainer dc = new DataContainer { _dataModels = dm };

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(FireworksData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private XElement _Version_3_to_4(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 3 to version 4. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}{0}" +
						"Minor changes to how the default brightness is handled in the Chase and Spin effect to make it easier to use in layer mixing.{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateChaseFrom3To4(content);
			MigrateSpinFrom3To4(content);

			return content;
		}

		private XElement _Version_4_to_5(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 4 to version 5. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}{0}" +
						"Minor change to allow LipSync Matrix elements to work with Bitmap pictures .{0}" +
						"Changes to allow compatibility with new Vixen3 ModuleStore.{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateLipSyncFrom4To5(content);
			return content;
		}

		private XElement _Version_5_to_6(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 5 to version 6. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}{0}" +
						"Changes to allow LipSync string elements to work with Face properties instead of maps.{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateLipSyncFrom5To6(content);
			return content;
		}

		private XElement _Version_6_to_7(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 6 to version 7. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}" +
						"Changes to the Wipe direction property, Add movement curve and a reverse direction option.{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateWipeFrom6To7(content);
			return content;
		}

		private XElement _Version_7_to_8(XElement content)
		{
			if (CanShowMessage())
			{
				var messageBox = new MessageBoxForm(string.Format(
						"Migrating sequence from version 7 to version 8. This may take a few minutes if the sequence is large.{0}{0}Changes include the following:{0}" +
						"Minor changes to the Liquid Effect, adding the ability to specify the start position of animated emitters.{0}" +
						"These changes are not backward compatible.", Environment.NewLine), "Sequence Upgrade",
					MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.ShowDialog();
			}

			MigrateLiquidFrom7To8(content);
			return content;
		}

		private void MigrateChaseFrom3To4(XElement content)
		{
			//This migration deals with changing the Fireworks effect to accomodate multiple gradients instead of miltiple colors
			//Get the standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Chase";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Chase effects.
			IEnumerable<XElement> chaseElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:ChaseData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var chaseElement in chaseElements.ToList())
			{
				var chasedata = DeSerializer<ChaseData>(chaseElement);

				if (chasedata.DefaultLevel > 0)
				{
					chasedata.EnableDefaultLevel = true;
				}

				//Remove the old version
				chaseElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = { chasedata };
				DataContainer dc = new DataContainer { _dataModels = dm };

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(ChaseData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private void MigrateSpinFrom3To4(XElement content)
		{
			//This migration deals with changing the Fireworks effect to accomodate multiple gradients instead of miltiple colors
			//Get the standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Spin";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Spin effects.
			IEnumerable<XElement> xElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:SpinData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var xElement in xElements.ToList())
			{
				var spindata = DeSerializer<SpinData>(xElement);

				if (spindata.DefaultLevel > 0)
				{
					spindata.EnableDefaultLevel = true;
				}

				//Remove the old version
				xElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = { spindata };
				DataContainer dc = new DataContainer { _dataModels = dm };

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(SpinData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private void MigrateLipSyncFrom4To5(XElement content)
		{	
		   //This migration deals with changing the LipSync matrix elements from version 4 to 5
		   //Get the standard namespaces that are needed in the sequence
		   var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.LipSync";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Chase effects.
			IEnumerable<XElement> lipSyncElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:LipSyncData']", namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			LipSyncMapLibrary _library = 
				ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;

			foreach (var lipSyncElement in lipSyncElements.ToList())
			{
				LipSyncMapData mapData = null;

				var lipSyncData = DeSerializer<LipSyncData>(lipSyncElement);

				if (_library.Library.TryGetValue(lipSyncData.PhonemeMapping, out mapData))
				{
					if ((null != mapData) && (mapData.IsMatrix))
					{
						if (lipSyncData.Level == 0)
						{
							lipSyncData.Level = 100;
						}

						if (lipSyncData.ScalePercent == 0)
						{
							lipSyncData.ScalePercent = 100;
						}

						lipSyncData.Orientation = (mapData.StringsAreRows) ? StringOrientation.Horizontal : StringOrientation.Vertical;

						lipSyncData.ScaleToGrid = true;
						mapData.UsingDefaults = false;
						
					}
				}

				//Remove the old version
				lipSyncElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = { lipSyncData };
				DataContainer dc = new DataContainer { _dataModels = dm };

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(LipSyncData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private void MigrateLipSyncFrom5To6(XElement content)
		{
			//This migration deals with changing the LipSync matrix elements from version 4 to 5
			//Get the standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.LipSync";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Chase effects.
			IEnumerable<XElement> lipSyncElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:LipSyncData']", namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			LipSyncMapLibrary library =
				ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;

			if (library != null)
			{
				foreach (var lipSyncElement in lipSyncElements.ToList())
				{
					LipSyncMapData mapData;

					var lipSyncData = DeSerializer<LipSyncData>(lipSyncElement);

					if (library.Library.TryGetValue(lipSyncData.PhonemeMapping, out mapData))
					{
						if (!mapData.IsMatrix)
						{
							lipSyncData.MappingType = MappingType.FaceDefinition;
						}
					}
					else
					{
						lipSyncData.MappingType = MappingType.FaceDefinition;
					}

					//Remove the old version
					lipSyncElement.Remove();

					//Build up a temporary container similar to the way sequences are stored to
					//make all the namespace prefixes line up.
					IModuleDataModel[] dm = { lipSyncData };
					DataContainer dc = new DataContainer { _dataModels = dm };

					//Serialize the object into a xelement
					XElement glp = Serializer(dc, new[] { typeof(LipSyncData), typeof(IModuleDataModel[]), typeof(DataContainer) });

					//Extract the new data model that we want and insert it in the tree
					datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
				}
			}
			
		}

		private void MigrateWipeFrom6To7(XElement content)
		{
			// This migration deals with changing the Wipe Direction property to reduce the number of options in the Direction and add in reverse option.
			// Add Wipe movement and movement curve.
			// Get the standard namespaces that are needed in the sequence
			var namespaces = GetStandardNamespaces();
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Wipe";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			//Find the Wipe effects.
			IEnumerable<XElement> wipeElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:WipeData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var wipeElement in wipeElements.ToList())
			{
				XElement direction = wipeElement.XPathSelectElement("d2p1:Direction", namespaces);
				XElement colorGradient = wipeElement.XPathSelectElement("d2p1:ColorGradient", namespaces);
				XElement colorHandling = wipeElement.XPathSelectElement("d2p1:ColorHandling", namespaces);
				XElement passCount = wipeElement.XPathSelectElement("d2p1:PassCount", namespaces);
				XElement pulsePercent = wipeElement.XPathSelectElement("d2p1:PulsePercent", namespaces);
				XElement pulseTime = wipeElement.XPathSelectElement("d2p1:PulseTime", namespaces);
				XElement wipeByCount = wipeElement.XPathSelectElement("d2p1:WipeByCount", namespaces);
				XElement wipeOn = wipeElement.XPathSelectElement("d2p1:WipeOn", namespaces);
				XElement wipeOff = wipeElement.XPathSelectElement("d2p1:WipeOff", namespaces);
				XElement curve = wipeElement.XPathSelectElement("d2p1:Curve", namespaces);

				XElement moduleInstanceId = wipeElement.XPathSelectElement("ModuleInstanceId", namespaces);
				XElement moduleTypeId = wipeElement.XPathSelectElement("ModuleTypeId", namespaces);

				WipeDirection dataDirection = WipeDirection.Burst;
				bool dataReverseDirection = false;

				switch (direction.Value)
				{
					case "Up":
						dataDirection = WipeDirection.Vertical;
						dataReverseDirection = true;
						break;
					case "Down":
						dataDirection = WipeDirection.Vertical;
						dataReverseDirection = false;
						break;
					case "Right":
						dataDirection = WipeDirection.Horizontal;
						dataReverseDirection = false;
						break;
					case "Left":
						dataDirection = WipeDirection.Horizontal;
						dataReverseDirection = true;
						break;
					case "In":
						dataDirection = WipeDirection.Burst;
						dataReverseDirection = true;
						break;
					case "Out":
						dataDirection = WipeDirection.Burst;
						dataReverseDirection = false;
						break;
				}

				WipeData data = new WipeData
				{
					ColorGradient = new ColorGradient(DeSerializer<ColorGradient>(colorGradient)),
					ColorHandling = DeSerializer<ColorHandling>(colorHandling),
					PassCount = DeSerializer<int>(passCount),
					PulsePercent = DeSerializer<double>(pulsePercent),
					PulseTime = DeSerializer<int>(pulseTime),

					WipeMovement = DeSerializer<bool>(wipeByCount) ? WipeMovement.Count : WipeMovement.PulseLength,

					WipeOn = DeSerializer<bool>(wipeOn),
					WipeOff = DeSerializer<bool>(wipeOff),
					ColorAcrossItemPerCount = true,
					ReverseColorDirection = true,
					Curve = DeSerializer<Curve>(curve),
					MovementCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 })),
					Direction = dataDirection,
					ReverseDirection = dataReverseDirection,
					ModuleInstanceId = DeSerializer<Guid>(moduleInstanceId),
					ModuleTypeId = DeSerializer<Guid>(moduleTypeId)
				};

				//Remove the old version
				wipeElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = { data };
				DataContainer dc = new DataContainer { _dataModels = dm };

				//Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(WipeData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private void MigrateLiquidFrom7To8(XElement content)
		{
			// This migration deals with changing the Liquid effect to accomodate allowing the user to control
			// the initial position of anmiated emitters.
			var namespaces = GetStandardNamespaces();
			//Add in the ones for this effect
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Liquid";				            
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);

			// Find the Liquid effects
			IEnumerable<XElement> liquidElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType[@i:type = 'd2p1:LiquidData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels", namespaces);

			foreach (var liquidElement in liquidElements.ToList())
			{
				var liquidData = DeSerializer<LiquidData>(liquidElement);

				foreach(EmitterData emitterData in liquidData.EmitterData)
				{
					emitterData.RandomStartingPosition = true;					
				}

				// Remove the old version
				liquidElement.Remove();

				// Build up a temporary container similar to the way sequences are stored to
				// make all the namespace prefixes line up.
				IModuleDataModel[] dm = { liquidData };
				DataContainer dc = new DataContainer { _dataModels = dm };

				// Serialize the object into a xelement
				XElement glp = Serializer(dc, new[] { typeof(LiquidData), typeof(IModuleDataModel[]), typeof(DataContainer) });

				// Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		}

		private static XmlNamespaceManager GetStandardNamespaces()
		{
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed";
			namespaces.AddNamespace("", ns.NamespaceName);
			XNamespace d1p1 = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			namespaces.AddNamespace("d1p1", d1p1.NamespaceName);
			XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
			namespaces.AddNamespace("i", i.NamespaceName);
			XNamespace a = "http://www.w3.org/2001/XMLSchema";
			namespaces.AddNamespace("a", a.NamespaceName);
			return namespaces;
		}

		static T DeSerializer<T>(XElement element, Type[] knownTypes = null)
		{
			if (element == null)
			{
				return default(T);
			}
			DataContractSerializer serializer;
			if (knownTypes == null)
			{
				serializer = new DataContractSerializer(typeof (T), element.Name.LocalName, element.Name.NamespaceName);
			}
			else
			{
				serializer = new DataContractSerializer(typeof(T), element.Name.LocalName, element.Name.NamespaceName, knownTypes);
			}
			return (T)serializer.ReadObject(element.CreateReader());
		}

		static XElement Serializer(DataContainer data, Type[] knownTypes)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					//Encoding = Encoding.ASCII,
					Indent = true,
					NamespaceHandling = NamespaceHandling.OmitDuplicates
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
				{
					_WriteSequenceDataToXmlWriter(data, xmlWriter, knownTypes);
					xmlWriter.Flush();
				}

				stream.Seek(0, SeekOrigin.Begin);

				using (var streamReader = new StreamReader(stream))
				{
					string result = streamReader.ReadToEnd();
					return XElement.Parse(result);
				}

				
			}
		}

		private static void _WriteSequenceDataToXmlWriter(DataContainer data, XmlWriter xmlWriter, Type[] knownTypes)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(DataContainer), knownTypes);
			serializer.WriteStartObject(xmlWriter, data);
			xmlWriter.WriteAttributeString("xmlns", "a", null, "http://www.w3.org/2001/XMLSchema");
			serializer.WriteObjectContent(xmlWriter, data);
			xmlWriter.WriteEndElement();
		}

		private bool IsNutcrackerResource(string s)
		{
			return s.Contains("VixenModules.Effect.Nutcracker");
		}

		
	}

	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed")]
	internal class DataContainer
	{
		[DataMember]
		internal IModuleDataModel[] _dataModels;
	}
}