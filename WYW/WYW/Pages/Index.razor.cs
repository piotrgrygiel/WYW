using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private Timer timer = null;
        private List<ExtendedFlightInfo> flightInfos;

        protected override void OnInitialized()
        {
            flightInfos = ApiService.RecentResponse.LastResponse
                                                    .Select(fi => new ExtendedFlightInfo(fi))
                                                    .ToList();

            if (timer == null)
            {
                timer = new Timer(async (e) => { await UpdateTimeSpans(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            }
        }

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");

            userFilledFlightNumber = true;

            flightInfo = ApiService.RecentResponse.LastResponse.FirstOrDefault(flight => flight.flight.number.Equals(inputfdModel.GetNumber(), StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task UpdateTimeSpans()
        {
            var now = await TimeZoneService.GetLocalDateTime(DateTimeService.UtcNow());

            foreach (var fi in flightInfos)
            {
                bool arrival = fi.FlightInfo.type == "arrival";
                var timeSchedulled = arrival ? fi.FlightInfo.arrival.scheduledTime : fi.FlightInfo.departure.scheduledTime;
                var timeSchedulledValidated = fi == null ? now : timeSchedulled;
                var timeTo = timeSchedulledValidated - now + now.Offset;

                if (arrival)
                {
                    if (timeTo < TimeSpan.Zero)
                        fi.TimeToArrival = TimeSpan.Zero;
                    else
                        fi.TimeToArrival = timeTo;
                }
                else
                {
                    if (timeTo < TimeSpan.Zero)
                        fi.TimeToDeparture = TimeSpan.Zero;
                    else
                        fi.TimeToDeparture = timeTo;
                }
            }
            
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
    }
}