namespace Common.AudioPlayer
{
	public class PlayerFactory
	{
		public static IPlayer CreateNew(Device device, string fileName)
		{
			return new CoreAudioPlayer(device, fileName);
		}
	}
}
