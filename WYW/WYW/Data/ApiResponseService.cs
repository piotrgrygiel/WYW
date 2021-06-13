using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WYW.Data
{
    //public delegate void SomeDataChangedEventHandler(object sender, SomeDataChangedEventArgs e);

    public class ApiResponseService : IApiResponseService
    {
        private string uri = "https://aviation-edge.com/v2/public/timetable?key=777139-df23dc&iataCode=POZ&type=departure";

        public RecentResponse RecentResponse { get; set; }
        //public event Action<FlightInfo> SomeDataChanged;

        private RecentResponse PreviousResponse { get; set; }
        private HttpClient client = new HttpClient();

        //protected virtual void OnSomeDataChanged(FlightInfo e)
        //{
        //    SomeDataChanged?.Invoke(e);
        //}

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
                    //OnSomeDataChanged(changed);

                    await Task.Delay(5*60000);
                }
            }
        }
    }
}