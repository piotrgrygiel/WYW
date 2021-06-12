using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WYW
{
    //public delegate void SomeDataChangedEventHandler(object sender, SomeDataChangedEventArgs e);

    public class ApiResponseService
    {

        public event Action<FlightInfo> SomeDataChanged;

        protected virtual void OnSomeDataChanged(FlightInfo e)
        {
            SomeDataChanged?.Invoke(e);
        }

        //imitacja danych odczytanych z jakiegoś API
        public List<FlightInfo> FetchedData { get; set; } = new List<FlightInfo>()
        {
           /* new FlightInfo(1, 100),
            new FlightInfo(2, 100),
            new FlightInfo(3, 100),
            new FlightInfo(4, 100),
            new FlightInfo(5, 100),*/
        };

        public async Task CheckTheApiEvery5s()
        {            
            //imitacja odpytywania API co określony czas - tu 0.5 sekundy
            while (true)
            {
                var status = "  ";

                var changed = FetchedData.First(x => x.status == status);

                //zgłaszamy zaistnienie zmiany
                OnSomeDataChanged(changed);

                await Task.Delay(5000);
            }
        }
    }
}