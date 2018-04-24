using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeamFolderDialogBox.ViewModel;

namespace TeamFolderDialogBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderDialogBoxViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            OKButton.Focus();
        }

        public bool Init(string path)
        {
            vm=new FolderDialogBoxViewModel();
            vm.InitPath(path);
            DataContext = vm;

            return true;
        }
        public static List<string> SeletedDirs { get; set; }
        public string Path { get; set;}


        public  DialogResult ShowFolderDialog()
        {
            this.ShowDialog();
            return vm.DialogResult;
        }
    }
}
