using StudentExamApp.Models;
using StudentExamApp.Data;

namespace StudentExamApp.Repositories;

public class TeacherRepository : IRepository<Teacher>
{
    private readonly string _path;
    private List<Teacher> _teachers = new();
    private Dictionary<int, Teacher> _byId = new();

    public TeacherRepository(string? dataDirectory = null)
    {
        _path = DataPaths.FilePath("teachers.json", dataDirectory);
        Load();
    }

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

    public void Save() => JsonStorage.Save(_path, _teachers);

    public void Load()
    {
        var loaded = JsonStorage.Load<Teacher>(_path);
        if (loaded.Any(item => item.Id <= 0) || loaded.Select(item => item.Id).Distinct().Count() != loaded.Count)
            throw new InvalidDataException("Invalid or duplicate teachers IDs in " + _path);
        _teachers = loaded;
        _byId = _teachers.ToDictionary(item => item.Id);
    }

}
