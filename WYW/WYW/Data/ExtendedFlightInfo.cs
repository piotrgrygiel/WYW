using System;

namespace WYW.Data
{
    public class ExtendedFlightInfo
    {
        public FlightInfo FlightInfo { get; set; }
        public TimeSpan TimeToDeparture { get; set; }
        public TimeSpan TimeToArrival { get; set; }
        public bool IsOpen { get; set; }
        public string iataNumber
        {
            get => FlightInfo.flight.iataNumber;
            set{}
        }
        public string iataCode
        {
            get => FlightInfo.arrival.iataCode;
            set{}
        }

        public ExtendedFlightInfo(FlightInfo flightInfo)
        {
            FlightInfo = flightInfo;
        }
    }
}
