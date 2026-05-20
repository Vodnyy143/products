using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class OrderListWindow : Window
{
    public OrderListWindow(User user)
    {
        InitializeComponent();
        DataContext = new ClientViewModel(user);
    }
}