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
using Microsoft.JSInterop;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WebPush;
using System.Text.Json;

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

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private WywDbContext DbContext { get; set; }

        private ExtendedFlightInfo chosenFlightInfo = null;
        private InputModel inputfdModel = new InputModel();
        private Timer timer = null;
        private List<ExtendedFlightInfo> flightInfos;
        private BSDataTable<ExtendedFlightInfo> SortableRef { get; set; }
        private BSTabGroup TabGroup;
        private BSModal FullWidth { get; set; }
        private ExtendedFlightInfo modalFlightInfo;

        private string exceptionDetails;

        private bool isSubscribing = false;

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

        private async Task Subscribe()
        {
            try
            {
                var subscription = await JSRuntime.InvokeAsync<NotificationSubscription>("blazorPushNotifications.requestSubscription");
                if (subscription != null)
                {
                    await Subscribe(subscription);
                }
                else
                {
                    var userId = GetUserId();
                    subscription = await DbContext.NotificationSubscriptions.FirstOrDefaultAsync(e => e.UserId == userId);
                }

                if (subscription != null)
                    isSubscribing = true;
            }
            catch (Exception ex)
            {
                exceptionDetails = ex.ToString();
            }
        }

        private async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
        {
            // We're storing at most one subscription per user, so delete old ones.
            // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
            var userId = GetUserId();
            var oldSubscriptions = DbContext.NotificationSubscriptions.Where(e => e.UserId == userId);
            DbContext.NotificationSubscriptions.RemoveRange(oldSubscriptions);

            // Store new subscription
            subscription.UserId = userId;
            DbContext.NotificationSubscriptions.Attach(subscription);

            await DbContext.SaveChangesAsync();
            return subscription;
        }

        private string GetUserId()
        {
            var user = AuthenticationStateProvider.GetAuthenticationStateAsync().Result.User;
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task SendPushMsgIfSubscribing(string pushMsgText)
        {
            if (!isSubscribing)
                return;

            var subscription = await DbContext.NotificationSubscriptions.Where(e => e.UserId == GetUserId()).SingleOrDefaultAsync();
            if (subscription != null)
            {
                _ = SendNotificationAsync(subscription, pushMsgText);
            }
        }

        private async Task SendNotificationAsync(NotificationSubscription subscription, string message)
        {
            // For a real application, generate your own
            var publicKey = "BLWey6JFyOAct0RHiJ4LIR6nciFAcizHPD1HRCU5z365YPOXtmvlM3BJkxjlUocd0xzNIWHKc57KQ9mSk15IGRo";
            var privateKey = "Z6sUZj7BEJftmw7yFTPJLhO3aHDblZYRouogmH8_8mo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"/",
                });
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error sending push notification: " + ex.Message);
                exceptionDetails = ex.ToString();
            }
        }

        private async void OnFlightChanged(FlightInfo flight)
        {
            var changedFlight = flightInfos.FirstOrDefault(fi => fi.FlightInfo.flight.iataNumber == flight.flight.iataNumber);
            ExtendedFlightInfo updatedFlight = new ExtendedFlightInfo(flight);

            if (changedFlight != null)
            {
                flightInfos.Remove(changedFlight);
                flightInfos.Add(updatedFlight);
            }
            else
            {
                //nowy lot na liście
                flightInfos.Add(updatedFlight);

            }

            if (chosenFlightInfo != null && chosenFlightInfo.FlightInfo.flight.iataNumber == flight.flight.iataNumber)
            {
                if (changedFlight.FlightInfo.status != flight.status)
                {
                    await SendPushMsgIfSubscribing($"Status Twojego lotu uległ zmianie na {flight.status}");
                    updatedFlight.IsStatusChanged = true;
                }
                else
                    updatedFlight.IsStatusChanged = false;

                if (changedFlight.FlightInfo.departure.scheduledTime != flight.departure.scheduledTime)
                {
                    await SendPushMsgIfSubscribing($"Czas odlotu Twojego lotu został zmieniony na {flight.departure.scheduledTime.ToLocalTime():g}");
                    updatedFlight.IsScheduledTimeChanged = true;
                }
                else
                    updatedFlight.IsScheduledTimeChanged = false;

                if (changedFlight.FlightInfo.departure.terminal != flight.departure.terminal)
                {
                    await SendPushMsgIfSubscribing($"Terminal Twojego lotu uległ zmianie na {flight.departure.terminal}");
                    updatedFlight.IsTerminalChanged = true;
                }
                else
                    updatedFlight.IsTerminalChanged = false;

                if (changedFlight.FlightInfo.departure.gate != flight.departure.gate)
                {
                    await SendPushMsgIfSubscribing($"Numer bramki Twojego lotu uległ zmianie na {flight.departure.gate}");
                    updatedFlight.IsGateChanged = true;
                }
                else
                    updatedFlight.IsGateChanged = false;

                chosenFlightInfo = updatedFlight;
            }
                
            await UpdateTimeSpans();
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