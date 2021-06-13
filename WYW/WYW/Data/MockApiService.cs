using System;

namespace WYW.Data
{
    public class MockApiService : IApiResponseService
    {
        public RecentResponse RecentResponse { get; set; }

        public MockApiService()
        {
            RecentResponse = new RecentResponse()
            {
                LastResponseDT = DateTime.Now,
                LastResponse = new FlightInfo[0]
            };
        }
    }
}
