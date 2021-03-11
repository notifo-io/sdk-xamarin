using Plugin.FirebasePushNotification;
using Xamarin.Forms;

namespace Sample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}

		private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine($"TOKEN : {e.Token}");
		}
	}
}
