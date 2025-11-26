using Microsoft.Extensions.DependencyInjection;

namespace frontend.Extension
{

    // This class will allow access to services from static methods.
    public static class ServiceLocator
    {
        // The static reference to the application's service provider
        public static IServiceProvider Provider { get; set; }
    }
}
