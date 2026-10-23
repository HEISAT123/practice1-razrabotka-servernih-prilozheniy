using DeliverySimulator.Services;
using DeliverySimulator.UI;

OrderService service = new OrderService();
ConsoleMenu menu = new ConsoleMenu(service);
menu.Run();
