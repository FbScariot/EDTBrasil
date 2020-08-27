using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EDTBrasilScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Geral geral = new Geral();
            geral.LerRegistros();

            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Scheduler()
            //};

            //ServiceBase.Run(ServicesToRun);
        }

        //private class Teste
        //{
        //    private DispatcherTimer _dtTimer = null;
        //    private System.Timers.Timer timer1;
        //    private String timeString;

        //    public void Chamada()
        //    {
        //        _dtTimer = new DispatcherTimer();
        //        _dtTimer.Tick += new System.EventHandler(HandleTick);
        //        //DateTime agora = DateTime.Now;
        //        //TimeSpan intervalo = agora.AddSeconds(-agora.Second).AddMilliseconds(-agora.Millisecond).ToShortDateString("");

        //        TimeSpan intervalo = GetNextInterval();
        //        _dtTimer.Interval = intervalo;
        //        _dtTimer.Start();
        //    }

        //    private TimeSpan GetNextInterval()
        //    {
        //        timeString = ConfigurationManager.AppSettings["StartTime"];
        //        DateTime t = DateTime.Parse(timeString);
        //        TimeSpan ts = new TimeSpan();

        //        ts = t - System.DateTime.Now;

        //        if (ts.TotalMilliseconds < 0)
        //        {
        //            ts = t.AddMinutes(1) - System.DateTime.Now;//Here you can increase the timer interval based on your requirments.   
        //        }
        //        return ts;
        //    }

        //    private void HandleTick(object sender, System.EventArgs e)
        //    {
        //        //_uiTextBlock.Text = "Timer ticked!";
        //    }
        //}
    }
}
