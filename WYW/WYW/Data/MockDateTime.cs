using System;
using System.Diagnostics;

namespace WYW.Data
{
    public class MockDateTime : IDateTime
    {
        private DateTime StartTime { get; set; }
        private Stopwatch Timer { get; set; }

        public void ResetTime(DateTime newTime)
        {
            StartTime = newTime;
            Timer = Stopwatch.StartNew();
        }

        public DateTime Now()
        {
            return StartTime + Timer.Elapsed;
        }
    }
}
