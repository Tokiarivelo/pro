using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Windows.Forms;
using System.Windows.Input;
using APMDB.Data;
using GalaSoft.MvvmLight.Command;
using IntégrationPDFGoPress.Model;
using IntégrationPDFGoPress.Utils;

namespace IntégrationPDFGoPress.ViewModel
{
    public class ServiceViewModel : ViewModelBase
    {


        

        public ServiceViewModel()
        {
            _bdd = new BddService();
            ValueHours = new List<string>();
            ValueMinutesSecond = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                ValueHours.Add(i.ToString("00"));
            }
            for (int i = 0; i < 60; i++)
            {
                ValueMinutesSecond.Add(i.ToString("00"));
            }
            Initializing();
        }
        private void Initializing()
        {
            StartTime = General.GetParamValue("StartServicePDFIntegrationGopress");
            EndTime = General.GetParamValue("EndServicePDFIntegrationGopress");
            MinutePerod = General.GetParamValue("PeriodServicePDFIntegrationGopress");
            DeletingDayRecord = General.GetParamValue("DeletingRecordPDFIntegrationGopress");
            Input = General.GetParamValue("IntegrationGopressDirIn");
            Output = General.GetParamValue("IntegrationGopressDirOut");

            HourServiceStart = StartTime.Split(':')[0];
            MinuteServiceStart = StartTime.Split(':')[1];

            HourServiceStop = EndTime.Split(':')[0];
            MinuteServiceStop = EndTime.Split(':')[1];

        }
        public ICommand SaveParameter => new RelayCommand(() =>
        {
            if (int.Parse(MinutePerod) > 60)
            {
                System.Windows.MessageBox.Show("le temps de relance du service ne doit pas dépasser les 60 minuteq", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (int.Parse(HourServiceStart) > int.Parse(HourServiceStop) 
                ||( int.Parse(HourServiceStart) == int.Parse(HourServiceStop) && int.Parse(MinuteServiceStart) > int.Parse(MinuteServiceStop)))
            {
                System.Windows.MessageBox.Show("l'heure de début du service ne doit pas ètre supérieure à l'heure de fin", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Logger.LogInfo("Save new paramèter");
            string start = HourServiceStart+":"+ MinuteServiceStart;
            string end = HourServiceStop+":"+ MinuteServiceStop;
            bool res = _bdd.SaveOrUpdateParameter(Input,Output,start,end,MinutePerod,DeletingDayRecord);
            if (res)
            {
                System.Windows.MessageBox.Show("la mise à jour la publication réussie", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Logger.LogInfo("Save  paramète succes");
                Locator.IsModified = false;
            }
            
        });

        public ICommand IsModifiedCommand => new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() =>
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
        public ICommand InputFolder => new RelayCommand(() =>
        {
            var currentDir = Input;
            Input = SelectFolder(currentDir);
        });
        public ICommand OutputFolder => new RelayCommand(() =>
        {
            var currentDir = Output;
            Output = SelectFolder(currentDir);
        });

        private static string SelectFolder(string defaultPaf)
        {
            var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = defaultPaf
            };
            var result = folderDialog.ShowDialog();

            if (result != DialogResult.OK) return defaultPaf;
            return folderDialog.SelectedPath;
        }

        private List<string> _valueHours;
        private List<string> _valueMinutesSecond;
        private string _hourServiceStart;
        private string _minuteServiceStart;
        private string _minuteServiceStop;
        private string _hourServiceStop;
        private string _minutePeriod;
        private string _deletingDayRecord;
        private string _startTime;
        private string _endTime;
        private readonly IBddService _bdd; 
        private string _input;
        private string _output;
        private bool _isModified;

        public List<string> ValueHours
        {
            get { return _valueHours; }
            set
            {
                _valueHours = value;
                RaisePropertyChanged(() => ValueHours);
            }
        }

        public List<string> ValueMinutesSecond
        {
            get { return _valueMinutesSecond; }
            set
            {
                _valueMinutesSecond = value;
                RaisePropertyChanged(() => ValueMinutesSecond);
            }
        }

        public string HourServiceStart
        {
            get { return _hourServiceStart; }
            set
            {
                _hourServiceStart = value;
                RaisePropertyChanged(() => HourServiceStart);
            }
        }

        public string MinuteServiceStart
        {
            get { return _minuteServiceStart; }
            set
            {
                _minuteServiceStart = value;
                RaisePropertyChanged(() => MinuteServiceStart);
            }
        }



        public string HourServiceStop
        {
            get { return _hourServiceStop; }
            set
            {
                _hourServiceStop = value;
                RaisePropertyChanged(() => HourServiceStop);
            }
        }

        public string MinuteServiceStop
        {
            get { return _minuteServiceStop; }
            set
            {
                _minuteServiceStop = value;
                RaisePropertyChanged(() => HourServiceStop);
            }
        }

        public string MinutePerod
        {
            get { return _minutePeriod; }
            set
            {
                _minutePeriod = value.Trim();
                RaisePropertyChanged(() => MinutePerod);
            }
        }
        public string DeletingDayRecord
        {
            get { return _deletingDayRecord; }
            set
            {
                _deletingDayRecord = value.Trim();
                RaisePropertyChanged(() => DeletingDayRecord);
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

        public string StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value; 
                RaisePropertyChanged(()=>StartTime);
            }
        }

        public string EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                RaisePropertyChanged(()=>EndTime);
            }
        }

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value; 
                RaisePropertyChanged(()=>Input);
            }
        }

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                RaisePropertyChanged(()=>Output);
            }
        }
    }

}
