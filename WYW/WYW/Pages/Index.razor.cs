using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WYW;

namespace WYW.Pages
{
    public partial class Index
    {
        [Inject]
        ILogger<inputModel> Logger {get; set;}
        private inputModel inputfdModel = new inputModel();
        [Inject]
        private ApiResponseService ApiService { get; set; }
        private FlightInfo flightInfo = null;
        private bool userFilledFlightNumber = false;

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");

            userFilledFlightNumber = true;

            flightInfo = ApiService.RecentResponse.LastResponse.FirstOrDefault(flight => flight.flight.number.Equals(inputfdModel.getNumber(), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}