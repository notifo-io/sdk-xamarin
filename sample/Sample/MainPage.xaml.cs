using NotifoIO.SDK;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Sample
{
	public partial class MainPage : ContentPage
	{
		public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>() { };

		public MainPage()
		{
			InitializeComponent();
			BindingContext = this;

			Notifo.Current.OnNotificationReceived += Current_OnNotificationReceived;
		}

		private void Current_OnNotificationReceived(object sender, NotificationDataEventArgs e)
		{
			Notifications.Add(e.Notification);
		}
	}
}
