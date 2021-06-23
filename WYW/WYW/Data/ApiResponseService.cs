using System;
using System.Linq;
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

        public ApiResponseService()
        {
            RecentResponse = new RecentResponse()
            {
                LastResponseDT = DateTime.Now,
                LastResponse = new FlightInfo[0]
            };
            PreviousResponse = new RecentResponse()
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
                var response = client.GetAsync(uri).Result;

                if (response.IsSuccessStatusCode)
                {
                    var getResponse = response.Content.ReadAsAsync<FlightInfo[]>().Result;
                    PreviousResponse = RecentResponse;
                    RecentResponse = new RecentResponse(){LastResponse = getResponse, LastResponseDT = DateTime.Now};

                    ////zgłaszamy zaistnienie zmiany
                    foreach(FlightInfo flight in RecentResponse.LastResponse)
                    {
                        var previousFlight = PreviousResponse.LastResponse.FirstOrDefault(prFl => prFl.flight.iataNumber == flight.flight.iataNumber);

                        if(previousFlight != null)
                        {
                            if(flight.status != previousFlight.status ||
                            flight.departure.scheduledTime != previousFlight.departure.scheduledTime ||
                            flight.departure.terminal != previousFlight.departure.terminal ||
                            flight.departure.gate != previousFlight.departure.gate)
                            {
                                OnSomeDataChanged(flight);
                            }
                        }
                        else
                        {
                            //zgłaszamy zmianę - nowy lot na liście
                            OnSomeDataChanged(flight);
                        }
                    }
                    
                    await Task.Delay(5*60000);
                }
            }
        }
    }
}