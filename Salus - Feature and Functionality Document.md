# Salus — Feature & Functionality Document

---

## 1. Overview

Salus is a Windows desktop application that helps individuals track their personal health metrics on a daily basis. It runs quietly in the background and prompts the user once per day to log key health data. All data is stored locally on the user's machine. The application supports multiple user profiles with fully isolated data per profile and requires no login or account.

---

## 2. Daily Prompt Window

### 2.1 Trigger Behavior
- Salus automatically presents the daily prompt window once per day, after 5:00 AM, when the user is actively using their desktop.
- The prompt is shown for the **last active profile** only — the most recent profile that was used in the application. It does not prompt for other profiles.
- The window appears only once per calendar day for that profile. If it has already been submitted or dismissed that day, it will not appear again until the following day.

### 2.2 Window Layout & Fields
The daily prompt window is a small, focused dialog. It contains the following input fields:

| Field | Input Type | Details |
|---|---|---|
| Sleep | Duration entry | How many hours of sleep the user got the previous night |
| Exercise duration | Duration entry | Total time spent exercising |
| Exercises performed | Multi-select list | Select from the user's custom exercise list (managed in Settings) |
| Weight | Numeric entry | User's weight for the day |
| Mood | 1–5 scale | A simple mood rating, presented as selectable options |
| Photo | Camera capture / file select | A photo of the user for that day |

### 2.3 Previous Day's Values
- The prompt window displays the values entered for the most recent previous day directly alongside each input field, giving the user a quick reference point.
- The previous day's photo is **not** shown in this reference view.

### 2.4 Photo Capture
- The user can take a photo using a connected camera directly within the prompt window.
- The user may also choose to skip the photo for that day. If skipped, or if no camera is available, a default placeholder image is stored for that day's entry.

### 2.5 Submission
- Once the user fills in their data and submits, the entry is saved to their profile's local data store.
- All fields are optional — the user may submit with partial data.

### 2.6 Dismissal Without Submitting
- If the user closes the daily prompt window without submitting, that day is recorded as **skipped**.
- A skipped day appears in the history view as a distinct state (no data entered, but the day is acknowledged).

---

## 3. Data Storage

- All data is stored **locally** on the user's machine. No data is sent to any external server or cloud service.
- Each user profile maintains a fully separate data store, including:
  - All daily metric entries
  - Stored photos (including default placeholder photos)
  - Profile-specific settings and custom exercise lists

---

## 4. User Profiles

### 4.1 Profile Selection
- Salus supports multiple user profiles on the same machine.
- There is no login system. Instead, the active profile is selected from within the application's Settings.
- The selected profile persists between sessions until manually changed.

### 4.2 Profile Isolation
- Each profile's health data, photos, exercise list, and display settings are fully separate from all other profiles.
- Switching profiles immediately changes the active data context throughout the application.

### 4.3 Profile Management
- Profiles can be created and named within Settings.
- Profile names are used to identify users throughout the application.

---

## 5. History & Data Viewing

### 5.1 Single Day Lookup
- The user can navigate to any specific past date and view the complete entry for that day.
- The full day view includes all metric values recorded and the stored photo (or placeholder) for that day.
- Days that were skipped are clearly indicated as such.

### 5.2 Trend Charts
- The application provides graphical trend views for the following metrics over a selectable time range:
  - Sleep duration
  - Exercise duration
  - Weight
  - Mood rating
- Charts display the data as line graphs, allowing the user to see changes over time at a glance.
- Skipped days are represented as gaps in the chart.

---

## 6. Editing Past Entries

- Any previously submitted entry can be edited at any time via the history view.
- All fields (including the photo) are editable after initial submission.
- Edits are saved back to the local data store and immediately reflected in history and trend views.
- Skipped days can be retroactively filled in by the user.

---

## 7. Settings

### 7.1 Profile Management
- Create new profiles with a user-defined name.
- Switch the active profile.
- Delete a profile (with confirmation to prevent accidental deletion of data).

### 7.2 Custom Exercise List
- Each profile maintains its own list of exercises that appear in the daily prompt's exercise selection.
- The user can add new exercises to the list at any time.
- The user can remove or rename existing exercises.
- Exercises removed from the list remain associated with any past entries that used them.

### 7.3 Display & Appearance
- The user can configure the visual appearance of the application, including theme and color options.
- Display settings are stored per profile.

### 7.4 Daily Prompt Timing
- The user can adjust the auto-launch behavior within the settings, subject to the constraint that the prompt will not appear before 5:00 AM.

---

## 8. Auto-Launch Behavior

- Salus runs in the background and monitors desktop activity.
- Each day, after 5:00 AM, the daily prompt window is shown once when the user is detected to be actively using the desktop.
- The daily prompt targets the last active profile only. Switching profiles does not trigger an additional prompt for the newly selected profile unless that profile has not yet been prompted today.
- The application does not interrupt the user mid-task; it waits for an appropriate moment when the desktop is idle or the user is switching contexts.

---

## 9. Out of Scope

The following are explicitly not part of this application:

- Cloud sync or remote backup
- Social sharing features
- Login / authentication
- Mobile or web versions
- Automated health recommendations or AI analysis
- Integration with third-party fitness devices or platforms
