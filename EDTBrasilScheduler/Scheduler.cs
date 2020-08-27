using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EDTBrasilScheduler
{
    public partial class Scheduler : ServiceBase
    {
        private int getCallType;
        private System.Timers.Timer timer1;
        private String timeString;
        private ServiceLog serviceLog;

        public Scheduler()
        {
            this.serviceLog = new ServiceLog();
            InitializeComponent();
            int strTime = 1; 
            getCallType = 1; 

            if (getCallType == 1)
            {
                timer1 = new System.Timers.Timer();
                double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
            else
            {
                timer1 = new System.Timers.Timer();
                timer1.Interval = strTime * 1000;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
        }

        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Geral geral = new Geral();
            geral.LerRegistros();

            if (getCallType == 1)
            {
                timer1.Stop();
                SetTimer();
            }
        }

        private void SetTimer()
        {
            try
            {
                double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Start();
            }
            catch (Exception)
            {
            }
        }

        private double GetNextInterval()
        {
            DateTime agora = DateTime.Now;
            timeString = agora.ToShortTimeString();
            DateTime t = DateTime.Parse(timeString);
            TimeSpan ts = new TimeSpan();

            ts = t - System.DateTime.Now;
            if (ts.TotalMilliseconds < 0)
            {
                ts = t.AddMinutes(1) - System.DateTime.Now;
            }
            return ts.TotalMilliseconds;
        }

        protected override void OnStart(string[] args)
        {
            timer1.AutoReset = true;
            timer1.Enabled = true;
            ServiceLog.WriteErrorLog("Daily Reporting service started");
        }

        protected override void OnStop()
        {
            timer1.AutoReset = false;
            timer1.Enabled = false;
            ServiceLog.WriteErrorLog("Daily Reporting service stopped");
        }

        
    }
}
