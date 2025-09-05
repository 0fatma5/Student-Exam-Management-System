using StudentExamApp.Models;
using System.Text.Json;

namespace StudentExamApp.Repositories;

public class ResultRepository : IRepository<Result>
{
    private readonly string _path = Path.Combine("Data", "results.json");
    private List<Result> _results = new();
    private Dictionary<int, Result> _byId = new();

    public ResultRepository() => Load();

    public void Add(Result r)
    {
        if (_byId.ContainsKey(r.Id)) throw new Exception("ID already exists");
        _results.Add(r);
        _byId[r.Id] = r;
        Save();
    }

    public void Update(Result r)
    {
        if (!_byId.ContainsKey(r.Id)) throw new Exception("Result not found");
        var idx = _results.FindIndex(x => x.Id == r.Id);
        _results[idx] = r;
        _byId[r.Id] = r;
        Save();
    }

    public bool Delete(int id)
    {
        if (!_byId.Remove(id)) return false;
        _results.RemoveAll(x => x.Id == id);
        Save();
        return true;
    }

    public Result? GetById(int id) => _byId.TryGetValue(id, out var r) ? r : null;
    public IEnumerable<Result> GetAll() => _results;

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_path, JsonSerializer.Serialize(_results, options));
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            _results = JsonSerializer.Deserialize<List<Result>>(json) ?? new();
        }
        _byId = _results.ToDictionary(r => r.Id);
    }
}
