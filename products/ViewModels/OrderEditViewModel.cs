using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using products.Models;

namespace products.ViewModels;

public partial class OrderEditViewModel: ObservableObject
{
     private readonly AppDbContext _db = new();
    private readonly bool _isNew;

    [ObservableProperty] private Order _order = null!;
    [ObservableProperty] private string _windowTitle = "";

    public List<OrderStatus> Statuses { get; }
    public List<PickupPoint> PickupPoints { get; }

    public OrderEditViewModel(Order? existing)
    {
        Statuses = _db.OrderStatuses.ToList();
        PickupPoints = _db.PickupPoints.ToList();

        if (existing == null)
        {
            _isNew = true;
            Order = new Order
            {
                OrderDate = DateTime.Today,
                StatusId = Statuses.FirstOrDefault()?.Id ?? 1,
                PickupPointId = PickupPoints.FirstOrDefault()?.Id
            };
            WindowTitle = "Добавление заказа";
        }
        else
        {
            _isNew = false;
            Order = _db.Orders.First(o => o.Id == existing.Id);
            WindowTitle = $"Редактирование заказа: {Order.Article}";
        }
    }

    [RelayCommand]
    private async Task Save(Window window)
    {
        if (string.IsNullOrWhiteSpace(Order.Article))
        {
            MessageBox.Show("Введите артикул заказа",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Order.PickupPointId == null)
        {
            MessageBox.Show("Выберите пункт выдачи",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (_isNew) _db.Orders.Add(Order);
            await _db.SaveChangesAsync();

            window.DialogResult = true;
            window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        window.DialogResult = false;
        window.Close();
    }
    
}