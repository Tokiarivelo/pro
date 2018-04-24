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
using GroupDocs.Comparison;
using GroupDocs.Comparison.Common;
using GroupDocs.Comparison.Common.Changes;
using GroupDocs.Comparison.Common.ComparisonSettings;

namespace GroupDocCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
            if (FileCompare(@"C:\Users\toky\Documents\GestionConge.pdf", @"C:\Users\toky\Documents\Axure\GestionConge.pdf"))
            {
                MessageBox.Show("Files are equal.");
            }
            else
            {
                MessageBox.Show("Files are not equal.");
            }
        }
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.

            //if (file1 == file2)
            //{
            //    // Return true to indicate that the files are the same.
            //    return true;
            //}

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            //if (fs1.Length != fs2.Length)
            //{
            //    // Close the file
            //    fs1.Close();
            //    fs2.Close();

            //    // Return false to indicate files are different
            //    return false;
            //}

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            var string1 = "";
            var string2 = "";

            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();

                string1 += " " + file1byte;
                string2 += " " + file2byte;
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            MessageBox.Show(string1);
            MessageBox.Show(string2);
            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }

    }
}
