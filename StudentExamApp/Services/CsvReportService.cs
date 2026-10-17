using System.Globalization;
using System.Text;
using StudentExamApp.Models;

namespace StudentExamApp.Services;

public static class CsvReportService
{
    /// <summary>Produces RFC 4180-style CSV and prevents spreadsheet formula injection.</summary>
    public static string BuildResults(
        IEnumerable<Result> results,
        IEnumerable<Student> students,
        IEnumerable<Exam> exams)
    {
        var studentLookup = students.ToDictionary(s => s.Id);
        var examLookup = exams.ToDictionary(e => e.Id);
        var csv = new StringBuilder("StudentName,Exam,Score\r\n");

        foreach (var result in results)
        {
            if (!studentLookup.TryGetValue(result.StudentId, out var student)
                || !examLookup.TryGetValue(result.ExamId, out var exam))
                continue;

            csv.Append(QuoteText(student.Name)).Append(',')
                .Append(QuoteText(exam.Subject)).Append(',')
                .Append(result.Score.ToString(CultureInfo.InvariantCulture))
                .Append("\r\n");
        }

        return csv.ToString();
    }

    private static string QuoteText(string? input)
    {
        var value = input ?? "";
        // Excel and other spreadsheet programs can execute formulas inside CSV cells.
        // Add a literal apostrophe to suspicious values so they display as text.
        var start = value.TrimStart();
        if (start.Length > 0 && "=+-@\t\r\n".Contains(start[0]))
            value = "'" + value;

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
