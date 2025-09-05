using StudentExamApp.Models;
using System.Text.Json;

namespace StudentExamApp.Repositories;

public class ExamRepository : IRepository<Exam>
{
    private readonly string _path = Path.Combine("Data", "exams.json");
    private List<Exam> _exams = new();
    private Dictionary<int, Exam> _byId = new();

    public ExamRepository() => Load();

    public void Add(Exam e)
    {
        if (_byId.ContainsKey(e.Id)) throw new Exception("ID already exists");
        _exams.Add(e);
        _byId[e.Id] = e;
        Save();
    }

    public void Update(Exam e)
    {
        if (!_byId.ContainsKey(e.Id)) throw new Exception("Exam not found");
        var idx = _exams.FindIndex(x => x.Id == e.Id);
        _exams[idx] = e;
        _byId[e.Id] = e;
        Save();
    }

    public bool Delete(int id)
    {
        if (!_byId.Remove(id)) return false;
        _exams.RemoveAll(x => x.Id == id);
        Save();
        return true;
    }

    public Exam? GetById(int id) => _byId.TryGetValue(id, out var e) ? e : null;
    public IEnumerable<Exam> GetAll() => _exams;

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_path, JsonSerializer.Serialize(_exams, options));
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _exams = JsonSerializer.Deserialize<List<Exam>>(json) ?? new();
        }
        _byId = _exams.ToDictionary(e => e.Id);
    }
}
