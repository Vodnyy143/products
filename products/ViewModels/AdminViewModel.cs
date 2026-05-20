using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class AdminViewModel: ManagerViewModel
{
    private ProductEditWindow? _editWindow;
    private readonly AppDbContext _db = new();
    
    public AdminViewModel(User user): base(user)
    {
    }

    [RelayCommand]
    public void AddProduct()
    {
        OpenEditWindow(null);
    }
    
    [RelayCommand]
    public void EditProduct(Product? product)
    {
        if (product == null) return;
        OpenEditWindow(product);
    }

    private void OpenEditWindow(Product? product)
    {
        if (_editWindow != null && _editWindow.IsVisible)
        {
            _editWindow.Activate();
            return;
        }
        
        _editWindow = new ProductEditWindow(product);
        bool? result =  _editWindow.ShowDialog();

        if (result == true)
        {
            LoadProducts();
        }
        
    }

    [RelayCommand]
    public async Task DeleteProduct(Product product)
    {
        if (product == null) return;

        bool inOrder = await _db.OrderProducts.AnyAsync(op => op.ProductId == product.Id);

        if (inOrder)
        {
            MessageBox.Show("You can't delete this product!");
            return;
        }
        
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        
        LoadProducts();
        
        MessageBox.Show("Product deleted successfully!");
    }

}