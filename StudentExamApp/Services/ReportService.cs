using StudentExamApp.Models;

namespace StudentExamApp.Services;

public static class ReportService
{
    public static void ShowTop3Students(List<Student> students, List<Exam> exams, List<Result> results, string subject)
    {
        var top3 = results
            .Where(r => exams.Any(e => e.Id == r.ExamId && e.Subject == subject))
            .GroupBy(r => r.StudentId)
            .Select(g => new { StudentId = g.Key, AvgScore = g.Average(x => x.Score) })
            .OrderByDescending(x => x.AvgScore)
            .Take(3)
            .Join(students, a => a.StudentId, s => s.Id, (a, s) => new { s.Name, a.AvgScore });

        Console.WriteLine($"Top 3 students in {subject}:");
        foreach (var item in top3)
            Console.WriteLine($"{item.Name} - {item.AvgScore:F2}");
    }
}
