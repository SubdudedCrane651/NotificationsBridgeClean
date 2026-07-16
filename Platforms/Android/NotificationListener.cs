using Android.Service.Notification;
using Android.App;
using Android.OS;
using System.Net.Http;
using System.Text;

namespace BotificationsBridgeClean.Platforms.Android
{
    [Service(
        Label = "GlucoseNotificationListener",
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
        Exported = true)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListener : NotificationListenerService
    {
        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            var extras = sbn.Notification.Extras;
            var text = extras?.GetString("android.text");

            if (text != null && text.ToLower().Contains("glucose"))
            {
                SendToBackend();
            }
        }

        private async void SendToBackend()
        {
            var client = new HttpClient();
            await client.PostAsync("http://solutiontech.3utilities.com:8123/api/webhook/glucosenotification",
                new StringContent("", Encoding.UTF8, "application/json"));
        }

        public override void OnListenerConnected()
        {
            base.OnListenerConnected();

            var notification = new Notification.Builder(this)
                .SetContentTitle("NotificationsBridge is running")
                .SetContentText("Listening for Google Assistant commands")
                .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo)
                .Build();

            StartForeground(1, notification);
        }
    }
}

