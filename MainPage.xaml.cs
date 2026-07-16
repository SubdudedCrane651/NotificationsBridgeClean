using Microsoft.Maui.Controls;

namespace NotificationsBridgeClean
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            Platforms.Android.NotificationAccessHelper.OpenSettings();
#endif
        }
    }
}

