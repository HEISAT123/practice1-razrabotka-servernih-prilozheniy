namespace DeliverySimulator.Delivery;

/// <summary>
/// Стандартная доставка курьером.
/// </summary>
public class StandardDelivery : IDeliveryMethod
{
    public string Name => "Стандартная доставка";

    public bool RequiresCourier => true;

    public decimal CalculateCost(decimal itemsCost)
    {
        // Бесплатно для заказов от 5000 ₽.
        if (itemsCost >= 5000m)
        {
            return 0m;
        }

        return 300m;
    }

    public TimeSpan GetEstimatedTime()
    {
        return TimeSpan.FromDays(2);
    }
}
