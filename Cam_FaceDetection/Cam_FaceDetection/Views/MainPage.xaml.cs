using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace Cam_FaceDetection.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        private void MyImplementationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FaceDetectionPage), "MyImplementation");
        }

        private void FilterEffectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FaceDetectionPage), "Built-in");
        }
    }
}
