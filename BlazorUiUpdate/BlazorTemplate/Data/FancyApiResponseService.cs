using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTemplate.Data
{
    //public delegate void SomeDataChangedEventHandler(object sender, SomeDataChangedEventArgs e);

    public class FancyApiResponseService
    {

        public event Action<SomeData> SomeDataChanged;

        protected virtual void OnSomeDataChanged(SomeData e)
        {
            SomeDataChanged?.Invoke(e);
        }

        //imitacja danych odczytanych z jakiegoś API
        public List<SomeData> FetchedData { get; set; } = new List<SomeData>()
        {
            new SomeData(1, 100),
            new SomeData(2, 100),
            new SomeData(3, 100),
            new SomeData(4, 100),
            new SomeData(5, 100),
        };

        public async Task CheckTheApiEvery500ms()
        {
            var rnd = new Random();
            
            //imitacja odpytywania API co określony czas - tu 0.5 sekundy
            while (true)
            {
                var id = rnd.Next(1, 6);
                var someInt = rnd.Next(0, 1000);

                var changed = FetchedData.First(x => x.Id == id);
                changed.SomeInteger = someInt;

                //zgłaszamy zaistnienie zmiany
                OnSomeDataChanged(changed);

                await Task.Delay(500);
            }
        }
    }

    //public class SomeDataChangedEventArgs : EventArgs
    //{
    //    public SomeData ChangedData { get; set; }

    //    public SomeDataChangedEventArgs(SomeData changedData)
    //    {
    //        ChangedData = changedData;
    //    }
    //}
}
