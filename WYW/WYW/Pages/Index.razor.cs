using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WYW;
using WYW.Data;

namespace WYW.Pages
{
    public partial class Index
    {
        [Inject]
        ILogger<InputModel> Logger {get; set;}

        private InputModel inputfdModel = new InputModel();

        [Inject]
        private IApiResponseService ApiService { get; set; }

        [Inject]
        private IDateTime DateTimeService { get; set; }

        private FlightInfo flightInfo = null;
        private bool userFilledFlightNumber = false;

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");

            userFilledFlightNumber = true;

            flightInfo = ApiService.RecentResponse.LastResponse.FirstOrDefault(flight => flight.flight.number.Equals(inputfdModel.GetNumber(), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}