using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text;

namespace CropPdfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TrimLeftandRightFoall(
                @"C:\Users\toky\Desktop\Destination\HetNieuwsblad_HetNieuwsbladAntwerpen_20180320_002.pdf",
                @"C:\Users\toky\Desktop\Destination\p.pdf", 50);
        }

        public void TrimLeftandRightFoall(string sourceFilePath, string outputFilePath, float cropwidth)
        {

            PdfReader pdfReader = new PdfReader(sourceFilePath);
            
            float widthTo_Trim = Utilities.MillimetersToPoints(cropwidth);


            




            using (var output = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                // Create a new document
                Document doc = new Document();

                // Make a copy of the document
                PdfSmartCopy smartCopy = new PdfSmartCopy(doc, output);

                // Open the newly created document
                doc.Open();
                
                // Loop through all pages of the source document
                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    // Get a page
                    var page = pdfReader.GetPageN(i);
                    iTextSharp.text.Rectangle cropbox = pdfReader.GetCropBox(i);

                    float width = cropbox.Width;
                    float height = cropbox.Height;
                    PdfRectangle rectLeftside = new PdfRectangle(widthTo_Trim, widthTo_Trim, width - widthTo_Trim, height - widthTo_Trim);

                    page.Put(PdfName.MEDIABOX, rectLeftside);




                    var copiedPage = smartCopy.GetImportedPage(pdfReader, i);
                    smartCopy.AddPage(copiedPage);
                }

                doc.Close();

            }

        }


    }
}
