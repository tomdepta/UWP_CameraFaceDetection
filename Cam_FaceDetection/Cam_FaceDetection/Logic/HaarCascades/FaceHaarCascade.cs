using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Accord.Vision.Detection;

namespace Cam_FaceDetection.Logic.HaarCascades
{
    internal class FaceHaarCascade : IFaceHaarCascade
    {
        private static HaarCascade _cascadeInstance;

        private FaceHaarCascade()
        {
        }

        /// <summary>
        /// Gets the HaarCascade instance created using "haarcascade_frontalface_alt.xml" file.
        /// </summary>
        /// <returns>The HaarCascade instance.</returns>
        public static async Task<HaarCascade> GetInstance()
        {
            if (_cascadeInstance == null)
            {
                StorageFile storageFile =
                    await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Resources/haarcascade_frontalface_alt.xml"));
                using (var stream = await storageFile.OpenReadAsync())
                {
                    _cascadeInstance = HaarCascade.FromXml(stream.AsStreamForRead());
                }
            }
            return _cascadeInstance;
        }

        Task<HaarCascade> IFaceHaarCascade.GetInstance()
        {
            return GetInstance();
        }
    }
}