using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Diacritics.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using IntégrationPDFGoPress.Utils;
using SourcesDB.Data;

namespace IntégrationPDFGoPress.ViewModel
{
    public class SourceContactViewModel : ViewModelBase
    {
       
        public SourceContactViewModel()
        {
            
        }

        private void Removing() //Enlever ceux qui sont deja mappé dans la base et celles qui sont dans le listbox
        {
            
           
        }

        private void ResultLabel(int listNumber)
        {
            if (listNumber <= 1)
                ResultCount = listNumber + " résultat";
            else
                ResultCount = listNumber + " résultats";
        }

        
        private string _stringFilter;
        private string _resultCount;

        
        public string StringFilter
        {
            get { return _stringFilter; }
            set
            {
                _stringFilter = value;
                RaisePropertyChanged(()=>StringFilter);
            }
        }

        public ICommand GoToSearchContact => new RelayCommand(() =>
        {
            if (StringFilter != null)
            {
                StringFilter = StringFilter.Trim();
            }
            
        });
        public ICommand ResetRoleSearchCommand => new RelayCommand(() =>
        {
           
        });
        public ICommand GoBack => new RelayCommand<Window>(windowToClose =>
        {
            windowToClose.Close();
        });

       
        // --------------------------- Ajout dans les champs -------------------- //
        public ICommand SaveContactAndBack => new RelayCommand<Window>(windowToClose =>
        {
           
            
        });
        //---------------- Comptage des résultats ------------------------//ResultCount
        public string ResultCount
        {
            get { return _resultCount; }
            set
            {
                _resultCount = value;
                RaisePropertyChanged(() => ResultCount);
            }
        }
    }
}
