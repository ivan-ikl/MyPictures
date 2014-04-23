using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using MyPictures.Web.Models;
using Newtonsoft.Json;
using TestClient.Wpf.Model;
using TestClient.Wpf.Properties;

namespace TestClient.Wpf.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private string _filePickerFullyQualifiedFileName;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                    }
                    //WelcomeTitle = item.Title;
                });

            ButtonCommandPost = new RelayCommand(DoApiPost);
            ButtonFileChooser = new RelayCommand(DoFileChooser);
            QrCodeImage =
                QrCodeUtility.GetQrCodeImage("Upload an image using this test client. This QR code will point to it.");
        }

        public RelayCommand ButtonCommandPost { get; private set; }
        public RelayCommand ButtonFileChooser { get; private set; }
        public BitmapSource QrCodeImage { get; set; }
        public string TextBlockResponse { get; set; }
        public string ImageSource { get; set; }
        public string TextBlockDescription { get; set; }

        private void DoFileChooser()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpeg;*.jpg;*.png;*.gif)|*.jpeg;*.jpg;*.png;*.gif";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                _filePickerFullyQualifiedFileName = dlg.FileName;
                Debug.WriteLine(_filePickerFullyQualifiedFileName);
            }
        }

        private static string AddQuotes(string str)
        {
            return '"' + str + '"';
        }

        private static byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private async void DoApiPost()
        {
            Debug.WriteLine("in the DoApiPost");
            TextBlockResponse = "Begin Post test";

            HttpMessageHandler handler = new HttpClientHandler();
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.BaseAddress = new Uri(Settings.Default.ApiUrl);

            string fileName = Path.GetFileName(_filePickerFullyQualifiedFileName);

            var picture = new Picture
            {
                Name = fileName,
                Description = TextBlockDescription,
                Tags = "Image,Test"
            };

            string filename = _filePickerFullyQualifiedFileName;

            var bitmap = Image.FromFile(filename) as Bitmap;

            var content = new MultipartFormDataContent
            {
                {new StringContent(picture.Name), AddQuotes("name")},
                {new StringContent(picture.Description), AddQuotes("description")},
                {new StringContent(picture.Tags), AddQuotes("tags")},
            };

            var image = new ByteArrayContent(ImageToByteArray(bitmap));
            image.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(image, "filename", "image.jpg");

            HttpResponseMessage response = await client.PostAsync("api/pictures", content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                String result = await response.Content.ReadAsStringAsync();
                var newpicture = JsonConvert.DeserializeObject<Picture>(result);
                TextBlockResponse += "\n" + newpicture.Url;
                ImageSource = newpicture.Url.ToString();
                QrCodeImage = QrCodeUtility.GetQrCodeImage(newpicture.Url.ToString());
            }
        }
    }
}