using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class ClientWindow : Window
{
    public ClientWindow(User user)
    {
        InitializeComponent();

        DataContext = new ClientViewModel(user);
    }
}