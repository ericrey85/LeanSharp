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
public async Task<Result<Customer, Exception>> Insert(Customer customer)
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
public async Task<Result<int, Exception>> Insert(Customer customer)
=> await Try.ExpressionAsync(async () =>
{
  // Try to asynchronously insert the Customer into the DB.
  // ....
  return customer;
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
public Maybe<Order> GetOrderById(int id)
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
  .Map(order => AssingOrderToCustomer(customer, order));
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
    CustomerId = customer.Id,
    CustomerName = customer.FullName,
    StreetName = customer.StreetName
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
                      StreetName = c.StreetName
                      ...
                  });
```
### Pipeline (Composable pipelines):
Composing a pipeline with a method.
```csharp
int AddFour(int number) => number + 4;

var pipeline = CreatePipeline.With(() => 5).Select(AddFour);
var task = pipeline.Flatten(); // Task(9)
```
Composing a pipeline with another pipeline:
```csharp
var initialPipeline = CreatePipeline.With(() => 5);
var resultingPipeline = initialPipeline.SelectMany(five => CreatePipeline.With(() => five + 4));

var task = resultingPipeline.Flatten(); // Task(9)
```
Hello Monadic Composition again!
```csharp
var firstPipeline = CreatePipeline.With(() => 5);
var secondPipeline = CreatePipeline.With(() => 6);
var thirdPipeline = CreatePipeline.With(() => 9);

var resultingPipeline = from firstValue in firstPipeline
                        from secondValue in secondPipeline
                        from thirdValue in thirdPipeline
                        select firstValue + secondValue + thirdValue;

var task = resultingPipeline.Flatten();  // Task(20)
```
To see a more real-world example using pipelines, you can check https://gist.github.com/ericrey85/da9671a22234ef981e5ee3653face4af.

## SafePipeline(Composable exception-free pipelines)
After a year writing ROP almost everyday, the evident downside that I found was a lot of consecutive await(s), in order to await all the chained tasks. In order to save developers from this, Pipeline was created, but then if I wanted to do ROP and at the same time use Pipeline, I needed to deal with two Monads at the same time (Pipeline and Result). After seeing how many times I ended up with something like Pipeline<Result<TSuccess, TFailure>> to create a Pipeline that was exception-free, I decided to create SafePipeline. SafePipeline is also a Monad Transformer, it gives you a pipeline that encapsulates ROP for you, and knows how to act depending of the underlying value being a success or a failure.
```csharp
async Task<Result<int, string>> GetFirstValue(int number) => await Result<int, string>.Succeeded(number + 4).AsTask();
async Task<Result<int, string>> GetSecondValue(int number) => await Result<int, string>.Succeeded(number + 5).AsTask();
async Task<int> GetThirdValue(int number) => await (number + 4).AsTask();
int GetFourthValue(int number) => number + 5;

SafePipeline<int, Exception> GetFithValue(int number) => CreateSafePipeline.TryWith(() => number + 6);

var firstPipe = CreateSafePipeline.With(() => GetFirstValue(5));
var secondPipe = firstPipe.Select(GetSecondValue);
var thirdPipe = secondPipe.Select(GetThirdValue);
var fourthPipe = secondPipe.Select(GetFourthValue);
var fithPipe = fourthPipe.SelectMany(value => GetFithValue(value).ToStringFailure());

var finalPipe = from firstValue in secondPipe
                from secondValue in thirdPipe
                from thirdValue in fithPipe
                select firstValue + secondValue + thirdValue;

var result = await finalPipe.Flatten(); // Success (57)
```
Operations that throw exceptions are safe to use now, and if an exception occurs, it is wrapped in a failed Result.
```csharp
int InvalidDivideByZeroOperation(int number) => number / 0;
async Task<int> GetValue(int number) => await (number + 4).AsTask();

var firstPipe = CreateSafePipeline.TryWith(() => InvalidDivideByZeroOperation(5));
var finalPipe = firstPipe.Select(GetValue);

var result = await finalPipe.Flatten(); // Failure (exception)
```
One thing to notice is that SafePipeline is pure and deterministic, it does not matter how many times you call it, as long as you pass the same parameters, you will get the same response (huge benefit in software development), and exceptions will not spoil that.

There are a lot of things that you can do with LeanSharp, hopefully these examples give you an idea of how to get started.
