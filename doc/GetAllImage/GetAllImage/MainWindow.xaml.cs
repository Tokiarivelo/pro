using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using PdfUtils;

namespace GetAllImage
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

        private void Navigate_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pdf file| *.pdf";
            bool? dialogOk = fileDialog.ShowDialog();
            string filename = "";
            if (dialogOk == true)
            {
                filename = fileDialog.FileName;
                var path = Path.GetDirectoryName(filename);

                try
                {
                    var images = PdfImageExtractor.ExtractImages(filename);
                    foreach (var name in images.Keys)
                    {
                        if (path != null) images[name].Save(Path.Combine(path, name));
                    }
                    MessageBox.Show("Done");

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
