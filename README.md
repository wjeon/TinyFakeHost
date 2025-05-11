# TinyFakeHost

[![Build Status](https://ci.appveyor.com/api/projects/status/9aluqo4apo4jbcd2?svg=true)](https://ci.appveyor.com/project/wjeon/tinyfakehost)

***Summary***

TinyFakeHost is a library designed to simulate backend web service responses in your tests. It allows you to easily fake HTTP responses for your application, enabling more comprehensive testing compared to mocking the client method directly.

***Why Use TinyFakeHost?***

When testing applications that make backend calls, mocking client code can fall short. TinyFakeHost provides a simple but powerful solution to fake real HTTP backend behavior in your tests.

Use TinyFakeHost when you want to:
* Simulate realistic backend responses, including query parameters, form posts, and HTTP methods.
* Test for timeouts, slow responses, or specific HTTP status codes (e.g., 404, 500).
* Assert that your application actually made the expected requests - not just return fake data.
* View all incoming requests during tests for easier debugging.

TinyFakeHost helps you validate the request construction (including query parameters and method), timing issues, and other edge cases that are difficult to test using client-side mocks alone.

***NuGet Package***

https://www.nuget.org/packages/TinyFakeHost/


***Port Conflict Handling***

When running multiple fake hosts on the same port, TinyFakeHost automatically prevents conflicts by having one fake host wait until the other finishes. This eliminates the need for manual port management, making it safe to run tests in parallel.

Example Usage:
---------
***Start and Stop a Fake Host***
```
[SetUp]
public void SetUp()
{
    _tinyFakeHost = new TinyFakeHost("http://localhost:5432/someService/v1/"); 
    _faker = _tinyFakeHost.GetFaker(); // Use this to fake backend responses
    _asserter = _tinyFakeHost.GetAsserter(); // Use this to assert expected queries
    _tinyFakeHost.Start();
}

[TearDown]
public void TearDown()
{
    _tinyFakeHost.Stop();
    _tinyFakeHost.Dispose();
}
```
***Fake a Web Service Response***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    // Setup fake responses
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
***Simulate Long Response Times (Timeout Testing)***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    // Setup a fake response with a delay (simulates a service timeout)
    _faker.Fake(f => f
        .IfRequest("/vendors")
        .ThenReturn(new FakeResponse{
            ContentType = "application/json",
            Content = @"{""message"":""Request Timeout""}",
            StatusCode = HttpStatusCode.RequestTimeout,
            MillisecondsSleep = 6000 // Simulate a timeout by sleeping for 6 seconds
        })
    );

    // your test code . . . .
}
```
***Assert the Requested Query***
```
[Test]
public void Your_test_method()
{
    // your test code . . . .

    // Assert that the expected request was made
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
***Check All Requested Queries***
```
private void PrintRequestedQueries()
{
    foreach (var requestedQuery in _tinyFakeHost.GetRequestedQueries())
        Console.WriteLine("Requested query - {0}", requestedQuery);
}
```
Alternatively, you can set the RequestedQueryPrint property to true to automatically print requested queries:
```
_tinyFakeHost.RequestedQueryPrint = true; // Automatically prints requested queries
```

***Additional Notes***
* TinyFakeHost is best suited for integration testing, where you need to test the actual flow of requests and responses.
* The fake host runs a real HTTP listener and responds to HTTP requests as configured, so you can simulate complex service behaviors, including failures, timeouts, and status codes.