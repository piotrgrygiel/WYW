using System;
using System.Threading.Tasks;

namespace WYW.Data
{
    public class MockApiService : IApiResponseService
    {
        public RecentResponse RecentResponse { get; set; }
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

        public async Task CheckTheApiEvery5m()
        {            
            while (true)
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
                    
                await Task.Delay(5*60000);
            }
        }
    }
}
