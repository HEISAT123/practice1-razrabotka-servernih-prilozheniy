using DeliverySimulator.Models;

Courier courier = new Courier(1,"Иванов Иван", "1234567890");

courier.TakeOrder();
courier.TakeOrder();
Console.WriteLine(courier.IsBusy);