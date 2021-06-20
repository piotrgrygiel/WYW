using System;

namespace WYW.Data
{
    public interface IApiResponseService
    {
        RecentResponse RecentResponse { get; set; }
        event Action<FlightInfo> SomeDataChanged;
    }
}
