# Salus

A personal daily health-metric tracker for Windows. Salus lives in your system tray and prompts you each morning to log sleep, exercise, weight, mood, and a photo — then lets you browse history and visualise trends over time.

---

## Features

- **Daily check-in prompt** — appears automatically after 5 AM if you haven't logged yet; records a skip if you dismiss it
- **Metric tracking** — sleep hours, exercise duration, exercises performed (custom list), body weight, mood (1–5)
- **Photo capture** — take a photo from within the app using your webcam, or fall back to a placeholder
- **History view** — browse any past date on a calendar; edit or retroactively fill skipped days
- **Trend charts** — line charts for all four metrics over a configurable date range
- **Multiple profiles** — each profile has its own entries, exercise list, and theme preference
- **System tray** — runs in the background; minimises to tray instead of closing
- **Windows startup** — optional auto-launch on login via registry
- **Light / Dark theming** — per-profile theme preference applied at startup

---

## Requirements

| Requirement | Version |
|---|---|
| Windows | 10 or 11 |
| .NET SDK | 7.0 |
| Visual Studio / Rider | Optional (any editor works) |
| Webcam | Optional (photo capture) |

---

## Getting Started

```bash
# Clone the repository
git clone <repo-url>
cd daily_metric_note

# Restore packages and build
dotnet build Salus/Salus.csproj

# Run
dotnet run --project Salus/Salus.csproj
```

The database is created automatically at `%LOCALAPPDATA%\Salus\salus.db` on first launch. No manual migration steps are needed — EF Core runs `MigrateAsync()` at startup.

---

## Project Structure

```
Salus/
├── App.xaml / App.xaml.cs          — Entry point, DI setup, tray icon, startup logic
├── Data/
│   ├── SalusDbContext.cs            — EF Core DbContext
│   ├── SalusDbContextFactory.cs     — Design-time factory for migrations
│   └── Migrations/                  — EF Core migration history
├── Models/                          — Profile, DailyEntry, Exercise, ExerciseLog
├── Services/
│   ├── ProfileService.cs            — Profile CRUD
│   ├── EntryService.cs              — Daily entry save/query
│   ├── ExerciseService.cs           — Exercise list management
│   ├── CameraService.cs             — AForge webcam capture
│   ├── ThemeService.cs              — Runtime theme switching
│   ├── PromptSchedulerService.cs    — Background timer for auto-prompt
│   └── SessionContext.cs            — Singleton holding the active profile
├── ViewModels/                      — One ViewModel per view (MVVM)
├── Views/                           — XAML windows and user controls
├── Resources/
│   ├── LightTheme.xaml              — Light colour palette
│   ├── DarkTheme.xaml               — Dark colour palette
│   └── Styles.xaml                  — Shared button, textbox, and label styles
└── Helpers/
    └── BoolToVisibilityConverter.cs — IValueConverter utilities
```

---

## Key Dependencies

| Package | Purpose |
|---|---|
| `Microsoft.EntityFrameworkCore.Sqlite` | ORM + SQLite storage |
| `CommunityToolkit.Mvvm` | ObservableObject, RelayCommand, source generators |
| `LiveChartsCore.SkiaSharpView.WPF` | Trend line charts |
| `Hardcodet.NotifyIcon.Wpf` | System tray icon |
| `AForge.Video.DirectShow` | Webcam capture |
| `Microsoft.Extensions.DependencyInjection` | DI container |

---

## Running Migrations (development)

If you modify the data models, add a new migration with:

```bash
cd Salus
dotnet ef migrations add <MigrationName>
```

The app applies pending migrations automatically at startup.

---

## Data Storage

| Path | Contents |
|---|---|
| `%LOCALAPPDATA%\Salus\salus.db` | SQLite database |
| `%LOCALAPPDATA%\Salus\Photos\{profileId}\{date}.jpg` | Daily photos |
