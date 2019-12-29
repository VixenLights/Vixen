namespace Common.AudioPlayer
{
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer Create()
        {
            return CoreAudioPlayer.Instance;
        }
    }
}
