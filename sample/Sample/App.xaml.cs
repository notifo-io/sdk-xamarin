using NotifoIO.SDK;
using NotifoIO.SDK.FirebasePlugin;
using Xamarin.Forms;


namespace Sample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Notifo.Current
				.SetBaseUrl(Constants.ApiUrl)
				.SetApiKey(Constants.UserApiKey)
				.UseFirebasePluginEventsProvider();

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
