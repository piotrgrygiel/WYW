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

        private ExtendedFlightInfo chosenFlightInfo = null;
        private bool userFilledFlightNumber = false;
        private InputModel inputfdModel = new InputModel();
        private Timer timer = null;
        private List<ExtendedFlightInfo> flightInfos;
        private BSDataTable<ExtendedFlightInfo> SortableRef { get; set; }
        private BSTabGroup TabGroup;
        private BSModal FullWidth { get; set; }
        private ExtendedFlightInfo modalFlightInfo;

        private string exceptionDetails;

        protected override void OnInitialized()
        {
            try
            {
                ApiService.SomeDataChanged += OnFlightChanged;
                flightInfos = ApiService.RecentResponse.LastResponse
                                                    .Select(fi => new ExtendedFlightInfo(fi))
                                                    .ToList();

                modalFlightInfo = flightInfos.FirstOrDefault();
                if (timer == null)
                {
                    timer = new Timer(async (e) => { await UpdateTimeSpans(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
                }
            }
            catch (Exception ex)
            {
                exceptionDetails = ex.ToString();
            }
        }

        private void OnFlightChanged(FlightInfo flight)
        {
            var changedFlight = flightInfos.FirstOrDefault(fi => fi.FlightInfo.flight.iataNumber == flight.flight.iataNumber);
            ExtendedFlightInfo updatedFlight = new ExtendedFlightInfo(flight);

            if (changedFlight != null)
            {
                updatedFlight = new ExtendedFlightInfo(flight);
                if(changedFlight.FlightInfo.status != flight.status)
                    updatedFlight.IsStatusChanged = true;
                else
                    updatedFlight.IsStatusChanged = false;
                if(changedFlight.FlightInfo.departure.scheduledTime != flight.departure.scheduledTime)
                    updatedFlight.IsScheduledTimeChanged = true;
                else
                    updatedFlight.IsScheduledTimeChanged = false;
                if(changedFlight.FlightInfo.departure.terminal != flight.departure.terminal)
                    updatedFlight.IsTerminalChanged = true;
                else
                    updatedFlight.IsTerminalChanged = false;
                if(changedFlight.FlightInfo.departure.gate != flight.departure.gate)
                    updatedFlight.IsGateChanged = true;
                else
                    updatedFlight.IsGateChanged = false;

                flightInfos.Remove(changedFlight);
                flightInfos.Add(updatedFlight);
            }

            if (chosenFlightInfo != null && chosenFlightInfo.FlightInfo.flight.iataNumber == flight.flight.iataNumber)
                chosenFlightInfo = updatedFlight;

            UpdateTimeSpans();
        }

        private async Task GetFlightDetailsIMainTab(ExtendedFlightInfo flight)
        {
            chosenFlightInfo = flight;
            await UpdateTimeSpans();
            await InvokeAsync(StateHasChanged);
            TabGroup.SelectTabById("flightTab");
        }

        private async Task UpdateTimeSpans()
        {
            try
            {
                var now = await TimeZoneService.GetLocalDateTime(DateTimeService.UtcNow());

                foreach (var fi in flightInfos)
                {
                    UpdateTimeFor(fi, now);
                }

                UpdateTimeFor(chosenFlightInfo, now);

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                exceptionDetails = ex.ToString();
            }
        }

        private void UpdateTimeFor(ExtendedFlightInfo fi, DateTimeOffset now)
        {
            if (fi == null)
                return;

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

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();

            ApiService.SomeDataChanged -= OnFlightChanged;
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

        private void ExpandFlight(ExtendedFlightInfo flight)
        {
            flight.IsOpen = !flight.IsOpen;
            StateHasChanged();
        }

        private void ExpandFlight2(ExtendedFlightInfo flight)
        {
            modalFlightInfo = flight;
            FullWidth.Show();
        }
    }
}