using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;

namespace Cam_FaceDetection.Logic
{
    class CameraCapture
    {
        private MediaCapture _mediaCapture;
        private MediaEncodingProfile _videoEncodingProperties;
        private ImageEncodingProperties _imgEncodingProperties;

        public VideoDeviceController VideoDeviceController
        {
            get { return _mediaCapture.VideoDeviceController; }
        }

        /// <summary>
        /// Initializes the MediaCapture object.
        /// </summary>
        /// <param name="primaryUse">The primary use of the camera.</param>
        /// <returns></returns>
        public async Task<MediaCapture> Initialize(CaptureUse primaryUse = CaptureUse.Video)
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            _mediaCapture.VideoDeviceController.PrimaryUse = primaryUse;
            _mediaCapture.VideoDeviceController.TorchControl.Enabled = false;

            var previewResolution = _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);

            foreach (var mediaEncodingProperties in previewResolution)
            {
                var properties = (VideoEncodingProperties) mediaEncodingProperties;
                if (properties.Width == 640 && properties.Height == 480)
                {
                    await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, properties);
                }
            }

            _imgEncodingProperties = ImageEncodingProperties.CreateJpeg();
            _imgEncodingProperties.Width = 1024;
            _imgEncodingProperties.Height = 768;

            _videoEncodingProperties = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Vga);
            return _mediaCapture;
        }

        /// <summary>
        /// Starts camera preview stream.
        /// </summary>
        public async Task StartPreview()
        {
            await _mediaCapture.StartPreviewAsync();
        }

        /// <summary>
        /// Stops camera preview stream.
        /// </summary>
        public async Task StopPreview()
        {
            await _mediaCapture.StopPreviewAsync();
        }

        /// <summary>
        /// Disposes the MediaCapture object.
        /// </summary>
        public void Dispose()
        {
            _mediaCapture.Dispose();
            _mediaCapture = null;
        }

        public async Task<VideoFrame> GetPreviewFrameAsync(VideoFrame videoFrame)
        {
            return await _mediaCapture.GetPreviewFrameAsync(videoFrame);
        }

        public async Task<IMediaExtension> AddVideoEffectAsync(FaceDetectionEffectDefinition definition, MediaStreamType type)
        {
            return await _mediaCapture.AddVideoEffectAsync(definition, type);
        }
    }
}
