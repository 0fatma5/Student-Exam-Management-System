using StudentExamApp.Models;
using System.Text.Json;

namespace StudentExamApp.Repositories;

public class TeacherRepository : IRepository<Teacher>
{
    private readonly string _path = Path.Combine("Data", "teachers.json");
    private List<Teacher> _teachers = new();
    private Dictionary<int, Teacher> _byId = new();

    public TeacherRepository() => Load();

    public void Add(Teacher t)
    {
        if (_byId.ContainsKey(t.Id)) throw new Exception("ID already exists");
        _teachers.Add(t);
        _byId[t.Id] = t;
        Save();
    }

    public void Update(Teacher t)
    {
        if (!_byId.ContainsKey(t.Id)) throw new Exception("Teacher not found");
        var idx = _teachers.FindIndex(x => x.Id == t.Id);
        _teachers[idx] = t;
        _byId[t.Id] = t;
        Save();
    }

    public bool Delete(int id)
    {
        if (!_byId.Remove(id)) return false;
        _teachers.RemoveAll(x => x.Id == id);
        Save();
        return true;
    }

    public Teacher? GetById(int id) => _byId.TryGetValue(id, out var t) ? t : null;
    public IEnumerable<Teacher> GetAll() => _teachers;

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_path, JsonSerializer.Serialize(_teachers, options));
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _teachers = JsonSerializer.Deserialize<List<Teacher>>(json) ?? new();
        }
        _byId = _teachers.ToDictionary(t => t.Id);
    }
}
