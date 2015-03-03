﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Nancy.Hosting.Self;
using TinyFakeHostHelper.Tests;

namespace TinyFakeHostHelper
{
    public class TinyFakeHost : IDisposable
    {
        private readonly NancyHost _nancyHost;
        private bool _hostStarted;
        private Guid _hostId;

        public TinyFakeHost(string uri)
        {
            _hostId = Guid.NewGuid();

            var nancyHostConfig = new HostConfiguration { UrlReservations = { CreateAutomatically = true } };

            _nancyHost = new NancyHost(nancyHostConfig, new Uri(uri.TrimEnd('/') + "/"));
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