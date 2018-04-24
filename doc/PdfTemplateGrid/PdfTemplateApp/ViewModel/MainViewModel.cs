using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ImageMagick;
using PdfTemplateApp.Utils;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Forms.Cursors;
using Font = System.Drawing.Font;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using TextBox = System.Windows.Controls.TextBox;

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
        private double _lambda = 1;
        private string _pathPdf;
        private Canvas _pdfBrowser;
        private Canvas _headeBrowser;
        private Window _mainWindow;
        private Canvas _selected;
        private int _i = 0;
        private int _actualIndex;
        private int _totalPage = 0;
        private List<PagePropertyClass> _pdfImages = new List<PagePropertyClass>();
        private bool _enableAddPdf;
        private ScrollViewer _scrollViewer;
        private Border _borderHead;
        private Border _borderBody;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            EnableAddPdf = true;
            //************* Control **************//
            GetPdf = new RelayCommand(BrowsePdf);
            GetImage = new RelayCommand(BrowseImg);
            PutText = new RelayCommand(PutTextInto);
            SavePdf = new RelayCommand(SavePdfContent);

            Initializer = new RelayCommand<Canvas>(WebBrowserProperty);
            GetBorders = new RelayCommand<Border>(BorderProperties);
            GetWindow = new RelayCommand<Window>(GetMainWindow);
            GetTScrollView = new RelayCommand<ScrollViewer>(GetTScrollViewPdf);


            SelectedElement = new RelayCommand<Canvas>(GetSelectedElement);
            NextPage = new RelayCommand(NextPagePdf);
            PreviousPage = new RelayCommand(PreviousPagePdf);
        }

        private void SavePdfContent()
        {
            if (PathPdf != null)
            {
                var pdfName = Path.GetFileNameWithoutExtension(PathPdf);
                var dir = Path.GetDirectoryName(PathPdf);
                if (dir != null)
                {
                    var dirResult = Directory.CreateDirectory(Path.Combine(dir, "Result", pdfName));
                    var pdfResult = $@"{dirResult.FullName}\{pdfName}.pdf";
                    SetText(PathPdf, pdfResult);
                }
            }
        }
        private void SetText(string oldF, string newF)
        {
            try
            {
                byte[] bytes;
                string oldFile = oldF;
                string newFile = newF;
                // open the writer
                //FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                using (MemoryStream fs = new MemoryStream())
                {

                    // open the reader
                    using (PdfReader reader = new PdfReader(oldFile))
                    {
                        Rectangle size = reader.GetPageSizeWithRotation(1);
                        using (Document document = new Document(size))
                        {
                            document.SetPageSize(PageSize.A4);

                            using (PdfWriter writer = PdfWriter.GetInstance(document, fs))
                            {
                                document.Open();

                                // the pdf content
                                PdfContentByte cb = writer.DirectContent;

                                // select the font properties
                                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                cb.SetColorFill(BaseColor.DARK_GRAY);
                                cb.SetFontAndSize(bf, 8);

                                var yOriginBody = document.PageSize.Top - 123 * _lambda;
                                var xOriginBody = 5 * _lambda;
                                var xMaxBody = document.PageSize.Right;
                                var yMaxBody = document.PageSize.Bottom;




                                // create the new page and add it to the pdf
                                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                                float Scale = 0.55f;
                                cb.AddTemplate(page, Scale, 0, 0, Scale, 0, 0); //e, f : position


                                // write the text in the pdf content : body
                                foreach (var child in _pdfBrowser.Children)
                                {
                                    if (child.GetType() == typeof(System.Windows.Controls.Label))
                                    {
                                        var label = child as System.Windows.Controls.Label;

                                        if (label != null)
                                        {
                                            cb.BeginText();
                                            string text = label.Content.ToString();

                                            // put the alignment and coordinates here
                                            var xpdf = (float)((xOriginBody - xMaxBody) / (-785 * _lambda) * Canvas.GetLeft(label) + xOriginBody);
                                            var ypdf = (float)((yOriginBody - yMaxBody) / (-955 * _lambda) * Canvas.GetTop(label) + yOriginBody);

                                            //document.PageSize.Right- sz.Width top right
                                            cb.ShowTextAligned(Element.ALIGN_LEFT, text, xpdf, ypdf, 0);
                                            cb.EndText();
                                        }

                                    }
                                }
                                // close the streams and voilá the file should be changed :)
                                document.Close();
                                writer.Close();
                            }

                        }
                        //fs.Close();
                        reader.Close();
                    }
                    bytes = fs.ToArray();
                    fs.Close();
                }
                using (FileStream ms = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {
                    //Bind a reader to the bytes that we created above
                    using (var rdr = new PdfReader(bytes))
                    {
                        Rectangle mediabox = rdr.GetPageSize(1);

                        var xOriginBody = 0;
                        //Bind a stamper to our reader
                        using (var stamper = new PdfStamper(rdr, ms))
                        {
                            var pdfContentByte = stamper.GetOverContent(1);
                            foreach (var child in _pdfBrowser.Children)
                            {
                                if (child.GetType() == typeof(Image))
                                {
                                    var img = child as Image;
                                    if (img != null && !Equals(img, _pdfImages[_actualIndex].Page))
                                    {
                                        var p = img.Source.ToString();
                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new Uri(p).LocalPath);

                                        image.ScalePercent(0.75f * 100);

                                        var xpdf = (float)((xOriginBody - mediabox.Width) / (-785 * _lambda) * Canvas.GetLeft(img) + xOriginBody);
                                        //var ypdf = (float)(((970 - (img.Height + Canvas.GetTop(img))) * mediabox.Height / 1108));
                                        var ypdf = (float)(((980 - (img.Height + Canvas.GetTop(img))) * mediabox.Height / 1165));

                                        image.SetAbsolutePosition(xpdf, ypdf);
                                        pdfContentByte.AddImage(image);
                                        //stamper.Close();
                                    }

                                }
                            }


                        }
                    }
                }
                MessageBox.Show("Saved");
            }
            catch (Exception e)
            {
                MessageBox.Show("le fichier est encore ouvert dans un autre processus");
            }

        }
        private void GetSelectedElement(Canvas obj)
        {
            if (obj != null && obj.Name == "PdfViewer")
            {
                _selected = obj;
                EnableAddPdf = true;
                //System.Windows.MessageBox.Show(obj.Name);
            }
            if (obj != null && obj.Name == "HeaderViewer")
            {
                _selected = obj;
                EnableAddPdf = false;
                //System.Windows.MessageBox.Show(obj.Name);
            }
        }

        private void PreviousPagePdf()
        {
            if (PageIndex > 0)
            {
                PageIndex--;
            }
        }

        private void NextPagePdf()
        {
            if (PageIndex < _totalPage-1)
            {
                PageIndex++;
            }
           
        }

        private void BrowseImg()
        {
            
            var pathImg = GetFile(@"png file|*.png| jpg file|*.jpg|jpeg file|*.jpeg| gif file|*.gif", @"*.png");
            if (pathImg != null)
            {
                BrowseImg(_selected, pathImg);
            }
        }
        private void PutTextInto()
        {
            var tempText = GetText(null, true);
            Canvas.SetTop(tempText, 0);
            Canvas.SetLeft(tempText, 0);
            _selected.Children.Add(tempText);
            tempText.Focus();
        }
        private TextBox GetText(System.Windows.Controls.Label labeltext, bool newLabel)
        {
            var textBoxtemp = new TextBox
            {
                Text = labeltext != null ? labeltext.Content.ToString() : null,
                MinWidth = 2,
                MinHeight = 2,
                Background = Brushes.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(0),
                Focusable = true,
                FontFamily = new FontFamily("Helvetica")
            };
            textBoxtemp.TextChanged += (sender, args) =>
            {
                Font arialBold = new Font("Helvetica", 11.0F);
                System.Drawing.Size size = TextRenderer.MeasureText(textBoxtemp.Text, arialBold);
                textBoxtemp.Width = size.Width;
            };
            var already = false;
            textBoxtemp.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    if (newLabel)
                    {
                        PutTextOption(_selected, textBoxtemp);
                    }
                    else
                    {
                        if (labeltext != null)
                        {
                            labeltext.Visibility = Visibility.Visible;
                            labeltext.Content = textBoxtemp.Text;
                        }
                    }
                    _selected.Children.Remove(textBoxtemp);
                    already = true;
                }
            };

            textBoxtemp.LostFocus += (s, e) =>
            {
                if (!already)
                {
                    if (newLabel)
                    {
                        PutTextOption(_selected, textBoxtemp);
                    }
                    else
                    {
                        //if (labeltext != null) labeltext.Visibility = Visibility.Visible;
                    }
                    // _selected.Children.Remove(textBoxtemp);
                    
                } 
            };
            return textBoxtemp;
        }
        private void PutTextOption(Canvas obj, TextBox temptext)
        {
            //Canvas.SetTop(temptext, 1);
            //Canvas.SetLeft(temptext, 1);
            Point mouseDownLocation = new Point();
            System.Windows.Controls.Label newText = new System.Windows.Controls.Label
            {
                Content = temptext.Text,
                Focusable = true,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                FontSize = 11,
                Visibility = Visibility.Visible
            };
            newText.FontSize *= _lambda;

            Canvas.SetTop(newText, Canvas.GetTop(temptext));
            Canvas.SetLeft(newText, Canvas.GetLeft(temptext));
            var drag = false;
            var edit = false;
            AdornerLayer myAdornerLayer = null;
            newText.MouseLeftButtonDown += (s, e) =>
            {
                edit = false;
                myAdornerLayer = AdornerLayer.GetAdornerLayer(newText);
                myAdornerLayer.Add(new AdornerClass(newText));
                Keyboard.Focus(newText);
                drag = true;
                mouseDownLocation = e.GetPosition(obj); //position % au canvas
                mouseDownLocation.Y -= (double)newText.GetValue(Canvas.TopProperty);
                mouseDownLocation.X -= (double)newText.GetValue(Canvas.LeftProperty);
                //System.Windows.MessageBox.Show(mouseDownLocation.ToString());
            };
            newText.MouseMove += (sender, args) =>
            {
                System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
            };
            newText.MouseLeftButtonUp += (s, e) =>
            {
                drag = false;
                //MessageBox.Show(Canvas.GetLeft(newText) + "," + Canvas.GetTop(newText));
            };
            newText.MouseDoubleClick += (s, e) =>
            {

                if (edit)
                {
                    newText.Visibility = Visibility.Collapsed;
                    var temp = GetText(newText, false);
                    Canvas.SetTop(temp, Canvas.GetTop(newText));
                    Canvas.SetLeft(temp, Canvas.GetLeft(newText));
                    _selected.Children.Add(temp);
                    Keyboard.Focus(temp);
                    temp.Focus();
                }
                //Canvas.SetTop(temp, Canvas.GetTop(newText));
                //Canvas.SetLeft(temp, Canvas.GetLeft(newText));
            };

            obj.MouseMove += (sender, args) =>
            {
                if (drag)
                {
                    //Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(newText);
                    //Adorner toRemove;
                    //if (toRemoveArray != null)
                    //{
                    //    toRemove = toRemoveArray[0];
                    //    myAdornerLayer.Remove(toRemove);
                    //}

                    var movelocation = args.GetPosition(obj); //position % à l'image
                    if (movelocation.X >= 0 && movelocation.Y >= 0)
                    {
                        Canvas.SetTop(newText, movelocation.Y - mouseDownLocation.Y);
                        Canvas.SetLeft(newText, movelocation.X - mouseDownLocation.X);
                       
                    }

                }
            };
            newText.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Delete)
                {
                    _selected.Children.Remove(newText);
                }
            };

            _selected.Children.Add(newText);

        }
        private void BrowseImg(Canvas obj, string pathImg)
        {
            Point mouseDownLocation = new Point();
            _i++;
            System.Drawing.Image im = System.Drawing.Image.FromFile(pathImg);
            Image img = new Image
            {
                AllowDrop = true,
                Focusable = true,
                Name = "img" + _i,
                Source =
                    new BitmapImage(new Uri(pathImg)),
                Width = im.Width*_lambda,
                Height = im.Height*_lambda

            };

            Canvas.SetTop(img, 0);
            Canvas.SetLeft(img, 0);
            var drag = false;
            img.MouseLeftButtonDown += (s, e) =>
            {
                Keyboard.Focus(img);
                AdornerLayer.GetAdornerLayer(img).Add(new AdornerClass(img));
                //System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
                drag = true;
                mouseDownLocation = e.GetPosition(obj); //position % au canvas
                mouseDownLocation.Y -= (double)img.GetValue(Canvas.TopProperty);
                mouseDownLocation.X -= (double)img.GetValue(Canvas.LeftProperty);
                //System.Windows.MessageBox.Show(mouseDownLocation.ToString());
            };
            img.MouseLeftButtonUp += (s, e) =>
            {
                drag = false;
            };
            img.MouseMove += (sender, args) =>
            {
                System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
            };

            obj.MouseMove += (sender, args) =>
            {
                if (drag)
                {
                    //System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
                    var movelocation = args.GetPosition(obj); //position % à l'image
                    if (movelocation.X >= 0 && movelocation.Y >= 0)
                    {
                        Canvas.SetTop(img, movelocation.Y - mouseDownLocation.Y);
                        Canvas.SetLeft(img, movelocation.X - mouseDownLocation.X);
                    }

                }
            };
            
            img.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Delete)
                {
                   _selected.Children.Remove(img);
                }
            };
            //img.MouseWheel += (s, e) =>
            //{
            //    if (!drag)
            //    {
            //        Canvas.SetTop(img, 0);
            //        Canvas.SetLeft(img, 0);
            //        if (e.Delta < 0 && img.Width > im.Width / 7 && img.Height > im.Height / 7)
            //        {
            //            img.Width /= 1.05;
            //            img.Height /= 1.05;
            //        }
            //        if (e.Delta > 0 && img.Width < im.Width * 5 && img.Height < im.Height * 5)
            //        {
            //            img.Width *= 1.05;
            //            img.Height *= 1.05;
            //        }
            //    }
            //};

            _selected.Children.Add(img);
        }

        

        private void BrowsePdf()
        {
             
            _pdfImages.Clear();
            PathPdf = GetFile(@"Pdf file|*.pdf", @"*.pdf");
            if (PathPdf != null)
            {
                
                //******************** create images for view ****************//
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                    MagickReadSettings settings = new MagickReadSettings();
                    // Settings the density to 300 dpi will create an image with a better quality
                    settings.Density = new Density(150, 150);
                    // Add all the pages of the pdf file to the collection
                    images.Read(PathPdf, settings);

                    var pdfName = Path.GetFileNameWithoutExtension(PathPdf);
                    var dir = Path.GetDirectoryName(PathPdf);
                    if (dir != null)
                    {
                        var dirtemp = Directory.CreateDirectory(Path.Combine(dir, "temp", pdfName));

                        int page = 1;
                        foreach (var magickImage in images)
                        {
                            var image = (MagickImage) magickImage;
                            image.Format = MagickFormat.Jpeg;
                            // Write page to file that contains the page number
                            image.Write($@"{dirtemp.FullName}\" + page + ".jpeg");
                            page++;
                        }

                        DirectoryInfo folderTemp = new DirectoryInfo(dirtemp.FullName);
                        foreach (var files in folderTemp.GetFiles("*.jpeg"))
                        {
                            var filepath = Path.Combine(dirtemp.FullName, files.Name);
                            System.Drawing.Image im = System.Drawing.Image.FromFile(filepath);
                            double w = im.Width;
                            double h = im.Height;
                            while (w  > _pdfBrowser.Width || h > _pdfBrowser.Height)
                            {
                                w /= 1.02;
                                h /= 1.02;
                            }

                            _pdfImages.Add( new PagePropertyClass { Page = new Image
                            {
                                AllowDrop = true,
                                Focusable = true,
                                Source = new BitmapImage(new Uri(filepath)),
                                Width = w,
                                Height = h
                            }, XPoint = 0, YPoint = 0});
                            im.Dispose();
                        }
                    }
                    _totalPage = _pdfImages.Count;
                }
                PageIndex = 0;
                System.Windows.Forms.Cursor.Current = Cursors.Default;

            }
                
        }

        private void ShowPdfAtPage(int index)
        {
            Point mouseDownLocation = new Point();
            //afficher le pdf
            //var height = _pdfImages[index].Page.Height;
            //var width = _pdfImages[index].Page.Width;

            Canvas.SetTop(_pdfImages[index].Page, _pdfImages[index].YPoint);
            Canvas.SetLeft(_pdfImages[index].Page, _pdfImages[index].XPoint);
            var drag = false;
            _pdfImages[index].Page.MouseLeftButtonDown += (s, e) =>
            {
                System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
                drag = true;
                mouseDownLocation = e.GetPosition(_pdfBrowser); //position % au canvas
                mouseDownLocation.Y -= (double)_pdfImages[index].Page.GetValue(Canvas.TopProperty);
                mouseDownLocation.X -= (double)_pdfImages[index].Page.GetValue(Canvas.LeftProperty);
                //System.Windows.MessageBox.Show(mouseDownLocation.ToString());
            };
            _pdfImages[index].Page.MouseLeftButtonUp += (s, e) =>
            {
                drag = false;
            };
            _pdfImages[index].Page.MouseMove += (sender, args) =>
            {
                System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
            };
            _pdfImages[index].Page.MouseEnter += (sender, args) =>
            {
                System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
            };
            //_pdfImages[index].Page.MouseWheel += (s, e) =>
            //{
            //    if (!drag)
            //    {
            //        Canvas.SetTop(_pdfImages[index].Page, 0);
            //        Canvas.SetLeft(_pdfImages[index].Page, 0);
            //        if (e.Delta < 0 && _pdfImages[index].Page.Width * _pdfImages[index].Page.Height > _pdfBrowser.Width*_pdfBrowser.Height)
            //        {
            //            _pdfImages[index].Page.Width /= 1.02;
            //            _pdfImages[index].Page.Height /= 1.02;
            //        }
            //        if (e.Delta > 0 && _pdfImages[index].Page.Width < width * 5 && _pdfImages[index].Page.Height < height * 5)
            //        {
            //            _pdfImages[index].Page.Width *= 1.02;
            //            _pdfImages[index].Page.Height *= 1.02;
            //        }
            //    }
            //};


            _pdfBrowser.MouseMove += (sender, args) =>
            {
                if (drag)
                {
                    System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
                    var movelocation = args.GetPosition(_pdfBrowser); //position % à l'image
                    if (movelocation.X >= 0 && movelocation.Y >= 0)
                    {
                        _pdfImages[index].YPoint = movelocation.Y - mouseDownLocation.Y;
                        _pdfImages[index].XPoint = movelocation.X - mouseDownLocation.X;
                        Canvas.SetTop(_pdfImages[index].Page, _pdfImages[index].YPoint);
                        Canvas.SetLeft(_pdfImages[index].Page, _pdfImages[index].XPoint);
                    }

                }
            };

            foreach (var chld in _selected.Children)
            {
                if (chld.GetType() != typeof(TextBox))
                {
                    if (chld.GetType() != typeof(System.Windows.Controls.Label))
                    {
                        var c = chld as System.Windows.Controls.Label;
                        _selected.Children.Remove(c);
                    }
                    if (chld.GetType() != typeof(Image))
                    {
                        var c = chld as Image;
                        _selected.Children.Remove(c);
                    }
                }
            }
           
            _selected.Children.Add(_pdfImages[index].Page);
        }

        private void WebBrowserProperty(Canvas obj)
        {
            if (obj != null && obj.Name == "PdfViewer")
            {
                _pdfBrowser = obj;
                _selected = obj;
            }
            if (obj != null && obj.Name == "HeaderViewer")
            {
                _headeBrowser = obj;
            }

        }
        private void BorderProperties(Border obj)
        {
            if (obj != null && obj.Name == "BorderHead")
            {
                _borderHead = obj;
            }
            if (obj != null && obj.Name == "BorderBody")
            {
                _borderBody = obj;
            }
        }

        private void GetMainWindow(Window obj)
        {
            if (obj != null && obj.Name == "Main")
            {
                _mainWindow = obj;
                var resizer = new WindowResizer.WindowResizer(_mainWindow);
               
            }

        }
        private void GetTScrollViewPdf(ScrollViewer obj)
        {
            if (obj != null && obj.Name == "ScrollViewerPdf")
            {
                var initialWidth = _pdfBrowser.Width;
                var initialHeight = _pdfBrowser.Height;
                _scrollViewer = obj;
                Keyboard.Focus(_scrollViewer);
                var canScroll = true;
                _scrollViewer.KeyDown += (s, e) =>
                {
                    if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                    {
                        //System.Windows.MessageBox.Show(e.Key.ToString());
                        canScroll = false;
                    }
                };
                _scrollViewer.KeyUp += (s, e) =>
                {
                    canScroll = true;
                };

                _scrollViewer.PreviewMouseWheel += (s, e) =>
                {
                    if (!canScroll)
                    {
                        e.Handled = true;
                        if (e.Delta < 0 && _pdfBrowser.Width > initialWidth / 7 && _pdfBrowser.Height > initialHeight / 7)
                        {
                            _lambda /= 1.05;
                            _pdfBrowser.Width /= 1.05;
                            _pdfBrowser.Height /= 1.05;
                            _headeBrowser.Width /= 1.05;
                            _headeBrowser.Height /= 1.05;

                            _borderBody.Height = _pdfBrowser.Height + 2;
                            _borderBody.Width = _pdfBrowser.Width + 2;
                            _borderHead.Height  = _headeBrowser.Height + 2;
                            _borderHead.Width = _headeBrowser.Width + 2;

                            ZoomOut(_pdfBrowser);
                            ZoomOut(_headeBrowser);
                        }
                        if (e.Delta > 0 && _pdfBrowser.Width < initialWidth * 5 && _pdfBrowser.Height < initialHeight * 5)
                        {
                            _lambda *= 1.05;
                            _pdfBrowser.Width *= 1.05;
                            _pdfBrowser.Height *= 1.05;
                            _headeBrowser.Width *= 1.05;
                            _headeBrowser.Height *= 1.05;

                            _borderBody.Height = _pdfBrowser.Height + 2;
                            _borderBody.Width = _pdfBrowser.Width + 2;
                            _borderHead.Height = _headeBrowser.Height + 2;
                            _borderHead.Width = _headeBrowser.Width + 2;

                            ZoomIn(_pdfBrowser);
                            ZoomIn(_headeBrowser);
                        }
                        

                    }
                    else
                    {
                        e.Handled = false;
                    }
                };
            }
        }

        private void ZoomIn(System.Windows.Controls.Panel element)
        {
            foreach (var chld in element.Children)
            {
                if (chld.GetType() == typeof(Image))
                {
                    var child = chld as Image;
                    if (child != null)
                    {
                        child.Width *= 1.05;
                        child.Height *= 1.05;
                        Canvas.SetTop(child, Canvas.GetTop(child) * 1.05);
                        Canvas.SetLeft(child, Canvas.GetLeft(child) * 1.05);
                    }
                }

                foreach (var pdfp in _pdfImages)
                {
                    if (pdfp != _pdfImages[PageIndex])
                    {
                        pdfp.Page.Width *= 1.05;
                        pdfp.Page.Height *= 1.05;
                        Canvas.SetTop(pdfp.Page, Canvas.GetTop(pdfp.Page) * 1.05);
                        Canvas.SetLeft(pdfp.Page, Canvas.GetLeft(pdfp.Page) * 1.05);
                    }
                   
                }
                if (chld.GetType() == typeof(System.Windows.Controls.Label))
                {
                    var child = chld as System.Windows.Controls.Label;
                    if (child != null)
                    {
                        const double fact = 1.05;
                        child.Width *= fact;
                        child.Height *= fact;
                        child.FontSize *= fact;
                        Canvas.SetTop(child, Canvas.GetTop(child) * fact);
                        Canvas.SetLeft(child, Canvas.GetLeft(child) * fact);
                    }
                }
            }
        }
        private void ZoomOut(System.Windows.Controls.Panel element)
        {
            foreach (var chld in element.Children)
            {
                if (chld.GetType() == typeof(Image))
                {
                    var child = chld as Image;
                    if (child != null)
                    {
                        child.Width /= 1.05;
                        child.Height /= 1.05;
                        Canvas.SetTop(child, Canvas.GetTop(child) / 1.05);
                        Canvas.SetLeft(child, Canvas.GetLeft(child) / 1.05);
                    }
                }
                foreach (var pdfp in _pdfImages)
                {
                    if (pdfp != _pdfImages[PageIndex])
                    {
                        pdfp.Page.Width /= 1.05;
                        pdfp.Page.Height /= 1.05;
                        Canvas.SetTop(pdfp.Page, Canvas.GetTop(pdfp.Page) / 1.05);
                        Canvas.SetLeft(pdfp.Page, Canvas.GetLeft(pdfp.Page) / 1.05);
                    }
                   
                }
                if (chld.GetType() == typeof(System.Windows.Controls.Label))
                {
                    var child = chld as System.Windows.Controls.Label;
                    if (child != null)
                    {
                        const double fact = 1.05;
                        child.Width /= fact;
                        child.Height /= fact;
                        child.FontSize /= fact;
                        Canvas.SetTop(child, Canvas.GetTop(child) / fact);
                        Canvas.SetLeft(child, Canvas.GetLeft(child) / fact);
                    }
                }
            }
        }
        private string GetFile(string files, string defaultFile)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = files,
                DefaultExt = defaultFile
            };
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

        private int PageIndex
        {
            get { return _actualIndex; }
            set
            {
                _actualIndex = value;
                RaisePropertyChanged(() => PageIndex);
                ShowPdfAtPage(PageIndex);
            }
        }
        public bool EnableAddPdf
        {
            get { return _enableAddPdf; }
            set
            {
                _enableAddPdf = value;
                RaisePropertyChanged(() => EnableAddPdf);
            }
        }
        public RelayCommand GetPdf { get; set; }
        public RelayCommand<Canvas> SelectedElement { get; set; }
        public RelayCommand PutText { get; set; }
        public RelayCommand SavePdf { get; set; }
        public RelayCommand NextPage { get; set; }
        public RelayCommand PreviousPage { get; set; }
        public RelayCommand GetImage { get; set; }
        public RelayCommand ControlKeyEvent { get; set; }
        public RelayCommand<Canvas> Initializer { get; set; }
        public RelayCommand<Border> GetBorders { get; set; }
        public RelayCommand<Window> GetWindow { get; set; }
        public RelayCommand<ScrollViewer> GetTScrollView { get; set; }

        
    }
}