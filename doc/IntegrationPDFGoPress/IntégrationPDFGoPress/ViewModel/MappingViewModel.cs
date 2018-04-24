using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using IntégrationPDFGoPress.Model;
using IntégrationPDFGoPress.Utils;
using IntégrationPDFGoPress.View;
using SourcesDB.Data;
using MessageBox = System.Windows.MessageBox;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace IntégrationPDFGoPress.ViewModel
{
    public class MappingViewModel : ViewModelBase
    {
        public MappingViewModel()
        {
            _bdd = Locator._BddService;
            DetailsMedia = Locator.SelectedMedia;
            _MasterOrChildOrNothing = 0;
            _Etat = true;
            if (DetailsMedia.PublicationMaster == true)
            {
                Master = true;
            }
            else
            {
                Master = false;
            }
            IsModified = false;
        }

        public ICommand IsModifiedCommand => new RelayCommand(() =>
        {   
            try
            {
                if (IsModified) return;
                IsModified = true;
                Locator.IsModified = true;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message);
            }

        });

        public ICommand GoToSearchRepository => new RelayCommand(() =>
        {
            TeamFolderDialogBox.MainWindow main = new TeamFolderDialogBox.MainWindow();
            string dir = General.GetParamValue("IntegrationGopressDirIn");
            main.Init(dir);
            DialogResult dialogResult = main.ShowFolderDialog();
            if (dialogResult == DialogResult.OK)
            {
                var select = TeamFolderDialogBox.MainWindow.SeletedDirs;
                if (select.Count > 0)
                {
                    string path = select[0];
                    DetailsMedia.FolderGoPress = path;
                    RaisePropertyChanged(()=>DetailsMedia);
                    main.Close();
                }
            }
        });

        public ICommand DeleteMapping => new RelayCommand(() =>
        {
            bool res = false;
            if (DetailsMedia.FolderGoPress != null && PublicationChild == null)
            {
                var result = MessageBox.Show("Voulez vous vraiment le mapping de cette publication", "Question",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    res = _bdd.DeleteMapping(DetailsMedia, PublicationChild);
                    if (res)
                    {
                        MessageBox.Show("Suppression du mapping de la publication réussie", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        GotoPublicationList.Execute("");
                        return;
                    }
                }
                else return;
            }
            if (PublicationChild!=null && PublicationChild.Count > 0)
            {
                var result = MessageBox.Show("Voulez vous vraiment le mapping et les publication enfant de cette publication", "Question",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    res = _bdd.DeleteMapping(DetailsMedia, PublicationChild);
                    if (res)
                    {
                        MessageBox.Show("Suppression du mapping de la publication réussie", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        GotoPublicationList.Execute("");
                        return;
                    }
                }
                else return;
            }
            
        });
        public ICommand SaveMapping => new RelayCommand(() =>
        {
            bool res = false;
            if (_MasterOrChildOrNothing == 0)
            {
                res = _bdd.SaveOrUpdatePublication(DetailsMedia, PublicationChild, 0);
            }
            if (_MasterOrChildOrNothing == 1)
            {
                if (PublicationChild.Count > 0)
                {
                    res = _bdd.SaveOrUpdatePublication(DetailsMedia, PublicationChild, 1);
                }else {
                    MessageBox.Show("Une Publication mère doit obligatoirement avoir des publication enfants", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            if (_MasterOrChildOrNothing == 2)
            {
                if (PublicationChild.Count > 0)
                {
                    res = _bdd.SaveOrUpdatePublication(DetailsMedia, PublicationChild, 2);
                }
                else
                {
                    MessageBox.Show("Une Publication mère doit obligatoirement avoir des publication enfants", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            if (_MasterOrChildOrNothing == 3)
            {
                res = _bdd.SaveOrUpdatePublication(DetailsMedia, PublicationChild, 3);
            }
            if (res)
            {
                MessageBox.Show("la mise à jour la publication réussie", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                IsModified = false;
                GotoPublicationList.Execute("");
                Locator.IsModified = false;
            }

        });
        public ICommand GotoPublicationList => new RelayCommand(() =>
        {
            if (IsModified)
            {
                var result = MessageBox.Show("Des données on été modifier, voulez-vous vraiment continuer", "Question",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Locator.PublicationLisVM.MediaList =
                        _bdd.GetAllMediaWithStringFilter(Locator.PublicationLisVM.StringFilter);
                    Messenger.Default.Send<ChangeView>(new ChangeView(ViewType.PublicationList));
                }
            }
            else
            {
                Locator.PublicationLisVM.MediaList =
                        _bdd.GetAllMediaWithStringFilter(Locator.PublicationLisVM.StringFilter);
                Messenger.Default.Send<ChangeView>(new ChangeView(ViewType.PublicationList));
            }
            
        });
        public ICommand AddPublicationToChild => new RelayCommand(() =>
        {
            if (SelectedListPublicationChild != null)
            {
                PublicationChild.Add(SelectedListPublicationChild);
                PublicationChild = PublicationChild.OrderBy(x => x.MediaName).ToList();
                ListPublicationChild.Remove(SelectedListPublicationChild);
                ListPublicationChild = ListPublicationChild.OrderBy(x => x.MediaName).ToList();
            }
            else
                MessageBox.Show("Veuillez choisir une publication", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        });

        public ICommand DeleteChildMaster => new RelayCommand(() =>
        {
            if (SelectedPublicationChild != null)
            {
                ListPublicationChild.Add(SelectedPublicationChild);
                ListPublicationChild = ListPublicationChild.OrderBy(x => x.MediaName).ToList();
                PublicationChild.Remove(SelectedPublicationChild);
                PublicationChild = PublicationChild.OrderBy(x => x.MediaName).ToList();
                
            }
            else MessageBox.Show("Veuillez choisir une publication dans la liste", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }); 
        private void SetMasterOrChildOrNothing(bool value)
        {
            try
            {
                if (value == true)
                {                    
                    if (DetailsMedia.PublicationMaster == true)
                    {
                        DetailsVisibility = "Show";
                        PublicationChild = _bdd.GetAllChildByPublication(DetailsMedia).OrderBy(x => x.MediaName).ToList();
                        ListPublicationChild = _bdd.GetAllPublicationGoPress();
                        _master = true;
                        _Etat = false;
                    }
                    else
                    {
                        Media masterTemp = new Media();
                        if (_bdd.IsPublicatinChild(DetailsMedia, ref masterTemp))
                        {
                            var result =
                                MessageBox.Show(
                                    "Cette publication est un enfant de la publication '" + masterTemp.MediaName +
                                    "'.   Voulez vous vraiment rendre cette publication mère ?", "Question",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                DetailsMedia.PublicationMaster = true;
                                DetailsVisibility = "Show";
                                ListPublicationChild = _bdd.GetAllPublicationGoPress();
                                PublicationChild = new List<Media>();
                                _MasterOrChildOrNothing = 1;
                            }
                            else
                            {
                                _master = false;
                                DetailsMedia.PublicationMaster = false;
                                return;
                            }
                        }
                        else
                        {
                            DetailsMedia.PublicationMaster = true;
                            DetailsVisibility = "Show";
                            ListPublicationChild = _bdd.GetAllPublicationGoPress();
                            PublicationChild = new List<Media>();
                            _MasterOrChildOrNothing = 2;
                            return;
                        }
                    }
                }
                else
                {
                    if (_Etat)
                    {
                        _Etat = false;
                        DetailsVisibility = "Hidden";
                        return;
                    }
                    else
                    {
                        var result = MessageBox.Show("Voulez-vous vraiment rendre cette publication ni mère ni enfant?", "Question",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            PublicationChild = new List<Media>();
                            DetailsVisibility = "Hidden";
                            _MasterOrChildOrNothing = 3;
                            _master = false;
                            DetailsMedia.PublicationMaster = false;
                        }
                        else
                        {
                            _master = true;
                            DetailsMedia.PublicationMaster = true;
                            return;
                        }
                    }
                    

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private IBddService _bdd;
        private Media _detailsMedia;
        private List<Media> _publicationChild;
        private List<Media> _listPublicationChild;
        private Media _selectedPublicationChild;
        private Media _selectedListPublicationChild ;
        private int _MasterOrChildOrNothing;
        private string _detailsVisibility;
        private bool _master;
        private bool _Etat;
        private bool _isModified;

        public bool Master
        {
            get { return _master; }
            set
            {
                SetMasterOrChildOrNothing(value);
                RaisePropertyChanged(() => Master);
            }
        }
        public Media DetailsMedia
        {
            get { return _detailsMedia; }
            set
            {
                _detailsMedia = value;
                RaisePropertyChanged(()=>DetailsMedia);
            }
        }

        public List<Media> PublicationChild
        {
            get { return _publicationChild; }
            set
            {
                _publicationChild = value; 
                RaisePropertyChanged(()=>PublicationChild);
            }
        }

        public List<Media> ListPublicationChild
        {
            get { return _listPublicationChild; }
            set
            {
                _listPublicationChild = value; 
                RaisePropertyChanged(()=> ListPublicationChild);
            }
        }

        public string DetailsVisibility
        {
            get { return _detailsVisibility; }
            set
            {
                _detailsVisibility = value; 
                RaisePropertyChanged(()=>DetailsVisibility);
            }
        }

        

        public Media SelectedPublicationChild
        {
            get { return _selectedPublicationChild; }
            set
            {
                _selectedPublicationChild = value;
                RaisePropertyChanged(()=>SelectedPublicationChild);
            }
        }

        public Media SelectedListPublicationChild
        {
            get { return _selectedListPublicationChild; }
            set
            {
                _selectedListPublicationChild = value; 
                RaisePropertyChanged(()=>SelectedListPublicationChild);
            }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                _isModified = value;
                RaisePropertyChanged(()=>IsModified);
            }
        }
    }
}
