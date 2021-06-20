using System;
using System.Threading.Tasks;

namespace WYW.Data
{
    public class MockApiService : IApiResponseService
    {
        private RecentResponse recentResponse;
        public RecentResponse RecentResponse
        {
            get => recentResponse;
            set
            {
                PreviousResponse = RecentResponse;

                recentResponse = value;

                if (PreviousResponse != null)
                    LookupForChanges();
            }
        }

        private RecentResponse PreviousResponse { get; set; }
        public event Action<FlightInfo> SomeDataChanged;

        public MockApiService()
        {
            RecentResponse = new RecentResponse()
            {
                LastResponseDT = DateTime.Now,
                LastResponse = new FlightInfo[0]
            };
        }

        protected virtual void OnSomeDataChanged(FlightInfo e)
        {
            SomeDataChanged?.Invoke(e);
        }

        private void LookupForChanges()
        {            

                ////zgłaszamy zaistnienie zmiany
                foreach(FlightInfo flight in RecentResponse.LastResponse)
                {
                    for(int i = 0; i < PreviousResponse.LastResponse.Length; i++)
                    {
                        if(PreviousResponse.LastResponse[i].flight.iataNumber == flight.flight.iataNumber)
                        {
                            if(flight.status != PreviousResponse.LastResponse[i].status ||
                            flight.departure.scheduledTime != PreviousResponse.LastResponse[i].departure.scheduledTime ||
                            flight.departure.terminal != PreviousResponse.LastResponse[i].departure.terminal ||
                            flight.departure.gate != PreviousResponse.LastResponse[i].departure.gate)
                            {
                                OnSomeDataChanged(flight);
                            }
                        }
                    }
                }

                PreviousResponse = RecentResponse;
        }
    }
}
