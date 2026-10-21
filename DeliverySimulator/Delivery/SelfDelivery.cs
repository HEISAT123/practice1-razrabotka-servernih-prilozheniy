namespace DeliverySimulator.Delivery;
/// <summary>
/// Самовывоз
/// </summary>
public class SelfDelivery : IDeliveryMethod
{
    public string Name => "Самовывоз";

    public bool RequiresCourier => false;

    public decimal CalculateCost(decimal itemsCost)
    {
        return 0m;
    }

    public TimeSpan GetEstimatedTime()
    {
        return TimeSpan.FromDays(0);
    }
}