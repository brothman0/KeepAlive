using KeepAlive;
using KeepAlive.External;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
            services
                .AddHostedService<KeepAliveService>()
                .AddSingleton<ICommonAdapter, CommonAdapter>()
                .AddSingleton<ICommonAgent, CommonAgent>()
                .AddTransient<IExternalAdapter, ExternalAdapter>()
                .AddTransient<IExternalAgent, ExternalAgent>())
    .Build();

await host.RunAsync();