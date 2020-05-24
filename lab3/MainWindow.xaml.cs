using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public byte redRange { get; set; } = 0;
        public byte greenRange { get; set; } = 0;
        public byte blueRange { get; set; } = 0;
        public WriteableBitmap redBmp { get; set; }
        public WriteableBitmap greenBmp { get; set; }
        public WriteableBitmap blueBmp { get; set; }
        public BitmapImage Image { get; set; }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "bmp|*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {

                Image = new BitmapImage(new Uri(openFileDialog.FileName));
                redBmp = new WriteableBitmap(Image.PixelWidth, Image.PixelHeight, Image.DpiX, Image.DpiY, PixelFormats.Pbgra32, null);
                greenBmp = new WriteableBitmap(Image.PixelWidth, Image.PixelHeight, Image.DpiX, Image.DpiY, PixelFormats.Pbgra32, null);
                blueBmp = new WriteableBitmap(Image.PixelWidth, Image.PixelHeight, Image.DpiX, Image.DpiY, PixelFormats.Pbgra32, null);

                Display();



            }

        }
        void Display()
        {

            rgb.Source = Image;

            var a = GetPixels(Image);
            var rbytes = (byte[])a.Clone();
            var gbytes = (byte[])a.Clone();
            var bbytes = (byte[])a.Clone();
            for (int i = 0; i < rbytes.Length; i += 4)
            {
                rbytes[i] = 0;
                rbytes[i + 1] = 0;
                rbytes[i + 2] = rbytes[i + 2] > redRange ? rbytes[i + 2] : (byte)0;
            }
            for (int i = 0; i < gbytes.Length; i += 4)
            {
                gbytes[i] = 0;
                gbytes[i + 1] = gbytes[i + 1] > greenRange ? gbytes[i + 1] : (byte)0;
                gbytes[i + 2] = 0;
            }
            for (int i = 0; i < bbytes.Length; i += 4)
            {
                bbytes[i + 0] = bbytes[i + 0] > blueRange ? bbytes[i + 0] : (byte)0;
                bbytes[i + 1] = 0;
                bbytes[i + 2] = 0;
            }


            redBmp.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight), rbytes, Image.PixelWidth * 4, 0);
            r.Source = redBmp;

            greenBmp.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight), gbytes, Image.PixelWidth * 4, 0);
            g.Source = greenBmp;

            blueBmp.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight), bbytes, Image.PixelWidth * 4, 0);
            b.Source = blueBmp;
        }

        private WriteableBitmap GetFromRGB()
        {
            var r = GetPixels(redBmp);
            var g = GetPixels(greenBmp);
            var b = GetPixels(blueBmp);
            byte[] newArray = new byte[Image.PixelWidth * Image.PixelHeight * 4];
            for (int i = 0; i < newArray.Length; i += 4)
            {
                newArray[i] = b[i];
                newArray[i + 1] = g[i + 1];
                newArray[i + 2] = r[i + 2];
                newArray[i + 3] = r[i + 3];
            }

            var newImg = new WriteableBitmap(Image.PixelWidth, Image.PixelHeight, Image.DpiX, Image.DpiY, PixelFormats.Pbgra32, null);
            newImg.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight), newArray, Image.PixelWidth * 4, 0);
            return newImg;
        }

        public byte[] GetPixels(BitmapSource bitmapImage)
        {
            int height = bitmapImage.PixelHeight;
            int width = bitmapImage.PixelWidth;
            int nStride = (bitmapImage.PixelWidth * bitmapImage.Format.BitsPerPixel + 7) / 8;
            byte[] pixelByteArray = new byte[bitmapImage.PixelHeight * nStride];
            bitmapImage.CopyPixels(pixelByteArray, nStride, 0);
            return pixelByteArray;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            redRange = (byte)e.NewValue;
            Display();
            rgb.Source = GetFromRGB();

        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            greenRange = (byte)e.NewValue;
            Display();
            rgb.Source = GetFromRGB();

        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blueRange = (byte)e.NewValue;
            Display();
            rgb.Source = GetFromRGB();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            rgb.Source = GetFromRGB();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
 
            var dlg = new SaveFileDialog
            {
                Filter = "bmp|*.bmp"
            };
            if (dlg.ShowDialog() == true)
            {
                using var fileStream = new FileStream(dlg.FileName, FileMode.Create);
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(GetFromRGB()));
                encoder.Save(fileStream);
            }
        }
    }
}
