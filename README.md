# LeanSharp

## About
LeanSharp allows you to write code in a more declarative and functional way in C#.

By using LeanSharp, a lot of boilerplate and imperative code can be removed from your codebase. After looking at the Getting Started section, you can use the unit tests in the repository to help you understand how to use each Class/Method.

One of the things provided by this project is the potential to use Railway-Oriented Programming (ROP) in C#. The code related to ROP was initially taken from https://github.com/habaneroofdoom/AltNetRop and then based on real world usage it was grown and tweaked to its current state. In the same way, the Maybe monad used in here was taken from https://gist.github.com/johnazariah/d95c03e2c56579c11272a647bab4bc38, feel free to navigate there to see a good explanation about it.

## Getting Started
Install LeanSharp Nuget Package:

```shell
Install-Package LeanSharp
```

Now make sure you add a namespace reference in the C# file you want to start using it:

```csharp
using LeanSharp;
```

## Examples:
### Railway-Oriented Programming (ROP):
```csharp
var success = Result<int, string>.Succeeded(2);
var newResult = success.Map(two => two + 3); // Success(5)
var finalResult = newResult.Bind(five => Result<int, string>.Succeeded(five + 5)); // Success(10)
```
Applying chaining:
```csharp
Result<int, string>.Succeeded(2)
   .Map(two => two + 3)
   .Bind(five => Result<int, string>.Succeeded(five + 5))
```
ROP is a functional approach to handling errors. The Result class is more commonly known as the Either Monad, which could be either a Right (success) or a Left (error). This allows us to get rid of Exceptions side-effect:

```csharp
public async Result<Task<Customer>, Exception> Insert(Customer customer)
{
  try
  {
      // Try to insert Customer into the DB.
      // ....
      return Result<Customer, Exception>.Succeeded(customer);
  }
  catch (Exception ex) 
  {
      return Result<Customer, Exception>.Failed(ex);
  }
}
```
You can take it even further and convert the try/catch statement to an expression, in which the boilerplate catch logic will be removed:
```csharp
public async Result<Task<Customer>, Exception> Insert(Customer customer)
=> await Try.ExpressionAsync(async () => 
  {
      // Try to insert Customer into the DB.
      // ....
      return Result<Customer, Exception>.Succeeded(customer);
  });
```
After getting the Result, methods like Map and Bind can be chained, and the passed-in delegates will only be run if the Result contains a success. You can also use LINQ Query syntax, since Result contains the necessary methods to allow Monadic Composition:
```csharp
var result = from s1 in Result<int, string>.Succeeded(1)
             from s2 in Result<int, string>.Succeeded(2)
             from s3 in Result<int, string>.Succeeded(3)
             select s1 + s2 + s3; // Success(6)
```
### Avoid dealing with null values, the Maybe Monad:
```csharp
public async Maybe<Order> GetOrderById(int id)
{
    var order = GetOrderFromDB(id);
    return Maybe<Order>.Some(order);
}
```
Instead of having to deal with null when calling GetOrderById (or forget about it and end up with a NullReferenceException), you can perform safe operations on Maybe, they are safe because if the order was not found, the operations will not be performed, and Maybe encapsulates an centralizes the logic to do this. Also, with this signature, GetOrderById is honest, it is very explicit about the fact that the order might not be found, which is a very important piece of information.

```csharp
// AssingOrderToCustomer returns a newly created Customer with the given order assgined to it.
// What if the order was not found? Leave that to the Maybe Monad.

var customerId = GetOrderById(5)
                    .Map(order => AssingOrderToCustomer(customer, order), Maybe<Customer>.None);
                    .GetOrElse(customer => customer.Id, 0);
```

### Declarative Dispose (made into an expression):
```csharp
var result = await Dispose.UsingAsync(() => new DisposableInstance, async disposableInstance =>
{
    // Add in here whatever you need in the body of the dipose.
    // ...
    return disposableInstance.DoTheWork();
});
```
In general, try to favor expressions over statements. Expressions are reusable, they can be returned out of a method or passed into one. Statements cannot do any of that. This is such a powerful technique that in C# 8 Microsoft added 'using' expressions to deal with disposable objects. And they did the same with 'switch' expressions.

### Object mapping:
```csharp
var greeting = "hello";
var message = greeting.MapTo(g => $"{g} world"); // "hello world"
```
Imperative code:
```csharp
var customer = GetCustomerById(id);
var billModel = new BillModel 
{
    customerId = customer.Id,
    customerName = customer.FullName,
    address = customer.Address
    ...
};
```
Declarative version:
```csharp
var billModel = GetCustomerById(id)
                  .MapTo(c => new BillModel 
                  {
                      customerId = c.Id,
                      customerName = c.FullName,
                      address = c.Address
                      ...
                  });
```
### AsyncLazyPipeline (Composable pipelines):
Composing a pipeline with a method.
```csharp
int AddFour(int number) => number + 4;

var pipeline = CreatePipeLine.With(() => 5).Select(AddFour);
var task = pipeline.Flatten(); // Task(9)
```
Composing a pipeline with another pipeline:
```csharp
var initialPipeline = CreatePipeLine.With(() => 5);
var resultingPipeline = initialPipeline.SelectMany(five => CreatePipeLine.With(() => five + 4));

var task = resultingPipeline.Flatten(); // Task(9)
```
Hello Monadic Composition again!
```csharp
var firstPipeline = CreatePipeLine.With(() => 5);
var secondPipeline = CreatePipeLine.With(() => 6);
var thirdPipeline = CreatePipeLine.With(() => 9);

var resultingPipeline = from firstValue in firstPipeline
                        from secondValue in secondPipeline
                        from thirdValue in thirdPipeline
                        select firstValue + secondValue + thirdValue;

var task = resultingPipeline.Flatten();  // Task(20)
```
To see a more real-world example using pipelines, you can check https://gist.github.com/ericrey85/da9671a22234ef981e5ee3653face4af.

There are a lot of things that you can do with LeanSharp, hopefully these examples give you an idea of how to get started.
