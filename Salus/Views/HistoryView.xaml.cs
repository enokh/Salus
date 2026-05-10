using Salus.ViewModels;
using System.Windows.Controls;

namespace Salus.Views
{
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) =>
            {
                if (DataContext is HistoryViewModel vm)
                    vm.EditRequested += OnEditRequested;
            };
        }

        private void OnEditRequested(Models.DailyEntry? entry)
        {
            var editWin = App.ServiceProvider.GetService(typeof(EditEntryWindow)) as EditEntryWindow;
            if (editWin == null) return;
            editWin.LoadEntry(entry);
            editWin.ShowDialog();
            if (DataContext is HistoryViewModel vm)
                _ = vm.LoadEntryAsync();
        }
    }
}
