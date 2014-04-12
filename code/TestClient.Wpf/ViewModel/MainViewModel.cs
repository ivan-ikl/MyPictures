using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using MyPictures.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TestClient.Wpf.Model;
using TestClient.Wpf.Properties;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;


namespace TestClient.Wpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
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
                            return;
                        }

                        //WelcomeTitle = item.Title;
                    });

            ButtonCommandPost = new RelayCommand(DoApiPost);
            ButtonFileChooser = new RelayCommand(DoFileChooser);
            QrCodeImage = QrCodeUtility.GetQrCodeImage("Upload an image using this test client. This QR code will point to it.");
            ImageSource = @"https://jeffamypictures.blob.core.windows.net/mypictures/d82b0d6d-996e-45e8-bf4b-d2eee3d37eb1";
        }

        private string filePickerFullyQualifiedFileName;
        private void DoFileChooser()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            // dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            dlg.Filter = "Image files (*.jpeg;*.jpg;*.png;*.gif)|*.jpeg;*.jpg;*.png;*.gif";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                filePickerFullyQualifiedFileName = dlg.FileName;
                Debug.WriteLine(filePickerFullyQualifiedFileName);
            }
        }

        public RelayCommand ButtonCommandPost { get; private set; }
        public RelayCommand ButtonFileChooser { get; private set; }
        public BitmapSource QrCodeImage { get; set; }
        public string TextBlockResponse { get; set; }
        public string ImageSource { get; set; }
        public string TextBlockDescription { get; set; }


        private static string AddQuotes(string str)
        {
            return '"' + str + '"';
        }

        private static byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private async void DoApiPost()
        {
            Debug.WriteLine("in the DoApiPost");
            TextBlockResponse = "Begin Post test";

            HttpMessageHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.BaseAddress = new Uri(Settings.Default.ApiUrl);

            string fileName = Path.GetFileName(filePickerFullyQualifiedFileName);

            Picture picture = new Picture()
                {
                    Name = fileName,
                    Description = TextBlockDescription,
                    Tags = "Image,Test"
                };

            var assembly = Assembly.GetExecutingAssembly();

            //string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string filename = Path.Combine(root, "../../Images/586px-Dottie0.jpg");

            var filename = filePickerFullyQualifiedFileName;

            Bitmap bitmap = Image.FromFile(filename) as Bitmap;

            String json = JsonConvert.SerializeObject(picture);

            var content = new MultipartFormDataContent
                {
                    {new StringContent(picture.Name), AddQuotes("name")},
                    {new StringContent(picture.Description), AddQuotes("description")},
                    {new StringContent(picture.Tags), AddQuotes("tags")},

                    //{ new StringContent(json, Encoding.UTF8, "application/json"), AddQuotes("data") }
                };

            var image = new ByteArrayContent(imageToByteArray(bitmap));
            image.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(image, "filename", "image.jpg");

            HttpResponseMessage response = await client.PostAsync("api/pictures", content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                String result = await response.Content.ReadAsStringAsync();
                Picture newpicture = JsonConvert.DeserializeObject<Picture>(result);
                TextBlockResponse += "\n" + newpicture.Url.ToString();
                //QrCodeText = newpicture.Url.ToString();
                ImageSource = newpicture.Url.ToString();
                QrCodeImage = QrCodeUtility.GetQrCodeImage(newpicture.Url.ToString());
            }           
        }

        ////public override void Cleanup()
            ////{
            ////    // Clean up if needed

            ////    base.Cleanup();
            ////}
        }
    }
