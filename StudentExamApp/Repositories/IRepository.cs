namespace StudentExamApp.Repositories;

public interface IRepository<T>
{
    void Add(T item);
    void Update(T item);
    bool Delete(int id);
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Save();
    void Load();
}
