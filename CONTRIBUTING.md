# Contributing to Systems-Programming-Assignment

Welcome! This is a C# Windows Forms assignment demonstrating OS-level interactions. Here's how to contribute.

## Getting Started

1. **Fork** the repository.
2. **Clone** your fork:
   ```powershell
   git clone https://github.com/<your-username>/Systems-Programming-Assignment.git
   ```
3. Open the solution in **Visual Studio 2022** (run as Administrator).
4. Create a feature branch:
   ```powershell
   git checkout -b feature/your-feature-name
   ```

## Making Changes

- Each module lives in its own form file:
  - `ProcessForm.cs` — Module 1
  - `ServicesForm.cs` — Module 2
  - `PerformanceForm.cs` — Module 3
  - `HardwareForm.cs` — Module 4
  - `ThreadMemoryForm.cs` — Module 5
  - `MainForm.cs` — Dashboard
- Follow the existing C# 5-compatible style (no interpolated strings, no exception filters).
- Wrap all OS API calls in `try-catch` blocks.
- Test on Windows with Administrator privileges.

## Committing

We use conventional commits with alternating prefixes:
`feat:` → `fix:` → `chore:` → `docs:` → `ci:` → (repeat)

Stage and commit each file individually with a unique, contextual message:

```powershell
git add SystemProgrammingApp/ProcessForm.cs
git commit -m "feat: your unique message here"
```

## Submitting

1. Push your branch:
   ```powershell
   git push origin feature/your-feature-name
   ```
2. Open a **Pull Request** against `main`.
3. Describe what changed and why.

## Code of Conduct

Be respectful. This is a learning assignment — help others learn too.

## Maintainer

**mwakidenis** — https://github.com/mwakidenis

## License

Educational use — systems programming assignment.