using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WYW.Data
{
    public class ApiResponseService : IApiResponseService
    {

        static string uri = "https://aviation-edge.com/v2/public/timetable?key=777139-df23dc&iataCode=POZ";
        public RecentResponse RecentResponse { get; set; }
        public event Action<FlightInfo> SomeDataChanged;

        private RecentResponse PreviousResponse { get; set; }
        private HttpClient client = new HttpClient();

        protected virtual void OnSomeDataChanged(FlightInfo e)
        {
            SomeDataChanged?.Invoke(e);
        }

        public async Task CheckTheApiEvery5m()
        {            
            while (true)
            {
                var response = client.GetAsync(uri).Result;

                if (response.IsSuccessStatusCode)
                {
                    var getResponse = response.Content.ReadAsAsync<FlightInfo[]>().Result;
                    PreviousResponse = RecentResponse;
                    RecentResponse = new RecentResponse(){LastResponse = getResponse, LastResponseDT = DateTime.Now};

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
                    
                    await Task.Delay(5*60000);
                }
            }
        }
    }
}