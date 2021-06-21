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
        public bool IsStatusChanged { get; set; }
        public bool IsScheduledTimeChanged { get; set; }
        public bool IsTerminalChanged { get; set; }
        public bool IsGateChanged { get; set; }

        public ExtendedFlightInfo(FlightInfo flightInfo)
        {
            FlightInfo = flightInfo;
        }

        public ExtendedFlightInfo(ExtendedFlightInfo flightInfo)
        {
            FlightInfo = flightInfo.FlightInfo;
            TimeToDeparture = flightInfo.TimeToDeparture;
            TimeToArrival = flightInfo.TimeToArrival;
            IsOpen = flightInfo.IsOpen;
            iataNumber = flightInfo.iataNumber;
            iataCode = flightInfo.iataCode;
            IsStatusChanged = flightInfo.IsStatusChanged;
            IsScheduledTimeChanged = flightInfo.IsScheduledTimeChanged;
            IsTerminalChanged = flightInfo.IsTerminalChanged;
            IsGateChanged = flightInfo.IsGateChanged;
        }
    }
}
