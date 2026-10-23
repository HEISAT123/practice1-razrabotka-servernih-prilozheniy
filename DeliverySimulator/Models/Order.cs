using DeliverySimulator.Delivery;

namespace DeliverySimulator.Models;

/// <summary>
/// Заказ службы доставки.
/// </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();

    public int Id { get; }
    public Client Client { get; }
    public string Address { get; }
    public IReadOnlyList<OrderItem> Items => _items;

    public IDeliveryMethod? DeliveryMethod { get; private set; }
    public Courier? Courier { get; private set; }
    public OrderStatus Status { get; private set; }

    public Order(int id, Client client, string address)
    {
        if (client == null)
        {
            throw new ArgumentException("Не указан клиент.");
        }
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Адрес не может быть пустым.");
        }

        Id = id;
        Client = client;
        Address = address.Trim();
        Status = OrderStatus.New;
    }

    /// <summary>
    /// Стоимость товаров без доставки.
    /// </summary>
    public decimal ItemsCost => _items.Sum(item => item.TotalPrice);

    /// <summary>
    /// Итоговая стоимость: товары + доставка.
    /// </summary>
    public decimal TotalCost
    {
        get
        {
            if (DeliveryMethod == null)
            {
                return ItemsCost;
            }

            return ItemsCost + DeliveryMethod.CalculateCost(ItemsCost);
        }
    }


    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.New)
        {
            throw new InvalidOperationException("Добавлять товары можно только в новый заказ.");
        }

        _items.Add(item);
    }

    public void SetDeliveryMethod(IDeliveryMethod method)
    {
        if (Status != OrderStatus.New)
        {
            throw new InvalidOperationException("Способ доставки можно менять только у нового заказа.");
        }

        if (!method.RequiresCourier && Courier != null)
        {
            Courier.Release();
            Courier = null;
        }

        DeliveryMethod = method;
    }


    public void AssignCourier(Courier courier)
    {
        if (Status != OrderStatus.New)
        {
            throw new InvalidOperationException("Назначить курьера можно только у нового заказа.");
        }

        if (DeliveryMethod == null)
        {
            throw new InvalidOperationException("Сначала выберите способ доставки.");
        }
        if (!DeliveryMethod.RequiresCourier)
        {
            throw new InvalidOperationException("Для этого способа доставки курьер не нужен.");
        }
        if (Courier != null)
        {
            throw new InvalidOperationException("Курьер уже назначен.");
        }

        courier.TakeOrder();
        Courier = courier;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        bool allowed =
            (Status == OrderStatus.New && newStatus == OrderStatus.InProgress) ||
            (Status == OrderStatus.New && newStatus == OrderStatus.Cancelled) ||
            (Status == OrderStatus.InProgress && newStatus == OrderStatus.Completed) ||
            (Status == OrderStatus.InProgress && newStatus == OrderStatus.Cancelled);

        if (!allowed)
        {
            throw new InvalidOperationException($"Нельзя перейти из {Status} в {newStatus}.");
        }

        if (newStatus == OrderStatus.InProgress)
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("В заказе нет товаров.");
            }
            if (DeliveryMethod == null)
            {
                throw new InvalidOperationException("Не выбран способ доставки.");
            }
            if (DeliveryMethod.RequiresCourier && Courier == null)
            {
                throw new InvalidOperationException("Не назначен курьер.");
            }
        }

        // Заказ закрыт — курьер свободен.
        if (newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled)
        {
            Courier?.Release();
        }

        Status = newStatus;
    }
}
