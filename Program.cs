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

Console.WriteLine($"Subtotal: ${order.SubTotal:F2}");
Console.WriteLine($"Discount: ${order.DiscountAmount:F2}");
Console.WriteLine($"Total: ${order.TotalAmount:F2}\n");

// Note: We CANNOT do these things (encapsulation prevents it):
// order.Items.Add(new OrderItem(...)); // ❌ Won't compile - Items is read-only
// var item = new OrderItem("Monitor", 300m, 1); // ❌ Won't compile - constructor is internal
// order._discountAmount = 100m; // ❌ Won't compile - field is private
// order.RecalculateDiscounts(); // ❌ Won't compile - method is private
// order.ValidateCanModify(); // ❌ Won't compile - method is private

// Add more mice - aggregate uses PRIVATE method MergeWithExistingItem()
Console.WriteLine("Adding 2 more mice (triggers private MergeWithExistingItem method)...");
order.AddItem("Mouse", 25.00m, 2);

Console.WriteLine($"Items in order: {order.ItemCount}"); // Still 3 unique items
Console.WriteLine($"Subtotal: ${order.SubTotal:F2}");
Console.WriteLine($"Discount: ${order.DiscountAmount:F2}");
Console.WriteLine($"Total: ${order.TotalAmount:F2}\n");

Console.WriteLine("Order contents:");
foreach (var item in order.Items)
{
    Console.WriteLine($"  - {item.ProductName}: {item.Quantity} x ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
}

// Add more items to trigger bulk discount (private method ApplyBulkDiscountIfEligible)
Console.WriteLine("\nAdding monitor to trigger bulk discount (5+ items)...");
order.AddItem("Monitor", 300.00m, 1);

Console.WriteLine($"Subtotal: ${order.SubTotal:F2}");
Console.WriteLine($"Discount: ${order.DiscountAmount:F2} (10% bulk discount automatically applied!)");
Console.WriteLine($"Total: ${order.TotalAmount:F2}");
Console.WriteLine("^ Notice: Private method ApplyBulkDiscountIfEligible() was called internally!\n");

// Apply coupon code (triggers private method ApplyCouponDiscountIfPresent)
Console.WriteLine("Applying coupon code 'SAVE20' (triggers private RecalculateDiscounts)...");
order.ApplyCoupon("SAVE20");

Console.WriteLine($"Subtotal: ${order.SubTotal:F2}");
Console.WriteLine($"Discount: ${order.DiscountAmount:F2} (bulk + coupon discount!)");
Console.WriteLine($"Total: ${order.TotalAmount:F2}");
Console.WriteLine("^ Notice: Multiple private methods coordinated the discount calculation!\n");

// Submit the order (uses private ValidateCanSubmit and TransitionToSubmitted)
Console.WriteLine("Submitting order (calls private validation & transition methods)...");
order.Submit();
Console.WriteLine($"Status: {order.Status}\n");

// Try to modify after submission (private ValidateCanModify will throw)
Console.WriteLine("Trying to add item after submission...");
try
{
    order.AddItem("Headphones", 100.00m, 1);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine("^ Private method ValidateCanModify() enforced this rule!\n");
}

// Ship the order (uses private ValidateCanShip and TransitionToShipped)
Console.WriteLine("Shipping order...");
order.Ship();
Console.WriteLine($"Status: {order.Status}\n");

// Try to cancel after shipping (private ValidateCanCancel will throw)
Console.WriteLine("Trying to cancel after shipping...");
try
{
    order.Cancel();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine("^ Private method ValidateCanCancel() enforced this rule!\n");
}

Console.WriteLine("=== Key Encapsulation Points ===");
Console.WriteLine("1. Private fields: _items, _discountAmount, _couponCode");
Console.WriteLine("2. Private validation methods: ValidateCanModify, ValidateCanSubmit, etc.");
Console.WriteLine("3. Private business logic: RecalculateDiscounts, ApplyBulkDiscountIfEligible, etc.");
Console.WriteLine("4. Private query methods: FindItemByProductName, FindItemById");
Console.WriteLine("5. Private mutation methods: MergeWithExistingItem, AddNewItem, RemoveItemInternal");
Console.WriteLine("6. Private state transitions: TransitionToSubmitted, TransitionToCancelled, etc.");
Console.WriteLine("7. Public methods are simple - they delegate to private methods");
Console.WriteLine("8. External code ONLY thinks about public interface (AddItem, Submit, Ship)");
Console.WriteLine("9. Complex logic is hidden and can be refactored without breaking external code!");
