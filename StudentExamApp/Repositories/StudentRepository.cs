using StudentExamApp.Models;
using System.Text.Json;

namespace StudentExamApp.Repositories;

public class StudentRepository : IRepository<Student>
{
    private readonly string _path = Path.Combine("Data", "students.json");
    private List<Student> _students = new();
    private Dictionary<int, Student> _byId = new();

    public StudentRepository() => Load();

    public void Add(Student s)
    {
        if (_byId.ContainsKey(s.Id)) throw new Exception("ID already exists");
        _students.Add(s);
        _byId[s.Id] = s;
        Save();
    }

    public void Update(Student s)
    {
        if (!_byId.ContainsKey(s.Id)) throw new Exception("Student not found");
        var idx = _students.FindIndex(x => x.Id == s.Id);
        _students[idx] = s;
        _byId[s.Id] = s;
        Save();
    }

    public bool Delete(int id)
    {
        if (!_byId.Remove(id)) return false;
        _students.RemoveAll(x => x.Id == id);
        Save();
        return true;
    }

    public Student? GetById(int id) => _byId.TryGetValue(id, out var s) ? s : null;
    public IEnumerable<Student> GetAll() => _students;

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_path, JsonSerializer.Serialize(_students, options));
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _students = JsonSerializer.Deserialize<List<Student>>(json) ?? new();
        }
        _byId = _students.ToDictionary(s => s.Id);
    }
}
