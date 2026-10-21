namespace DeliverySimulator.Delivery;

/// <summary>
/// Экспресс-доставка курьером.
/// </summary>
public class ExpressDelivery : IDeliveryMethod
{
    public string Name => "Экспресс-доставка";

    public bool RequiresCourier => true;

    public decimal CalculateCost(decimal itemsCost)
    {
        // 500 ₽ + 10% от суммы товаров
        return 500m + itemsCost * 0.1m;
    }

    public TimeSpan GetEstimatedTime()
    {
        return TimeSpan.FromHours(3);
    }
}