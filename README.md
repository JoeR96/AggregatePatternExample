# Aggregate Pattern - Encapsulation Example

This project demonstrates the **Aggregate Pattern** from Domain-Driven Design (DDD), showcasing how proper encapsulation allows you to focus on public accessors while letting classes internally manage their own data **using private methods**.

## Key Concepts Demonstrated

### 1. Aggregate Root (Order)
The `Order` class is the aggregate root - the only entry point for modifying the aggregate.

**Encapsulation techniques:**
- `_items`, `_discountAmount`, `_couponCode` are **private fields** - mutable internally only
- `Items` is exposed as `IReadOnlyCollection<OrderItem>` - immutable externally
- All properties have **private setters**
- **15+ private methods** encapsulate internal logic
- Public methods are thin wrappers that delegate to private methods
- State changes only through public methods

### 2. Entity (OrderItem)
The `OrderItem` class is an entity within the aggregate.

**Encapsulation techniques:**
- Constructor is **internal** - only `Order` can create instances
- `UpdateQuantity()` is **internal** - only `Order` can modify quantity
- All properties have **private setters**

### 3. Benefits Shown

**External code only thinks about the public interface:**
```csharp
order.AddItem("Laptop", 1200m, 1);
order.Submit();
order.Ship();
```

**Impossible operations (won't compile):**
```csharp
// Can't create items directly
var item = new OrderItem("Monitor", 300m, 1); // ❌ Constructor is internal

// Can't modify the collection
order.Items.Add(item); // ❌ Items is read-only

// Can't modify item quantity
order.Items.First().UpdateQuantity(5); // ❌ Method is internal
```

**Business rules enforced automatically:**
- Can't modify an order after submission
- Can't submit an empty order
- Can't cancel a shipped order
- Duplicate products merge into existing items

### 4. Private Methods - The Key to Encapsulation

The `Order` aggregate has **15+ private methods** that encapsulate internal logic:

**Private Validation Methods:**
- `ValidateCanModify()` - Ensures order is in draft state
- `ValidateCanSubmit()` - Checks if order can be submitted
- `ValidateCanCancel()` - Validates cancellation rules
- `ValidateCanShip()` - Validates shipping rules

**Private Query Methods:**
- `FindItemByProductName()` - Locates items by name
- `FindItemById()` - Locates items by ID

**Private Mutation Methods:**
- `MergeWithExistingItem()` - Combines duplicate products
- `AddNewItem()` - Creates and adds new items
- `RemoveItemInternal()` - Removes items from collection

**Private Business Logic Methods:**
- `RecalculateDiscounts()` - Orchestrates discount calculations
- `ApplyBulkDiscountIfEligible()` - Applies 10% discount for 5+ items
- `ApplyCouponDiscountIfPresent()` - Applies coupon discounts

**Private State Transition Methods:**
- `TransitionToSubmitted()` - Changes status to Submitted
- `TransitionToCancelled()` - Changes status to Cancelled
- `TransitionToShipped()` - Changes status to Shipped

**Why This Matters:**
```csharp
// External code thinks simply:
order.AddItem("Mouse", 25m, 2);

// But internally, the aggregate orchestrates:
ValidateCanModify();              // ✓ Check business rules
FindItemByProductName("Mouse");   // ✓ Find existing item
MergeWithExistingItem(...);       // ✓ Merge if found
RecalculateDiscounts();           // ✓ Update discounts
  ApplyBulkDiscountIfEligible();  //   ✓ Check bulk discount
  ApplyCouponDiscountIfPresent(); //   ✓ Apply coupon
```

External code has **no idea** this complexity exists. You can refactor, optimize, or change private methods without breaking any external code!

### 5. Internal Mutation

The classes freely mutate their own state internally:
- `Order` mutates `_items`, `_discountAmount`, `_couponCode` via private methods
- `OrderItem` mutates `Quantity` via internal `UpdateQuantity()`
- External code is unaware of these internal details
- All complexity is hidden behind simple public methods

## Running the Example

```bash
dotnet run
```

## Project Structure

```
AggregatePatternExample/
├── Order.cs          # Aggregate root
├── OrderItem.cs      # Entity within aggregate
└── Program.cs        # Demo
```

## Encapsulation Summary

| Aspect | Implementation | Benefit |
|--------|---------------|---------|
| Internal state | Private fields (`_items`, `_discountAmount`) | Hidden complexity |
| Internal logic | **Private methods** (15+ methods) | Modular, testable, refactorable |
| Public view | Read-only properties | Safe data access |
| Mutations | Public methods delegate to private methods | Clean separation |
| Business rules | Private validation methods | Enforced consistently |
| Entity creation | Internal constructors | Controlled lifecycle |
| State transitions | Private transition methods | Centralized state management |

**The Power of Private Methods:**
- Public interface stays simple and stable
- Complex logic is broken into small, focused private methods
- Private methods can be refactored without breaking external code
- Each private method has a single responsibility
- Internal implementation can evolve independently

This pattern ensures **invariants are always maintained** because all state changes flow through the aggregate root's public interface, which delegates to well-encapsulated private methods.
