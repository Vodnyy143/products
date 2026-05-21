using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class OrderEditWindow : Window
{
    public OrderEditWindow(Order? order)
    {
        InitializeComponent();
        DataContext = new OrderEditViewModel(order);
    }
}