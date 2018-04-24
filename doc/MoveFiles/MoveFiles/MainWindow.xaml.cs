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
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;

namespace MoveFiles
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
        //-------- function to remove ------------//
        private void Moving(System.Windows.Controls.ListBox filesToMove, string sourceDir, string destinationDir)
        {
            foreach (var files in filesToMove.Items)
            {
                string filesource = sourceDir + "\\" + files;
                string filesout = destinationDir + "\\" + files;

                if (File.Exists(filesource))
                {
                    File.Move(filesource, filesout);
                }
            }

            RefreshSource(SourceParh.Text);
            RefreshOut(OutParh.Text);
            System.Windows.MessageBox.Show("Fichiers déplacés");
        }

        private void MovingSome(System.Windows.Controls.ListBox filesToMove, string sourceDir, string destinationDir)
        {
            foreach (var files in filesToMove.SelectedItems)
            {
                string filesource = sourceDir + "\\" + files;
                string filesout = destinationDir + "\\" + files;

                if (File.Exists(filesource))
                {
                    File.Move(filesource, filesout);
                }
            }

            RefreshSource(SourceParh.Text);
            RefreshOut(OutParh.Text);
            System.Windows.MessageBox.Show("Fichiers déplacés");
        }


        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            Moving(FileMoved, OutParh.Text, SourceParh.Text);
        }

        private void MoveAll_Click(object sender, RoutedEventArgs e)
        {

            Moving(FileToMove, SourceParh.Text, OutParh.Text);
        }

        private void MoveAny_Click(object sender, RoutedEventArgs e)
        {
            MovingSome(FileToMove, SourceParh.Text, OutParh.Text);
        }


        private void Source_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = @"C:\Users\toky\Downloads\";
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                String sPath = folderDialog.SelectedPath;
                RefreshSource(sPath);
            }
            else
                return;

        }

        private void Out_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = true;
            folderDialog.SelectedPath = @"C:\Users\toky\Desktop\out\";
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                String sPath = folderDialog.SelectedPath;
                RefreshOut(sPath);
            }
            else
                return;
        }

        private void RefreshSource(string path)
        {
            FileToMove.Items.Clear();
            DirectoryInfo folder = new DirectoryInfo(path);
            SourceParh.Text = folder.ToString();
            if (folder.Exists)
            {
                foreach (FileInfo fileInfo in folder.GetFiles())
                {
                    FileToMove.Items.Add(fileInfo.Name);
                }

            }
        }
        private void RefreshOut(string path)
        {
            FileMoved.Items.Clear();
            DirectoryInfo folder = new DirectoryInfo(path);
            OutParh.Text = folder.ToString();
            if (folder.Exists)
            {
                foreach (FileInfo fileInfo in folder.GetFiles())
                {
                    FileMoved.Items.Add(fileInfo.Name);
                }

            }
        }
    }
}
