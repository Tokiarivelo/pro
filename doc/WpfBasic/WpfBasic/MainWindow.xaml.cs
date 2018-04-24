using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfBasic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion
        #region On loaded
        //--- first open -----//
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //get every logical in the machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                //header and full path
                var item = new TreeViewItem {Header = drive, Tag = drive};

                //add a dummy item
                item.Items.Add(null);

                //listen out for item expanded
                item.Expanded += Folder_Expanded;

                FolderView.Items.Add(item);
            }
        }
        #region folder Expanded
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initialize check
            var item = (TreeViewItem)sender;

            //if the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;
            item.Items.Clear();

            //get full path
            var fullPath = (string)item.Tag;
            #endregion

            #region Get directories
            var directories = new List<string>();
            try
            {
                var dir = Directory.GetDirectories(fullPath);
                if (dir.Length > 0)
                    directories.AddRange(dir);
            }
            catch (Exception)
            {
                // ignored
            }

            //for each directories....
            directories.ForEach(directoryPath =>
            {
                var subitem = new TreeViewItem
                {
                    //header as folder name
                    Header = GetFileFolderName(directoryPath),
                    //tag for full path
                    Tag = directoryPath

                };
                //dummy item for expanding folder
                subitem.Items.Add(null);

                //Handle expanding
                subitem.Expanded += Folder_Expanded;

                //add folder to this parents
                item.Items.Add(subitem);
            });
            #endregion

            #region Get files
            var files = new List<string>();
            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch (Exception)
            {
                // ignored
            }

            //for each file....
            files.ForEach(filePath =>
            {
                var subitem = new TreeViewItem
                {
                    //header as file name
                    Header = GetFileFolderName(filePath),
                    //tag for full path
                    Tag = filePath

                };

                //add folder to this parents
                item.Items.Add(subitem);
            });
            #endregion


        }
        #endregion


        public static string GetFileFolderName(string path)
        {
            // c:\something\a folder
            // c:\something\a file.png
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            //replace all / into \
            var normalizePath = path.Replace('/', '\\');

            var lastIndex = normalizePath.LastIndexOf('\\');

            if (lastIndex <= 0)
                return path;

            //Returns substring of all chars after first lastIndex + 1
            return path.Substring(lastIndex + 1);
        }

        #endregion


    }
}
