using System.Globalization;
using StudentExamApp.Data;
using StudentExamApp.Models;
using StudentExamApp.Repositories;
using StudentExamApp.Services;
using Xunit;

namespace StudentExamApp.Tests;

public sealed class StorageAndReportsTests
{
    [Fact]
    public void Repositories_SaveAndLoad_IntoExplicitDirectory()
    {
        var folder = Path.Combine(Path.GetTempPath(), "student-exam-" + Guid.NewGuid().ToString("N"));
        try
        {
            var repo = new StudentRepository(folder);
            repo.Add(new Student { Id = 101, Name = "Sample Student", Email = "student@example.test", Level = "3", GPA = 3.5 });
            var reloaded = new StudentRepository(folder);
            Assert.Single(reloaded.GetAll());
            Assert.Equal("Sample Student", reloaded.GetById(101)!.Name);
            Assert.True(File.Exists(Path.Combine(folder, "students.json")));
        }
        finally
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void InvalidJson_DoesNotGetSilentlyReplaced()
    {
        var folder = Path.Combine(Path.GetTempPath(), "student-exam-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        var file = Path.Combine(folder, "students.json");
        try
        {
            File.WriteAllText(file, "{invalid json");
            Assert.Throws<InvalidDataException>(() => new StudentRepository(folder));
            Assert.Equal("{invalid json", File.ReadAllText(file));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void DuplicateIds_AreRejectedWhenLoading()
    {
        var folder = Path.Combine(Path.GetTempPath(), "student-exam-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            File.WriteAllText(Path.Combine(folder, "students.json"), "[{\"Id\":1},{\"Id\":1}]");
            Assert.Throws<InvalidDataException>(() => new StudentRepository(folder));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void Csv_QuotesTextAndNeutralizesSpreadsheetFormulas()
    {
        var csv = CsvReportService.BuildResults(
            new[] { new Result { Id = 1, StudentId = 7, ExamId = 8, Score = 89.5 } },
            new[] { new Student { Id = 7, Name = "=SUM(1,2) \"demo\"" } },
            new[] { new Exam { Id = 8, Subject = "C#, Math" } });
        Assert.Contains("\"'=SUM(1,2) \"\"demo\"\"\"", csv);
        Assert.Contains("\"C#, Math\"", csv);
        Assert.Contains(",89.5", csv);
    }

    [Fact]
    public void Csv_UsesInvariantNumberFormatting()
    {
        var oldCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var csv = CsvReportService.BuildResults(
                new[] { new Result { Id = 1, StudentId = 3, ExamId = 4, Score = 2.5 } },
                new[] { new Student { Id = 3, Name = "Test" } },
                new[] { new Exam { Id = 4, Subject = "Science" } });
            Assert.Contains(",2.5", csv);
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
        }
    }

    [Fact]
    public void JsonStorage_KeepsSavedResults()
    {
        var folder = Path.Combine(Path.GetTempPath(), "student-exam-" + Guid.NewGuid().ToString("N"));
        try
        {
            var repo = new ResultRepository(folder);
            repo.Add(new Result { Id = 44, StudentId = 7, ExamId = 1, Score = 91 });
            Assert.Equal(91, new ResultRepository(folder).GetById(44)!.Score);
            Assert.Empty(Directory.GetFiles(folder, "*.tmp"));
        }
        finally
        {
            if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
        }
    }
}
