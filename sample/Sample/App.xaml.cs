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
				.SetBaseUrl("https://notifo.easierlife.com")
				.SetApiKey("7hht0pqtsqyvkeco9h7434wrnbrpld6s1hmdu08e0tkx")
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
