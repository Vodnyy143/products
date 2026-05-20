using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class ClientViewModel: ObservableObject
{
    private readonly AppDbContext _db = new();

    public ObservableCollection<Product> Products { get; } = new();
    public User CurrentUser { get; }

    // Для отображения ФИО в правом верхнем углу
    public string UserFullName => CurrentUser.FullName;

    public ClientViewModel(User user)
    {
        CurrentUser = user;
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