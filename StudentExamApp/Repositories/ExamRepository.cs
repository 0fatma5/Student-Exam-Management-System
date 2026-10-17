using StudentExamApp.Models;
using StudentExamApp.Data;

namespace StudentExamApp.Repositories;

public class ExamRepository : IRepository<Exam>
{
    private readonly string _path;
    private List<Exam> _exams = new();
    private Dictionary<int, Exam> _byId = new();

    public ExamRepository(string? dataDirectory = null)
    {
        _path = DataPaths.FilePath("exams.json", dataDirectory);
        Load();
    }

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

    public void Save() => JsonStorage.Save(_path, _exams);

    public void Load()
    {
        var loaded = JsonStorage.Load<Exam>(_path);
        if (loaded.Any(item => item.Id <= 0) || loaded.Select(item => item.Id).Distinct().Count() != loaded.Count)
            throw new InvalidDataException("Invalid or duplicate exams IDs in " + _path);
        _exams = loaded;
        _byId = _exams.ToDictionary(item => item.Id);
    }

}
