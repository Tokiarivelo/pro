

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using IntégrationPDFGoPress.Utils;

namespace IntégrationPDFGoPress.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            try
            {
                Messenger.Default.Register<ChangeView>(this, (action) => ReceiveMessage(action));
                BgService = "x:Null";
                BgMapping = "#2490BE";
                CurrentView = Locator.PublicationLisVM;
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    System.Deployment.Application.ApplicationDeployment ad =
                    System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    Version += "Version " + ad.CurrentVersion;
                }
                else
                {
                    Version += "Version " +
                    System.Windows.Forms.Application.ProductVersion.Substring(0,
                    System.Windows.Forms.Application.ProductVersion.LastIndexOf(".", StringComparison.Ordinal));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            
        }

        private void ReceiveMessage(ChangeView vierwname)
        {
            switch (vierwname.Viewx)
            {
                case ViewType.Mapping:
                    Locator.CleanObject<MappingViewModel>();
                    CurrentView = Locator.MappingVM;
                    break;

                case ViewType.PublicationList:
                    //Locator.CleanObject<PublicationListViewModel>();
                    CurrentView = Locator.PublicationLisVM;
                    break;
                case ViewType.Service:
                    Locator.CleanObject<ServiceViewModel>();
                    CurrentView = Locator.ServiceVM;
                    break;
            }
            Messenger.Default.Unregister<ChangeView>(this, (action) => ReceiveMessage(action));
        }

        #region ICommand

        public ICommand GotoPublicationList => new RelayCommand(() =>
        {
            try
            {
                Logger.LogInfo("Go to ListPublication");
                if (Locator.IsModified)
                {
                    var result = MessageBox.Show("Des champs ont été modifiés, voullez vous vraiment quitter!!", "Information", MessageBoxButton.OKCancel,
                       MessageBoxImage.Information);
                    if (result == MessageBoxResult.Cancel) return;
                    Messenger.Default.Send(new ChangeView(ViewType.PublicationList));
                    BgService = "x:Null";
                    BgMapping = "#2490BE";
                }
                else
                {
                    Messenger.Default.Send(new ChangeView(ViewType.PublicationList));
                    BgService = "x:Null";
                    BgMapping = "#2490BE";
                }
                Locator.IsModified = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message); ;
            }
            
            
        });

        public ICommand GotoMapping => new RelayCommand(() =>
        {
            Messenger.Default.Send(new ChangeView(ViewType.Mapping));
        });

        public ICommand GotoParameter => new RelayCommand(() =>
        {
            try
            {
                Logger.LogInfo("Go to service parameter");
                if (Locator.IsModified)
                {
                    var result = MessageBox.Show("Des champs ont été modifiés, voullez vous vraiment quitter!!", "Information", MessageBoxButton.OKCancel,
                       MessageBoxImage.Information);
                    if (result == MessageBoxResult.Cancel) return;
                    Messenger.Default.Send(new ChangeView(ViewType.Service));
                    //CurrentView = Locator.ServiceVM;
                    BgMapping = "x:Null";
                    BgService = "#2490BE";
                }
                else
                {
                    Messenger.Default.Send(new ChangeView(ViewType.Service));
                    BgMapping = "x:Null";
                    BgService = "#2490BE";
                }
                Locator.IsModified = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message) ;
            }
            
            
        });

        public ICommand GoOut => new RelayCommand(() =>
        {
            try
            {
                if ((CurrentView == Locator.MappingVM && Locator.MappingVM.IsModified) || (CurrentView == Locator.ServiceVM))
                {
                    var result = MessageBox.Show("Des champs ont été modifiés, voullez vous vraiment quitter?", "Information", MessageBoxButton.OKCancel,
                       MessageBoxImage.Information);
                    if (result == MessageBoxResult.Cancel) return;
                    Environment.Exit(0);
                }
                else
                {
                    var result = MessageBox.Show("Voullez vous vraiment quitter?", "Information", MessageBoxButton.OKCancel,
                       MessageBoxImage.Information);
                    if (result == MessageBoxResult.Cancel) return;
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message) ;
            }
            
           
        });

        //CloseApplication
        public ICommand CloseApplication
        {
            get
            {
                return new RelayCommand<System.ComponentModel.CancelEventArgs>(
                args =>
                {
                    try
                    {
                        if (Locator.IsModified)
                        {
                            var result = MessageBox.Show("Des champs ont été modifiés, voullez vous vraiment quitter!!", "Information", MessageBoxButton.OKCancel,
                                MessageBoxImage.Information);
                            if (result == MessageBoxResult.Cancel) args.Cancel = true;
                        }
                        else
                        {
                            var result = MessageBox.Show("Voullez vous vraiment quitter!!", "Information", MessageBoxButton.OKCancel,
                                MessageBoxImage.Information);
                            if (result == MessageBoxResult.Cancel) args.Cancel = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                    }
                        
                });
            }
        }

        #endregion Icommand

        #region Fields

        private string _userName;
        private string _bgMapping, _bgService;
        private ViewModelBase _currentView;
        private string _version;

        #endregion Fields



        #region Getters && Setters
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string BgMapping
        {
            get { return _bgMapping; }
            set
            {
                _bgMapping = value;
                RaisePropertyChanged(()=>BgMapping);
            }
        }

        public string BgService
        {
            get { return _bgService; }
            set
            {
                _bgService = value;
                RaisePropertyChanged(()=>BgService);
            }
        }

        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value; 
                RaisePropertyChanged(()=>CurrentView);
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                RaisePropertyChanged(()=>Version);
            }
        }

        #endregion Getters && Setters
    }
}