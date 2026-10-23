using DeliverySimulator.Delivery;
using DeliverySimulator.Models;

namespace DeliverySimulator.Services;

/// <summary>
/// Сервис управления заказами: хранит заказы, курьеров и способы доставки.
/// </summary>
public class OrderService
{
    private readonly List<Order> _orders = new();
    private readonly List<Courier> _couriers = new();
    private readonly List<IDeliveryMethod> _deliveryMethods = new();
    private int _nextOrderId = 1;

    public OrderService()
    {
        _couriers.Add(new Courier(1, "Иван Иванов", "+7 900 111-22-33"));
        _couriers.Add(new Courier(2, "Петр Петрович", "+7 900 444-55-66"));
        _couriers.Add(new Courier(3, "Олег Олегович", "+7 900 777-88-99"));
        _couriers.Add(new Courier(4, "Андрей Андреевич", "+7 999 123-32-23"));
        _couriers.Add(new Courier(5, "Алексей Алексеевич", "+7 966 432-43-23"));

        _deliveryMethods.Add(new StandardDelivery());
        _deliveryMethods.Add(new ExpressDelivery());
        _deliveryMethods.Add(new SelfDelivery());
    }

    public IReadOnlyList<Order> Orders => _orders;

    public IReadOnlyList<Courier> Couriers => _couriers;

    public IReadOnlyList<IDeliveryMethod> DeliveryMethods => _deliveryMethods;

    public Order CreateOrder(string clientName, string clientPhone, string address)
    {
        Client client = new Client(clientName, clientPhone);
        Order order = new Order(_nextOrderId, client, address);

        _orders.Add(order);
        _nextOrderId++;

        return order;
    }

    public void AddItem(int orderId, string productName, int quantity, decimal price)
    {
        Order order = GetOrder(orderId);
        OrderItem item = new OrderItem(productName, quantity, price);

        order.AddItem(item);
    }

    public void SetDeliveryMethod(int orderId, IDeliveryMethod method)
    {
        Order order = GetOrder(orderId);

        order.SetDeliveryMethod(method);
    }

    /// <summary>
    /// Возвращает курьеров, которые сейчас свободны.
    /// </summary>
    public List<Courier> GetAvailableCouriers()
    {
        return _couriers.Where(c => !c.IsBusy).ToList();
    }

    /// <summary>
    /// Назначает курьера на заказ.
    /// </summary>
    public void AssignCourier(int orderId, int courierId)
    {
        Order order = GetOrder(orderId);
        Courier courier = GetCourier(courierId);

        order.AssignCourier(courier);
    }

    public void ChangeStatus(int orderId, OrderStatus newStatus)
    {
        Order order = GetOrder(orderId);

        order.ChangeStatus(newStatus);
    }

    public Order GetOrder(int orderId)
    {
        Order? order = _orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Заказ №{orderId} не найден.");
        }

        return order;
    }

    private Courier GetCourier(int courierId)
    {
        Courier? courier = _couriers.FirstOrDefault(c => c.Id == courierId);

        if (courier == null)
        {
            throw new InvalidOperationException($"Курьер №{courierId} не найден.");
        }

        return courier;
    }
}
