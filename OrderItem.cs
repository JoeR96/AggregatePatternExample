namespace AggregatePatternExample;

/// <summary>
/// Entity within the Order aggregate.
/// Note: Constructor is internal - only the aggregate root (Order) can create items.
/// Properties have private setters - only this class can mutate its own state.
/// </summary>
public class OrderItem
{
    public Guid Id { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    // Internal constructor - only Order aggregate can create items
    internal OrderItem(string productName, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be positive", nameof(unitPrice));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Id = Guid.NewGuid();
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    // Internal method - only the Order aggregate can modify quantity
    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
    }
}
