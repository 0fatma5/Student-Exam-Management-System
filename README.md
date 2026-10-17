# Student Exam Management System

A **C# / .NET 8 console application** for managing students, teachers, exams, results, and reports. The project demonstrates object-oriented programming, repositories, JSON persistence, LINQ, and CSV export.

## Features

- Add, update, delete, search, and sort students.
- Maintain teacher and exam records.
- Register student scores for existing exams.
- Review a student's exam results and show the top three students for a subject.
- Save and load JSON data for students, teachers, exams, and results.
- Export results to a spreadsheet-friendly CSV file.

## Technologies

C# · .NET 8 · JSON / System.Text.Json · LINQ · xUnit · GitHub Actions

## Run locally

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), then run from the repository root:

```bash
dotnet run --project StudentExamApp/StudentExamApp.csproj
```

The app also works when launched from the `StudentExamApp` directory:

```bash
cd StudentExamApp
dotnet run
```

The console displays a numbered menu for managing records and exporting reports.

### Data location

By default, the app looks for `StudentExamApp/Data` or `Data` in the current working directory. Otherwise, it uses a `Data` folder beside the application executable. To keep your working records in a dedicated location, set the optional environment variable `STUDENT_EXAM_DATA_DIR` to an absolute directory path.

Example in PowerShell:

```powershell
$env:STUDENT_EXAM_DATA_DIR = "$HOME\StudentExamData"
dotnet run --project StudentExamApp/StudentExamApp.csproj
```

The JSON files are saved after changes, as in the original application. The new storage layer writes to a temporary file and replaces the destination when the write succeeds. Invalid JSON now produces a descriptive error instead of being silently replaced. **Back up your data before using a new version.**

### Export CSV

Choose option **10** in the console menu. `results.csv` is generated in the resolved data directory.

CSV fields containing quotes and commas are escaped. Suspicious spreadsheet-formula prefixes are neutralized, and numeric scores always use a decimal point.

## Run tests

```bash
dotnet test StudentExamApp.Tests/StudentExamApp.Tests.csproj
```

Automated tests cover JSON round-trip, invalid JSON handling, duplicate ID detection, CSV escaping/formula neutralization, culture-invariant score formatting, and persistence of exam results. GitHub Actions runs a .NET 8 build and test job on pull requests.

## Repository layout

```text
StudentExamApp/
  Models/             Student, Teacher, Exam, Result
  Repositories/       JSON-backed CRUD repositories
  Services/           Rankings and CSV report generator
  Data/               Local JSON data files
  Program.cs          Console menu and workflows
StudentExamApp.Tests/ Automated tests
.github/workflows/    CI build and test
```

## Limitations and next improvements

- The project is currently a **console application**—not a deployed website. Any future demo should be recorded from the real terminal.
- Results are linked by student ID and exam ID. Deleting a student with recorded results is now prevented in the console to reduce orphaned records.
- Score validation currently requires a non-negative finite number; a maximum score depends on the chosen grading policy.
- The top-three report groups results by student and uses the average score for the requested subject.
- Existing `bin/` and `obj/` files were previously committed. A new `.gitignore` prevents new build artifacts, but it does not remove already-tracked ones.
- **Privacy:** the repository contains JSON data. Check that those records are fictional/anonymized before promoting the project publicly or including data in a demo.
- Future improvements could include transaction-safe multi-file updates, more validation, automated console integration tests, and a clean multi-layer architecture.

## Portfolio description

Suggested GitHub description: *C# .NET 8 console app for managing students, exams, grades, JSON persistence, LINQ rankings, and CSV reports.*

Suggested topics: `csharp`, `dotnet`, `console-app`, `json`, `linq`, `unit-testing`, `education`.

No demo video is included yet. Add one only after recording the real running application.
