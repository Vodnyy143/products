using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class ManagerWindow : Window
{
    public ManagerWindow(User user)
    {
        InitializeComponent();
        DataContext = new ManagerViewModel(user);
    }
}