using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncTextConvertForm : Form
    {
        public event EventHandler<NewTranslationEventArgs> NewTranslation = null;
        public event EventHandler<TranslateFailureEventArgs> TranslateFailure = null;
        public static bool Active = false; 
        private int unMarkedPhonemes;
        private int _lastMarkIndex;

        public LipSyncTextConvertForm()
        {
            InitializeComponent();

			markCollectionLabel.ForeColor = Color.Gray;
			markAlignmentLabel.ForeColor = Color.Gray;
			markStartOffsetLabel.ForeColor = Color.Gray;
			markCollectionRadio.ForeColor = Color.Gray;
			buttonConvert.BackgroundImage = Resources.HeadingBackgroundImage;
			Icon = Resources.Icon_Vixen3;
            unMarkedPhonemes = 0;
            _lastMarkIndex = -1;
        }

        public List<MarkCollection> MarkCollections { get; set; }

        private List<string> CreateSubstringList()
        {
            List<string> retVal = new List<string>();
            foreach (string line in textBox.Lines)
            {
                if ((alignCombo.SelectedIndex == -1) || !(alignCombo.SelectedItem.Equals("Phrase")))
                {
                    retVal.AddRange(line.Split());
                }
                else
                {
                    retVal.Add(line);
                }
            }
            return retVal;
        }

        private Tuple<TimeSpan, TimeSpan> CalcPhonemeTimespans(MarkCollection mc, int index, int subelements)
        {
            TimeSpan duration;
            TimeSpan relStart;
            TimeSpan lastOffset;

            if (mc != null)
            {
                //Sort the Marklist by time.
                mc.Marks.Sort();
                lastOffset = mc.Marks.Last();
            }
            else
            {
                lastOffset = new TimeSpan(0, 0, 0, 0);
            }

            if ((mc != null) && (index < mc.MarkCount - 1))
            {
                duration =
                    new TimeSpan((mc.Marks[index + 1] - mc.Marks[index]).Ticks / subelements);
            }
            else
            {
                //Default Phoneme timing to 250ms
                duration = new TimeSpan(0, 0, 0, 0, 250);
            }

            if (index == 0)
            {
                unMarkedPhonemes = 0;
            }

            if ((mc != null) && (index < mc.MarkCount))
            {
                relStart = mc.Marks[index] - mc.Marks[0];
                unMarkedPhonemes = 0;
            }
            else
            {
                relStart = new TimeSpan(lastOffset.Ticks + (duration.Ticks * unMarkedPhonemes));
                unMarkedPhonemes += subelements;
            }

            return new Tuple<TimeSpan, TimeSpan>(relStart, duration);
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            int mcIndex = startOffsetCombo.SelectedIndex;
            Tuple<TimeSpan, TimeSpan> timing = Tuple.Create(new TimeSpan(), new TimeSpan());
            List<LipSyncConvertData> convertData = new List<LipSyncConvertData>();

            if (LipSyncTextConvert.StandardDictExists() == false)
            {
                MessageBox.Show("Unable to find Standard Phoneme Dictionary", "Error", MessageBoxButtons.OK);
                return;
            }

            LipSyncTextConvert.InitDictionary();

            if (NewTranslation != null)
            {
                List<string> subStrings = CreateSubstringList();
                MarkCollection selMC = MarkCollections.Find(x => x.Name.Equals(markCollectionCombo.SelectedItem));
                bool doPhonemeAlign = 
                    (alignCombo.SelectedIndex != -1) && (alignCombo.SelectedItem.Equals("Phoneme"));

                if (mcIndex == -1)
                {
                    selMC = null;
                    mcIndex = 0;
                }

                foreach (string strElem in subStrings)
                {
                    if (string.IsNullOrWhiteSpace(strElem))
                    {
                        continue;
                    }

                    int phonemeIndex = 0;
                    List<PhonemeType> phonemeList = LipSyncTextConvert.TryConvert(strElem.Trim());
                    if (phonemeList.Count == 0)
                    {
                        EventHandler<TranslateFailureEventArgs> failHandler = TranslateFailure;
                        TranslateFailureEventArgs failArgs = new TranslateFailureEventArgs();
                        failArgs.FailedWord = strElem;
                        failHandler(this, failArgs);

                        //At this point, we should have it corrected, if not, then ignore
                        phonemeList = LipSyncTextConvert.TryConvert(strElem);
                    }

                    if (phonemeList.Count == 0)
                    {
                        //User has bailed on one of the conversions
                        return;
                    }

                    if (doPhonemeAlign == false)
                    {
                        timing = CalcPhonemeTimespans(selMC, mcIndex++, phonemeList.Count);
                    }

                    foreach (PhonemeType phoneme in phonemeList)
                    {
                        if (doPhonemeAlign == true)
                        {
                            timing = CalcPhonemeTimespans(selMC, mcIndex++, 1);
                            phonemeIndex = 0;
                        }

                        long startTicks = timing.Item1.Ticks + (timing.Item2.Ticks * phonemeIndex++);
                        convertData.Add(new LipSyncConvertData(startTicks, timing.Item2.Ticks, phoneme, strElem));
                    }
                }

                EventHandler<NewTranslationEventArgs> handler = NewTranslation;
                NewTranslationEventArgs args = new NewTranslationEventArgs();
                args.PhonemeData = convertData;
                if (markCollectionRadio.Checked)
                {
                    args.Placement = TranslatePlacement.Mark;
                }
                else if (cursorRadio.Checked)
                {
                    args.Placement = TranslatePlacement.Cursor;
                }
                else
                {
                    args.Placement = TranslatePlacement.Clipboard;
                }

                if (startOffsetCombo.SelectedItem != null)
                {
                    args.FirstMark = (TimeSpan)startOffsetCombo.SelectedItem;
                }

                handler(this, args);
            }

        }

        private void LipSyncTextConvert_Load(object sender, EventArgs e)
        {
            buttonConvert.Enabled = false;
            cursorRadio.Checked = true;
            setMarkControls(false);
        }

        void populateStartOffsetCombo()
        {
            int lastIndex = startOffsetCombo.SelectedIndex;
            int lastCount = startOffsetCombo.Items.Count;

            startOffsetCombo.Items.Clear();
            if (markCollectionCombo.SelectedIndex > -1)
            {
                MarkCollection mc =
                    MarkCollections.Find(x => x.Name.Equals(markCollectionCombo.SelectedItem));
                if (mc != null)
                {
                    foreach (TimeSpan ts in mc.Marks)
                    {
                        startOffsetCombo.Items.Add(ts);
                    }
                }

                if (startOffsetCombo.Items.Count > 0)
                {
                    if (lastIndex < startOffsetCombo.Items.Count)
                    {
                        startOffsetCombo.SelectedIndex =
                            (lastIndex == -1) ? 0 : lastIndex;
                    }
                    else
                    {
                        startOffsetCombo.SelectedIndex = startOffsetCombo.Items.Count - 1;
                    }
                }
            }
        }


        private void markCollectionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lastMarkIndex != markCollectionCombo.SelectedIndex)
            {
                startOffsetCombo.SelectedIndex = -1;
            }

            populateStartOffsetCombo();

            _lastMarkIndex = markCollectionCombo.SelectedIndex;
        }

        private void startOffsetCombo_DropDown(object sender, EventArgs e)
        {
            populateStartOffsetCombo();
        }

        private void markCollectionRadio_CheckedChanged(object sender, EventArgs e)
        {
            setMarkControls(markCollectionRadio.Checked);
        }

        private void setMarkControls(bool enable)
        {
            Control markCtrl = marksGroupBox.GetNextControl(null, true);
            while (markCtrl != null)
            {
                markCtrl.Enabled = enable;
                markCtrl = marksGroupBox.GetNextControl(markCtrl, true);
            }

            int lastIndex = markCollectionCombo.SelectedIndex;
            int lastCount = markCollectionCombo.Items.Count;

            markCollectionCombo.Items.Clear();
            foreach (MarkCollection mc in MarkCollections)
            {
                if ((mc.Name != null) && (mc.Marks.Count != 0))
                {
                    markCollectionCombo.Items.Add(mc.Name);
                }
            }

            markCollectionRadio.AutoCheck = false;

            if (markCollectionCombo.Items.Count > 0)
            {
                if (markCollectionCombo.Items.Count == lastCount) 
                {
                    if (lastIndex == -1)
                    {
                        markCollectionCombo.SelectedIndex = 0;
                    }
                    else
                    {
                        markCollectionCombo.SelectedIndex = lastIndex;
                    }
                }
                else
                {
                    markCollectionCombo.SelectedIndex = 0;
                }
                populateStartOffsetCombo();

				markCollectionRadio.AutoCheck = 
                    (markCollectionCombo.Items.Count > 0) && (startOffsetCombo.Items.Count > 0);
	            markCollectionRadio.ForeColor = markCollectionRadio.AutoCheck ? DarkThemeColorTable.ForeColor : Color.Gray;
            }

			
            if (enable)
            {
				markCollectionCombo.Enabled = true;
				startOffsetCombo.Enabled = true;
				alignCombo.Enabled = true;

                alignCombo.Items.Clear();
                alignCombo.Items.Add("Phoneme");
                alignCombo.Items.Add("Word");
                alignCombo.Items.Add("Phrase");
                alignCombo.SelectedIndex = 1;
            }
            else
            {
				markCollectionCombo.Enabled = false;
				startOffsetCombo.Enabled = false;
				alignCombo.Enabled = false;

                markCollectionCombo.SelectedIndex = -1;
                startOffsetCombo.SelectedIndex = -1;
            }
        }

        private void LipSyncTextConvertForm_Activated(object sender, EventArgs e)
        {
            setMarkControls(markCollectionRadio.Checked);
        }

        private void LipSyncTextConvertForm_Shown(object sender, EventArgs e)
        {
            Active = true;
        }

        private void LipSyncTextConvertForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Active = false;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            buttonConvert.Enabled = !String.IsNullOrWhiteSpace(textBox.Text);
        }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		#region Draw lines and GroupBox borders
		
		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			DarkThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
		#endregion

		private void buttonTextColorChange(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.ForeColor = btn.Enabled ? DarkThemeColorTable.ForeColor : Color.Gray;
		}

		private void markCollectionCombo_EnabledChanged(object sender, EventArgs e)
		{
			markCollectionLabel.ForeColor = markCollectionCombo.Enabled ? DarkThemeColorTable.ForeColor : Color.Gray;
			markAlignmentLabel.ForeColor = markCollectionCombo.Enabled ? DarkThemeColorTable.ForeColor : Color.Gray;
			markStartOffsetLabel.ForeColor = markCollectionCombo.Enabled ? DarkThemeColorTable.ForeColor : Color.Gray;
		}

		private void comboBoxes_DrawItem(object sender, DrawItemEventArgs e)
		{
			var btn = (ComboBox)sender;
			int index = e.Index;
			if (index < 0)
			{
				return;
			}
			var brush = new SolidBrush(DarkThemeColorTable.ForeColor);
			e.DrawBackground();
			e.Graphics.DrawString(btn.Items[index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
		}
	}

    public class LipSyncConvertData
    {
        public LipSyncConvertData()
        {

        }

        public LipSyncConvertData(TimeSpan start, TimeSpan dur, PhonemeType phoneme)
        {
            StartOffset = start;
            Duration = dur;
            Phoneme = phoneme;
        }

        public LipSyncConvertData(long startTicks, long durTicks, PhonemeType phoneme, string lyricData)
        {
            StartOffset = new TimeSpan(startTicks);
            Duration = new TimeSpan(durTicks);
            Phoneme = phoneme;
            LyricData = lyricData;
        }

        public TimeSpan StartOffset { get; set; }
        public TimeSpan Duration { get; set; }
        public PhonemeType Phoneme { get; set; }
        public string LyricData { get; set; }
    }

    public enum TranslatePlacement { Clipboard, Cursor, Mark };

    public class NewTranslationEventArgs : EventArgs
    {
        public TranslatePlacement Placement { get; set; }
        public TimeSpan FirstMark { get; set; }
        public List<LipSyncConvertData> PhonemeData { get; set; }
    }

    public class TranslateFailureEventArgs : EventArgs
    {
        public string FailedWord;
    }
}
