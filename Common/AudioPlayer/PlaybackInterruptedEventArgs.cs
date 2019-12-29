using System;

namespace Common.AudioPlayer
{
    public class PlaybackInterruptedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
