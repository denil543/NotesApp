using System.IO;
using System.Text.Json;
using WpfApp1.Models;

namespace WpfApp1.Services;

public class NoteStorage
{
    private readonly string _path;

    public NoteStorage()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NotesApp");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "notes.json");
    }

    public List<Note> Load()
    {
        if (!File.Exists(_path)) return new List<Note>();
        try
        {
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
        }
        catch
        {
            return new List<Note>();
        }
    }

    public void Save(IEnumerable<Note> notes)
    {
        var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_path, json);
    }
}