using System.Threading.Tasks;
using Accord.Vision.Detection;

namespace Cam_FaceDetection.Logic.HaarCascades
{
    interface IFaceHaarCascade
    {
        Task<HaarCascade> GetInstance();
    }
}
