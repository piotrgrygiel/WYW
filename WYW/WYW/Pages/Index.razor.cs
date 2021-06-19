using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using BlazorStrap;
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

        [Inject]
        private UserTimeZoneService TimeZoneService { get; set; }

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

            if (flightInfo != null)
            {
                if (timer == null)
                {
                    timer = new Timer((e) => { UpdateTimeToDeparture(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
                }
                else
                {
                    UpdateTimeToDeparture();
                }
            }
        }

        private async void UpdateTimeToDeparture()
        {
            var now = await TimeZoneService.GetLocalDateTime(DateTimeOffset.UtcNow);
            var departureTime = flightInfo == null ? now : flightInfo.departure.scheduledTime;

            TimeToDeparture = departureTime - now + now.Offset;

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }

    private void Show (BSTabEvent e)
    {
        Console.WriteLine($"Show   -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
    }
    private void Shown (BSTabEvent e)
    {
        Console.WriteLine($"Shown  -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
    }
    private void Hide (BSTabEvent e)
    {
        Console.WriteLine($"Hide   ->  Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
    }
    private void Hidden (BSTabEvent e)
    {
        Console.WriteLine($"Hidden -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
    }

    }
}