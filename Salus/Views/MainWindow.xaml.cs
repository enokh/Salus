using Salus.ViewModels;
using System.Windows;

namespace Salus.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
            vm.ShowHistoryCommand.Execute(null);
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            if (App.ServiceProvider.GetService(typeof(Services.SessionContext)) is Services.SessionContext session
                && session.ActiveProfile != null)
            {
                ProfileNameText.Text = session.ActiveProfile.Name;
            }
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            var promptWin = App.ServiceProvider.GetService(typeof(DailyPromptWindow)) as DailyPromptWindow;
            promptWin?.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
