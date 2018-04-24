using System.IO;
using System.Windows;

namespace PdfExtract
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (var pdfStream = File.OpenRead(@"C:\Users\toky\Documents\Autogids_Autogids_20180131_008.pdf"))
            using (var extractor = new Extractor())
            {
                var extractedText = extractor.ExtractToString(pdfStream);

                MessageBox.Show(extractedText);
            }
        }
    }
}
