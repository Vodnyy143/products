using System.Windows;
using products.Models;
using products.ViewModels;

namespace products.Views;

public partial class ProductEditWindow : Window
{
    public ProductEditWindow(Product product)
    {
        InitializeComponent();
        DataContext = new ProductEditViewModel(product);
    }
}