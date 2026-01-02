using AggregatePatternExample;

Console.WriteLine("=== Aggregate Pattern Encapsulation Demo ===\n");

// Create an order (aggregate root)
var order = new Order();
Console.WriteLine($"Order created: {order.Id}");
Console.WriteLine($"Status: {order.Status}\n");

// Add items through the public interface
Console.WriteLine("Adding items to order...");
order.AddItem("Laptop", 1200.00m, 1);
order.AddItem("Mouse", 25.00m, 2);
order.AddItem("Keyboard", 75.00m, 1);

// Note: We CANNOT do this (Items is read-only):
// order.Items.Add(new OrderItem(...)); // Won't compile!

// Note: We CANNOT do this (OrderItem constructor is internal):
// var item = new OrderItem("Monitor", 300m, 1); // Won't compile!

Console.WriteLine($"Items in order: {order.ItemCount}");
Console.WriteLine($"Total amount: ${order.TotalAmount:F2}\n");

// Display items (read-only access is fine)
Console.WriteLine("Order contents:");
foreach (var item in order.Items)
{
    Console.WriteLine($"  - {item.ProductName}: {item.Quantity} x ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
}

// Add more of an existing product - aggregate handles the logic
Console.WriteLine("\nAdding 2 more mice...");
order.AddItem("Mouse", 25.00m, 2);

Console.WriteLine($"Items in order: {order.ItemCount}"); // Still 3 unique items
Console.WriteLine($"Total amount: ${order.TotalAmount:F2}\n");

Console.WriteLine("Updated order contents:");
foreach (var item in order.Items)
{
    Console.WriteLine($"  - {item.ProductName}: {item.Quantity} x ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
}

// Submit the order
Console.WriteLine("\nSubmitting order...");
order.Submit();
Console.WriteLine($"Status: {order.Status}");

// Try to modify after submission (will fail with business rule)
Console.WriteLine("\nTrying to add item after submission...");
try
{
    order.AddItem("Monitor", 300.00m, 1);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// Ship the order
Console.WriteLine("\nShipping order...");
order.Ship();
Console.WriteLine($"Status: {order.Status}");

// Try to cancel after shipping (will fail with business rule)
Console.WriteLine("\nTrying to cancel after shipping...");
try
{
    order.Cancel();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\n=== Key Encapsulation Points ===");
Console.WriteLine("1. OrderItem constructor is internal - only Order can create them");
Console.WriteLine("2. OrderItem.UpdateQuantity() is internal - only Order can call it");
Console.WriteLine("3. Order._items is private - external code can't access it");
Console.WriteLine("4. Order.Items is IReadOnlyCollection - external code can read but not modify");
Console.WriteLine("5. All mutations go through Order's public methods");
Console.WriteLine("6. Order enforces business rules (can't modify submitted orders, etc.)");
Console.WriteLine("7. External code only needs to know the public interface");
