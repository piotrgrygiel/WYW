using BlazorTemplate.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace BlazorTemplate.Pages
{
    public partial class SubscribeSomeData : IDisposable
    {

        [Inject]
        private FancyApiResponseService FancyApiService { get; set; }

        private bool requestSent = false;
        private int id = 1;

        private int currentValue;

        private void SubmitIntegerValue()
        {
            currentValue = FancyApiService.FetchedData.First(d=>d.Id == id).SomeInteger;
            FancyApiService.SomeDataChanged += FancyApiService_SomeDataChanged;
            requestSent = true;
        }

        private void FancyApiService_SomeDataChanged(SomeData newData)
        {
            if (id == newData.Id)
            {
                currentValue = newData.SomeInteger;
                InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            if (requestSent)
                FancyApiService.SomeDataChanged -= FancyApiService_SomeDataChanged;
        }
    }
}
