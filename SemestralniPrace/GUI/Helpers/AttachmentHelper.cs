using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI.Helpers
{
    public static class AttachmentHelper
    {
        /// <summary>
        /// Metoda pro načtění ImageSource z pole byte.
        /// </summary>
        /// <param name="blobData">Pole bytů obsahujicí obrázek.</param>
        /// <returns>ImageSource, zdroj dat pro vykreslení obrázku.</returns>
        public static ImageSource LoadImageSource(byte[] blobData)
        {
            using var ms = new MemoryStream(blobData);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }
}
