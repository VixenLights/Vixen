using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace VixenModules.Output.CommandController
{
	public class RdsSerialPort : SerialPort
	{
		public RdsSerialPort(string portName, int baudRate)
			: base(portName, baudRate, Parity.None, 8, StopBits.One)
		{
			Handshake = Handshake.None;
			Encoding = Encoding.UTF8;
			Open();
			//Set the board to support RadioText
			Write(new byte[] {
				0xFE, //Start Command
				0x1F, 
				0xFF  //End Command
			}, 0, 3);
			Close();
		}

		public RdsSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits) :
			base(portName, baudRate, parity, dataBits, stopBits)
		{

			Handshake = Handshake.None;
			Encoding = Encoding.UTF8;
			Open();
			//Set the board to support RadioText
			Write(new byte[] {
				0xFE, //Start Command
				0x1F, 
				0xFF  //End Command
			}, 0, 3);
			Close();
		}

		public void Send(string radioText)
		{
			Open();
			var Data = new List<byte>();

			Data.Add(0xFE); //Start Command
			Data.Add(0x20); // buffer address for RadioText


			for (int i = 0; i < radioText.Length; i++) {
				Data.Add(Convert.ToByte(radioText[i]));
			}

			for (int j = 0; j < 64-radioText.Length; j++) {
				Data.Add(0x20);
			}

			Data.Add(0xFF); //End Command

			Write(Data.ToArray(), 0, Data.Count());
			Close();
		}

		public enum MusicSpeech : byte
		{
			Speech,
			Music
		}
		public enum ProgramType : byte
		{
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
