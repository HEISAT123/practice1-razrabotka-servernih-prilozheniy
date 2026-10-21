namespace DeliverySimulator.Delivery;

/// <summary>
/// Общий контракт способа доставки.
/// </summary>
public interface IDeliveryMethod
{
    string Name { get; }

    /// <summary>
    /// Определяет, требуется ли для доставки курьер.
    /// </summary>
    bool RequiresCourier { get; }

    decimal CalculateCost(decimal itemsCost);

    TimeSpan GetEstimatedTime();
}
