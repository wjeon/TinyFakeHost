# TinyFakeHost

[![Build Status](https://ci.appveyor.com/api/projects/status/9aluqo4apo4jbcd2?svg=true)](https://ci.appveyor.com/project/wjeon/tinyfakehost)

***Summary***

TinyFakeHost is a library that can help you to fake a backend web service when you test an application that calls the backend service.

Faking the backend service sometime can be more helpful than faking the service client method to return the fake result for the situations like:
* the service client method may have a bug
* you need to test for the backend service timing out

and more.


With TinyFakeHost, you can:
* fake a backend web service’s response for the expected query request with optional parameters
* assert the expected query with optional parameters has been requested
* check all the requested queries


***NuGet***

https://www.nuget.org/packages/TinyFakeHost/


***Port conflict management:***

When two fake hosts run concurrently with the same port number, one fake host waits until the other finishes.

Examples:
---------
***Start and stop a fake host***
```
[SetUp]
public void SetUp()
{
    _tinyFakeHost = new TinyFakeHost("http://localhost:5432/someService/v1/"); 
    _faker = _tinyFakeHost.GetFaker(); // When you fake backend web service responses
    _asserter = _tinyFakeHost.GetAsserter(); // When you assert expected queries have been requested
    _tinyFakeHost.Start();
}

[TearDown]
public void TearDown()
{
    _tinyFakeHost.Stop();
    _tinyFakeHost.Dispose();
}
```
***Fake web service response***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    _faker.Fake(f => f
        .IfRequest("/vendors/9876-5432-1098-7654/products")
        .WithUrlParameters("type=desk&manufactureYear=2013")
        .ThenReturn(products)
    );

    _faker.Fake(f => f
        .IfRequest("/vendors")
        .ThenReturn(vendors)
    );

    _faker.Fake(f => f
        .IfRequest("/vendors/6543-2109-8765-4321/products")
		.WithMethod(Method.POST)
        .WithFormParameters("type=chair&manufactureYear=2014")
        .ThenReturn(result)
    );

    // your test code . . . .
}
```
***Fake web service processing long time (useful for testing the service timeout)***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    _faker.Fake(f => f
        .IfRequest("/vendors")
        .ThenReturn(new FakeResponse{
            ContentType = "application/json",
            Content = @"{""message"":""Request Timeout""}",
            StatusCode = HttpStatusCode.RequestTimeout,
            MillisecondsSleep = 6000
        })
    );

    // your test code . . . .
}
```
***Assert requested query***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    _asserter.Assert(a => a
        .Resource("/vendors/9876-5432-1098-7654/products")
        .WithUrlParameters("type=desk&manufactureYear=2013")
        .WasRequested()
    );

    _asserter.Assert(a => a
        .Resource("/vendors")
        .WasRequested()
    );

    _asserter.Assert(a => a
        .Resource("/vendors/6543-2109-8765-4321/products")
        .WithMethod(Method.POST)
        .WithFormParameters("type=chair&manufactureYear=2014")
        .WasRequested()
    );
}
```
***Check all the requested queries***
```
private void PrintRequestedQueries()
{
    foreach (var requestedQuery in _tinyFakeHost.GetRequestedQueries())
        Console.WriteLine("Requested query - {0}", requestedQuery);
}
```
You can also set the RequestedQueryPrint property to true instead. With this way, the requested queries will still be printed directly from the fake host even if there is an error in the service client method call.
```
    _tinyFakeHost.RequestedQueryPrint = true;
```