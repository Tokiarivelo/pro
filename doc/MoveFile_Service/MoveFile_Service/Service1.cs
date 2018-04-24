using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoveFile_Service
{
    public partial class Service1 : ServiceBase
    {
        int _startTime = 0;
        int _endTime = 0;
        int _periodTime = 0;
        int _firstStart = 0;
        System.Threading.Timer _timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(SetParametters);
            aTimer.Start();
        }

        protected override void OnStop()
        {
            _timer.Dispose();
        }

        public void StartService()
        {
            OnStart(null);
        }

        private void SetParametters(object sender, System.Timers.ElapsedEventArgs le)
        {
            string[] timesParametters = File.ReadAllLines(@"C:\Users\toky\Desktop\Logg\Logg.txt");
            DateTime startTimeTemp = DateTime.ParseExact(timesParametters[0],"HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endTimeTemp = DateTime.ParseExact(timesParametters[1],"HH:mm:ss", CultureInfo.InvariantCulture);
            TimeSpan endTimeToday = new TimeSpan(24, 0, 0);
            int startTime;
            if (_firstStart == 0)
            {
                double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
                startTime = startTimeTemp.TimeOfDay.TotalMilliseconds > current ?  Convert.ToInt32( startTimeTemp.TimeOfDay.TotalMilliseconds - current) : Convert.ToInt32((endTimeToday.TotalMilliseconds - current) + startTimeTemp.TimeOfDay.TotalMilliseconds);
            }
            else
            {
                startTime = Convert.ToInt32(endTimeToday.TotalMilliseconds);
            }

            int endTime = Convert.ToInt32(endTimeTemp.TimeOfDay.TotalMilliseconds);
            int periodTime = Convert.ToInt32(timesParametters[2]);

            if (startTime != _startTime)
            {
                _startTime = startTime;

            }
               

            if (endTime != _endTime)
            {
                _endTime = endTime;
            }
               

            if (periodTime != _periodTime)
            {
                _periodTime = periodTime;
            }

            if(_firstStart == 0)
            {
                _firstStart = 1;
                _timer = new System.Threading.Timer((e) =>
                {
                    MoveFiles();
                }, null, _startTime, 60000);
               
            }
            //if (_firstStart == 1)
            //{
            //    _firstStart = 3;
            //    _startTime = 24 * 60 * 60 * 1000;

            //    _timer.Change(_startTime, _periodTime);
                
            //}
           

        }


        private void MoveFiles()
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
                        //File.Move(SourcePath + "\\" + fileInfo.Name, DestinationPath + "\\" + fileInfo.Name);
                        
                    }

                }
                
            }
            MessageBox.Show("ok");
            _firstStart = 1;

            int current = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalMilliseconds);
            if (current > _endTime)
            {
                _timer.Dispose();
                _firstStart = 0;
            }

            // MessageBox.Show(SourcePath);

        }



    }
}
