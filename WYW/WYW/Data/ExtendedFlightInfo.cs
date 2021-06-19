using System;

namespace WYW.Data
{
    public class ExtendedFlightInfo
    {
        public FlightInfo FlightInfo { get; set; }
        public TimeSpan TimeToDeparture { get; set; }

        public ExtendedFlightInfo(FlightInfo flightInfo)
        {
            FlightInfo = flightInfo;
        }
    }
}
