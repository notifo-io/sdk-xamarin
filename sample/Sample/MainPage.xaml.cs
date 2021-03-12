using NotifoIO.SDK;
using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            GreetingLabel.Text = Notifo.Greeting;
        }
    }
}
