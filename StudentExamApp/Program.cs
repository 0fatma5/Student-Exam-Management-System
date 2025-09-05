using StudentExamApp.Models;
using StudentExamApp.Repositories;
using System.Text.RegularExpressions;
using StudentExamApp.Services;


class Program
{
    static void Main()
    {
        var studentRepo = new StudentRepository();
        var teacherRepo = new TeacherRepository();
        var examRepo = new ExamRepository();
        var resultRepo = new ResultRepository();
    

        while (true)
        {
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Add Teacher");
            Console.WriteLine("3. Show/Search/Sort Students");
            Console.WriteLine("4. Add Exam");
            Console.WriteLine("5. Register Exam Result");
            Console.WriteLine("6. Show Student Results");
            Console.WriteLine("7. Show Top 3 Students in a Subject");
            Console.WriteLine("8. Save Data");
            Console.WriteLine("9. Load Data");
            Console.WriteLine("10. Export Results to CSV");
            Console.WriteLine("11. Update Student");
            Console.WriteLine("12. Delete Student");
            Console.WriteLine("13. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddStudent(studentRepo); break;
                case "2": AddTeacher(teacherRepo); break;
                case "3": ShowSearchSortStudents(studentRepo); break;
                case "4": AddExam(examRepo); break;
                case "5": AddResult(resultRepo, studentRepo, examRepo); break;
                case "6": ShowStudentResults(studentRepo, examRepo, resultRepo); break;
                case "7":
                    Console.Write("Enter Subject: ");
                    var subject = Console.ReadLine()!;
                    ReportService.ShowTop3Students(studentRepo.GetAll().ToList(), examRepo.GetAll().ToList(), resultRepo.GetAll().ToList(), subject);
                    break;
                case "8": SaveAll(studentRepo, teacherRepo, examRepo, resultRepo); break;
                case "9": LoadAll(studentRepo, teacherRepo, examRepo, resultRepo); break;
                case "10": ExportResultsToCsv(resultRepo, studentRepo, examRepo); break;
                case "11": UpdateStudent(studentRepo); break;
                case "12": DeleteStudent(studentRepo); break;
                case "13": return;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }

    }
    static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }


    static void AddStudent(StudentRepository repo)
    {
        int id;
        while (true)
        {
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID must be a number.");
                continue;
            }
            if (repo.GetById(id) != null)
            {
                Console.WriteLine("This ID already exists.");
                continue;
            }
            break;
        }

        string name;
        do
        {
            Console.Write("Name: ");
            name = Console.ReadLine()!;
        } while (string.IsNullOrWhiteSpace(name));

