using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Vixen;

namespace VixenModules.Editor.TimedSequenceEditor
{
    public enum PhonemeType { AI, E, O, U, FV, L, MBP, WQ, etc, Rest };

    public abstract class PapagayoImportObject
    {

        protected int m_state = 0;

        public static float FPS { get; set; }
        public static int SoundFrames { get; set; }

        public static float SampleDurationMS
        {
            get
            {
                return (1 / FPS) * 1000;
            }
        }

        public int FileDurationMS
        {
            get
            {
                return (PapagayoImportObject.SoundFrames - 1) * 1000 / (int)PapagayoImportObject.FPS;
            }
        }

        public int StartFrame { get; set; }
        public int EndFrame { get; set; }

        public float StartMS
        {
            get
            {
                return SampleDurationMS * StartFrame;
            }
        }

        public float EndMS
        {
            get
            {
                return SampleDurationMS * (EndFrame + 1);
            }
        }

        public float DurationMS
        {
            get
            {
                return EndMS - StartMS;
            }
        }
    }

    public class PapagayoDoc
    {
        int m_state = 0;
        string m_soundPath;
        int m_numVoices = 0;
        Dictionary<string, PapagayoVoice> m_voices = null;
        string m_fileNameStr = null;

        public PapagayoDoc()
        {
            m_voices = new Dictionary<string, PapagayoVoice>();
        }


        public List<string> VoiceList
        {
            get
            {
                List<string> retval = new List<string>();
                foreach (KeyValuePair<string, PapagayoVoice> voice in m_voices)
                {
                    retval.Add(voice.Value.VoiceName);
                }

                return retval;
            }
        }



        public bool IsValid { get; set; }

        public void Clear()
        {
            m_state = 0;
            m_numVoices = 0;
            m_voices = null;

        }

        public void Load(string fileName)
        {
            string line;
            PapagayoVoice voice = null;

            m_state = 0;
            m_soundPath = null;
            PapagayoImportObject.SoundFrames = 0;
            m_numVoices = 0;

            m_voices.Clear();


            StreamReader file = new StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                //Trim leading whitespace on the read string. 
                line = line.TrimStart(null);

                switch (m_state)
                {
                    //Papagayo File Header
                    case 0:
                        if ((line.Contains("lipsync") == false))
                        {
                            throw new IOException("Invalid File Format");
                        }
                        else
                        {
                            m_state++;
                        }
                        break;

                    //Read the sound path
                    case 1:
                        m_soundPath = line;
                        m_state++;
                        break;

                    //Decode FPS
                    case 2:
                        PapagayoImportObject.FPS = Convert.ToInt32(line);
                        m_state++;
                        break;

                    //Read the sound duration
                    case 3:
                        PapagayoImportObject.SoundFrames = Convert.ToInt32(line);
                        m_state++;
                        break;

                    //Read the number of voices
                    case 4:
                        m_numVoices = Convert.ToInt32(line);
                        for (int j = 0; j < m_numVoices; j++)
                        {
                            voice = new PapagayoVoice();
                            voice.EndFrame = PapagayoImportObject.SoundFrames;
                            voice.Load(file);
                            m_voices.Add(voice.VoiceName.Trim(), voice);
                        }

                        m_state++;
                        break;

                    default:
                        throw new IOException();
                }
            }

            m_fileNameStr = fileName;
            file.Close();
        }

        public PapagayoPhoneme GetEventPhoneme(string voiceStr, int eventNum) 
        {
            PapagayoPhoneme retVal = null;
            try
            {
                PapagayoVoice voice = m_voices[voiceStr.Trim()];
                if (voice != null)
                {
                    retVal = voice.GetEventPhoneme(eventNum);
                }
            }
            catch (KeyNotFoundException) { }

            return retVal;
        }

