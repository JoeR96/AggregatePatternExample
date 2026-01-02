namespace AggregatePatternExample;

/// <summary>
/// Aggregate Root - controls all access to OrderItems.
/// Notice how:
/// 1. _items is a private List (mutable internally)
/// 2. Items is exposed as IReadOnlyCollection (immutable externally)
/// 3. All mutations happen through public methods that enforce business rules
/// 4. External code can't directly add/remove items - must go through the aggregate
/// </summary>
public class Order
{
    // Private mutable collection - internal state management
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public OrderStatus Status { get; private set; }

    // Public read-only view - encapsulation!
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);
    public int ItemCount => _items.Count;

    public Order()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        Status = OrderStatus.Draft;
    }

    // Public interface - only way to add items
    public void AddItem(string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a submitted order");

        // Check if item already exists
        var existingItem = _items.FirstOrDefault(i => i.ProductName == productName);

        if (existingItem != null)
        {
            // Update existing item quantity
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            // Add new item - aggregate controls creation
            var newItem = new OrderItem(productName, unitPrice, quantity);
            _items.Add(newItem);
        }
    }

    // Public interface - only way to remove items
    public void RemoveItem(Guid itemId)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a submitted order");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException($"Item with ID {itemId} not found");

        _items.Remove(item);
    }

    // Public interface - business rule enforcement
    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order is already submitted");

        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot submit an empty order");

        Status = OrderStatus.Submitted;
    }

    // Public interface - business rule enforcement
    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order");

        Status = OrderStatus.Cancelled;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Only submitted orders can be shipped");

        Status = OrderStatus.Shipped;
    }
}

public enum OrderStatus
{
    Draft,
    Submitted,
    Shipped,
    Cancelled
}
