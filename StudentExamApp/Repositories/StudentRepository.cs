using StudentExamApp.Models;
using StudentExamApp.Data;

namespace StudentExamApp.Repositories;

public class StudentRepository : IRepository<Student>
{
    private readonly string _path;
    private List<Student> _students = new();
    private Dictionary<int, Student> _byId = new();

    public StudentRepository(string? dataDirectory = null)
    {
        _path = DataPaths.FilePath("students.json", dataDirectory);
        Load();
    }

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

    public void Save() => JsonStorage.Save(_path, _students);

    public void Load()
    {
        var loaded = JsonStorage.Load<Student>(_path);
        if (loaded.Any(item => item.Id <= 0) || loaded.Select(item => item.Id).Distinct().Count() != loaded.Count)
            throw new InvalidDataException("Invalid or duplicate students IDs in " + _path);
        _students = loaded;
        _byId = _students.ToDictionary(item => item.Id);
    }

}
