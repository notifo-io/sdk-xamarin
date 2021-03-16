using NotifoIO.SDK;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;


namespace Sample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var pushEventsProvider = new CrossPushPluginEventsProvider(CrossFirebasePushNotification.Current);

			Notifo.Current
				.SetBaseUrl(Constants.ApiUrl)
				.SetApiKey(Constants.UserApiKey)
				.SetPushEventsProvider(pushEventsProvider);

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{

		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
