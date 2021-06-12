using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WYW
{
    public class Program
    {
        static string uri = "https://aviation-edge.com/v2/public/timetable?key=777139-df23dc&iataCode=POZ&type=departure";

        static RecentResponse RecentResponse { get; set; }
        static RecentResponse PreviousResponse { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            HttpClient client = new HttpClient();
            var response = client.GetAsync(uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var getResponse = response.Content.ReadAsAsync<FlightInfo[]>().Result;
                RecentResponse.LastResponse = getResponse;

                //var getResponse = response.Content.ReadAsStringAsync().Result;

                //var getResponse = JsonConvert.DeserializeObject<GetDeparturesResponse>(stringResp);

                Console.WriteLine($"ilość odlotów:\t{getResponse.Count()}");
                Console.WriteLine($"linia:\t{getResponse.FirstOrDefault().airline.name}");
                Console.WriteLine($"arrival:\t{getResponse.FirstOrDefault().arrival.scheduledTime}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
