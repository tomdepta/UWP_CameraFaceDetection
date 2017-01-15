using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Cam_FaceDetection.Logic;

namespace Cam_FaceDetection.Controllers
{
    class FaceDetectImplController : IDetectionController
    {
        internal delegate void DetectionCall(FaceDetectImplController controller);

        public List<Rectangle> FacesFound { get; set; }
        public event EventHandler DetectionFinished;

        private readonly CameraFaceDetector _detector;

        private CameraCapture _cameraCapture;
        private WriteableBitmap _writeableBitmap;
        private VideoEncodingProperties _previewProperties;
        private VideoFrame _videoFrame;
        private bool _isDetecting;

        public FaceDetectImplController()
        {
            _cameraCapture = new CameraCapture();
            _detector = new CameraFaceDetector();
            _isDetecting = false;
        }

        public Task<MediaCapture> InitializeCamera()
        {
            return _cameraCapture.Initialize();
        }

        public async Task RunCamera()
        {
            await _cameraCapture.StartPreview();
        }

        public async Task ReleaseCamera()
        {
            _isDetecting = false;
            if (_cameraCapture != null)
            {
                await _cameraCapture.StopPreview();
                _cameraCapture.Dispose();
                _cameraCapture = null;
            }
        }

        public async Task StartDetecting()
        {
            _isDetecting = true;
            while (_isDetecting)
            {
                if (_previewProperties == null || _videoFrame == null)
                {
                    _previewProperties =
                        _cameraCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as
                            VideoEncodingProperties;
                }

                _videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)_previewProperties.Width,
                    (int)_previewProperties.Height);

                using (var currentFrame = await _cameraCapture.GetPreviewFrameAsync(_videoFrame))
                {
                    SoftwareBitmap frameBitmap = currentFrame.SoftwareBitmap;

                    _writeableBitmap = new WriteableBitmap(frameBitmap.PixelWidth, frameBitmap.PixelHeight);
                    frameBitmap.CopyToBuffer(_writeableBitmap.PixelBuffer);

                    await
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.High,
                        () =>
                        {
                            FacesFound = _detector.GetFaceRectangle(_writeableBitmap);
                            DetectionFinished?.Invoke(null, null);
                        });
                }
            }
        }
    }
}
