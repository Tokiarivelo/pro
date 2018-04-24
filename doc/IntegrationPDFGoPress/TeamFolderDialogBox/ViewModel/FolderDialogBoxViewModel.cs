using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using TeamFolderDialogBox.Model;

namespace TeamFolderDialogBox.ViewModel
{
    public class FolderDialogBoxViewModel : ViewModelBase
    {
        public List<Item> GetItems(string path)
        {
            var items = new List<Item>();
            try
            {
                var dirinfo = new DirectoryInfo(path);
                if (root)
                {
                    root = false;
                    items.Add(new DirectoryItem()
                    {
                        Name = dirinfo.Name,
                        Path = dirinfo.FullName,
                        Items = GetItems(dirinfo.FullName)
                    });
                    return items;
                }
                else
                {
                    if (firstchild)
                    {
                        firstchild = false;
                        foreach (var directory in dirinfo.GetDirectories())
                        {
                            var item = new DirectoryItem()
                            {
                                Name = directory.Name,
                                Path = directory.FullName,
                                Items = new List<Item>()
                            };
                            items.Add(item);
                        }
                    }
                    FormatResultCount(items.Count);
                    return items;

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return items;
            }
        }

        private void FormatResultCount(int resultCount)
        {
            if (resultCount > 1)
                Libelle = resultCount + " résulats";
            else Libelle = resultCount + " résulat";
        }
        public FolderDialogBoxViewModel()
        {
            

        }
        public bool InitPath(string path)
        {
            root = true;
            firstchild = true;
            try
            {
                Path = new Item()
                {
                    Name = new DirectoryInfo(path).Name,
                    Path = path
                };
                SelectedDir = new List<string>();
            }
            catch (Exception)
            {
              // ignored
            }
          return true;
        }

        private List<string> selectedDir;
        public List<string> SelectedDir
        {
            get { return selectedDir; }
            set
            {
                selectedDir = value;
                RaisePropertyChanged(() => SelectedDir);
            }
        }
        public ICommand SelectDirCommand
        {
            get
            {
                return new RelayCommand<object[]>((param) =>
                {
                    try
                    {
                        var path = (string)param[0];
                        TreeViewItem treeViewItem = param[1] as TreeViewItem;
                        SelectedItemChanger(treeViewItem, path);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                });
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return new RelayCommand(() =>
               {
                   try
                   {
                       SelectedDir.Clear();
                       Dictionary<TreeViewItem, string>.ValueCollection testDictionary = selectedItems.Values;

                       foreach (var item in testDictionary)
                       {
                           SelectedDir.Add(item);
                       }
                       if (SelectedDir.Count == 0)
                       {
                           SelectedDir.Add(Path.Path);
                       }
                       SelectedDir.ForEach(item =>
                       {
                       //Console.WriteLine(item);
                   });
                       MainWindow.SeletedDirs = SelectedDir;
                       CurentWindows.Close();
                       /*Dialog result modifier*/
                       DialogResult = DialogResult.OK;
                   }
                   catch (Exception exception)
                   {
                       Console.WriteLine(exception.Message);
                   }
               });
            }
        }
        [Browsable(false)]
        public DialogResult DialogResult { get; set; }
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(() =>
               {
                   CurentWindows.Close();
                   DialogResult = DialogResult.Cancel;
               });
            }
        }

        public List<Item> Items => GetItems(Path.Path);

        public Item Path { get; set; }
        public static bool root { get; set; }
        public static bool firstchild { get; set; }

        public DirectoryItem SelectedTreeViewItem
        {
            get { return selectedTreeViewItem; }
            set
            {
                selectedTreeViewItem = value;
                RaisePropertyChanged(() => SelectedTreeViewItem);
            }
        }

        private DirectoryItem selectedTreeViewItem;

        static Dictionary<TreeViewItem, string> selectedItems = new Dictionary<TreeViewItem, string>();

        private void SelectedItemChanger(TreeViewItem treeViewItem, string path)
        {
            try
            {
                if (treeViewItem == null)
                    return;

                // prevent the WPF tree item selection 
                treeViewItem.IsSelected = false;

                treeViewItem.Focus();

                if (!CtrlPressed)
                {
                    List<TreeViewItem> selectedTreeViewItemList = new List<TreeViewItem>();
                    foreach (TreeViewItem treeViewItem1 in selectedItems.Keys)
                    {
                        selectedTreeViewItemList.Add(treeViewItem1);
                    }

                    foreach (TreeViewItem treeViewItem1 in selectedTreeViewItemList)
                    {
                        Deselect(treeViewItem1, path);
                    }
                }

                ChangeSelectedState(treeViewItem, path);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        void Deselect(TreeViewItem treeViewItem, string path)
        {
            treeViewItem.Background = Brushes.Transparent;// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }


        void ChangeSelectedState(TreeViewItem treeViewItem, string path)
        {
            try
            {
                if (!selectedItems.ContainsKey(treeViewItem))
                { // select
                    treeViewItem.Background = (Brush)(new BrushConverter()).ConvertFrom("#5E2AA9F1"); // change background and foreground colors
                    treeViewItem.Foreground = Brushes.Black;
                    selectedItems.Add(treeViewItem, path);
                }
                else
                { // deselect
                    Deselect(treeViewItem, path);

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        bool CtrlPressed
        {
            get
            {
                return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl);
            }
        }
        public Window CurentWindows { get; set; }
        private string _libelle;
        public string Libelle
        {
            get { return _libelle; }
            set
            {
                _libelle = value;
                RaisePropertyChanged(() => Libelle);
            }
        }

        public ICommand AppLoadedCommand
        {
            get
            {
                return  new RelayCommand<Window>((param) =>
                {
                    CurentWindows = param;
                });
            }
        }
    }
}
