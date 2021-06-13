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
        ILogger<inputModel> Logger {get; set;}

        private inputModel inputfdModel = new inputModel();

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
            var flNumber = inputfdModel.Name.ToUpper();

            flightInfo = ApiService.RecentResponse.LastResponse.FirstOrDefault(flight => flight.flight.number.Equals(inputfdModel.getNumber(), StringComparison.InvariantCultureIgnoreCase));

        }
    }
}