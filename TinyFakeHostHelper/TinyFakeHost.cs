﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;
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
        private readonly NancyHost _nancyHost;
        private bool _hostStarted;
        private Guid _hostId;
        private readonly TinyIoCContainer _container;

        public TinyFakeHost(string uri, ITinyFakeHostConfiguration fakeHostConfig = null)
        {
            _hostId = Guid.NewGuid();
            var bootstrapper = new TinyFakeHostBootstrapper(fakeHostConfig ?? new TinyFakeHostConfiguration());
            var nancyHostConfig = new HostConfiguration { UrlReservations = { CreateAutomatically = true } };

            _nancyHost = new NancyHost(bootstrapper, nancyHostConfig, new Uri(uri.TrimEnd('/') + "/"));

            _container = bootstrapper.GetTinyIoCContainer();
        }

        public void Start()
        {
            _hostStarted = false;

            TryStart();

            if (_hostStarted) return;

            _nancyHost.Dispose();
            throw new Exception("Fake host failed to start because it has conflicted with other host too long time.");
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
                    if (e.Message.StartsWith("Failed to listen on prefix 'http://+:") &&
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
            return new RequestResponseFaker(_container);
        }

        public RequestedQueryAsserter GetAsserter()
        {
            return new RequestedQueryAsserter(_container);
        }

        public IEnumerable<FakeRequest> GetRequestedQueries()
        {
            var requestedQueryRepository = _container.Resolve<IRequestedQueryRepository>();
            return requestedQueryRepository.GetAll();
        }

        public void Stop()
        {
            if (!_hostStarted)
            {
                Console.WriteLine(@"Fake Host '{0}' not started.", _hostId.FirstSegment());
                return;
            }
            _nancyHost.Stop();
            Console.WriteLine(@"Fake Host '{0}' stopped.", _hostId.FirstSegment());
        }

        public void Dispose()
        {
            _nancyHost.Dispose();
        }
    }
}