using StudentExamApp.Models;
using StudentExamApp.Data;

namespace StudentExamApp.Repositories;

public class ResultRepository : IRepository<Result>
{
    private readonly string _path;
    private List<Result> _results = new();
    private Dictionary<int, Result> _byId = new();

    public ResultRepository(string? dataDirectory = null)
    {
        _path = DataPaths.FilePath("results.json", dataDirectory);
        Load();
    }

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

    public void Save() => JsonStorage.Save(_path, _results);

    public void Load()
    {
        var loaded = JsonStorage.Load<Result>(_path);
        if (loaded.Any(item => item.Id <= 0) || loaded.Select(item => item.Id).Distinct().Count() != loaded.Count)
            throw new InvalidDataException("Invalid or duplicate results IDs in " + _path);
        _results = loaded;
        _byId = _results.ToDictionary(item => item.Id);
    }

}
