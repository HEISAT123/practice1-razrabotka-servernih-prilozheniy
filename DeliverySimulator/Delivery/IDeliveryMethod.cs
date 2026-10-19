namespace DeliverySimulator.Delivery;

/// <summary>
/// Общий контракт способа доставки.
/// </summary>
public interface IDeliveryMethod
{
    string Name { get; }

    bool RequiresCourier { get; }

    decimal CalculateCost(decimal itemsCost);

    TimeSpan GetEstimatedTime();
}
