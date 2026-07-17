using Android.App;
using Android.Service.Notification;
using Android.OS;
using Android.Content;
using Android.Util;

namespace NotificationsBridgeClean.Platforms.Android
{
    [Service(
        Name = "com.maui.notificationsbridgeclean.NotificationsBridgeClean.Platforms.Android.NotificationListener",
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
        Exported = true)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListener : NotificationListenerService
    {
        public override void OnListenerConnected()
        {
            try
            {
                Log.Info("NB", "OnListenerConnected() entered");

                var manager = (NotificationManager)Android.App.Application.Context
                    .GetSystemService(NotificationService);

                if (manager == null)
                {
                    Log.Warn("NB", "NotificationManager is null");
                    return;
                }

                const string channelId = "nb_channel";

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channel = new NotificationChannel(
                        channelId,
                        "NotificationsBridge",
                        NotificationImportance.Low);

                    manager.CreateNotificationChannel(channel);
                }

                var notification = new Notification.Builder(Android.App.Application.Context, channelId)
                    .SetContentTitle("NotificationsBridge is running")
                    .SetContentText("Listening for notifications")
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                    .Build();

                StartForeground(1, notification);

                Log.Info("NB", "OnListenerConnected() completed");
            }
            catch (Exception ex)
            {
                Log.Error("NB", $"OnListenerConnected crashed: {ex}");
            }
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            try
            {
                var extras = sbn.Notification.Extras;
                var text = extras?.GetString("android.text");

                if (string.IsNullOrEmpty(text))
                    return;

                // ⭐ Only react when the notification text contains "glucose"
                if (!text.Contains("glucose", StringComparison.OrdinalIgnoreCase))
                    return;

                Log.Info("NB", $"Glucose notification: {text}");

                var client = new System.Net.Http.HttpClient();
                client.PostAsync(
                    "http://10.0.0.205:8123/api/webhook/glucosenotification",
                    new StringContent(text)
                );
            }
            catch (Exception ex)
            {
                Log.Error("NB", ex.ToString());
            }
        }
    }
}
