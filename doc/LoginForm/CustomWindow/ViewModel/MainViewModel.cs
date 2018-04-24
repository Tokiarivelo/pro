using System.Windows;
using System.Windows.Input;
using CustomWindow.DataModels;
using CustomWindow.WindowResizer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomWindow.ViewModel
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
        #region Private Member

        private Window _myWindow;
        //margin around the window to allow for a drop shadow
        private int _myOuterMarginSize = 10;
        //radius of the edge of the window
        private int _myWindowRadius = 10;
        private WindowResizer.WindowDockPosition _mDockPosition = WindowDockPosition.Undocked;

        #endregion


        #region Public properties

        public double WindowMinimumWidth { get; set; } = 400;
        public double WindowMinimumHeight { get; set; } = 400;
        public int ResizeBorder => Borderless ? 0 : 6;
        public bool Borderless => (_myWindow.WindowState == WindowState.Maximized || _mDockPosition != WindowDockPosition.Undocked);
        public Thickness InnerContentPadding => new Thickness(0);
        public Thickness ResizeBorderThikness => new Thickness(ResizeBorder + OuterMarginSize);

        public int OuterMarginSize
        {
            get { return _myWindow.WindowState == WindowState.Maximized ? 0 : _myOuterMarginSize; }
            set
            {
                _myOuterMarginSize = value;
                RaisePropertyChanged(() => OuterMarginSize);
            }
        }
        public Thickness OuterMarginSizeThickness => new Thickness(OuterMarginSize);

        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : _myWindowRadius;
            }
            set
            {
                _myWindowRadius = value;
            }
        }
        public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);

        public int TitleHeight { get; set; } = 42;

        public GridLength TitelHightGridLength => new GridLength(TitleHeight + ResizeBorder);

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;
        #endregion

        #region Commands
        public ICommand MinimizeCommand => new RelayCommand(() =>
        {
            _myWindow.WindowState = WindowState.Minimized;
        });
        public ICommand MaximizeCommand => new RelayCommand(() =>
        {
            _myWindow.WindowState ^= WindowState.Maximized;
        });
        public ICommand CloseCommand => new RelayCommand(() =>
        {
            _myWindow.Close();
        });
        public ICommand MenuCommand => new RelayCommand(() =>
        {
            SystemCommands.ShowSystemMenu(_myWindow, GetMousePosition());
        });
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(Window window)
        {
            _myWindow = window;

            _myWindow.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            var resizer = new WindowResizer.WindowResizer(_myWindow);
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                _mDockPosition = dock;

                // Fire off resize events
                WindowResized();
            };
        }

        #endregion
        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(_myWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + _myWindow.Left, position.Y + _myWindow.Top);
        }
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            RaisePropertyChanged(() => Borderless);
            RaisePropertyChanged(() => ResizeBorderThikness);
            RaisePropertyChanged(() => OuterMarginSize);
            RaisePropertyChanged(() => OuterMarginSizeThickness);
            RaisePropertyChanged(() => WindowRadius);
            RaisePropertyChanged(() => WindowCornerRadius);
        }
        #endregion
    }
}