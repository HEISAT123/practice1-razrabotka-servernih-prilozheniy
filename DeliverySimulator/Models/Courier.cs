namespace DeliverySimulator.Models;

public class Courier
{
    public int Id { get; }
    public string Name { get; }
    public string Phone { get; }
    public bool IsBusy { get; private set; }

    public Courier(int id, string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Имя курьера не может быть пустым.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Телефон курьера не может быть пустым.", nameof(phone));
        }
        if (id <= 0)
        {
            throw new ArgumentException("Id курьера должен быть положительным числом.", nameof(id));
        }
        
        Id = id;
        Name = name.Trim();
        Phone = phone.Trim();
        IsBusy = false;
    }

    public void TakeOrder()
    {
        if (IsBusy)
        {
            throw new InvalidOperationException($"Курьер {Name} уже занят другим заказом.");
        }

        IsBusy = true;
    }

    public void Release()
    {
        IsBusy = false;
    }

    public override string ToString()
    {
        return $"{Name}, тел. {Phone}";
    }
}