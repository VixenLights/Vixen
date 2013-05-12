using System;
using System.Diagnostics;

namespace VixenModules.Preview.VixenPreview
{
    class DebugTimer
    {
        private Stopwatch timer = new Stopwatch();
        public double TotalMilliseconds = 0;

        public DebugTimer()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            TotalMilliseconds = timer.ElapsedMilliseconds;
        }
    }
}
