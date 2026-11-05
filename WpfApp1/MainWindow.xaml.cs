using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        Closing += (_, _) => _vm.SaveCurrent();
    }

    private void AddNote_Click(object sender, RoutedEventArgs e) => _vm.AddNote();
    private void DeleteNote_Click(object sender, RoutedEventArgs e) => _vm.DeleteNote();
}