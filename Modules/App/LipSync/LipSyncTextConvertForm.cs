using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncTextConvertForm : Form
    {
        public event EventHandler<NewTranslationEventArgs> NewTranslation = null;
        public event EventHandler<TranslateFailureEventArgs> TranslateFailure = null;
        private int unMarkedPhonemes;

        public LipSyncTextConvertForm()
        {
            InitializeComponent();
            unMarkedPhonemes = 0;
        }

        public List<MarkCollection> MarkCollections { get; set; }

        private List<string> CreateSubstringList()
        {
            List<string> retVal = new List<string>();
            foreach (string line in textBox.Lines)
            {
                if (alignCombo.SelectedItem.Equals("Phrase"))
                {
                    retVal.Add(line);
                }
                else
                {
                    retVal.AddRange(line.Split());
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
                bool doPhonemeAlign = alignCombo.SelectedItem.Equals("Phoneme");

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
                        convertData.Add(new LipSyncConvertData(startTicks, timing.Item2.Ticks, phoneme));
                    }
                }

                EventHandler<NewTranslationEventArgs> handler = NewTranslation;
                NewTranslationEventArgs args = new NewTranslationEventArgs();
                args.PhonemeData = convertData;
                if (startOffsetCombo.SelectedItem != null)
                {
                    args.FirstMark = (TimeSpan)startOffsetCombo.SelectedItem;
                }

                handler(this, args);
            }

        }

        private void populateMarkCombo()
        {
            markCollectionCombo.Items.Clear();
            markCollectionCombo.Items.Add("");
            markCollectionCombo.SelectedIndex = 0;
            foreach (MarkCollection mc in MarkCollections)
            {
                if (mc.Name != null)
                {
                    markCollectionCombo.Items.Add(mc.Name);
                }
            }
        }

        private void LipSyncTextConvert_Load(object sender, EventArgs e)
        {
            alignCombo.SelectedIndex = 1;
            populateMarkCombo();
        }

        void populateStartOffsetCombo()
        {
            startOffsetCombo.Items.Clear();
            if (!markCollectionCombo.SelectedItem.Equals(""))
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

            }
        }


        private void markCollectionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateStartOffsetCombo();
            if (startOffsetCombo.Items.Count > 0)
            {
                startOffsetCombo.SelectedIndex = 0;
            }
            
        }

        private void startOffsetCombo_DropDown(object sender, EventArgs e)
        {
            populateStartOffsetCombo();
        }

        private void markCollectionCombo_DropDown(object sender, EventArgs e)
        {
            populateMarkCombo();
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

        public LipSyncConvertData(long startTicks, long durTicks, PhonemeType phoneme)
        {
            StartOffset = new TimeSpan(startTicks);
            Duration = new TimeSpan(durTicks);
            Phoneme = phoneme;
        }

        public TimeSpan StartOffset { get; set; }
        public TimeSpan Duration { get; set; }
        public PhonemeType Phoneme { get; set; }
    }

    public class NewTranslationEventArgs : EventArgs
    {
        public TimeSpan FirstMark { get; set; }
        public List<LipSyncConvertData> PhonemeData { get; set; }
    }

    public class TranslateFailureEventArgs : EventArgs
    {
        public string FailedWord;
    }
}
