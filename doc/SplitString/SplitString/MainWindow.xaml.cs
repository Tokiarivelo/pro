using System;
using System.Collections.Generic;
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

namespace SplitString
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            String tgt = "MM_MMoiJ_en_VeuxAussi_I_need_a";
            List<string> StringSplited = new List<string>();
            string res = "";

            string temp = tgt[0].ToString();
            var i = 1;
            do
            {
                while (i < tgt.Length && tgt[i - 1] == Char.ToUpper(tgt[i - 1]) && tgt[i] == Char.ToUpper(tgt[i]) && tgt[i] != '_')
                {
                    temp += tgt[i];
                    i++;
                }

                while (i < tgt.Length && tgt[i] != Char.ToUpper(tgt[i]) && tgt[i] != '_')
                {
                    temp += tgt[i];
                    i++;
                }

                if (temp[0] == '_')
                    temp = temp.Remove(0,1);
                StringSplited.Add(temp);
                if (i < tgt.Length)
                {
                    temp = tgt[i].ToString();
                    i++;
                    if (i == tgt.Length && tgt[tgt.Length - 1] != '_')
                        StringSplited.Add(tgt[tgt.Length-1].ToString());
                }
                    

            } while (i < tgt.Length);


            foreach (var show in StringSplited)
            {
                //StringSplit.Content += "\n" + show;
                res = res + " " + show;
            }
            MessageBox.Show(res);
        }

        

        private void Splitter_Click(object sender, RoutedEventArgs e)
        {
            //tgt = StringToSplit.Text;
            
        }
    }
}
