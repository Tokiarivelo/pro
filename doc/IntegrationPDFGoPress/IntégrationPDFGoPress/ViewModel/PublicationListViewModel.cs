using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Diacritics.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IntégrationPDFGoPress.Model;
using IntégrationPDFGoPress.Utils;
using SourcesDB.Data;

namespace IntégrationPDFGoPress.ViewModel
{
    public class PublicationListViewModel : ViewModelBase
    {
        public PublicationListViewModel()
        {
            _bdd = Locator._BddService;
            MediaList = _bdd.GetAllMediaWithStringFilter("");
            FormatResultCount(MediaList.Count);
        }

        #region Methodes

        private void FormatResultCount(int resultCount)
        {
            if (resultCount > 1)
                Libelle = resultCount + " résulats";
            else Libelle = resultCount + " résulat";
        }

        #endregion Methodes


        #region Command
        public ICommand GotoMapping => new RelayCommand(() =>
        {
            if (SelectedMedia != null)
            {
                Locator.SelectedMedia = SelectedMedia;
                Messenger.Default.Send<ChangeView>(new ChangeView(ViewType.Mapping));
            }
            
        });
        public ICommand SearchPublication => new RelayCommand(() =>
        {
            if (!string.IsNullOrEmpty(StringFilter))
                MediaList = _bdd.GetAllMediaWithStringFilter(StringFilter);
            if(StringFilter == "")
                MediaList = _bdd.GetAllMediaWithStringFilter("");
            FormatResultCount(MediaList.Count);
        });
        public ICommand RefreshFiltre => new RelayCommand(() =>
        {
            StringFilter = "";
        });
        public ICommand RefreshList => new RelayCommand(() =>
        {
            StringFilter = "";
            MediaList = _bdd.GetAllMediaWithStringFilter("");
            FormatResultCount(MediaList.Count);
        });
        #endregion Command

        #region fields

        private readonly IBddService _bdd;
        private List<Media> _mediaList;
        private Media _selectedMedia;
        private string _stringFilter;
        private int _resultCount;
        private string _libelle;

        #endregion fields

        #region Getters && Setters
        public List<Media> MediaList
        {
            get { return _mediaList; }
            set
            {
                _mediaList = value;
                RaisePropertyChanged(()=>MediaList);
            }
        }


        

        public Media SelectedMedia
        {
            get { return _selectedMedia; }
            set
            {
                _selectedMedia = value; 
                RaisePropertyChanged(()=>SelectedMedia);
            }
        }

        public string StringFilter
        {
            get { return _stringFilter; }
            set
            {
                _stringFilter = value;
                RaisePropertyChanged(()=>StringFilter);
            }
        }

        public int ResultCount
        {
            get { return _resultCount; }
            set
            {
                _resultCount = value;
                RaisePropertyChanged(()=>ResultCount);
            }
        }

        public string Libelle
        {
            get { return _libelle; }
            set
            {
                _libelle = value;
                RaisePropertyChanged(()=>Libelle);
            }
        }

        #endregion Getters && Setters
    }
}
