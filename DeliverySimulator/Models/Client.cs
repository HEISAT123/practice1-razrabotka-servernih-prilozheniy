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
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Телефон клиента не может быть пустым.", nameof(phone));
        }

        Name = name.Trim();
        Phone = phone.Trim();
    }

    public override string ToString()
    {
        return $"{Name}, тел. {Phone}";
    }
}