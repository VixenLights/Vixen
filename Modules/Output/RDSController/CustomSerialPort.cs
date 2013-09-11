using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Output.RDSController {
	public class CustomSerialPort : SerialPort {

		public CustomSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits) :
			base(portName, baudRate, parity, dataBits, stopBits) {

			Handshake = Handshake.None;
			Encoding = Encoding.UTF8;
			Open();

		}

		public void Send(string radioText, MusicSpeech speech) {
			var bytes = GenerateRDSData(radioText, speech);
			Write(bytes, 0, bytes.Length);
		}

		public static byte[] GenerateRDSData(string radioText, MusicSpeech speech) {

			List<byte> Data = new List<byte>();
			int idx = 0;
			Data.Add(0xFE);

			for (int i = 0; i < 8; i++) {

				Data.Add(0);
			}
			//Program Type
			Data.Add((byte)ProgramType.None);
			//Decoder Identification
			Data.Add(0);
			//Music/Speech Switch
			Data.Add((byte)speech);
			//Traffic Program
			Data.Add(0);
			//Traffic Announcement
			Data.Add(0);
			while (Data.Count < 31) {
				Data.Add(0);
			}
			//Enables or Disables RadioText
			if (!string.IsNullOrWhiteSpace(radioText)) {
				Data.Add((string.IsNullOrWhiteSpace(radioText) ? Convert.ToByte(0) : Convert.ToByte(1)));
				int stringLength = radioText.Length;
				if (stringLength > 64) stringLength = 64;
				var radioTextBytes = radioText.Substring(0, stringLength).Select(x => Convert.ToByte(x)).ToArray();
				for (int i = 0; i < radioTextBytes.Length; i++) {
					Data.Add(radioTextBytes[i]);
				}
			}
			Data.Add(0xFF);
			return Data.ToArray();

		}
		public enum MusicSpeech : byte {
			Speech,
			Music
		}
		public enum ProgramType : byte {
			None,
			News,
			Affairs_Information,
			InfoSports,
			SportTalk,
			EducationRock,
			DramaClassicRock,
			CulturesAdultHits,
			ScienceSoftRock,
			VariedSpeechTop40,
			PopMusicCountry,
			RockMusicOldies,
			EasyMusicSoft,
			LightClassicsMusicNostalgia,
			SeriousClassicsJazz,
			OtherMusicClassical,
			WeatherRhythmandBlues,
			FinanceSoftRhythmandBlues,
			ChildrenForeignLanguage,
			SocialAffairsReligiousMusic,
			ReligionReligiousTalk,
			PhoneInPersonality,
			TravelPublic,
			LeisureCollege,
			JazzMusicunassigned,
			CountryMusicunassigned,
			NationalMusicunassigned,
			OldiesMusicunassigned,
			FolkMusicunassigned,
			DocumentaryWeather,
			AlarmTestEmergencyTest,
			AlarmEmergency,
		}
	}
}
