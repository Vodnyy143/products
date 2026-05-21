using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class ManagerViewModel: ObservableObject
{
       protected readonly AppDbContext _db = new();
    protected List<Product> _allProducts = new();

    public User CurrentUser { get; }
    public string UserFullName => CurrentUser.FullName;

    // Поставщики для фильтра (с "Все поставщики" первым элементом)
    public ObservableCollection<Supplier> Suppliers { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string _searchText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private Supplier? _selectedSupplier;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private bool _sortAscending = true;

    public string SortLabel => SortAscending ? "По возрастанию ▲" : "По убыванию ▼";

    public ManagerViewModel(User user)
    {
        CurrentUser = user;
        LoadProducts();
        LoadSuppliers();
    }

    protected void LoadProducts()
    {
        _allProducts = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.Supplier)
            .Include(p => p.Unit)
            .ToList();

        OnPropertyChanged(nameof(FilteredProducts));
    }

    private void LoadSuppliers()
    {
        Suppliers.Add(new Supplier { Id = 0, Name = "Все поставщики" });
        foreach (var s in _db.Suppliers.OrderBy(s => s.Name))
            Suppliers.Add(s);

        SelectedSupplier = Suppliers.First();
    }

    public IEnumerable<Product> FilteredProducts
    {
        get
        {
            IEnumerable<Product> query = _allProducts;

            if (SelectedSupplier is not null && SelectedSupplier.Id != 0)
                query = query.Where(p => p.SupplierId == SelectedSupplier.Id);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(p =>
                    (p.Name ?? "").ToLower().Contains(s) ||
                    (p.Article ?? "").ToLower().Contains(s) ||
                    (p.Description ?? "").ToLower().Contains(s) ||
                    (p.Category?.Name ?? "").ToLower().Contains(s) ||
                    (p.Manufacturer?.Name ?? "").ToLower().Contains(s) ||
                    (p.Supplier?.Name ?? "").ToLower().Contains(s));
            }

            return SortAscending
                ? query.OrderBy(p => p.Quantity)
                : query.OrderByDescending(p => p.Quantity);
        }
    }

    [RelayCommand]
    private void ToggleSort()
    {
        SortAscending = !SortAscending;
        OnPropertyChanged(nameof(SortLabel));
    }

    [RelayCommand]
    private void OpenOrders()
    {
        new OrderListWindow(CurrentUser).Show();
    }

    [RelayCommand]
    private void Logout(Window currentWindow)
    {
        new LoginWindow().Show();
        currentWindow.Close();
    }
    
}