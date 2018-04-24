using System.Drawing;
using System.Windows;
using tessnet2;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var image = new Bitmap(@"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180212_001.jpg");
            var ocr = new Tesseract();
            ocr.Init(@"C:\Users\toky\Downloads\tesseract-1.03\tesseract-1.03\tessdata","eng",false);
            var result = ocr.DoOCR(image, Rectangle.Empty);
            foreach (var word in result)
            {
                MessageBox.Show(word.Text);

            }
        }
    }
}
