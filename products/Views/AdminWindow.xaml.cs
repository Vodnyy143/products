using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class AdminWindow : Window
{
    public AdminWindow(User user)
    {
        InitializeComponent();
        DataContext = new AdminViewModel(user);
    }
}