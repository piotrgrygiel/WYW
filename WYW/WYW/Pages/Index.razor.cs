using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlazorStrap;
using System.Threading.Tasks;
using WYW.Data;
using BlazorStrap.Extensions.BSDataTable;

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

        public List<SimpleFlightInfo> departureFlights { get; set; }
        public List<SimpleFlightInfo> arrivalFlights { get; set; }
        private BSDataTable<SimpleFlightInfo> SortableRef { get; set; }

        protected override void OnInitialized()
        {
            flightInfos = ApiService.RecentResponse.LastResponse
                                                    .Select(fi => new ExtendedFlightInfo(fi))
                                                    .ToList();

            if (timer == null)
            {
                timer = new Timer(async (e) => { await UpdateTimeSpans(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            }

            arrivalFlights = new List<SimpleFlightInfo>();
            departureFlights = new List<SimpleFlightInfo>();

            updateSimpleArrivals();
            updateSimpleDepartures();
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

            updateSimpleArrivals();
            updateSimpleDepartures();
            
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

        private void updateSimpleArrivals()
        {
            arrivalFlights.Clear();

            foreach (var record in flightInfos)
            {
                if (record.FlightInfo.type == "arrival")
                {
                    arrivalFlights.Add(new SimpleFlightInfo()
                    {
                        iataNumber = record.FlightInfo.flight.iataNumber,
                        iataCode = record.FlightInfo.arrival.iataCode,
                        TimeTo = ($"{record.TimeToArrival:h\\:mm}")
                    });
                }
            }
        }
        private void updateSimpleDepartures()
        {
            departureFlights.Clear();

            foreach (var record in flightInfos)
            {
                if (record.FlightInfo.type == "departure")
                {
                    departureFlights.Add(new SimpleFlightInfo()
                    {
                        iataNumber = record.FlightInfo.flight.iataNumber,
                        iataCode = record.FlightInfo.arrival.iataCode,
                        TimeTo = ($"{record.TimeToDeparture:h\\:mm}")
                    });
                }
            }
        }
    }
}