using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Salus.Data;
using Salus.Services;
using Salus.ViewModels;
using Salus.Views;
using System;
using System.IO;
using System.Windows;

namespace Salus
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        private TaskbarIcon? _trayIcon;
        private PromptSchedulerService? _scheduler;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Run EF migrations
            var db = ServiceProvider.GetRequiredService<SalusDbContext>();
            await db.Database.MigrateAsync();

            // Setup tray icon
            _trayIcon = new TaskbarIcon
            {
                ToolTipText = "Salus",
                Visibility = Visibility.Visible
            };

            var contextMenu = new System.Windows.Controls.ContextMenu();
            var openItem = new System.Windows.Controls.MenuItem { Header = "Open Salus" };
            openItem.Click += (_, _) => ShowMainWindow();
            var exitItem = new System.Windows.Controls.MenuItem { Header = "Exit" };
            exitItem.Click += (_, _) => ExitApp();
            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(new System.Windows.Controls.Separator());
            contextMenu.Items.Add(exitItem);
            _trayIcon.ContextMenu = contextMenu;
            _trayIcon.TrayMouseDoubleClick += (_, _) => ShowMainWindow();

            // Load active profile
            var profileService = ServiceProvider.GetRequiredService<ProfileService>();
            var session = ServiceProvider.GetRequiredService<SessionContext>();
            var themeService = ServiceProvider.GetRequiredService<ThemeService>();

            var lastProfile = await profileService.GetLastActiveProfileAsync();

            if (lastProfile == null)
            {
                // First launch — must create a profile
                var profileWin = ServiceProvider.GetRequiredService<ProfileSelectorWindow>();
                if (profileWin.ShowDialog() != true || session.ActiveProfile == null)
                {
                    ExitApp();
                    return;
                }
                lastProfile = session.ActiveProfile;
            }
            else
            {
                session.ActiveProfile = lastProfile;
            }

            // Apply saved theme
            var theme = lastProfile.ThemePreference ?? "Light";
            themeService.ApplyTheme(theme);

            // Start scheduler
            _scheduler = ServiceProvider.GetRequiredService<PromptSchedulerService>();
            _scheduler.PromptRequested += ShowDailyPrompt;
            _scheduler.Start();

            // Show prompt if not yet done today
            var entryService = ServiceProvider.GetRequiredService<EntryService>();
            bool hasEntry = await entryService.HasEntryForTodayAsync(session.ActiveProfile.Id);
            if (!hasEntry && DateTime.Now.Hour >= 5)
            {
                ShowDailyPrompt();
            }
            else
            {
                ShowMainWindow();
            }
        }

        private void ShowMainWindow()
        {
            var win = ServiceProvider.GetRequiredService<MainWindow>();
            win.Show();
            win.Activate();
        }

        private void ShowDailyPrompt()
        {
            var session = ServiceProvider.GetRequiredService<SessionContext>();
            if (session.HasShownPromptToday) { ShowMainWindow(); return; }

            var promptWin = ServiceProvider.GetRequiredService<DailyPromptWindow>();
            promptWin.ShowDialog();
            ShowMainWindow();
        }

        private void ExitApp()
        {
            _scheduler?.Dispose();
            _trayIcon?.Dispose();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _scheduler?.Dispose();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Salus", "salus.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            services.AddDbContext<SalusDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"), ServiceLifetime.Transient);

            // Services
            services.AddSingleton<SessionContext>();
            services.AddSingleton<ThemeService>();
            services.AddSingleton<PromptSchedulerService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<EntryService>();
            services.AddTransient<ExerciseService>();
            services.AddTransient<CameraService>();

            // ViewModels
            services.AddTransient<ProfileSelectorViewModel>();
            services.AddTransient<DailyPromptViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<HistoryViewModel>();
            services.AddTransient<TrendsViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<EditEntryViewModel>();

            // Views
            services.AddTransient<ProfileSelectorWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<DailyPromptWindow>();
            services.AddTransient<CameraPreviewWindow>();
            services.AddTransient<EditEntryWindow>();
        }
    }
}