        public List<PapagayoPhoneme> PhonemeList(string voiceStr)
        {
            List<PapagayoPhoneme> retVal = null;
            PapagayoVoice voice;

            Dictionary<string,PapagayoVoice>.Enumerator voiceEnumerator;

            if (voiceStr == null)
            {
                voiceEnumerator = m_voices.GetEnumerator();
                voiceEnumerator.MoveNext();
                retVal = voiceEnumerator.Current.Value.PhonemeList;
            }
            else if (m_voices.TryGetValue(voiceStr.Trim(),out voice))
            {
                retVal = voice.PhonemeList;
            }

            return retVal;

        }
    }

    class PapagayoVoice : PapagayoImportObject
    {
        string m_voiceName = null;
        string m_voicePhrase = null;
        int m_numPhrases = 0;
        PapagayoPhrase[] m_phrases = null;
        List<PapagayoPhoneme> m_phonemes = null;

        public string VoiceName
        {
            get { return m_voiceName; }
        }

        public PapagayoVoice()
        {

        }

        public void Load(StreamReader file)
        {
            m_voiceName = null;
            m_voicePhrase = null;
            m_numPhrases = 0;

            string line;
            while (m_state < 3)
            {
                line = file.ReadLine();
                if (line == null)
                {
                    throw new IOException("Corrupt File Format");
                }
                //Trim leading whitespace on the read string. 
                line = line.TrimStart(null);

                switch (m_state)
                {
                    //Read the voice name
                    case 0:
                        m_voiceName = line;
                        m_state++;
                        break;

                    //Read the Voice Phrase Text
                    case 1:
                        m_voicePhrase = line.Replace('|', '\n');
                        m_state++;
                        break;

                    //Read the number of Voice Phrases
                    case 2:
                        m_numPhrases = Convert.ToInt32(line);
                        m_phrases = new PapagayoPhrase[m_numPhrases];
                        m_phonemes = new List<PapagayoPhoneme>();

                        for (int j = 0; j < m_numPhrases; j++)
                        {
                            m_phrases[j] = new PapagayoPhrase();
                            m_phrases[j].Load(file, ref m_phonemes);
                        }

                        Coalesce();

                        m_state++;
                        break;

                    default:
                        break;

                }

                Console.WriteLine(m_state.ToString() + " - " + line);
                
            }
        }

        public void Coalesce()
        {
            List<PapagayoPhoneme> eventList = new List<PapagayoPhoneme>();
            List<PapagayoPhoneme> newList = new List<PapagayoPhoneme>();
            PapagayoPhoneme coalescedPhoneme = null;
            PapagayoPhoneme newPhoneme = null;
            for (int eventIndex = 0; eventIndex < SoundFrames; eventIndex++)
            {
                eventList.Add(this.GetEventPhoneme(eventIndex));
            }

            foreach(PapagayoPhoneme phoneme in eventList)
            {
                if (coalescedPhoneme == null)
                {
                    coalescedPhoneme = phoneme;
                }

                if (coalescedPhoneme.Type == phoneme.Type)
                {
                    coalescedPhoneme.EndFrame = phoneme.EndFrame;
                }
                else
                {
                    newPhoneme = new PapagayoPhoneme(coalescedPhoneme);
                    newList.Add(newPhoneme);
                    coalescedPhoneme = phoneme;
                }

            }
            newList.Add(new PapagayoPhoneme(coalescedPhoneme));

            m_phonemes = newList;
        }
        
        public PapagayoPhoneme GetEventPhoneme(int eventNum)
        {
            foreach (PapagayoPhrase phrase in m_phrases)
            {
                if (eventNum >= phrase.StartFrame &&
                    eventNum <= phrase.EndFrame)
                {
                    return phrase.GetEventPhoneme(eventNum);
                }
            }

            PapagayoPhoneme defaultPhoneme = new PapagayoPhoneme(null, null);
            defaultPhoneme.StartFrame = eventNum;
            defaultPhoneme.EndFrame = eventNum;
            return defaultPhoneme;

        }

        public List<PapagayoPhoneme> PhonemeList
        {
            get
            {
                return m_phonemes;
            }
        }
    }


    class PapagayoPhrase : PapagayoImportObject
    {
        string m_text;
        int m_numWords;
        PapagayoWord[] m_words = null;

        public PapagayoPhrase()
        {

        }

        public void Load(StreamReader file, ref List<PapagayoPhoneme> phonemes)
        {
            string line;
            while (m_state < 4)
            {
                line = file.ReadLine();
                if (line == null)
                {
                    throw new IOException("Corrupt File Format");
                }

                //Trim leading whitespace on the read string. 
                line = line.TrimStart(null);

                switch (m_state)
                {
                    //Read the phrase text
                    case 0:
                        m_text = line;
                        m_state++;
                        break;

                    //Read the start frame
                    case 1:
                        StartFrame = Convert.ToInt32(line);
                        m_state++;
                        break;

                    //Read the end frame
                    case 2:
                        EndFrame = Convert.ToInt32(line);
                        m_state++;
                        break;

                    //Read the end frame
                    case 3:
                        m_numWords = Convert.ToInt32(line);
                        m_words = new PapagayoWord[m_numWords];

                        for (int j = 0; j < m_numWords; j++)
                        {
                            m_words[j] = new PapagayoWord(file, ref phonemes);
                        }
                        m_state++;
                        break;

                    default:
                        break;
                }
                Console.WriteLine(m_state.ToString() + " - " + line);
            }
        }

        public PapagayoPhoneme GetEventPhoneme(int eventNum)
        {
            foreach (PapagayoWord word in this.m_words)
            {
                if (eventNum >= word.StartFrame &&
                    eventNum <= word.EndFrame)
                {
                    return word.GetEventPhoneme(eventNum);
                }
            }
            PapagayoPhoneme defaultPhoneme = new PapagayoPhoneme(null, null);
            defaultPhoneme.StartFrame = eventNum;
            defaultPhoneme.EndFrame = eventNum;
            return defaultPhoneme;
        }
    }


    class PapagayoWord : PapagayoImportObject
    {
        string line = null;
        string m_wordText = null;
        int m_numPhoneme = 0;
        PapagayoPhoneme[] m_phoneme = null;

        public PapagayoWord(StreamReader file,
            ref List<PapagayoPhoneme> phonemes)
        {
            line = file.ReadLine();
            if (line == null)
            {
                throw new IOException("Corrupt File Format");
            }

            line = line.TrimStart(null);
            string[] split = line.Split(' ');
            if (split.Length == 4)
            {
                m_wordText = split[0].TrimStart(null);
                StartFrame = Convert.ToInt32(split[1].TrimStart(null));
                EndFrame = Convert.ToInt32(split[2].TrimStart(null));
                m_numPhoneme = Convert.ToInt32(split[3].TrimStart(null));
                m_phoneme = new PapagayoPhoneme[m_numPhoneme];

                PapagayoPhoneme lastObj = null;
                for (int j = 0; j < m_numPhoneme; j++)
                {
                    if ((line = file.ReadLine()) != null)
                    {
                        line = line.TrimStart(null);
                        m_phoneme[j] = new PapagayoPhoneme(line, lastObj);
                        lastObj = m_phoneme[j];
                        phonemes.Add(m_phoneme[j]);
                    }
                }

                if (lastObj != null)
                {
                    lastObj.EndFrame = EndFrame;
                }
            }
        }

        public PapagayoPhoneme GetEventPhoneme(int eventNum)
        {
            foreach (PapagayoPhoneme phoneme in m_phoneme)
            {
                if (eventNum >= phoneme.StartFrame &&
                    eventNum <= phoneme.EndFrame)
                {
                    return phoneme;
                }
            }

            PapagayoPhoneme defaultPhoneme = new PapagayoPhoneme(null, null);
            defaultPhoneme.StartFrame = eventNum;
            defaultPhoneme.EndFrame = eventNum;
            return defaultPhoneme;
        }
    }

    public class PapagayoPhoneme : PapagayoImportObject
    {
        string m_Text = null;
        PhonemeType m_type = PhonemeType.Rest;

        public PapagayoPhoneme(PapagayoPhoneme copyObj)
        {
            this.m_state = copyObj.m_state;
            this.m_Text = copyObj.m_Text;
            this.StartFrame = copyObj.StartFrame;
            this.EndFrame = copyObj.EndFrame;
            this.m_type = copyObj.m_type;
        }

        public PapagayoPhoneme(string pair, PapagayoPhoneme lastObj)
        {
            m_type = PhonemeType.Rest;
            m_Text = "Rest";
            if (pair != null)
            {
                string[] split = pair.Split(' ');
                if (split.Length == 2)
                {
                    StartFrame = Convert.ToInt32(split[0].TrimStart(null));
                    m_Text = split[1].TrimStart(null);
                    try
                    {
                        m_type = (PhonemeType)Enum.Parse(typeof(PhonemeType), m_Text);
                    }
                    catch (Exception)
                    {
                        m_type = PhonemeType.Rest;
                    }

                    if (lastObj != null)
                    {
                        lastObj.EndFrame = StartFrame - 1;
                    }
                }

            }

        }

        public PhonemeType Type
        {
            get
            {
                return m_type;
            }
        }

        public string TypeName
        {
            get
            {
                return m_type.ToString();
            }
        }

        public bool isPhonemeType(string testVal)
        {
            return ((testVal != null) &&
                (this.m_Text.Equals(testVal.ToUpper())));
        }

    }

}
