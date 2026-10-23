using System.Globalization;
using DeliverySimulator.Delivery;
using DeliverySimulator.Models;
using DeliverySimulator.Services;

namespace DeliverySimulator.UI;

/// <summary>
/// Консольное меню: читает ввод пользователя и выводит информацию о заказах.
/// </summary>
public class ConsoleMenu
{
    private readonly OrderService _service;

    public ConsoleMenu(OrderService service)
    {
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            PrintMainMenu();

            try
            {
                int choice = ReadInt("Выберите пункт: ", 0, 5);
                Console.WriteLine();
                Console.WriteLine();

                switch (choice)
                {
                    case 0:
                        Console.WriteLine("Работа завершена.");
                        return;
                    case 1:
                        CreateOrder();
                        break;
                    case 2:
                        ShowOrders();
                        break;
                    case 3:
                        ChooseDeliveryMethod();
                        break;
                    case 4:
                        AssignCourier();
                        break;
                    case 5:
                        ChangeStatus();
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("Возврат в главное меню.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Некорректные данные: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Операция невозможна: {ex.Message}");
            }

            Console.WriteLine();
        }
    }


    private static void PrintMainMenu()
    {
        Console.WriteLine();
        Console.WriteLine("===== Служба доставки =====");
        Console.WriteLine("1. Создать заказ");
        Console.WriteLine("2. Список заказов");
        Console.WriteLine("3. Выбрать способ доставки");
        Console.WriteLine("4. Назначить курьера");
        Console.WriteLine("5. Изменить статус заказа");
        Console.WriteLine("0. Выход");
        Console.WriteLine("(вернуться в меню: пустой ввод + Enter)");
    }

    // ---------- Пункты меню ----------

    private void CreateOrder()
    {
        string name = ReadString("Имя клиента: ");
        string phone = ReadPhone("Телефон клиента: ");
        string address = ReadString("Адрес доставки: ");

        Order order = _service.CreateOrder(name, phone, address);
        Console.WriteLine($"Создан заказ №{order.Id}. Добавьте товары.");

        do
        {
            string productName = ReadString("Название товара: ");
            int quantity = ReadInt("Количество: ", 1, 1000);
            decimal price = ReadDecimal("Цена за штуку: ");

            _service.AddItem(order.Id, productName, quantity, price);
            Console.WriteLine("Товар добавлен.");
        }
        while (ReadYesNo("Добавить ещё товар? (д/н): "));

        PrintOrder(order);
    }

    private void ShowOrders()
    {
        if (_service.Orders.Count == 0)
        {
            Console.WriteLine("Заказов пока нет.");
            return;
        }

        foreach (Order order in _service.Orders)
        {
            string delivery = order.DeliveryMethod?.Name ?? "доставка не выбрана";
            Console.WriteLine(
                $"№{order.Id} | {order.Client.Name} | {GetStatusName(order.Status)} | {delivery} | {FormatMoney(order.TotalCost)}");
        }
    }

    private void ChooseDeliveryMethod()
    {
        Order order = ReadOrder();
        IReadOnlyList<IDeliveryMethod> methods = _service.DeliveryMethods;

        Console.WriteLine("Способы доставки:");
        for (int i = 0; i < methods.Count; i++)
        {
            IDeliveryMethod method = methods[i];
            decimal cost = method.CalculateCost(order.ItemsCost);
            Console.WriteLine(
                $"{i + 1}. {method.Name} — {FormatMoney(cost)}, срок: {FormatTime(method.GetEstimatedTime())}");
        }

        int choice = ReadInt("Выберите способ: ", 1, methods.Count);
        _service.SetDeliveryMethod(order.Id, methods[choice - 1]);

        Console.WriteLine("Способ доставки выбран.");
        PrintOrder(order);
    }

    private void AssignCourier()
    {
        Order order = ReadOrder();
        List<Courier> couriers = _service.GetAvailableCouriers();

        if (couriers.Count == 0)
        {
            Console.WriteLine("Свободных курьеров нет.");
            return;
        }

        Console.WriteLine("Свободные курьеры:");
        foreach (Courier courier in couriers)
        {
            Console.WriteLine($"{courier.Id}. {courier}");
        }

        int courierId = ReadInt("Номер курьера: ", 1, int.MaxValue);
        _service.AssignCourier(order.Id, courierId);

        Console.WriteLine("Курьер назначен.");
        PrintOrder(order);
    }

