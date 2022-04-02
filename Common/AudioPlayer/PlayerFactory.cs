namespace Common.AudioPlayer
{
	public class PlayerFactory
	{
		
		public static IPlayer CreateNew(string fileName)
		{
			return new CoreAudioPlayer(fileName);
		}
	}
}
