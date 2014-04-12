using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace TestClient.Wpf.Model
{
    class QrCodeUtility
    {
        public static BitmapSource GetQrCodeImage(string qrValue)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 1
                }
            };

            using (var bMap = barcodeWriter.Write(qrValue))
            {
                var hbmp = bMap.GetHbitmap();
                try
                {
                    var source = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    // QRCode.Source = source;
                    // QrCodeImage = source;
                    return source;
                }
                finally
                {
                    // DeleteObject(hbmp); // TODO huh? what or where is DeleteObject ???

                }
            }
        }
    }
}
