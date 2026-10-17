using System.Text.Json;

namespace StudentExamApp.Data;

/// <summary>Atomic JSON persistence; does not replace valid data after a failed write.</summary>
public static class JsonStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static List<T> Load<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        try
        {
            var contents = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(contents)
                ?? throw new InvalidDataException($"JSON file is empty or invalid: {path}");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Invalid JSON in {path}. Back up or repair the file before retrying.", ex);
        }
    }

    public static void Save<T>(string path, IReadOnlyList<T> items)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path))
            ?? throw new InvalidOperationException("Unable to determine data directory.");
        Directory.CreateDirectory(directory);

        // Write to a different file first, then replace the destination.
        // This avoids partial JSON when serialization/write fails.
        var temporaryPath = Path.Combine(directory, Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(items, Options));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
}
