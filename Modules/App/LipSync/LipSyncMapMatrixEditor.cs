using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;
using Common.Resources.Properties;
using Vixen.Sys;
using Common.Controls;
using Common.Controls.Theme;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapMatrixEditor: Common.Controls.BaseForm
	{
		private static int MIN_PICTUREBOX_WIDTH = 96;
		private static int MIN_PICTUREBOX_HEIGHT = 96;
		private static int MAX_PICTUREBOX_WIDTH = 1024;
		private static int MAX_PICTUREBOX_HEIGHT = 1024;

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private LipSyncMapData _origMapping;
		private LipSyncMapData _newMapping;
		private int currentPhonemeIndex;

		private static Dictionary<PhonemeType, Bitmap> _iconBitmaps = null;
		private PhonemeType[] iconArray;

		private Dictionary<string, Bitmap> _pictureBitmaps = null;

		private static Bitmap _noImageBmp = null;

		private string _moduleDirPath = null;
		private string _pictureDirPath = null;

		public LipSyncMapMatrixEditor()
		{
			this.LibraryMappingName = "Default";
			InitializeComponent();
			renderedPicture.MinimumSize = new Size(MIN_PICTUREBOX_WIDTH, MIN_PICTUREBOX_HEIGHT);
			renderedPicture.MaximumSize = new Size(MAX_PICTUREBOX_WIDTH, MAX_PICTUREBOX_HEIGHT);
			renderedPicture.SizeMode = PictureBoxSizeMode.CenterImage;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			SizeGripStyle = SizeGripStyle.Hide;

			_moduleDirPath = Paths.ModuleDataFilesPath + "\\LipSync";
			_pictureBitmaps = new Dictionary<string, Bitmap>();
			this.MapData = new LipSyncMapData();
			LoadIconsAndBitmaps();
		}

		public LipSyncMapMatrixEditor(LipSyncMapData mapData)
		{
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X - 150, ActiveForm.Location.Y - 100) : new Point(200, 100);
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			this.MapData = (LipSyncMapData)mapData.Clone();
			renderedPicture.MinimumSize = new Size(MIN_PICTUREBOX_WIDTH, MIN_PICTUREBOX_HEIGHT);
			renderedPicture.MaximumSize = new Size(MAX_PICTUREBOX_WIDTH, MAX_PICTUREBOX_HEIGHT);
			renderedPicture.SizeMode = PictureBoxSizeMode.CenterImage;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			SizeGripStyle = SizeGripStyle.Hide;

			_moduleDirPath = Paths.ModuleDataFilesPath + "\\LipSync";
			_pictureBitmaps = new Dictionary<string, Bitmap>();
			LoadIconsAndBitmaps();
		}

		public string LibraryMappingName
		{
			get { return nameTextBox.Text; }
			set
			{
				nameTextBox.Text = value;
			}
		}

		public LipSyncMapData MapData
		{
			get
			{
				return _newMapping;
			}

			set
			{
				_origMapping = value;
				_newMapping = (LipSyncMapData)_origMapping.Clone();

			}
		}

		public string HelperName
		{
			get { return "Phoneme Mapping"; }
		}

		public bool RenameMappingFiles(string newName)
		{
			bool retVal = CloneMappingFiles(newName);
			if (retVal)
			{
				retVal = RemoveMappingFiles();
			}
			return retVal;
		}

		public bool CloneMappingFiles(string newName)
		{
			bool retVal = false;
			string newDirName = _moduleDirPath + "\\" + newName;
			if (Directory.Exists(newDirName))
			{
				try
				{
					Directory.Delete(newDirName,true);
				}
				catch (Exception err) { }
			}

			if (Directory.Exists(PictureDirPath))
			{
				try
				{
					Directory.CreateDirectory(newDirName);

					//Copy all the files & Replaces any files with the same name
					foreach (string newPath in Directory.GetFiles(PictureDirPath, "*.*", SearchOption.AllDirectories))
					{
						File.Copy(newPath, newPath.Replace(PictureDirPath, newDirName), true);
					}
					retVal = true;
				}
				catch (IOException err)
				{
					var messageBox = new MessageBoxForm(
						"Unable to copy mapping data!" +
						Environment.NewLine + Environment.NewLine +
						err.Message,
						"Error!",
						MessageBoxButtons.OK,
						SystemIcons.Error);

					messageBox.ShowDialog();
				}
			}
			return retVal;
		}
		public bool RemoveMappingFiles()
		{
			bool retVal = false;
			if (Directory.Exists(PictureDirPath))
			{
				try
				{
					Directory.Delete(PictureDirPath,true);
					retVal = true;
				}
				catch (IOException err)
				{
					var messageBox = new MessageBoxForm(
						"Unable to mapping data!" +
						Environment.NewLine + Environment.NewLine +
						err.Message,
						"Error!",
						MessageBoxButtons.OK,
						SystemIcons.Error);

					messageBox.ShowDialog();
				}
			}
			return retVal;
		}

		private void LoadPictureMaps()
		{
			foreach (PhonemeType phoneme in Enum.GetValues(typeof(PhonemeType)))
			{
				string phonemeString = phoneme.ToString();
				string picPath = PictureDirPath + "\\" + phonemeString + ".bmp";
				if (File.Exists(picPath))
				{
					Bitmap fileBmp = new Bitmap(picPath);
					_pictureBitmaps[phonemeString] = new Bitmap(fileBmp);
					fileBmp.Dispose();
					fileBmp = null;
					
				}
			}
		} 

		private string PictureDirPath
		{
			get
			{
				if (_pictureDirPath == null)
				{
					_pictureDirPath = _moduleDirPath + "\\" + MapData.LibraryReferenceName;

					if (!Directory.Exists(_pictureDirPath))
					{
						Directory.CreateDirectory(_pictureDirPath);
					}
				}
				return _pictureDirPath;
			}

			set
			{
				_pictureDirPath = value;
			}
		}


		private void NextPhonmeIndex()
		{
			currentPhonemeIndex++;
			currentPhonemeIndex %= iconArray.Count();
			SetPhonemeIconAndPicture();
		}

		private void PrevPhonemeIndex()
		{
			currentPhonemeIndex = 
				(currentPhonemeIndex == 0) ? iconArray.Count() : currentPhonemeIndex;
			currentPhonemeIndex--;
			SetPhonemeIconAndPicture();
		}

		private string CurrentPhonemeString
		{
			get
			{
				return iconArray[currentPhonemeIndex].ToString();
			}
		}

		private void SetPhonemeIconAndPicture()
		{
			phonemeIcon.Image = 
				new Bitmap(_iconBitmaps[iconArray[currentPhonemeIndex]], 48, 48);
			phonemeLabel.Text = CurrentPhonemeString;

			renderPictureBoxImage();
		}

		private void LoadIconsAndBitmaps()
		{
			Assembly assembly = Assembly.Load("LipSyncApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
			if (assembly != null)
			{
				ResourceManager lipSyncRM = new ResourceManager("VixenModules.App.LipSyncApp.LipSyncResources", assembly);
				if (_iconBitmaps == null)
				{
					_iconBitmaps = new Dictionary<PhonemeType, Bitmap>();
					_iconBitmaps.Add(PhonemeType.AI, (Bitmap)lipSyncRM.GetObject("AI"));
					_iconBitmaps.Add(PhonemeType.E, (Bitmap)lipSyncRM.GetObject("E"));
					_iconBitmaps.Add(PhonemeType.ETC, (Bitmap)lipSyncRM.GetObject("etc"));
					_iconBitmaps.Add(PhonemeType.FV, (Bitmap)lipSyncRM.GetObject("FV"));
					_iconBitmaps.Add(PhonemeType.L, (Bitmap)lipSyncRM.GetObject("L"));
					_iconBitmaps.Add(PhonemeType.MBP, (Bitmap)lipSyncRM.GetObject("MBP"));
					_iconBitmaps.Add(PhonemeType.O, (Bitmap)lipSyncRM.GetObject("O"));
					_iconBitmaps.Add(PhonemeType.REST, (Bitmap)lipSyncRM.GetObject("rest"));
					_iconBitmaps.Add(PhonemeType.U, (Bitmap)lipSyncRM.GetObject("U"));
					_iconBitmaps.Add(PhonemeType.WQ, (Bitmap)lipSyncRM.GetObject("WQ"));

					_noImageBmp = (Bitmap)lipSyncRM.GetObject("NoImage");
				}

				if (this.MapData.UsingDefaults)
				{
					_pictureBitmaps.Add(PhonemeType.AI.ToString(), (Bitmap)lipSyncRM.GetObject("AI_Transparent"));
					_pictureBitmaps.Add(PhonemeType.E.ToString(), (Bitmap)lipSyncRM.GetObject("E_Transparent"));
					_pictureBitmaps.Add(PhonemeType.ETC.ToString(), (Bitmap)lipSyncRM.GetObject("etc_Transparent"));
					_pictureBitmaps.Add(PhonemeType.FV.ToString(), (Bitmap)lipSyncRM.GetObject("FV_Transparent"));
					_pictureBitmaps.Add(PhonemeType.L.ToString(), (Bitmap)lipSyncRM.GetObject("L_Transparent"));
					_pictureBitmaps.Add(PhonemeType.MBP.ToString(), (Bitmap)lipSyncRM.GetObject("MBP_Transparent"));
					_pictureBitmaps.Add(PhonemeType.O.ToString(), (Bitmap)lipSyncRM.GetObject("O_Transparent"));
					_pictureBitmaps.Add(PhonemeType.REST.ToString(), (Bitmap)lipSyncRM.GetObject("rest_Transparent"));
					_pictureBitmaps.Add(PhonemeType.U.ToString(), (Bitmap)lipSyncRM.GetObject("U_Transparent"));
					_pictureBitmaps.Add(PhonemeType.WQ.ToString(), (Bitmap)lipSyncRM.GetObject("WQ_Transparent"));
					nameTextBox.Text = MapData.LibraryReferenceName;
					savePicBitmaps();
					_pictureBitmaps.Clear();
					this.MapData.UsingDefaults = false;
				}
			}

			currentPhonemeIndex = 0;
			iconArray = _iconBitmaps.Keys.ToArray();
			LoadPictureMaps();
			SetPhonemeIconAndPicture();
		}

		private void OnLoad(object sender, EventArgs e)
		{
			this.ForeColor = ThemeColorTable.ForeColor;
			this.BackColor = ThemeColorTable.BackgroundColor;

			nameLabel.ForeColor = ThemeColorTable.ForeColor;
			nameLabel.BackColor = ThemeColorTable.BackgroundColor;
			nameTextBox.ForeColor = ThemeColorTable.ForeColor;
			nameTextBox.BackColor = ThemeColorTable.TextBoxBackgroundColor;
			phonemeLabel.ForeColor = ThemeColorTable.ForeColor;
			phonemeLabel.BackColor = ThemeColorTable.BackgroundColor;
			prevPhonemeButton.ForeColor = ThemeColorTable.ButtonTextColor;
			prevPhonemeButton.BackColor = ThemeColorTable.ButtonBackColor;
			nextPhonemeButton.ForeColor = ThemeColorTable.ButtonTextColor;
			nextPhonemeButton.BackColor = ThemeColorTable.ButtonBackColor;
			phonemeIcon.ForeColor = ThemeColorTable.ForeColor;
			phonemeIcon.BackColor = ThemeColorTable.BackgroundColor;
			openButton.ForeColor = ThemeColorTable.ButtonTextColor;
			openButton.BackColor = ThemeColorTable.ButtonBackColor;
			editButton.ForeColor = ThemeColorTable.ButtonTextColor;
			editButton.BackColor = ThemeColorTable.ButtonBackColor;
			clearButton.ForeColor = ThemeColorTable.ButtonTextColor;
			clearButton.BackColor = ThemeColorTable.ButtonBackColor;
			buttonOK.ForeColor = ThemeColorTable.ButtonTextColor;
			buttonOK.BackColor = ThemeColorTable.ButtonBackColor;
			buttonCancel.ForeColor = ThemeColorTable.ButtonTextColor;
			buttonCancel.BackColor = ThemeColorTable.ButtonBackColor;
			notesLabel.ForeColor = ThemeColorTable.ForeColor;
			notesLabel.BackColor = ThemeColorTable.BackgroundColor;
			notesTextBox.ForeColor = ThemeColorTable.ForeColor;
			notesTextBox.BackColor = ThemeColorTable.TextBoxBackgroundColor;

			nameTextBox.Text = MapData.LibraryReferenceName;
			notesTextBox.Text = MapData.Notes;
			
		}

		private void OnResizeEnd(object sender, EventArgs e)
		{
		}

		private void savePicBitmaps()
		{
			string fileName = null;
			Bitmap saveBmp = null;

			resetNewLibraryName();
			MapData.Notes = notesTextBox.Text;

			MapData.IsMatrix = false;
			this.UseWaitCursor = true;
			foreach(PhonemeType phoneme in Enum.GetValues(typeof(PhonemeType)))
			{
				try
				{
					fileName = phoneme.ToString() + ".bmp";
					DirectoryInfo pictureDirInfo = new DirectoryInfo(PictureDirPath);
					FileInfo[] fi = pictureDirInfo.GetFiles(fileName);

					foreach (FileInfo file in fi)
					{
						fi[0].Delete();
					}
				}
				catch (Exception e)
				{
					String errorMsg = "Unable to delete LipSync bitmap " + fileName + " from Module Data Directory";
					Logging.LogException(NLog.LogLevel.Warn, errorMsg, e);
				}

				try
				{
					if (_pictureBitmaps.TryGetValue(phoneme.ToString(),out saveBmp))
					{
						saveBmp.Save(PictureDirPath + "\\" + fileName);
						saveBmp.Dispose();
						saveBmp = null;
						MapData.IsMatrix = true;
					}
				}
				catch (Exception e)
				{
					String errorMsg = "Unable to save copy of phoneme bitmap to Module Data Directory";
					Logging.LogException(NLog.LogLevel.Warn, errorMsg, e);
				}
			}
			this.UseWaitCursor = false;
		}

		private void buttonOK_Click(object sender, EventArgs args)
		{
			savePicBitmaps();
		}

		private void nextPhonemeButton_Click(object sender, EventArgs e)
		{
			NextPhonmeIndex();
		}

		private void prevPhonemeButton_Click(object sender, EventArgs e)
		{
			PrevPhonemeIndex();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void renderedPicture_DoubleClick(object sender, EventArgs e)
		{
			if (renderedPicture.Image == null)
			{
				openButton_Click(sender, e);
			}
			else
			{
				editButton_Click(sender, e);
			}
		}

		private void renderPictureBoxImage()
		{
			Bitmap rawBitmap = null;
			if (_pictureBitmaps.TryGetValue(CurrentPhonemeString,out rawBitmap))
			{
				int newRenderWidth = 0;
				int newRenderHeight = 0;
				float aspectRatio = (float)rawBitmap.Size.Width / (float)rawBitmap.Size.Height;

				if (aspectRatio < 1)  //Width is less than height
				{
					newRenderWidth = (rawBitmap.Width < MIN_PICTUREBOX_WIDTH) ? MIN_PICTUREBOX_WIDTH :
						(rawBitmap.Width > MAX_PICTUREBOX_WIDTH) ? MAX_PICTUREBOX_WIDTH : rawBitmap.Width;

					newRenderHeight = 
						(int)((float)rawBitmap.Height * ((float)newRenderWidth / (float)rawBitmap.Width));
				}
				else if (aspectRatio > 1) //Width is greater than height
				{
					newRenderHeight = (rawBitmap.Height < MIN_PICTUREBOX_HEIGHT) ? MIN_PICTUREBOX_HEIGHT:
						(rawBitmap.Height> MAX_PICTUREBOX_HEIGHT) ? MAX_PICTUREBOX_HEIGHT: rawBitmap.Height;

					newRenderWidth =
						(int)((float)rawBitmap.Width * ((float)newRenderHeight / (float)rawBitmap.Height));
					
				}
				else //Width and Height are equal
				{
					newRenderWidth = (rawBitmap.Width < MIN_PICTUREBOX_WIDTH) ? MIN_PICTUREBOX_WIDTH :
						(rawBitmap.Width > MAX_PICTUREBOX_WIDTH) ? MAX_PICTUREBOX_WIDTH : rawBitmap.Width;
					newRenderHeight = (rawBitmap.Height < MIN_PICTUREBOX_HEIGHT) ? MIN_PICTUREBOX_HEIGHT :
						(rawBitmap.Height > MAX_PICTUREBOX_HEIGHT) ? MAX_PICTUREBOX_HEIGHT : rawBitmap.Height;
				}

				bool doShrink = (newRenderWidth >= MAX_PICTUREBOX_WIDTH || newRenderHeight >= MAX_PICTUREBOX_HEIGHT);
				bool doStretch = (newRenderHeight < MIN_PICTUREBOX_HEIGHT || newRenderHeight < MIN_PICTUREBOX_HEIGHT);

				renderedPicture.SizeMode = PictureBoxSizeMode.StretchImage;
				renderedPicture.Image = rawBitmap;
				renderedPicture.Size = new Size(newRenderWidth, newRenderHeight);
				renderedPicture.BackgroundImage = null;
				renderedPicture.BorderStyle = BorderStyle.None;

				if (doShrink)
				{
					Size = new Size(newRenderWidth - MAX_PICTUREBOX_WIDTH + MaximumSize.Width,
								newRenderHeight - MAX_PICTUREBOX_HEIGHT + MaximumSize.Height);
				}
				else
				{
						Size = new Size(newRenderWidth - MIN_PICTUREBOX_WIDTH + MinimumSize.Width,
									newRenderHeight - MIN_PICTUREBOX_HEIGHT + MinimumSize.Height);
				}
			}
			else
			{
				renderedPicture.Image = null; 
				renderedPicture.BackgroundImage = _noImageBmp;
				renderedPicture.BackgroundImageLayout = ImageLayout.Tile;
			}
			Refresh();
		}

		private void openPicFile(string fileName, string phonemeString = null)
		{
			if (null == phonemeString)
			{
				phonemeString = CurrentPhonemeString;
			}

			try
			{
				Bitmap existingBmp = null;
				Bitmap saveBmp = new Bitmap(fileName);
				if (_pictureBitmaps.TryGetValue(phonemeString,out existingBmp))
				{
					_pictureBitmaps[phonemeString].Dispose();
					_pictureBitmaps[phonemeString] = null;
				}
				_pictureBitmaps[phonemeString] = saveBmp;
			}
			catch (Exception err)
			{
				String errorMsg = "Unable to save copy of phoneme bitmap to Module Data Directory";
				Logging.LogException(NLog.LogLevel.Warn, errorMsg, err);
			}

		}

		private void openButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDlg = new OpenFileDialog();
			fileDlg.Multiselect = true;
			fileDlg.Filter = "Image Files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All Files(*.*)|*.*";
			DialogResult result = fileDlg.ShowDialog();
			if (result == DialogResult.OK)
			{
				loadNewPics(fileDlg.FileNames);
			}
		}

		private bool ValidateDrag(out string[] filenames, DragEventArgs e)
		{
			bool validData = false;

			filenames = null;

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach (string filePath in filenames)
				{
					validData = true;
					string ext = Path.GetExtension(filePath).ToLower();
					if ((ext != ".jpg") && (ext != ".png") && (ext != ".bmp") && (ext != "gif"))
					{
						validData = false;
						break;
					}
				}
			}
			return validData;
		}


		private void OnDragEnter(object sender, DragEventArgs e)
		{

			string[] fileNames = null;
			bool validData = ValidateDrag(out fileNames,e);

			if (validData)
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void OnDragDrop(object sender, DragEventArgs e)
		{
			string[] fileNames = null;
			bool validData = ValidateDrag(out fileNames, e);
			if (validData)
			{
				loadNewPics(fileNames);
			}
		}

		private void loadNewPics(string[] fileNames)
		{
			if (fileNames.Length == 1)
			{
				this.UseWaitCursor = true;
				openPicFile(fileNames[0]);
				renderPictureBoxImage();
				this.UseWaitCursor = false;
			}
			else
			{
				LipSyncMultiPicSelect _mps = new LipSyncMultiPicSelect();
				_mps.DropFileNames = fileNames;
				_mps.CurrentMappings = _pictureBitmaps;
				_mps.CurrentPhonemeString = CurrentPhonemeString; 
				DialogResult result = _mps.ShowDialog();
				if (DialogResult.OK == result)
				{
					this.UseWaitCursor = true;
					Dictionary<string, string> files = _mps.PicMappings;
					foreach (KeyValuePair<string,string> kvp in files)
					{
						if (string.Compare(kvp.Value,"!") == 0)
						{
							clearPicImage(kvp.Key);
						}
						else
						{
							openPicFile(kvp.Value, kvp.Key);
						}
					}
					renderPictureBoxImage();
					this.UseWaitCursor = false;
				}
			}
		}

		private void clearPicImage(string imageStr)
		{
			Bitmap existingBmp = null;
			if (_pictureBitmaps.TryGetValue(imageStr,out existingBmp))
			{
				_pictureBitmaps[imageStr].Dispose();
				_pictureBitmaps[imageStr] = null;
				_pictureBitmaps.Remove(imageStr);
			}
		}

		private void clearButton_Click(object sender, EventArgs e)
		{
			clearPicImage(CurrentPhonemeString);
			renderPictureBoxImage();
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			string sysFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
			string fileName = PictureDirPath + "\\" + CurrentPhonemeString + "_tmp.bmp";
			Bitmap editBmap = null;	 

			try
			{
				DirectoryInfo pictureDirInfo = new DirectoryInfo(PictureDirPath);
				FileInfo[] fi = pictureDirInfo.GetFiles(CurrentPhonemeString + "_tmp.bmp");

				foreach (FileInfo file in fi)
				{
					fi[0].Delete();
				}
			}
			catch (Exception err)
			{
				String errorMsg = "Unable to delete LipSync bitmap " + fileName + " from Module Data Directory";
				Logging.LogException(NLog.LogLevel.Warn, errorMsg, err);
			}

			if (_pictureBitmaps.TryGetValue(CurrentPhonemeString, out editBmap))
			{
				editBmap.Save(fileName);
				editBmap.Dispose();
				editBmap = null;
			}
			
			System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
			procInfo.FileName = (sysFolder + @"\mspaint.exe");
			procInfo.Arguments = "\"" + fileName + "\""; // Full Path to an image
			var process = System.Diagnostics.Process.Start(procInfo);
			process.WaitForExit();
			process.Dispose();

			try
			{
				Bitmap newBmp = new Bitmap(fileName);
				_pictureBitmaps[CurrentPhonemeString] = new Bitmap(newBmp);
				newBmp.Dispose();
				newBmp = null;

				File.Delete(fileName);
			}
			catch (Exception err)
			{
				String errorMsg = "Error processing edited LipSync bitmap " + fileName + " from Module Data Directory";
				Logging.LogException(NLog.LogLevel.Warn, errorMsg, err);
			}

			renderPictureBoxImage();
		}

		private void resetNewLibraryName()
		{
			if (nameTextBox.Text !=MapData.LibraryReferenceName) 
			{
				string newPictureDirPath = _moduleDirPath + "\\" + nameTextBox.Text;
				if (Directory.Exists(newPictureDirPath))
				{
					var messageBox = new MessageBoxForm(
						"Can not rename, this new name already exists!",
						"Error!",
						MessageBoxButtons.OK,
						SystemIcons.Error);

					messageBox.ShowDialog();
					nameTextBox.Text = MapData.LibraryReferenceName;
				}
				else
				{
					try
					{
						Directory.Move(PictureDirPath, newPictureDirPath);
					}
					catch (IOException err)
					{
						var messageBox = new MessageBoxForm(
							"Can not set new name!" +
							Environment.NewLine + Environment.NewLine +
							err.Message,
							"Error!",
							MessageBoxButtons.OK,
							SystemIcons.Error);

						messageBox.ShowDialog();
						nameTextBox.Text = MapData.LibraryReferenceName;
						return;
					}

					PictureDirPath = newPictureDirPath;
				}
			}
			
		}
	}
}
