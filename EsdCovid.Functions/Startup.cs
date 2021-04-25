using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(EsdCovid.Functions.Startup))]

namespace EsdCovid.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Data>((s) =>
            {
                var data = Data.GetInstance().GetAwaiter().GetResult();
                return data;
            });
            builder.Services.AddScoped<QueriesRepository>(s =>
            {
                var data = s.GetRequiredService<Data>();
                return new QueriesRepository(data.Container);
            });

            // builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}