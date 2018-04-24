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
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace EditPdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var o = @"C:\Users\toky\Desktop\temp\LeSoir_LeSoirBruxellesBrabant_20180202_032.pdf";
            var n = @"C:\Users\toky\Desktop\temp\result.pdf";
            var i = @"C:\Users\toky\Desktop\Capture.PNG";
            SetText(o,n);
            //SetImage(o, n, i);
        }

        private void SetText(string oldF, string newF)
        {
            string oldFile =oldF;
            string newFile = newF;

            // open the reader
            PdfReader reader = new PdfReader(oldFile);
            iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);
            document.SetPageSize(PageSize.A4);
            // open the writer
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            
            // the pdf content
            PdfContentByte cb = writer.DirectContent;


            // select the font properties
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.DARK_GRAY);
            cb.SetFontAndSize(bf, 8);

            // write the text in the pdf content
            cb.BeginText();
            string text = "Some random blablablabla...";
            // put the alignment and coordinates here
            cb.ShowTextAligned(1, text, 30, 30, 0);
            cb.EndText();
            cb.BeginText();
            text = "Other random blabla...Toky";
            // put the alignment and coordinates here
            cb.ShowTextAligned(2, text, 100, 200, 0);
            cb.EndText();

            // create the new page and add it to the pdf
            //PdfImportedPage page = writer.GetImportedPage(reader, 1);
            //cb.AddTemplate(page, 0, 0);

            PdfImportedPage page2 = writer.GetImportedPage(reader, 1);
            float Scale = 0.56f;
            cb.AddTemplate(page2, Scale, 0, 0, Scale, 0, 0);

            // close the streams and voilá the file should be changed :)
            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();
        }

        private void SetImage(string oldF, string newF, string img)
        {
            using (Stream inputPdfStream = new FileStream(oldF, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream inputImageStream = new FileStream(img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(newF, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new PdfReader(inputPdfStream);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(1);

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
                image.SetAbsolutePosition(1, 1);
                pdfContentByte.AddImage(image);
                stamper.Close();
            }
        }

    }
}
