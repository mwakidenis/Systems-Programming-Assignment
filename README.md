# Systems Programming Assignment

A single Windows Forms application featuring a dark-mode Main Menu (Dashboard) that opens five distinct module forms, each interacting directly with Windows operating system resources using `System.Diagnostics`, `System.ServiceProcess`, `System.Management`, `System.IO`, and Win32 API P/Invoke.

---

## Author

**mwakidenis** — Denis Mwakide
- GitHub: https://github.com/mwakidenis
- Repository: https://github.com/mwakidenis/Systems-Programming-Assignment

---

## Project Structure

```
Systems-Programming-Assignment/
├── Program.cs                  # Application entry point
├── MainForm.cs                 # Dark-mode dashboard with 5 navigation buttons
├── ProcessForm.cs              # Module 1: Process Management
├── ServicesForm.cs             # Module 2: Windows Services Control
├── PerformanceForm.cs          # Module 3: System Performance Monitor
├── HardwareForm.cs             # Module 4: Hardware & OS Information
├── ThreadMemoryForm.cs         # Module 5: Thread & Memory Inspector
├── .gitignore                  # Excludes bin/, obj/, *.exe, *.dll, Visual Studio files
└── assign.txt                  # Original assignment brief
```

---

## Modules

| Module | Form | OS Resources Used | Key Features |
|--------|------|-------------------|--------------|
| 1 | `ProcessForm` | `System.Diagnostics`, `kernel32.dll`, `ntdll.dll` | List processes (PID, name, memory, start time), Start via `Process.Start()`, Kill via `process.Kill()`, Suspend/Resume via `NtSuspendProcess`/`NtResumeProcess` |
| 2 | `ServicesForm` | `System.ServiceProcess` | List installed services via `ServiceController.GetServices()`, Start/Stop/Restart with status guards |
| 3 | `PerformanceForm` | `System.Diagnostics.PerformanceCounter` | Real-time CPU %, Available Memory, Disk Activity via `System.Windows.Forms.Timer` |
| 4 | `HardwareForm` | `Environment`, `System.Management`, `System.IO.DriveInfo` | OS version, computer name, processor count, total/available memory, logical disk drives |
| 5 | `ThreadMemoryForm` | `System.Diagnostics` | Inspect `process.Threads` (ID, state, start address) and `process.Modules` (DLL name, base address) |

---

## Prerequisites

- **Windows** (the app uses Windows-specific APIs)
- **.NET Framework 4.8** reference assemblies (included with Windows)
- **Visual Studio 2022** (optional, for editing/debugging) — **must run as Administrator**
- **C# compiler** (`csc.exe`) at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

> **Important:** Run Visual Studio (or the compiled executable) **as Administrator** for full OS access. Without elevated privileges, process kill/suspend/resume and service start/stop will fail with "Access denied."

---

## How to Build

### Option A: Visual Studio
1. Open Visual Studio **as Administrator**.
2. Create a **Windows Forms App (.NET Framework 4.8)** project named `SystemProgrammingApp`.
3. Replace/add the source files: `Program.cs`, `MainForm.cs`, `ProcessForm.cs`, `ServicesForm.cs`, `PerformanceForm.cs`, `HardwareForm.cs`, `ThreadMemoryForm.cs`.
4. Set `MainForm` as the startup object.
5. Add references: `System`, `System.Drawing`, `System.Windows.Forms`, `System.ServiceProcess`, `System.Management`, `System.Core`, `System.Xml`.
6. Press **F5** to build and run.

### Option B: Command Line (`csc.exe`)
```powershell
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /target:winexe `
  /out:bin\SystemProgrammingApp.exe `
  /reference:System.dll,System.Drawing.dll,System.Windows.Forms.dll,System.ServiceProcess.dll,System.Management.dll,System.Core.dll,System.Xml.dll `
  /recurse:*.cs
```

Then run:
```powershell
bin\SystemProgrammingApp.exe
```

---

## How to Run

1. Ensure you are running **as Administrator**.
2. Launch `SystemProgrammingApp.exe`.
3. The **Main Dashboard** appears with five buttons — click one to open its module:
   - **Module 1: Process Management** — list, start, kill, suspend, resume processes
   - **Module 2: Windows Services Control** — refresh, start, stop, restart services
   - **Module 3: System Performance Monitor** — start/stop real-time CPU, memory, disk monitoring
   - **Module 4: Hardware & OS Information** — view system specs (OS, CPU, memory, drives)
   - **Module 5: Thread & Memory Inspector** — select a process, inspect its threads and loaded modules

---

## Error Handling & Threading

- Every OS-interacting button is wrapped in `try-catch` blocks.
- `Win32Exception` with native error code `5` (access denied) is caught specifically and prompts the user to run as Administrator.
- The Performance Monitor uses `System.Windows.Forms.Timer` (1-second interval) so the UI thread remains responsive during polling.
- `PerformanceCounter`, `ServiceController`, and `Process` objects are properly disposed.

---

## Repository Notes

- Each source file was committed individually with alternating conventional prefixes (`feat:`, `fix:`, `chore:`, `docs:`, `ci:`).
- Compiled binaries (`bin/`, `obj/`, `*.exe`, `*.dll`) are excluded via `.gitignore`.
- Remote: `https://github.com/mwakidenis/Systems-Programming-Assignment.git`

---

## License

Educational use — systems programming assignment.