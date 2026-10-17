namespace StudentExamApp.Data;

/// <summary>Locates JSON/CSV files regardless of where the app is started.</summary>
public static class DataPaths
{
    public static string ResolveDirectory(string? explicitDirectory = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitDirectory))
            return Path.GetFullPath(explicitDirectory);

        var configured = Environment.GetEnvironmentVariable("STUDENT_EXAM_DATA_DIR");
        if (!string.IsNullOrWhiteSpace(configured))
            return Path.GetFullPath(configured);

        var workingDirectory = Directory.GetCurrentDirectory();
        var nested = Path.Combine(workingDirectory, "StudentExamApp", "Data");
        if (Directory.Exists(nested)) return nested;

        var direct = Path.Combine(workingDirectory, "Data");
        if (Directory.Exists(direct)) return direct;

        // Fresh install: create an independent data directory next to the app binary.
        return Path.Combine(AppContext.BaseDirectory, "Data");
    }

    public static string FilePath(string fileName, string? explicitDirectory = null)
    {
        if (Path.GetFileName(fileName) != fileName || string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Provide a file name, not a path.", nameof(fileName));

        return Path.Combine(ResolveDirectory(explicitDirectory), fileName);
    }
}
