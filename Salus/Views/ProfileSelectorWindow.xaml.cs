using Salus.ViewModels;
using System.Windows;

namespace Salus.Views
{
    public partial class ProfileSelectorWindow : Window
    {
        private readonly ProfileSelectorViewModel _vm;

        public ProfileSelectorWindow(ProfileSelectorViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
            vm.ProfileSelected += () => DialogResult = true;
        }

        protected override async void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            await _vm.LoadAsync();
        }
    }
}
