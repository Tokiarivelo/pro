using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Data;
using System.Windows.Input;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Compare.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private DataTable _datagridsData;
        private string _newText;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            NewText = "MyString";
            Compare = new RelayCommand<DataGrid>(CompareString);
        }
        public ICommand Compare { get; set; }
        private void CompareString(DataGrid MyGrid)
        {
            DatagridsData = new DataTable();

            var charstring = new DataGridTextColumn();
            charstring.Header = "Strings";
            charstring.Width = 50;
            MyGrid.Columns.Add(charstring);
            var none = new DataGridTextColumn();
            none.Header = "0";
            none.Width = 50;
            MyGrid.Columns.Add(none);

            DataGridTextColumn[] colName = new DataGridTextColumn[NewText.Length];
            var i = 0;
            foreach (var c in NewText)
            {
                colName[i] = new DataGridTextColumn();
                colName[i].Header = c.ToString();
                colName[i].Width = 50;
                MyGrid.Columns.Add(colName[i]);
                i++;
            }
            

        }

        public DataTable DatagridsData
        {
            get { return _datagridsData; }
            set
            {
                _datagridsData = value;
                RaisePropertyChanged(() => DatagridsData);
            }
        }
        public string NewText
        {
            get { return _newText; }
            set
            {
                _newText = value;
                RaisePropertyChanged(() => NewText);
            }
        }
    }
}