        string email;
        while (true)
        {
            Console.Write("Email: ");
            email = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email cannot be empty.");
                continue;
            }
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format. Example: user@example.com");
                continue;
            }
            break;
        }

        string level;
        do
        {
            Console.Write("Level: ");
            level = Console.ReadLine()!;
        } while (string.IsNullOrWhiteSpace(level));

        double gpa;
        while (true)
        {
            Console.Write("GPA: ");
            if (!double.TryParse(Console.ReadLine(), out gpa))
            {
                Console.WriteLine("GPA must be a number.");
                continue;
            }
            if (gpa < 0 || gpa > 4)
            {
                Console.WriteLine("GPA must be between 0 and 4.");
                continue;
            }
            break;
        }

        var s = new Student { Id = id, Name = name, Email = email, Level = level, GPA = gpa };
        repo.Add(s);
        Console.WriteLine("Student added successfully!");
    }


    static void AddTeacher(TeacherRepository repo)
    {
        int id;
        while (true)
        {
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID must be a number.");
                continue;
            }
            if (repo.GetById(id) != null)
            {
                Console.WriteLine("This ID already exists.");
                continue;
            }
            break;
        }

        string name;
        do
        {
            Console.Write("Name: ");
            name = Console.ReadLine()!;
        } while (string.IsNullOrWhiteSpace(name));

        string email;
        while (true)
        {
            Console.Write("Email: ");
            email = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email cannot be empty.");
                continue;
            }
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format. Example: user@example.com");
                continue;
            }
            break;
        }

        string subject;
        do
        {
            Console.Write("Subject: ");
            subject = Console.ReadLine()!;
        } while (string.IsNullOrWhiteSpace(subject));

        var t = new Teacher { Id = id, Name = name, Email = email, Subject = subject };
        repo.Add(t);
        Console.WriteLine("Teacher added successfully!");
    }


    static void ShowStudents(StudentRepository repo)
    {
        var students = repo.GetAll().ToList();
        if (students.Count == 0)
        {
            Console.WriteLine("No students found.");
            return;
        }

        foreach (var s in students)
        {
            Console.WriteLine($"{s.Id} - {s.Name} ({s.Level}) GPA: {s.GPA}");
        }
    }
    static void AddExam(ExamRepository repo)
{
    int id;
    while (true)
    {
        Console.Write("Exam ID: ");
        if (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("ID must be a number.");
            continue;
        }
        if (repo.GetById(id) != null)
        {
            Console.WriteLine("This ID already exists.");
            continue;
        }
        break;
    }

    string subject;
    do
    {
        Console.Write("Subject: ");
        subject = Console.ReadLine()!;
    } while (string.IsNullOrWhiteSpace(subject));

    DateTime date;
    while (true)
    {
        Console.Write("Date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out date))
        {
            Console.WriteLine("Invalid date format.");
            continue;
        }
        break;
    }

    repo.Add(new Exam { Id = id, Subject = subject, Date = date });
    Console.WriteLine("Exam added successfully!");
}
    static void AddResult(ResultRepository resultRepo, StudentRepository studentRepo, ExamRepository examRepo)
    {
        int id;
        while (true)
        {
            Console.Write("Result ID: ");
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID must be a number.");
                continue;
            }
            if (resultRepo.GetById(id) != null)
            {
                Console.WriteLine("This ID already exists.");
                continue;
            }
            break;
        }

        int studentId;
        while (true)
        {
            Console.Write("Student ID: ");
            if (!int.TryParse(Console.ReadLine(), out studentId) || studentRepo.GetById(studentId) == null)
            {
                Console.WriteLine("Student not found.");
                continue;
            }
            break;
        }

        int examId;
        while (true)
        {
            Console.Write("Exam ID: ");
            if (!int.TryParse(Console.ReadLine(), out examId) || examRepo.GetById(examId) == null)
            {
                Console.WriteLine("Exam not found.");
                continue;
            }
            break;
        }

        double score;
        while (true)
        {
            Console.Write("Score: ");
            if (!double.TryParse(Console.ReadLine(), out score) || score < 0)
            {
                Console.WriteLine("Invalid score.");
                continue;
            }
            break;
        }

        resultRepo.Add(new Result { Id = id, StudentId = studentId, ExamId = examId, Score = score });
        Console.WriteLine("Result added successfully!");
    }
    static void ShowStudentResults(StudentRepository studentRepo, ExamRepository examRepo, ResultRepository resultRepo)
{
    Console.Write("Enter Student ID: ");
    if (!int.TryParse(Console.ReadLine(), out int studentId) || studentRepo.GetById(studentId) == null)
    {
        Console.WriteLine("Student not found.");
        return;
    }

    var results = resultRepo.GetAll().Where(r => r.StudentId == studentId).ToList();
    if (results.Count == 0)
    {
        Console.WriteLine("No results found for this student.");
        return;
    }

    Console.WriteLine($"Results for {studentRepo.GetById(studentId)!.Name}:");
    foreach (var r in results)
    {
        var exam = examRepo.GetById(r.ExamId);
        Console.WriteLine($"{exam!.Subject} ({exam.Date:yyyy-MM-dd}): {r.Score}");
    }
}
    static void ExportResultsToCsv(ResultRepository resultRepo, StudentRepository studentRepo, ExamRepository examRepo)
    {
        var path = Path.Combine("Data", "results.csv");
        using var writer = new StreamWriter(path);
        writer.WriteLine("StudentName,Exam,Score");
        foreach (var r in resultRepo.GetAll())
        {
            var student = studentRepo.GetById(r.StudentId);
            var exam = examRepo.GetById(r.ExamId);
            if (student != null && exam != null)
                writer.WriteLine($"{student.Name},{exam.Subject},{r.Score}");
        }
        Console.WriteLine($"Results exported to {path}");
    }
    static void ShowSearchSortStudents(StudentRepository repo)


    {
        var students = repo.GetAll().ToList();
        if (students.Count == 0)
        {
            Console.WriteLine("No students found.");
            return;
        }

        Console.WriteLine("a) Show All");
        Console.WriteLine("b) Search by Name");
        Console.WriteLine("c) Sort by GPA");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        if (choice == "a")
        {
            foreach (var s in students)
                Console.WriteLine($"{s.Id} - {s.Name} ({s.Level}) GPA: {s.GPA}");
        }
        else if (choice == "b")
        {
            Console.Write("Enter part of name: ");
            var namePart = Console.ReadLine()!;
            var results = students.Where(s => s.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase));
            foreach (var s in results)
                Console.WriteLine($"{s.Id} - {s.Name} ({s.Level}) GPA: {s.GPA}");
        }
        else if (choice == "c")
        {
            var sorted = students.OrderByDescending(s => s.GPA);
            foreach (var s in sorted)
                Console.WriteLine($"{s.Id} - {s.Name} ({s.Level}) GPA: {s.GPA}");
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
    }
    static void SaveAll(StudentRepository sRepo, TeacherRepository tRepo, ExamRepository eRepo, ResultRepository rRepo)

        
    {
        sRepo.Save();
        tRepo.Save();
        eRepo.Save();
        rRepo.Save();
        Console.WriteLine("All data saved successfully!");
    }

    static void LoadAll(StudentRepository sRepo, TeacherRepository tRepo, ExamRepository eRepo, ResultRepository rRepo)
    {
        sRepo.Load();
        tRepo.Load();
        eRepo.Load();
        rRepo.Load();
        Console.WriteLine("All data loaded successfully!");
    }
    static void UpdateStudent(StudentRepository repo)
    {
        Console.Write("Enter Student ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || repo.GetById(id) == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        var student = repo.GetById(id)!;

        Console.Write($"New Name ({student.Name}): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name)) student.Name = name;

        Console.Write($"New Email ({student.Email}): ");
        var email = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(email) && IsValidEmail(email)) student.Email = email;

        Console.Write($"New Level ({student.Level}): ");
        var level = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(level)) student.Level = level;

        Console.Write($"New GPA ({student.GPA}): ");
        var gpaStr = Console.ReadLine();
        if (double.TryParse(gpaStr, out double gpa) && gpa >= 0 && gpa <= 4) student.GPA = gpa;

        repo.Update(student);
        Console.WriteLine("Student updated successfully!");
    }

    static void DeleteStudent(StudentRepository repo)
    {
        Console.Write("Enter Student ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        if (repo.Delete(id))
            Console.WriteLine("Student deleted successfully!");
        else
            Console.WriteLine("Student not found.");
    }

}
