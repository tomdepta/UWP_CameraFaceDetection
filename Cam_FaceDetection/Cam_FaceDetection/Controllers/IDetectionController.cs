using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Shapes;

namespace Cam_FaceDetection.Controllers
{
    interface IDetectionController
    {
        event EventHandler DetectionFinished;
        List<Rectangle> FacesFound { get; set; }

        Task<MediaCapture> InitializeCamera();

        Task RunCamera();

        Task StartDetecting();

        Task ReleaseCamera();
    }
}
