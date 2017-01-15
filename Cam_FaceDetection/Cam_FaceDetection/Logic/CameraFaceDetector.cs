using System.Collections.Generic;
using System.Drawing;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Accord.Vision.Detection;
using Cam_FaceDetection.Logic.HaarCascades;

namespace Cam_FaceDetection.Logic
{
    class CameraFaceDetector
    {
        private HaarObjectDetector _detector;
        private readonly List<Windows.UI.Xaml.Shapes.Rectangle> _realTimeDetectionFaceRectangles = new List<Windows.UI.Xaml.Shapes.Rectangle>();
        private Rectangle[] _faces;

        /// <summary>
        /// Creates the detector instance and initializes it with the Haar cascade read from an XML.
        /// Sets detector modes so as to provide fast response and sufficient results.
        /// </summary>
        public CameraFaceDetector()
        {
            InitializeDetector();
        }

        private async void InitializeDetector()
        {
            HaarCascade cascade = await FaceHaarCascade.GetInstance();
            _detector = new HaarObjectDetector(
                cascade,
                25,
                ObjectDetectorSearchMode.NoOverlap,
                1.2f,
                ObjectDetectorScalingMode.SmallerToGreater)
            {
                UseParallelProcessing = true,
                Suppression = 2
            };
        }

        /// <summary>
        /// Performs face detection on a single camera frame passed as a WriteableBitmap image.
        /// </summary>
        /// <param name="writeableBitmap">Camera frame.</param>
        /// <returns>FaceRectangle - a rectangle surrounding the detected face.</returns>
        public List<Windows.UI.Xaml.Shapes.Rectangle> GetFaceRectangle(WriteableBitmap writeableBitmap)
        {
            float xscale = writeableBitmap.PixelWidth / 160f;
            float yscale = writeableBitmap.PixelHeight / 90f;

            var resizedBitmap = writeableBitmap.Resize(160, 90, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

            _faces = _detector.ProcessFrame((Bitmap)resizedBitmap);

            if (_faces.Length <= 0)
            {
                return null;
            }

            SetRectangleSettings(xscale, yscale);

            return _realTimeDetectionFaceRectangles;
        }

        private void SetRectangleSettings(float xscale, float yscale)
        {
            _realTimeDetectionFaceRectangles.Clear();
            foreach (var face in _faces)
            {
                Thickness margin = new Thickness(face.X * xscale, face.Y * yscale, 0, 0);
                _realTimeDetectionFaceRectangles.Add(new Windows.UI.Xaml.Shapes.Rectangle
                {
                    Margin = margin,
                    Width = face.Width*xscale,
                    Height = face.Height*yscale,
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(Colors.Blue)
                });
            }
        }
    }
}
