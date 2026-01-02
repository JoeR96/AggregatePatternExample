namespace AggregatePatternExample;

/// <summary>
/// Aggregate Root - controls all access to OrderItems.
/// Notice how:
/// 1. _items is a private List (mutable internally)
/// 2. Items is exposed as IReadOnlyCollection (immutable externally)
/// 3. All mutations happen through public methods that enforce business rules
/// 4. Private methods encapsulate complex internal logic
/// 5. External code can't directly add/remove items - must go through the aggregate
/// </summary>
public class Order
{
    // Private mutable state - internal state management
    private readonly List<OrderItem> _items = new();
    private decimal _discountAmount = 0m;
    private string? _couponCode;

    public Guid Id { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public OrderStatus Status { get; private set; }

    // Public read-only view - encapsulation!
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal SubTotal => _items.Sum(item => item.TotalPrice);
    public decimal DiscountAmount => _discountAmount;
    public decimal TotalAmount => SubTotal - _discountAmount;
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
        ValidateCanModify();

        var existingItem = FindItemByProductName(productName);

        if (existingItem != null)
        {
            MergeWithExistingItem(existingItem, quantity);
        }
        else
        {
            AddNewItem(productName, unitPrice, quantity);
        }

        RecalculateDiscounts();
    }

    // Public interface - only way to remove items
    public void RemoveItem(Guid itemId)
    {
        ValidateCanModify();

        var item = FindItemById(itemId);
        RemoveItemInternal(item);
        RecalculateDiscounts();
    }

    // Public interface - apply coupon code
    public void ApplyCoupon(string couponCode)
    {
        ValidateCanModify();

        if (string.IsNullOrWhiteSpace(couponCode))
            throw new ArgumentException("Coupon code cannot be empty");

        _couponCode = couponCode;
        RecalculateDiscounts();
    }

    // Public interface - business rule enforcement
    public void Submit()
    {
        ValidateCanSubmit();
        TransitionToSubmitted();
    }

    // Public interface - business rule enforcement
    public void Cancel()
    {
        ValidateCanCancel();
        TransitionToCancelled();
    }

    public void Ship()
    {
        ValidateCanShip();
        TransitionToShipped();
    }

    // ============================================
    // PRIVATE METHODS - Internal Logic Encapsulation
    // ============================================

    // Private validation methods - encapsulate business rules
    private void ValidateCanModify()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a submitted order");
    }

    private void ValidateCanSubmit()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order is already submitted");

        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot submit an empty order");
    }

    private void ValidateCanCancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order");
    }

    private void ValidateCanShip()
    {
        if (Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Only submitted orders can be shipped");
    }

    // Private query methods - encapsulate item lookup logic
    private OrderItem? FindItemByProductName(string productName)
    {
        return _items.FirstOrDefault(i =>
            i.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
    }

    private OrderItem FindItemById(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException($"Item with ID {itemId} not found");

        return item;
    }

    // Private mutation methods - encapsulate item management logic
    private void MergeWithExistingItem(OrderItem existingItem, int additionalQuantity)
    {
        existingItem.UpdateQuantity(existingItem.Quantity + additionalQuantity);
    }

    private void AddNewItem(string productName, decimal unitPrice, int quantity)
    {
        var newItem = new OrderItem(productName, unitPrice, quantity);
        _items.Add(newItem);
    }

    private void RemoveItemInternal(OrderItem item)
    {
        _items.Remove(item);
    }

    // Private business logic methods - encapsulate discount calculation
    private void RecalculateDiscounts()
    {
        _discountAmount = 0m;

        ApplyBulkDiscountIfEligible();
        ApplyCouponDiscountIfPresent();
    }

    private void ApplyBulkDiscountIfEligible()
    {
        // Business rule: 10% off orders with 5+ items
        if (_items.Sum(i => i.Quantity) >= 5)
        {
            _discountAmount += SubTotal * 0.10m;
        }
    }

    private void ApplyCouponDiscountIfPresent()
    {
        if (_couponCode == null) return;

        // Business rule: Different coupons apply different discounts
        var couponDiscount = _couponCode.ToUpper() switch
        {
            "SAVE10" => SubTotal * 0.10m,
            "SAVE20" => SubTotal * 0.20m,
            "WELCOME" => 50m, // Flat $50 off
            _ => 0m
        };

        _discountAmount += couponDiscount;
    }

    // Private state transition methods - encapsulate status changes
    private void TransitionToSubmitted()
    {
        Status = OrderStatus.Submitted;
    }

    private void TransitionToCancelled()
    {
        Status = OrderStatus.Cancelled;
    }

    private void TransitionToShipped()
    {
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
