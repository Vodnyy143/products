using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class  OrderListViewModel: ObservableObject
{
    private readonly AppDbContext _db = new();

    public ObservableCollection<Order> Orders { get; } = new();
    
    public User CurrentUser { get; }
    public bool IsAdmin => CurrentUser.Role?.Name == "Администратор";
    
    private OrderEditWindow? _orderEditWindow;
    
    public OrderListViewModel(User user)
    {
        CurrentUser = user;
        LoadOrders();
    }

    private void LoadOrders()
    {
        Orders.Clear();  
        
        foreach (var order in _db.Orders
                     .Include(o => o.Status)
                     .Include(o => o.PickupPoint)
                     .OrderByDescending(o => o.OrderDate)
                     .ToList())
        {
            Orders.Add(order);
        }
    }

    [RelayCommand]
    private void AddOrder()
    {
        if (!IsAdmin)
        {
            MessageBox.Show("Только админ может добавлять заказы");
            return;
        }
        OpenEditWindow(null);  // ✅ открываем форму для нового заказа
    }

    private void OpenEditWindow(Order? order)
    {
        if (_orderEditWindow != null && _orderEditWindow.IsVisible)
        {
            _orderEditWindow.Activate();
            return;
        }

        _orderEditWindow = new OrderEditWindow(order);
        bool? result = _orderEditWindow.ShowDialog();
        
        if(result == true) 
            LoadOrders();
    }
    
    [RelayCommand]
    private void EditOrder(Order? order)
    {
        if (order == null) return;
        if (!IsAdmin)
        {
            MessageBox.Show("Только администратор может редактировать заказы",
                "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        OpenEditWindow(order);
    }

    [RelayCommand]
    private async Task DeleteOrder(Order? order)
    {
        if (order == null) return;
        if (!IsAdmin)
        {
            MessageBox.Show("Только администратор может удалять заказы");
            return;
        }
        
        
        var confirm = MessageBox.Show(
            $"Удалить заказ {order.Article}?\nЭто действие необратимо.",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        LoadOrders();
    }
    
    [RelayCommand]
    private void Close(Window window) => window.Close();
}