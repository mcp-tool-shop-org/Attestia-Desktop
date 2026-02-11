using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attestia.App;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration will be wired up in later commits:
        // - Attestia.Client (HTTP client + typed options)
        // - Attestia.Sidecar (Node.js process manager)
        // - Attestia.ViewModels (page ViewModels)
    }
}
