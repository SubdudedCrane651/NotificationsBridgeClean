using Microsoft.Maui.Controls;

namespace NotificationsBridgeClean
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //#if ANDROID
        //            Platforms.Android.NotificationAccessHelper.OpenSettings();
        //#endif

        private async void OnTestWebhookClicked(object sender, EventArgs e)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.PostAsync(
                    "https://homeassistant-mini.com/api/webhook/glucosenotification",
                    null
                );

                await DisplayAlert("Webhook", $"Status: {response.StatusCode}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

