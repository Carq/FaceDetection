using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FaceDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VideoCapture _videoCapture;

        private readonly CascadeClassifier _cascadeClassifier;

        private readonly CascadeClassifier _eyeClassifier;

        private readonly Bgr red = new Bgr(Color.Red);

        private readonly Bgr green = new Bgr(Color.Green);

        public MainWindow()
        {
            _videoCapture = new VideoCapture();
            _cascadeClassifier = new CascadeClassifier("Classifiers/haarcascade_frontalface_default.xml");
            _eyeClassifier = new CascadeClassifier("Classifiers/haarcascade_eye.xml");

            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 10)
            };

            timer.Start();
            timer.Tick += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            var frame = _videoCapture.QueryFrame();
            var grayImage = frame?.ToImage<Gray, byte>();
            var imageFrame = frame?.ToImage<Bgr, byte>();

            var faces = _cascadeClassifier.DetectMultiScale(imageFrame, 1.1, 10);
            var eyes = _eyeClassifier.DetectMultiScale(grayImage, 1.1, 8);

            foreach (var face in faces)
            {
                imageFrame.Draw(face, red);
            }

            foreach (var eye in eyes)
            {
                imageFrame.Draw(eye, green);
            }


            image.Source = Convert(imageFrame.ToBitmap());
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
