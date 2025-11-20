namespace NotionAutomation.Api.Helpers;

public static class DirectoryHelper
{
    public static void CreateFolderIfNecessary(string? fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath)) throw new ArgumentNullException(nameof(fullPath));
        var folderPath = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));
        Directory.CreateDirectory(folderPath);
    }
}