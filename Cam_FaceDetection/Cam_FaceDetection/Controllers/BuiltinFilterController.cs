using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cam_FaceDetection.Logic;

namespace Cam_FaceDetection.Controllers
{
    class BuiltinFilterController : IDetectionController
    {
        public List<Rectangle> FacesFound { get; set; }
        public event EventHandler DetectionFinished;

        private CameraCapture _cameraCapture;
        private FaceDetectionEffect _faceDetectionEffect;

        public BuiltinFilterController()
        {
            Initialize();
        }

        private void Initialize()
        {
            _cameraCapture = new CameraCapture();
        }

        private async void OnFaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => MarkFaces(args.ResultFrame.DetectedFaces));
        }

        public Task<MediaCapture> InitializeCamera()
        {
            return _cameraCapture.Initialize();
        }

        public async Task RunCamera()
        {
            await _cameraCapture.StartPreview();
        }

        public async Task StartDetecting()
        {
            var definition = new FaceDetectionEffectDefinition
            {
                SynchronousDetectionEnabled = false,
                DetectionMode = FaceDetectionMode.HighPerformance
            };
            _faceDetectionEffect =
                (FaceDetectionEffect)await _cameraCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);
            _faceDetectionEffect.FaceDetected += OnFaceDetected;
            _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
            _faceDetectionEffect.Enabled = true;
        }

        public async Task ReleaseCamera()
        {
            if (_cameraCapture != null)
            {
                await _cameraCapture.StopPreview();
                _cameraCapture.Dispose();
                _cameraCapture = null;
            }
        }

        private void MarkFaces(IReadOnlyList<DetectedFace> detectedFaces)
        {
            FacesFound = null;

            foreach (var face in detectedFaces)
            {
                if (FacesFound == null)
                {
                    FacesFound = new List<Rectangle>();
                }
                Rectangle faceRoundingRectangle = new Rectangle();
                Thickness margin = faceRoundingRectangle.Margin;
                margin.Left = face.FaceBox.X;
                margin.Top = face.FaceBox.Y;
                faceRoundingRectangle.Margin = margin;
                faceRoundingRectangle.Width = face.FaceBox.Width;
                faceRoundingRectangle.Height = face.FaceBox.Height;

                faceRoundingRectangle.StrokeThickness = 2;

                faceRoundingRectangle.Stroke = new SolidColorBrush(Colors.Blue);

                FacesFound.Add(faceRoundingRectangle);
            }
            DetectionFinished?.Invoke(null, null);
        }
    }
}
