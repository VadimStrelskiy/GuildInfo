using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Input;
using GuildInfo.Wpf.Controls;
using GuildInfo.Wpf.ViewModels;

namespace GuildInfo.Wpf
{
    public partial class MainWindow : Window
    {
        private GuildViewModel ViewModel { get; } = new GuildViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void Fetch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MainGrid.IsEnabled = false;
            var progressBarViewModel = new ProgressBarViewModel();
            ViewModel.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "Progress")
                {
                    progressBarViewModel.Progress = ViewModel.Progress;
                }
            };
            var progressBarWindow = new ProgressBarWindow(progressBarViewModel);
            progressBarWindow.Owner = this;
            progressBarWindow.Show();

            try
            {
                await ViewModel.Fetch();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response == null)
                {
                    MessageBox.Show(this, "Blizzard server unavailable!", "OMG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show(this, "Not Found!", "OMG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(this, "Forbidden!", "OMG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    throw;
                }
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
            }

            progressBarWindow.Close();
            Mouse.OverrideCursor = null;
            MainGrid.IsEnabled = true;
        }

        private void Filter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Filter();
        }
        
        private void Fetch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanFetch;
        }

        private void Filter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanFilter;
        }
    }
}
