using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace WYW
{
    //public delegate void SomeDataChangedEventHandler(object sender, SomeDataChangedEventArgs e);

    public class ApiResponseService
    {
        static string uri = "https://aviation-edge.com/v2/public/timetable?key=777139-df23dc&iataCode=POZ";
        public RecentResponse RecentResponse { get; set; }
        private RecentResponse PreviousResponse { get; set; }

        public event Action<FlightInfo> SomeDataChanged;
        HttpClient client = new HttpClient();
        protected virtual void OnSomeDataChanged(FlightInfo e)
        {
            SomeDataChanged?.Invoke(e);
        }

        //imitacja danych odczytanych z jakiegoś API
        public List<FlightInfo> FetchedData { get; set; } = new List<FlightInfo>()
        {
            
        };

        public async Task CheckTheApiEvery5m()
        {            
            //imitacja odpytywania API co określony czas - tu 0.5 sekundy
            while (true)
            {
                
            var response = client.GetAsync(uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var getResponse = response.Content.ReadAsAsync<FlightInfo[]>().Result;
                var status = "  ";
                PreviousResponse = RecentResponse;
                RecentResponse = new RecentResponse(){LastResponse = getResponse, LastResponseDT = DateTime.Now};

                var changed = FetchedData.First(x => x.status == status);

                //zgłaszamy zaistnienie zmiany
                OnSomeDataChanged(changed);

                await Task.Delay(5*60000);
            }
            }
        }
    }
}