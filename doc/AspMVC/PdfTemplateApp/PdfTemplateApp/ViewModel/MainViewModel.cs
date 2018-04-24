using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WebBrowser = System.Windows.Controls.WebBrowser;

using mshtml;

namespace PdfTemplateApp.ViewModel
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
        private string _pathPdf;
        private WebBrowser _browser;
        public HTMLDocument Doc;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GetPdf = new RelayCommand<WebBrowser>(BrowsePdf);
            GetImage = new RelayCommand<WebBrowser>(BrowseImg);
            Initializer = new RelayCommand<WebBrowser>(WebBrowserProperty);
        }

        private void BrowseImg(WebBrowser obj)
        {
            PathPdf = GetFile(@"png file|*.png| jpg file|*.jpg|jpeg file|*.jpeg| gif file|*.gif| All| *.*", @"*.png");
            if (PathPdf != null)
            {
                if (Doc != null)
                {
                    dynamic r = Doc.selection.createRange();
                    r.pasteHTML($@"<img alt=""{"Image"}"" src=""{PathPdf}""/>");
                }
            }
        }

        private void BrowsePdf(WebBrowser obj)
        {
            PathPdf = GetFile(@"Pdf file|*.pdf| All| *.*", @"*.pdf");
            if (PathPdf != null)
            {
                if (Doc != null)
                {
                    dynamic r = Doc.selection.createRange();
                    //r.pasteHTML($@"<object type=""{"application/pdf"}"" data=""{PathPdf}"">
                    //           <embed  type=""{"application/pdf"}"" src=""{PathPdf}""/>
                    //    </object>
                    //");

                }
                //obj.Navigate(PathPdf);
            }
                
        }

        private void WebBrowserProperty(WebBrowser obj)
        {
            if (obj != null)
            {
                 _browser = obj;
                _browser.NavigateToString(Properties.Resources.New);
                Doc = _browser.Document as HTMLDocument;
                if (Doc != null) Doc.designMode = "On";

            }
           
        }
        private string GetFile(string files, string defaultFile)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = files;
            fileDialog.DefaultExt = defaultFile;
            var dialogOk = fileDialog.ShowDialog();
            if (dialogOk == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return null;
        }


        public string PathPdf
        {
            get { return _pathPdf; }
            set
            {
                _pathPdf = value;
                RaisePropertyChanged(() => PathPdf);
            }
        }

        public RelayCommand<WebBrowser> GetPdf { get; set; }
        public RelayCommand<WebBrowser> GetImage { get; set; }
        public RelayCommand<WebBrowser> Initializer { get; set; }

        
    }
}