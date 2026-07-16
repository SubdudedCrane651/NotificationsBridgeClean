#if ANDROID
using Android.Content;

namespace NotificationsBridgeClean.Platforms.Android
{
    public static class NotificationAccessHelper
    {
        public static void OpenSettings()
        {
            var intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            Platform.CurrentActivity.StartActivity(intent);
        }
    }
}
#endif
