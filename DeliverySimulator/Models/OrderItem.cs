namespace DeliverySimulator.Models;

public class OrderItem
{
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    public OrderItem(string productName, int quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Название товара не может быть пустым.", nameof(productName));
        }
        if (quantity <= 0)
        {
            throw new ArgumentException("Количество должно быть больше нуля.", nameof(quantity));
        }
        if (price < 0)
        {
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(price));
        }

        ProductName = productName.Trim();
        Quantity = quantity;
        Price = price;
    }

    /// <summary>
    /// Стоимость позиции
    /// </summary>
    public decimal TotalPrice => Price * Quantity;
}