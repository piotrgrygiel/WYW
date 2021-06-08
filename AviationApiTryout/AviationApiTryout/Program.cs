using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AviationApiTryout
{
    class Program
    {
        static string uri = "https://aviation-edge.com/v2/public/timetable?key=777139-df23dc&iataCode=POZ&type=departure";

        static RecentResponse RecentResponse { get; set; }
        static RecentResponse PreviousResponse { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpClient client = new HttpClient();

            var response = client.GetAsync(uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var getResponse = response.Content.ReadAsAsync<FlightInfo[]>().Result;

                //var stringResp = response.Content.ReadAsStringAsync().Result;

                //var getResponse = JsonConvert.DeserializeObject<GetDeparturesResponse>(stringResp);

                Console.WriteLine($"ilość odlotów:\t{getResponse.Count()}");
                Console.WriteLine($"linia:\t{getResponse.FirstOrDefault().airline.name}");
                Console.WriteLine($"arrival:\t{getResponse.FirstOrDefault().arrival.scheduledTime}");
            }

            Console.ReadKey();
        }
    }
}
