using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gobbler
{
    public static class ProgressBarWorker
    {
        public delegate void WorkAction(BackgroundWorker worker);

        public volatile static object Passage = new object();

        public static void Work(ProgressBar bar, WorkAction work, Action update, Action workdone = null)
        {
            bar.Minimum = 0;
            bar.Maximum = 101;
            bar.Value = 0;
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                update();
                bar.Value = e.ProgressPercentage;
            };

            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                if (workdone != null)
                    workdone();
            };

            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                work(sender as BackgroundWorker);
                Thread.Sleep(100);
                (sender as BackgroundWorker).ReportProgress(101);
                // truukje om hem ook nog even op 100% te  tonen
               
            };
            
            worker.RunWorkerAsync();
        }
        
    }
}
