using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class GuestViewModel: ObservableObject
{
    private readonly AppDbContext _db = new();

    public ObservableCollection<Product> Products { get; } = new();

    public GuestViewModel()
    {
        foreach (var product in _db.Products
                     .Include(p => p.Category)
                     .Include(p => p.Manufacturer)
                     .Include(p => p.Supplier)
                     .Include(p => p.Unit)
                     .ToList())
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private void Logout(Window currentWindow)
    {
        new LoginWindow().Show();
        currentWindow.Close();
    }

}