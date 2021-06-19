using System;

namespace WYW.Data
{
    public class RecentResponse
    {
        public FlightInfo[] LastResponse { get; set; }
        public DateTime LastResponseDT { get; set; }
    }

    public class FlightInfo
    {
        public Airline airline { get; set; }
        public Arrival arrival { get; set; }
        public object codeshared { get; set; }
        public Departure departure { get; set; }
        public Flight flight { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Airline
    {
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public string name { get; set; }
    }

    public class Arrival
    {
        public object actualRunway { get; set; }
        public object actualTime { get; set; }
        public object baggage { get; set; }
        public object delay { get; set; }
        public object estimatedRunway { get; set; }
        public object estimatedTime { get; set; }
        public object gate { get; set; }
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public DateTime scheduledTime { get; set; }
        public object terminal { get; set; }
    }

    public class Departure
    {
        public DateTime? actualRunway { get; set; }
        public DateTime? actualTime { get; set; }
        public object baggage { get; set; }
        public string delay { get; set; }
        public DateTime? estimatedRunway { get; set; }
        public object estimatedTime { get; set; }
        public object gate { get; set; }
        public string iataCode { get; set; }
        public string icaoCode { get; set; }
        public DateTime scheduledTime { get; set; }
        public object terminal { get; set; }
    }

    public class Flight
    {
        public string iataNumber { get; set; }
        public string icaoNumber { get; set; }
        public string number { get; set; }
    }

}