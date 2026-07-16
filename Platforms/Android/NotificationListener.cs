using Android.App;
using Android.Service.Notification;
using Android.OS;
using Android.Content;
using Android.Util;

namespace NotificationsBridgeClean.Platforms.Android
{
    [Service(
        Label = "GlucoseNotificationListener",
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
        Exported = true)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListener : NotificationListenerService
    {
        public override void OnListenerConnected()
        {
            base.OnListenerConnected();

            var notification = new Notification.Builder(this)
                .SetContentTitle("NotificationsBridge is running")
                .SetContentText("Listening for notifications")
                .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo)
                .Build();

            StartForeground(1, notification);
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            try
            {
                var extras = sbn.Notification.Extras;
                var text = extras?.GetString("android.text");

                if (!string.IsNullOrEmpty(text))
                {
                    var client = new System.Net.Http.HttpClient();
                    client.PostAsync(
                        "http://10.0.0.205:8123/api/webhook/glucosenotification",
                        null
                    );
                }
            }
            catch (Exception ex)
            {
                Log.Error("NotificationsBridge", ex.ToString());
            }
        }
    }
}
