using System;
using Nancy.Hosting.Self;

namespace TinyFakeHostHelper
{
    public class TinyFakeHost : IDisposable
    {
        private readonly NancyHost _nancyHost;

        public TinyFakeHost(string uri)
        {
            var nancyHostConfig = new HostConfiguration { UrlReservations = { CreateAutomatically = true } };

            _nancyHost = new NancyHost(nancyHostConfig, new Uri(uri.TrimEnd('/') + "/"));
        }

        public void Start()
        {
            _nancyHost.Start();
        }

        public void Stop()
        {
            _nancyHost.Stop();
        }

        public void Dispose()
        {
            _nancyHost.Dispose();
        }
    }
}