using System;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cam_FaceDetection.Controllers;


namespace Cam_FaceDetection.Views
{
    public sealed partial class FaceDetectionPage : Page
    {
        private IDetectionController _controller;
        private DispatcherTimer _timer;
        private int _frameCounter;

        public FaceDetectionPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += GoBack;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void GoBack(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var chosenDetector = e.Parameter as string;
            _controller = chosenDetector.Equals("MyImplementation")
                ? (IDetectionController) new FaceDetectImplController()
                : new BuiltinFilterController();

            _controller.DetectionFinished += UpdateOnDetectionFinished;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            await SetupCamera();
        }

        private async Task SetupCamera()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
            CameraPreview.Source = await _controller.InitializeCamera();
            await _controller.RunCamera();
            await _controller.StartDetecting();
        }

        private void timer_Tick(object sender, object e)
        {
            FacesCanvas.Children.Clear();
            FpsTextBlock.Text = string.Format("{0}", _frameCounter);
            _frameCounter = 0;
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await _controller.ReleaseCamera();
            CameraPreview.Source = null;
            _timer.Stop();
            _timer = null;
        }

        private void UpdateOnDetectionFinished(object sender, EventArgs e)
        {
            if (_controller.FacesFound != null)
            {
                FacesCanvas.Children.Clear();
                foreach (var face in _controller.FacesFound)
                {
                    FacesCanvas.Children.Add(face);
                }
            }
            _frameCounter++;
        }
    }
}
