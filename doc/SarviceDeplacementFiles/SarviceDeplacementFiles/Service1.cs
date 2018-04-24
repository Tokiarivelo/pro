using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SarviceDeplacementFiles
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer _timer;
        double _scheduleTime;
        double _endTime;
        TimeSpan _scheduledRunTime;
        TimeSpan _scheduleEndTime;
        public Service1()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer();
            //_scheduleTime = DateTime.Now.AddMinutes(3);

            _scheduledRunTime = new TimeSpan(14, 47, 00);
            _scheduleEndTime = new TimeSpan(14, 50, 00);

            double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
            TimeSpan endTimeToday = new TimeSpan(24, 0, 0);

            //----------- First start the service, compare time noow and time to start ---------------------//
            if(_scheduledRunTime.TotalMilliseconds > current &&  _scheduleEndTime.TotalMilliseconds > current)
            {
                _scheduleTime = _scheduledRunTime.TotalMilliseconds - current;
            }
            else if(_scheduledRunTime.TotalMilliseconds < current && _scheduleEndTime.TotalMilliseconds > current)
            {
                _scheduleTime =1f;
            }
            else
            {
                _scheduleTime = (endTimeToday.TotalMilliseconds - current) + _scheduledRunTime.TotalMilliseconds;
            }
            //_scheduleTime = _scheduledRunTime.TotalMilliseconds > current ? _scheduledRunTime.TotalMilliseconds - current : (endTimeToday.TotalMilliseconds - current) + _scheduledRunTime.TotalMilliseconds;

            _endTime = _scheduleEndTime.TotalMilliseconds;

        }

        public void StartService()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            _timer.Enabled = true;
            _timer.Interval = _scheduleTime;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(ExcecutionService); // call the function every _timer.Interval (ms)

        }
        System.Timers.Timer _movingInterval;
        private void ExcecutionService(object sender, System.Timers.ElapsedEventArgs e)
        {



            
            _movingInterval = new System.Timers.Timer();
            _movingInterval.Enabled = true;
            _movingInterval.Interval = 1;
            _movingInterval.Elapsed += new System.Timers.ElapsedEventHandler(MoveFiles); // Excecute the moving every 1minute
            //_movingInterval.Start();

            File.WriteAllText(@"C:\Users\toky\Desktop\Logg\Logg.txt", DateTime.Now +" : "+ _movingInterval.Enabled.ToString() +"\n");

            //----------- Met le timer à demain à la même heure après l'execution ---------------------//
            if (_timer.Interval != 24 * 60 * 60 * 1000)
            {
                _timer.Interval = 24 * 60 * 60 * 1000;
            }
        }

        private void MoveFiles(object sender, System.Timers.ElapsedEventArgs e)
        {
            string SourcePath = @"C:\Users\toky\Downloads\";
            string DestinationPath = @"C:\Users\toky\Desktop\out\";

            DirectoryInfo folder = new DirectoryInfo(SourcePath);
            if (folder.Exists)
            {
                foreach (FileInfo fileInfo in folder.GetFiles())
                {
                    if (File.Exists(SourcePath + "\\" + fileInfo.Name))
                    {
                        File.Move(SourcePath + "\\"+fileInfo.Name, DestinationPath + "\\" + fileInfo.Name);
                    }
                }
                
            }
            //------------- set the intervel of excecution 1mn ------------//
            if (_movingInterval.Interval != 60000)
                _movingInterval.Interval = 60000;
            double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
            if(current >= _endTime)
            {
                // _movingInterval.Interval = double.MaxValue;
                _movingInterval.Enabled = false;
                //_movingInterval = null;

                File.WriteAllText(@"C:\Users\toky\Desktop\Logg\Logg.txt", DateTime.Now + " : " + _movingInterval.Enabled.ToString() + "\n");
            }
                

        }
        protected override void OnStop()
        {

        }
    }
}
