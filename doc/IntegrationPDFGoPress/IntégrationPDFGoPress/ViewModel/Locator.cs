/*
  In App.xaml:
  <Application.Resources>
      <vm:Locator xmlns:vm="clr-namespace:IntégrationPDFGoPress.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using IntégrationPDFGoPress.Model;
using Microsoft.Practices.ServiceLocation;
using SourcesDB.Data;

namespace IntégrationPDFGoPress.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Locator
    {
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                // SimpleIoc.Default.Register<IDataService, DataService>();
            }
            _BddService = new BddService();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ServiceViewModel>();
            SimpleIoc.Default.Register<MappingViewModel>();
            SimpleIoc.Default.Register<PublicationListViewModel>();
            SimpleIoc.Default.Register<SourceContactViewModel>();
            //ModificationTest = false;

        }

        public static void CleanObject<T>() where T : class
        {
            try
            {
                SimpleIoc.Default.Unregister<T>();
                SimpleIoc.Default.Register<T>();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public static ServiceViewModel ServiceVM => ServiceLocator.Current.GetInstance<ServiceViewModel>();
        public static MappingViewModel MappingVM => ServiceLocator.Current.GetInstance<MappingViewModel>();
        public static PublicationListViewModel PublicationLisVM => ServiceLocator.Current.GetInstance<PublicationListViewModel>();
        public static SourceContactViewModel SourceContactVM => ServiceLocator.Current.GetInstance<SourceContactViewModel>();

        public static readonly IBddService _BddService;
        public static Media SelectedMedia { get; set; }
        public static string TypeSourceContact { get; set; } //Affichage pour child ou Master
        public static bool IsModified { get; set; }  //teste de modification

    }
}