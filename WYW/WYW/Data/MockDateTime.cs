using System;
using System.Diagnostics;

namespace WYW.Data
{
    public class MockDateTime : IDateTime
    {
        private DateTimeOffset StartTime { get; set; }
        private Stopwatch Timer { get; set; }

        public void ResetTime(DateTime newTime)
        {
            StartTime = new DateTimeOffset(newTime);
            Timer = Stopwatch.StartNew();
        }

        public DateTimeOffset UtcNow()
        {
            return StartTime + Timer.Elapsed;
        }
    }
}
