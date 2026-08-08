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

                // Replace 'Android.App.Application.Context' with 'Application.Context' to fix CS0234
                var manager = (NotificationManager)global::Android.App.Application.Context.GetSystemService(NotificationService);

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

                var notification = new Notification.Builder(global::Android.App.Application.Context, channelId)
                    .SetContentTitle("NotificationsBridge is running")
                    .SetContentText("Listening for notifications")
                    .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo)
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
                if (extras == null)
                    return;

                // Extract text from common fields
                string[] keys =
                {
            "android.text",
            "android.bigText",
            "android.title",
            "android.title.big"
        };

                string text = null;

                foreach (var key in extras.KeySet())
                {
                    var value = extras.Get(key);
                    Log.Info("NB", $"EXTRA KEY: {key} = {value}");
                text=value?.ToString();
                    if (!string.IsNullOrEmpty(text))
                        break;
                }

                Log.Info("NB", $"Notification Text: {text}");

                if (string.IsNullOrEmpty(text))
                    return;

                // ⭐ Correct filter for Google Home glucose notifications
                if (!text.Contains("glucose", StringComparison.OrdinalIgnoreCase) &&
                    !text.Contains("GlucoseInfo", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                Log.Info("NB", $"Google Home glucose notification: {text}");

                var client = new System.Net.Http.HttpClient();
                client.PostAsync(
                    "https://homeassistant-mini.com/api/webhook/glucosenotification",
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
