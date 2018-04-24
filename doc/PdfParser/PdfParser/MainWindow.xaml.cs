using System;
using System.Text;
using System.Windows;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using iTextSharp.text.pdf;
using ceTe.DynamicPDF.Rasterizer;

namespace PdfParser
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pdf file| *.pdf";
            bool? dialogOk = fileDialog.ShowDialog();
            string filename = "";
            if (dialogOk == true)
            {
                filename = fileDialog.FileName;

                try
                {
                    string strText = "";
                    PdfReader reader = new PdfReader(filename);
                    for(int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        ITextExtractionStrategy its = new LocationTextExtractionStrategy();
                        string s = PdfTextExtractor.GetTextFromPage(reader, page, its);
                        s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                        strText += s;
                        ContentPdfItext.Text = strText;
                    }

                    reader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
                return;
        }

        private void OcrButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pdf file| *.pdf";
            bool? dialogOk = fileDialog.ShowDialog();
            string filename = "";
            if (dialogOk == true)
            {
                filename = fileDialog.FileName;
                var path = System.IO.Path.GetDirectoryName(filename);

                try
                {
                    using (PdfRasterizer rasterizer = new PdfRasterizer(filename))
                    {
                        rasterizer.Draw(path + "/" + System.IO.Path.GetFileNameWithoutExtension(filename) + ".jpg", ImageFormat.Jpeg, ImageSize.Dpi72);
                        rasterizer.Dispose();
                        MessageBox.Show("Conversion terminé!!");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
                return;
        }
    }
}
