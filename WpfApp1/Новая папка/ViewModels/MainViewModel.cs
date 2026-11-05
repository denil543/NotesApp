using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly NoteStorage _storage = new();

    public ObservableCollection<Note> Notes { get; } = new();

    private Note? _selectedNote;
    public Note? SelectedNote
    {
        get => _selectedNote;
        set
        {
            if (_selectedNote == value) return;
            SaveCurrent();
            _selectedNote = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSelection));
        }
    }

    public bool HasSelection => SelectedNote != null;

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
    }

    private List<Note> _allNotes = new();
    public List<Note> AllNotes
    {
        get => _allNotes;
        set { _allNotes = value; OnPropertyChanged(); ApplyFilter(); }
    }

    public MainViewModel()
    {
        AllNotes = _storage.Load();
        if (AllNotes.Count == 0)
        {
            AllNotes.Add(new Note
            {
                Title = "Добро пожаловать",
                Content = "Это первая заметка. Создай новую кнопкой «+ Заметка»."
            });
        }
        ApplyFilter();
        SelectedNote = Notes.FirstOrDefault();
    }

    public void AddNote()
    {
        var note = new Note { Title = "Новая заметка", Content = "" };
        AllNotes.Insert(0, note);
        ApplyFilter();
        SelectedNote = note;
        Persist();
    }

    public void DeleteNote()
    {
        if (SelectedNote == null) return;
        AllNotes.Remove(SelectedNote);
        ApplyFilter();
        SelectedNote = Notes.FirstOrDefault();
        Persist();
    }

    public void SaveCurrent()
    {
        if (SelectedNote != null)
            SelectedNote.UpdatedAt = DateTime.Now;
        Persist();
    }

    private void Persist() => _storage.Save(AllNotes);

    private void ApplyFilter()
    {
        var q = SearchText.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(q)
            ? AllNotes
            : AllNotes.Where(n =>
                n.Title.ToLower().Contains(q) ||
                n.Content.ToLower().Contains(q)).ToList();

        Notes.Clear();
        foreach (var n in filtered.OrderByDescending(n => n.UpdatedAt))
            Notes.Add(n);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}