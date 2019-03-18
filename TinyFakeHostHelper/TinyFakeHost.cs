using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

            _tinyHost.Dispose();
            throw new Exception("Fake host failed to start because the port is in use too long time.");
        }

        private void TryStart()
        {
            const int waitMinutesUntilConflictedHostStops = 10;
            var port = ParsePortNumberFrom(GetHostAddress(_tinyHost));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!_hostStarted && stopwatch.Elapsed < TimeSpan.FromMinutes(waitMinutesUntilConflictedHostStops))
            {
                if (!IsPortFree(port))
                {
                    Console.WriteLine(@"The port {0} for fake Host '{1}' is in use now ({2:o}). Wait until the port becomes available!!!",
                        port, _hostId.FirstSegment(), DateTime.Now);
                    Thread.Sleep(100);
                    continue;
                }

                _tinyHost.Start();
                _hostStarted = true;
                Console.WriteLine(@"Fake Host '{0}' started at {1:o}.", _hostId.FirstSegment(), DateTime.Now);
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
            _tinyHost.StopAsync(timeout ?? TimeSpan.FromMilliseconds(100)).Wait();
            Console.WriteLine(@"Fake Host '{0}' stopped at {1:o}.", _hostId.FirstSegment(), DateTime.Now);
        }

        public void Dispose()
        {
            _tinyHost.Dispose();
        }

        private static int ParsePortNumberFrom(string hostAddress)
        {
            return int.Parse(hostAddress.Split(':')[2].Split('/')[0]);
        }

        private static string GetHostAddress(IWebHost host)
        {
            return ((IServerAddressesFeature)host.ServerFeatures.First().Value).Addresses.First();
        }

        private static bool IsPortFree(int port)
        {
            var tcpListener = default(TcpListener);

            try
            {
                var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();

                return true;
            }
            catch (SocketException)
            {
            }
            finally
            {
                if (tcpListener != null)
                    tcpListener.Stop();
            }

            return false;
        }
    }
}