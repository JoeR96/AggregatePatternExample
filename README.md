# Aggregate Pattern - Encapsulation Example

This project demonstrates the **Aggregate Pattern** from Domain-Driven Design (DDD), showcasing how proper encapsulation allows you to focus on public accessors while letting classes internally manage their own data.

## Key Concepts Demonstrated

### 1. Aggregate Root (Order)
The `Order` class is the aggregate root - the only entry point for modifying the aggregate.

**Encapsulation techniques:**
- `_items` is a **private** `List<OrderItem>` - mutable internally
- `Items` is exposed as `IReadOnlyCollection<OrderItem>` - immutable externally
- All properties have **private setters**
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

### 4. Internal Mutation

The classes freely mutate their own state internally:
- `Order` mutates `_items`, `Status`, etc.
- `OrderItem` mutates `Quantity` via `UpdateQuantity()`
- External code is unaware of these internal details

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
| Internal state | Private fields | Hidden complexity |
| Public view | Read-only properties | Safe data access |
| Mutations | Public methods only | Controlled changes |
| Business rules | Enforced in aggregate | Consistency guaranteed |
| Entity creation | Internal constructors | Controlled lifecycle |

This pattern ensures **invariants are always maintained** because all state changes flow through the aggregate root's public interface.
