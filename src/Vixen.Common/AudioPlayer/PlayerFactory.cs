namespace Common.AudioPlayer
{
	public class PlayerFactory
	{
		
		public static IPlayer CreateNew(string fileName)
		{
			try
			{
				IPlayer player = new CoreAudioPlayer(fileName);
				return player;
			} catch
			{
				return null;
			}

		}
	}
}
