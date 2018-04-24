using System;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using FeatureMatchingExample;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            long matchTime;
            long score;
            using (Mat modelImage = CvInvoke.Imread("../../images/im2.png", ImreadModes.Grayscale))
            using (Mat observedImage = CvInvoke.Imread("../../images/im1.jpg", ImreadModes.Grayscale))
            {
                Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime, out score);
                ImageViewer.Show(result, String.Format("Score : {0} matches", score));
            }
        }
    }
}
