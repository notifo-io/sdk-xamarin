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
				.SetApiKey("ca2emdxkkrkp4kevlbwkcyvscvqejgg1e5pznlgk8dex")
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
