﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TinyFakeHostApp.Persistence;
using TinyFakeHostApp.ServiceModules;
using TinyFakeHostApp.Supports;

namespace TinyFakeHostApp
{
    public class TinyFakeHostBootstrapper
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IFakeRequestResponseRepository, FakeRequestResponseRepository>();
            services.AddScoped<IRequestedQueryRepository, RequestedQueryRepository>();
            services.AddTransient<IRequestValidator, RequestValidator>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<FakeServiceModule>();
        }
    }
}
