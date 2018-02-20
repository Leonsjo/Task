using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Taski
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnTaskStop.IsEnabled = false;
            btnTaskCancel.IsEnabled = false;
        }

        BackgroundWorker worker = new BackgroundWorker();


        //BackgroundWorker


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            for (int i = 0; i <= 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbWorker.Value = e.ProgressPercentage;
        }

        private void Button_Click_Worker(object sender, RoutedEventArgs e)
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }


        //Task
        bool taskHolder = false;

        CancellationTokenSource tokenSource;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btnTaskStop.IsEnabled = true;
            btnTaskCancel.IsEnabled = true;
            btnTaskStart.IsEnabled = false;

            taskHolder = false;

            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var progress = new Progress<int>(value => pbTask.Value = value);

            await Task.Run(async () =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    if (token.IsCancellationRequested) { break; }
                    while (taskHolder) { }
                    await PutTaskDelay();
                    ((IProgress<int>)progress).Report(i);
                }
            }, token);

            Task.WaitAll();
            pbTask.Value = 0;
            btnTaskStart.IsEnabled = true;
            btnTaskStop.IsEnabled = false;
        }

        private void btnTaskStop_Click(object sender, RoutedEventArgs e)
        {
            if (taskHolder)
            {
                taskHolder = false;
                btnTaskCancel.IsEnabled = true;
            }
            else
            {
                taskHolder = true;
                btnTaskCancel.IsEnabled = false;
            }
        }

        private void btnTaskCancel_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            btnTaskStop.IsEnabled = false;
            btnTaskCancel.IsEnabled = false;
        }

        //Dispatcher

        private async void Button_Click_Dispatcher(object sender, RoutedEventArgs e)
        {
            int i = 0;
            while (i <= 100)
            {
                await PutTaskDelay();
                await this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    pbDispatcher.Value = i;
                }));
                i++;
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(100);
        }


    }
}
