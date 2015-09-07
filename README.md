# TinyFakeHost [![Build Status](https://ci.appveyor.com/api/projects/status/9aluqo4apo4jbcd2?svg=true)](https://ci.appveyor.com/project/wjeon/tinyfakehost/branch/master)

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

***Configuration:***

The default maximum number of the resource path segments you can request is 10. If you want to request the resource path with more than 10 segments, you need to add `MaximumNumberOfPathSegments` app setting entry with bigger number value.

E.g.
```
<appSettings>
  <add key="MaximumNumberOfPathSegments" value="15" />
</appSettings>
```

***Url Reservation:***

TinyFakeHost reserves the url automatically.
You may try reserving the url manually with sufficient privileges if it is not reserved automatically with any reason like your automated test is running under insufficient privileges.
You can use `netsh http add urlacl` command to manually reserve the rul.

E.g.
```
netsh http add urlacl url=http://+:5432/ user=everyone
```
For more information about `netsh http add urlacl` command, please visit https://msdn.microsoft.com/en-us/library/windows/desktop/cc307223(v=vs.85).aspx

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
        .WithParameters("type=desk&manufactureYear=2013")
        .ThenReturn(products)
    );

    _faker.Fake(f => f
        .IfRequest("/vendors")
        .ThenReturn(vendors)
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
        .WithParameters("type=desk&manufactureYear=2013")
        .WasRequested()
    );

    _asserter.Assert(a => a
        .Resource("/vendors")
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
