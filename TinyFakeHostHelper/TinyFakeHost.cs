using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Hosting.Server.Features;
using TinyFakeHostHelper.Asserters;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Extensions;
using TinyFakeHostHelper.Fakers;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper
{
    public class TinyFakeHost : IDisposable
    {
        private readonly IWebHost _tinyHost;
        private bool _hostStarted;
        private Guid _hostId;
        private readonly ITinyFakeHostConfiguration _fakeHostConfig;

        public TinyFakeHost(string uri, ITinyFakeHostConfiguration fakeHostConfig = null) :
            this(new WebHostBuilder().UseKestrel().UseUrls(uri.TrimEnd('/') + "/"), fakeHostConfig)
        {
        }

        public TinyFakeHost(IWebHostBuilder webHostBuilder, ITinyFakeHostConfiguration fakeHostConfig = null)
        {
            _hostId = Guid.NewGuid();

            _fakeHostConfig = fakeHostConfig ?? new TinyFakeHostConfiguration();

            _tinyHost = webHostBuilder
                        .ConfigureServices(s => s.AddSingleton(_fakeHostConfig))
                        .UseStartup<TinyFakeHostBootstrapper>()
                        .Build();
        }

        public bool RequestedQueryPrint
        {
            set { _fakeHostConfig.RequestedQueryPrint = value; }
        }

        public void Start()
        {
            _hostStarted = false;

            TryStart();

            if (_hostStarted) return;

            throw new Exception("Fake host failed to start because it has conflicted with other host too long time.");
            _tinyHost.Dispose();
        }

        private void TryStart()
        {
            const int waitMinutesUntilConflictedHostStops = 10;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!_hostStarted &&
                   stopwatch.Elapsed < TimeSpan.FromMinutes(waitMinutesUntilConflictedHostStops))
                try
                {
                    _nancyHost.Start();
                    _hostStarted = true;
                    Console.WriteLine(@"Fake Host '{0}' started.", _hostId.FirstSegment());
                }
                catch (HttpListenerException e)
                {
                    if (e.Message.StartsWith("Failed to listen on prefix 'http") &&
                        e.Message.EndsWith("/' because it conflicts with an existing registration on the machine."))
                    {
                        Console.WriteLine(@"Fake Host '{0}' conflicted with other host. Wait for the other host stops.",
                                          _hostId.FirstSegment());
                        Thread.Sleep(100);
                    }
                    else throw;
                }

            stopwatch.Stop();
        }

        public RequestResponseFaker GetFaker()
        {
            return new RequestResponseFaker(_tinyHost.Services.GetService<IFakeRequestResponseRepository>());
        }

        public RequestedQueryAsserter GetAsserter()
        {
            return new RequestedQueryAsserter(_tinyHost.Services);
        }

        public IEnumerable<FakeRequest> GetRequestedQueries()
        {
            var requestedQueryRepository = _tinyHost.Services.GetService<IRequestedQueryRepository>();
            return requestedQueryRepository.GetAll();
        }

        public void Stop(TimeSpan? timeout = null)
        {
            if (!_hostStarted)
            {
                Console.WriteLine(@"Fake Host '{0}' not started.", _hostId.FirstSegment());
                return;
            }
            Console.WriteLine(@"Fake Host '{0}' stopped.", _hostId.FirstSegment());
            _tinyHost.StopAsync(timeout ?? TimeSpan.FromMilliseconds(100)).Wait();
        }

        public void Dispose()
        {
            _tinyHost.Dispose();
        }
    }
}