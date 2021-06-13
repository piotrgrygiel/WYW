using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using WYW.Data;

namespace WYW.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        private ILogger<InputModel> Logger {get; set;}

        [Inject]
        private IApiResponseService ApiService { get; set; }

        [Inject]
        private IDateTime DateTimeService { get; set; }

        private FlightInfo flightInfo = null;
        private bool userFilledFlightNumber = false;
        private InputModel inputfdModel = new InputModel();
        private TimeSpan TimeToDeparture;
        private Timer timer = null;

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");

            userFilledFlightNumber = true;

            flightInfo = ApiService.RecentResponse.LastResponse.FirstOrDefault(flight => flight.flight.number.Equals(inputfdModel.GetNumber(), StringComparison.InvariantCultureIgnoreCase));

            if (flightInfo != null && timer == null)
            {
                timer = new Timer((e) => { UpdateTimeToDeparture(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            }
        }

        private void UpdateTimeToDeparture()
        {
            var now = DateTimeService.Now();
            var departureTime = flightInfo == null ? now : flightInfo.departure.scheduledTime.ToLocalTime();

            TimeToDeparture = departureTime - now;

            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
    }
}