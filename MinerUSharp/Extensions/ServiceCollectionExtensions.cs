using Microsoft.Extensions.DependencyInjection;

namespace MinerUSharp.Extensions
{
    /// <summary>
    /// Extension methods for configuring MinerU services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the MinerU client to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUrl">The base URL of the MinerU API.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddMineruClient(this IServiceCollection services, string baseUrl)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or whitespace.", nameof(baseUrl));

            services.AddHttpClient<IMineruClient, MineruClient>((httpClient, serviceProvider) =>
            {
                return new MineruClient(baseUrl, httpClient);
            });

            return services;
        }

        /// <summary>
        /// Adds the MinerU client to the service collection with a configuration action.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUrl">The base URL of the MinerU API.</param>
        /// <param name="configureClient">An action to configure the HttpClient.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddMineruClient(
            this IServiceCollection services,
            string baseUrl,
            Action<HttpClient> configureClient)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or whitespace.", nameof(baseUrl));

            if (configureClient == null)
                throw new ArgumentNullException(nameof(configureClient));

            services.AddHttpClient<IMineruClient, MineruClient>((httpClient, serviceProvider) =>
            {
                configureClient(httpClient);
                return new MineruClient(baseUrl, httpClient);
            });

            return services;
        }
}
}