    private void ChangeStatus()
    {
        Order order = ReadOrder();
        Console.WriteLine($"Текущий статус: {GetStatusName(order.Status)}");

        OrderStatus[] statuses = Enum.GetValues<OrderStatus>();
        for (int i = 0; i < statuses.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {GetStatusName(statuses[i])}");
        }

        int choice = ReadInt("Новый статус: ", 1, statuses.Length);
        _service.ChangeStatus(order.Id, statuses[choice - 1]);

        Console.WriteLine("Статус изменён.");
        PrintOrder(order);
    }

    // ---------- Вывод ----------

    private static void PrintOrder(Order order)
    {
        Console.WriteLine();
        Console.WriteLine($"--- Заказ №{order.Id} ({GetStatusName(order.Status)}) ---");
        Console.WriteLine($"Клиент: {order.Client}");
        Console.WriteLine($"Адрес: {order.Address}");
        Console.WriteLine("Товары:");

        foreach (OrderItem item in order.Items)
        {
            Console.WriteLine(
                $"  - {item.ProductName} x {item.Quantity} по {FormatMoney(item.Price)} = {FormatMoney(item.TotalPrice)}");
        }

        if (order.DeliveryMethod == null)
        {
            Console.WriteLine("Доставка: не выбрана");
        }
        else
        {
            IDeliveryMethod method = order.DeliveryMethod;
            decimal cost = method.CalculateCost(order.ItemsCost);
            Console.WriteLine(
                $"Доставка: {method.Name}, {FormatMoney(cost)}, срок: {FormatTime(method.GetEstimatedTime())}");
        }

        Console.WriteLine($"Курьер: {order.Courier?.ToString() ?? "не назначен"}");
        Console.WriteLine($"Итого: {FormatMoney(order.TotalCost)}");
    }

    private static string FormatMoney(decimal amount)
    {
        return $"{amount:0.##} руб.";
    }

    private static string FormatTime(TimeSpan time)
    {
        if (time.TotalDays >= 1)
        {
            return $"{time.TotalDays:0.#} дн.";
        }
        if (time.TotalHours >= 1)
        {
            return $"{time.TotalHours:0.#} ч";
        }
        if (time.TotalMinutes >= 1)
        {
            return $"{time.TotalMinutes:0} мин";
        }

        return "сразу";
    }

    private static string GetStatusName(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.New => "Новый",
            OrderStatus.InProgress => "В пути",
            OrderStatus.Completed => "Доставлен",
            OrderStatus.Cancelled => "Отменён",
            _ => status.ToString()
        };
    }

    // ---------- Ввод ----------

    /// <summary>
    /// Читает строку. Пустой ввод отменяет текущую операцию.
    /// </summary>
    private static string ReadInput(string prompt)
    {
        Console.Write(prompt);
        string input = (Console.ReadLine() ?? "").Trim();

        if (input.Length == 0)
        {
            throw new OperationCanceledException();
        }

        return input;
    }

    private Order ReadOrder()
    {
        if (_service.Orders.Count == 0)
        {
            throw new InvalidOperationException("Сначала создайте заказ.");
        }

        int orderId = ReadInt("Номер заказа: ", 1, int.MaxValue);
        return _service.GetOrder(orderId);
    }

    private static string ReadString(string prompt)
    {
        return ReadInput(prompt);
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string input = ReadInput(prompt);

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            if (max == int.MaxValue)
            {
                Console.WriteLine($"Введите целое число не меньше {min}.");
            }
            else
            {
                Console.WriteLine($"Введите целое число от {min} до {max}.");
            }
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            string input = ReadInput(prompt).Replace(',', '.');

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
                && value >= 0)
            {
                return value;
            }

            Console.WriteLine("Введите неотрицательное число, например 199.99");
        }
    }

    private static string ReadPhone(string prompt)
    {
        while (true)
        {
            string input = ReadInput(prompt);

            if (PhoneValidator.IsValid(input))
            {
                return input;
            }

            Console.WriteLine("Неверный номер. Пример: +7 900 123-45-67 или 89001234567");
        }
    }

    private static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            string input = ReadInput(prompt).ToLower();

            if (input == "д" || input == "да")
            {
                return true;
            }
            if (input == "н" || input == "нет")
            {
                return false;
            }

            Console.WriteLine("Введите «д» или «н».");
        }
    }                                                               

}
