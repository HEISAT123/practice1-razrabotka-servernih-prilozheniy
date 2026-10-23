namespace DeliverySimulator.Models;

public class Client
{
    public string Name { get; }
    public string Phone { get; }

    public Client(string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Имя клиента не может быть пустым.", nameof(name));
        }

        Name = name.Trim();
        Phone = PhoneValidator.Normalize(phone);
    }

    public override string ToString()
    {
        return $"{Name}, тел. {Phone}";
    }
}